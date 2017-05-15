using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace WeederDigitalOutput
{
    /// <summary>
    /// AUTHOR: Adam Bertsch
    /// DATE:   12-9-2011
    /// 
    /// DESIGN/PURPOSE:
    /// This class is a collection of functionality for the Weeder Technologies WTDOT-M Digital Output Module.
    /// It is primarily meant to be used as a means of controlling solenoids for the keydrop kiosks and as such,
    /// some functionality deemed irrelevant has been omitted. This class does not have it's own COM port and expects
    /// to be provided with one already instantiated and opened by the primary program.
    /// </summary>
    class Weeder_Solenoid_Control_Board
    {
        SerialPort CommandOutput;

        private const int READ_COMPORT_TIMEOUT = 10000; // 10 

        public Weeder_Solenoid_Control_Board(SerialPort OutputPort)
        {
            CommandOutput = OutputPort;

            CommandOutput.ReadTimeout = READ_COMPORT_TIMEOUT;

            CommandOutput.NewLine = ((char)13).ToString(); //Commands to the module must be terminated with a carriage return - "/r" 
        }

        /// <summary>
        /// The Write command uses all 8 output ports simultaneously as if they were a single data port. In this
        /// way one byte can be sent at a time with one bit corresponding to each output. 
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="data">The single byte of data to be translated to the 8 output ports of the board. The 
        /// most significant bit goes with channel A and the least significant with channel H.</param>
        /// <returns>A string value returned by the board. If the command was successful, then the command is returned
        /// just as it was sent. If unsuccessful, the address of the board followed by a '?' will be returned.</returns>
        public string Write(char address, byte data)
        {
            CommandOutput.Write(address.ToString() + ((char)87).ToString() + data.ToString() + CommandOutput.NewLine);
            return CommandOutput.ReadLine();
        }

        /// <summary>
        /// This method sends the specified output pin to a floating state as current is drawn away from the base of the 
        /// corresponding transistor base. This version of the method allows the channel to be set floating for an 
        /// amount of time specified by MilliTime. 
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="channel">Designates which pin (A->H) is to be utilized.</param>
        /// <param name="MilliTime">Time in milliseconds that the output will remain floating before being set low again.
        /// Range of this time is 0 -> 65535 milliseconds.</param>
        /// <returns>A string value returned by the board. If the command was successful, then the command is returned
        /// just as it was sent. If unsuccessful, the address of the board followed by a '?' will be returned.</returns>
        public string outputFloating(char address, char channel, int MilliTime)
        {
            CommandOutput.Write(address.ToString() + ((char)72).ToString() + channel.ToString() + MilliTime.ToString() + CommandOutput.NewLine);
            return CommandOutput.ReadLine();
        }

        /// <summary>
        /// This method sends the specified output pin to a floating state as current is drawn away from the base of the 
        /// corresponding transistor base. This version of the method will set the output floating until told otherwise.
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="channel">Designates which pin (A->H) is to be utilized.</param>
        /// <returns>A string value returned by the board. If the command was successful, then the command is returned
        /// just as it was sent. If unsuccessful, the address of the board followed by a '?' will be returned.</returns>
        public string outputFloating(char address, char channel)
        {
            CommandOutput.Write(address.ToString() + ((char)72).ToString() + channel.ToString() + CommandOutput.NewLine);
            return CommandOutput.ReadLine();
        }

        /// <summary>
        /// This method pulls the specified channel output to GND and thus will pull the solenoid attached to it.
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="channel">Designates which pin (A->H) is to be utilized.</param>
        /// <param name="MilliTime">Time in milliseconds that the output will remain grounded before being set high again.
        /// Range of this time is 0 -> 65535 milliseconds.</param>
        /// <param name="Iterations">How many times should the pulling process be repeated</param>
        /// <returns>An array of string responses from the board. One for each time the pull command was run.
        /// If the command was successful, then the command is returned just as it was sent. If unsuccessful, the 
        /// address of the board followed by a '?' will be returned.</returns>
        public string[] solenoidPull(char address, char channel, int MilliTime, int Iterations)
        {
          
            string[] responses = new string[Iterations];

            try
            {
                for (int i = 0; i < Iterations; i++)
                {
                    CommandOutput.Write(address.ToString() + ((char)76).ToString() + channel.ToString() + MilliTime.ToString() + CommandOutput.NewLine);
                    responses[i] = CommandOutput.ReadLine();
                    Thread.Sleep(MilliTime + 100); //100 millseconds added to allow for some space between commands because if they are too close
                }                                //together they can cause problems with each other.

            }
            catch (TimeoutException)
            {
                KeyCabinetKiosk.Program.logDebug("timeout on selnoid open command");
                return null;
            }
            catch (Exception ex)
            {
                KeyCabinetKiosk.Program.logError("exception on selnoid open command" + ex.Message);
                return null;
            }
            return responses;
        }

        /// <summary>
        /// This method sets channel H to be pulse width modulated
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="value">This value is used to set the duty cycle. Valid values are 0 -> 1024</param>
        /// <returns>A string value returned by the board. If the command was successful, then the command is returned
        /// just as it was sent. If unsuccessful, the address of the board followed by a '?' will be returned.</returns>
        public string pwmChannelH(char address, int value)
        {
            CommandOutput.Write(address.ToString() + ((char)80).ToString() + value.ToString() + CommandOutput.NewLine);
            return CommandOutput.ReadLine();
        }

        /// <summary>
        /// This method is used to read the current state of a given channel (high or low)
        /// </summary>
        /// <param name="address">The ASCII address of the board you want to talk to. Valid addresses are A->P and a->p. 
        /// Addresses are set with the DIP switches on each board with 32 combinations possible.</param>
        /// <param name="channel">Designates which pin (A->H) is to be read.</param>
        /// <returns>The value of the chosen output</returns>
        public string readChannel(char address, char channel)
        {
            CommandOutput.Write(address.ToString() + ((char)82).ToString() + channel.ToString() + CommandOutput.NewLine);
            return CommandOutput.ReadLine();
        }               
    }
}
