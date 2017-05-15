using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeederDigitalOutput;
using System.IO.Ports;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// WeederControlBoardPort  - controls the port that is used for the Weeder Digital Control board
    ///                         currently only works with a single board with 8 relays
    /// </summary>
    class WeederControlBoardPort:SolenoidControlBoardPort
    {
        private Weeder_Solenoid_Control_Board ControlBoard;

        override protected SerialPort port { get; set; }

        private char[] relayLookUpArray = {'A','B','C','D','E','F','G','H'}; //x is bad locations - location are 1-8
        private char[] boardAddrLookUpArray = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P',
                                               'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p'};

        public WeederControlBoardPort(string portName)
        {
            try
            {
                //  9600, no parity, 1 stop bits - 8 bits
                port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);

                if (port == null)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port null");
                }
                port.Open();

                if (port.IsOpen == false)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port failed to open");
                }

                ControlBoard = new Weeder_Solenoid_Control_Board(port);
            }
            catch (Exception e)
            {
                StringBuilder message = new StringBuilder(portName);
                message.Append(": ");
                message.Append(e.Message);

                throw new Exception(message.ToString());
            }
        }


        /// <summary>
        /// ClosePort - close the weeder serial port
        /// </summary>
        override public void ClosePort()
        {
            try
            {
                if ((port != null) && (port.IsOpen))
                {
                    port.Close();
                }
            }
            catch (Exception ex)
            {
                Program.logError("Error closing weeder digital io port " + ex.Message);
            }
        }

        /// <summary>
        /// OpenAndCloseRelay - sends command messages to the Weeder to open and close relay
        ///                     to activate solenoid in order to drop a key - delays between commands
        /// </summary>
        /// <param name="loc"></param>
        override public void OpenAndCloseRelay(int loc)
        {
            try
            {
                // 0 is an invalid location
                // valid locations are 1 - 8 for boards 1-32
                if ((loc == 0) || (loc > 255)) { throw new Exception("OpenAndCloseRelay - invalid location " + loc.ToString()); }
                
                char locationChar = relayLookUpArray[(loc - 1) % 8];
                char boardAddrChar = boardAddrLookUpArray[(loc - 1)/8];

                string[] response = ControlBoard.solenoidPull(boardAddrChar, locationChar , 700, 1);

                if (response == null) { throw new Exception("OpenAndCloseRelay - read had no response"); }

                StringBuilder sb = new StringBuilder();

                foreach (string s in response)
                {
                    sb.AppendLine(s);
                }

                Program.logDebug("Port opening " + loc.ToString() + " " + sb.ToString());

            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error closing weeder digital io port " + ex.Message;
                throw new Exception(exceptionMessage);
            }
        }

    }
}
