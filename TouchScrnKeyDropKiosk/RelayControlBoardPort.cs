using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// RelayControlBoardPort - this is the port that sends message to the Relay Control board 
    ///             which turns on and off relays to drop the keys
    ///             
    ///             will throw exceptions
    /// </summary>
    public class RelayControlBoardPort:SolenoidControlBoardPort
    {
        override protected SerialPort port { get; set; }

        private byte MessageSequence; //If this sequence number is not maintained, these relay boards have potential to 'lock up' because it could think it is seeing
                                      //the same message multiple times if the sequence number is the same.

        private const int RELAY_OPEN_TIME = 500; // 1/2 second 

        private const int RELAY_CLOSE_TIME = 500; // 1/2 second
       
        /// <summary>
        /// RelayControlBoardPort - constructor - opens port or throws
        /// </summary>
        /// <param name="portName" - must be com1..com4></param>      
        public RelayControlBoardPort(string portName)
        {
            try
            {
                //  19200, no parity, 2 stop bits - 8 bits
                port = new SerialPort(portName, 19200, Parity.None, 8, StopBits.Two);

                if (port == null)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port null");        
                }
                port.Open();

                if (port.IsOpen == false)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port failed to open");
                }
                MessageSequence = 0xD0;//sequence number set to the first in the sequence
            }
            catch(Exception e)
            {
                StringBuilder message = new StringBuilder(portName);
                message.Append(": ");
                message.Append(e.Message);
                
                throw new Exception(message.ToString());
            }

        }
        /// <summary>
        /// ClosePort - close the RCB port
        /// </summary>
        public override void ClosePort()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }
        /// <summary>
        /// OpenAndCloseRelay - sends command messages to the Relay Control board to open and close relay
        ///                     to activate selenoid in order to drop a key - delays between commands
        /// </summary>
        /// <param name="loc"></param>
        public override void OpenAndCloseRelay(int loc)
        {      
            OpenRelay(loc, MessageSequence);

            System.Threading.Thread.Sleep(Program.OPEN_DOOR_INTERVAL);

            CloseRelay(loc, MessageSequence);

            //increment sequence number
            MessageSequence++;
            //if the sequence number has reached 0xE0, roll it over back to the beginning
            if (MessageSequence > 0xDF)
                MessageSequence = 0xD0;              
        }

        /// <summary>
        /// OpenRelay - creates and sends a message to open a relay at the given location
        /// </summary>
        /// <param name="loc" - the location - logical address></param>
        private void OpenRelay(int loc, byte seqnum)
        {
            KeyDispenseMessage command = new KeyDispenseMessage(KeyDispenseMessage.MessageType.OPEN_RELAY, loc, seqnum);
            Program.logDebug("Open relay " + loc.ToString() + " - "+ command.ToString());
            SendMessage(command);
        }
        /// <summary>
        ///  CloseRelay - creates and sends a message to close a relay at the given location
        /// </summary>
        /// <param name="loc"></param>
        private void CloseRelay(int loc, byte seqnum)
        {
            KeyDispenseMessage command = new KeyDispenseMessage(KeyDispenseMessage.MessageType.CLOSE_RELAY, loc, seqnum);
            Program.logDebug("Close relay " + loc.ToString() + " - "+ command.ToString());
            SendMessage(command);
        }
       
        /// <summary>
        /// SendMessage - writes a byte stream to the RCB port
        /// </summary>
        /// <param name="command" - the command array of bytes></param>
        private void SendMessage(KeyDispenseMessage command)
        {

            if (command == null)
            {
                throw new Exception("null command");
            }
            try
            {
                if (port.IsOpen)
                {
                    
                    port.Write(command.commandBytes, 0, command.commandSize);
                }
            }
            catch (Exception e)
            {     
                string message = "Error writing to RCB port: " + e.Message;
                throw new Exception(message);
            }
        }
    }
}

//////////////////////////////////////////////////////////////
/////////////
/////////////
/////////////    this is a portion of the "C" code written for the rabbit boards
/////////////                 left here as history
/////////////
///////////////////////////////////////////////////////////////////
///*** Beginheader */
//const char keyLocs[20]={'\xC0','\xC1','\xC2','\xC3','\xC4','\xC5','\xC6','\xC7','\xC8','\xC9','\xD0','\xD1','\xD2','\xD3','\xD4','\xD5','\xD6','\xD7','\xD8','\xD9'};
//const char keyCkSum[22]={'\x78','\x79','\x7A','\x7B','\x7C','\x7D','\x7E','\x7F','\x80','\x81','\x82','\x88','\x89','\x8A','\x8B','\x8C','\x8D','\x8E','\x8F','\x90','\x91','\x92'};
///*** endheader */
///


    //solinoidOn(loc);
    //#ifdef USE_CTASK
    //    cd_timer( TM_SECOND*.25, ctask_switch );
    //#else
    //    OSTimeDlySec( .25 );
    //#endif
    ////cd_timer( TM_SECOND*.25, ctask_switch );
    //solinoidOff(loc);

//void solinoidOn(int loc)
//{
//    char cLoc;
//    char cCkSum;

//    if(loc<10)
//    {
//        cLoc=keyLocs[loc];
//        cCkSum=keyCkSum[loc];
//    }
//    else
//    {
//        cLoc=keyLocs[loc];
//        cCkSum=keyCkSum[loc+1];
//    }

//    serCputc('\xFF');
//    serCputc('\xE2');
//    serCputc(cLoc);
//    serCputc('\x01');
//    serCputc('\xD5');
//    serCputc('\x01');
//    serCputc('\x00');
//    serCputc(cCkSum);
//}

///* START FUNCTION DESCRIPTION ********************************************

//SYNTAX:	      void solinoidOff(int loc)

//DESCRIPTION:	turns off solinoid

//PARAMETER1:    	int loc

//RETURN VALUE:  	None

//END DESCRIPTION **********************************************************/
//void solinoidOff(int loc)
//{
//    char cLoc;
//    char cCkSum;

//    if(loc<10)
//    {
//        cLoc=keyLocs[loc];
//        cCkSum=keyCkSum[loc+1];
//    }
//    else
//    {
//        cLoc=keyLocs[loc];
//        cCkSum=keyCkSum[loc+2];
//    }

//    serCputc('\xFF');
//    serCputc('\xE2');
//    serCputc(cLoc);
//    serCputc('\x01');
//    serCputc('\xD5');
//    serCputc('\x02');
//    serCputc('\x00');
//    serCputc(cCkSum);
//}


    //serCopen(19200);
    //serCparity(PARAM_2STOP);