using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace RAJ_KD8_Board
{
    /// <summary>
    /// This is the driver class for Allen Siefken's KD8 relay controller board. These boards can be linked together via 6-wire RJ-22 phone
    /// cables to create a network of boards and thus a much larger capacity for relays in a given system.
    /// </summary>
    class RAJ_KD8_Board
    {
        public event EventHandler<KD8ResponseEventArgs> KD8Response;
        public event EventHandler<KD8ErrorEventArgs> KD8Error;
        SerialPort CommandPort;
        int SequenceNum;

        public RAJ_KD8_Board(SerialPort Port)
        {
            SequenceNum = 0;
            CommandPort = Port;
            CommandPort.DataReceived += new SerialDataReceivedEventHandler(CommandPort_DataReceived);
        }

        /// <summary>
        /// Handles any data received from the KD8 boards and decides if it is a valid communication or not. 
        /// If not, it will give an error and exit. Otherwise it will handle the communication.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommandPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] lookforstartofcommand = new byte[1];
            byte[] commandresponse = new byte[9];

            try
            {
                for (int j = 0; j < 10; j++)
                {
                    CommandPort.Read(lookforstartofcommand, 0, 1);
                    if (lookforstartofcommand[0] == 0x5D) //look for the start of command byte. Give up after 10 bytes
                    {
                        commandresponse[0] = lookforstartofcommand[0]; //if start of command is found then copy it over so it is not lost
                        for (int i = 1; i < 9; i++)
                        {
                            CommandPort.Read(commandresponse, i, 1);
                        }
                        //do something with response
                        ProcessCommandResponse(commandresponse);
                        return;
                    }
                }
                OnKD8Error(new KD8ErrorEventArgs("From MainApp: Start of Command Not Found"));
            }
            catch (Exception ex)
            {
                OnKD8Error(new KD8ErrorEventArgs(ex.Message));
            }            
        }

        /// <summary>
        /// This method opens a relay in the network of KD8 boards
        /// </summary>
        /// <param name="relayNum">The integer address of the desired relay</param>
        /// <param name="pulseCnt">How many times to open and close the relay for this command</param>
        /// <param name="OnTime">This is the time that the relay will be open in milliseconds</param>
        /// <param name="OffTime">This is the time that the relay will be closed in milliseconds</param>
        public void OpenRelay(int relayNum, int pulseCnt, int OnTime, int OffTime)
        {
            byte[] commandresponse = new byte[9]; int checkSum = 0; int relaynumUpper = 0; int relaynumLower = 0; int SeqCtrl = 0;

            if (relayNum <= 255)
                relaynumLower = relayNum;
            else if (relayNum > 255)
            {
                relaynumLower = relayNum; //this should truncate all of the high-end bytes
                relaynumUpper = relayNum / 256;
            }
            else
            {
                //give error
            }

            /* bit 0-2 = sequence number          (between 0 & 7)
             * bit  3  = (msg from Main App)      (usually 1)
             * bit  4  = no-request msg ACK       (usually 0)
             * bit  5  = msg to KD8               (usually 0)
             * bit  6  = Send msg on reverse loop (usually 1)
             * bit  7  = Send msg on forward loop (usually 1)
            */ 
            SeqCtrl = 8 + SequenceNum + 192;

            checkSum = CreateChecksum(new byte[] { 0x8E, (byte)relaynumLower, (byte)relaynumUpper, (byte)SeqCtrl, (byte)pulseCnt, (byte)(OnTime / 10), (byte)(OnTime / 10) });

            CommandPort.Write(new byte[] { 0x5D, 0x8E, (byte)relaynumLower, (byte)relaynumUpper, (byte)SeqCtrl, (byte)pulseCnt, (byte)(OnTime / 10), (byte)(OnTime / 10), (byte)checkSum }, 0, 9);

            IncSequenceNum();
        }

        /// <summary>
        /// This method inquires of the opened/closed status of a group of 16 relays in the network of KD8 boards
        /// </summary>
        /// <param name="relayNum">The integer address of the desired relay</param>
        public void RelayOpenStatus(int relayNum)
        {
            byte[] commandresponse = new byte[9]; int checkSum = 0; int relaynumLower = 0; int SeqCtrl = 0;

            #region Relay Switch
            if (relayNum >= 1 && relayNum <= 16)
            {
                relaynumLower = 1;
            }
            else if (relayNum >= 17 && relayNum <= 32)
            {
                relaynumLower = 17;
            }
            else if (relayNum >= 33 && relayNum <= 48)
            {
                relaynumLower = 33;
            }
            else if (relayNum >= 49 && relayNum <= 64)
            {
                relaynumLower = 49;
            }
            else if (relayNum >= 65 && relayNum <= 80)
            {
                relaynumLower = 65;
            }
            else if (relayNum >= 81 && relayNum <= 96)
            {
                relaynumLower = 81;
            }
            else if (relayNum >= 97 && relayNum <= 112)
            {
                relaynumLower = 97;
            }
            else if (relayNum >= 113 && relayNum <= 128)
            {
                relaynumLower = 113;
            }
            else if (relayNum >= 129 && relayNum <= 144)
            {
                relaynumLower = 129;
            }
            else if (relayNum >= 145 && relayNum <= 160)
            {
                relaynumLower = 145;
            }
            else if (relayNum >= 161 && relayNum <= 176)
            {
                relaynumLower = 161;
            }
            else if (relayNum >= 177 && relayNum <= 192)
            {
                relaynumLower = 177;
            }
            else if (relayNum >= 193 && relayNum <= 208)
            {
                relaynumLower = 193;
            }
            else if (relayNum >= 209 && relayNum <= 224)
            {
                relaynumLower = 209;
            }
            else if (relayNum >= 225 && relayNum <= 240)
            {
                relaynumLower = 225;
            }
            else if (relayNum >= 241 && relayNum <= 255)
            {
                relaynumLower = 241;
            }
            else
            {
                //give error
            }
            #endregion

            /* bit 0-2 = sequence number          (between 0 & 7)
             * bit  3  = (msg from Main App)      (usually 1) (8)
             * bit  4  = no-request msg ACK       (usually 0) (16)
             * bit  5  = msg to KD8               (usually 0) (32)
             * bit  6  = Send msg on reverse loop (usually 1) (64)
             * bit  7  = Send msg on forward loop (usually 1) (128)
            */
            SeqCtrl = 8 + SequenceNum + 192;

            checkSum = CreateChecksum(new byte[] { 0x8A, (byte)relaynumLower, 0, (byte)SeqCtrl, 0, 0, 0 });

            CommandPort.Write(new byte[] { 0x5D, 0x8A, (byte)relaynumLower, 0, (byte)SeqCtrl, 0, 0, 0, (byte)checkSum }, 0, 9);

            IncSequenceNum();
        }

        /// <summary>
        /// This method takes care of a valid response from the KD8 board. Error responses are considered valid and are handled here.
        /// </summary>
        /// <param name="command"></param>
        private int ProcessCommandResponse(byte[] command)
        {
            if (command[0] != 0x5D)
            {
                //start of command not found exception
                OnKD8Error(new KD8ErrorEventArgs("From MainApp: Start of Command not found"));
            }

            if (!CheckChecksum(command))
            {
                //checksum not correct exception
                OnKD8Error(new KD8ErrorEventArgs("From MainApp: Checksum not correct"));
            }

            if (command[1] == 0x8E)//open door response
            {
                if ((command[4] & 32) == 32)
                {
                    int address = command[2] + (command[3] * 256);
                    int sequence = command[4] & 7;
                    //throw message with address and sequence number
                    OnKD8Response(new KD8ResponseEventArgs("Open Door Response: Address - " + address + " Sequence - " + sequence));
                }
                else
                {
                    //message is not from a KD8 board. Ignore
                }
            }
            else if (command[1] == 0x8A)//doors open/closed status response
            {
                int address = command[2];
                int sequence = command[4] & 7;

                int datalocation = address % 16; //get the location within the two data bytes of the bit you want to test.
                int[] data = {};

                for (int i=0; i < 16; i++)
                {
                    if (i < 8)
                        data[i] = command[6] & (byte)(Math.Pow(2, i));
                    else
                        data[i] = command[7] & (byte)(Math.Pow(2, i-8));
                }

                return data[datalocation];
            }
            else if (command[1] == 0x89)//error
            {
                if ((command[4] & 32) == 32)
                {
                    int address = command[2] + (command[3] * 256);
                    int sequence = command[4] & 7;

                    #region errors
                    if ((command[5] & 1) == 1)
                    {
                        //Tx Queue Error
                        OnKD8Error(new KD8ErrorEventArgs("Tx Queue Error: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 2) == 2)
                    {
                        //stack overflow
                        OnKD8Error(new KD8ErrorEventArgs("Stack Overflow: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 4) == 4)
                    {
                        //stack underflow
                        OnKD8Error(new KD8ErrorEventArgs("Stack Underflow: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 8) == 8)
                    {
                        //Rx Queue Reset
                        OnKD8Error(new KD8ErrorEventArgs("Rx Queue Reset: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 16) == 16)
                    {
                        //EEPROM writes not enabled
                        OnKD8Error(new KD8ErrorEventArgs("EEPROM writes not enabled: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 32) == 32)
                    {
                        //Inc_EEPROM_Word_Overflow
                        OnKD8Error(new KD8ErrorEventArgs("Inc_EEPROM_Word_Overflow: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[5] & 128) == 128)
                    {
                        //Queue Over-write problem, queue reset
                        OnKD8Error(new KD8ErrorEventArgs("Queue Over-write problem, queue reset: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[7] & 1) == 1)
                    {
                        //checksum error
                        OnKD8Error(new KD8ErrorEventArgs("From KD8: Checksum Error : Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[7] & 2) == 2)
                    {
                        //Invalid Opcode
                        OnKD8Error(new KD8ErrorEventArgs("From KD8: Invalid Opcode : Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[7] & 4) == 4)
                    {
                        //Opcode not implemented
                        OnKD8Error(new KD8ErrorEventArgs("Opcode not implemented: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[7] & 64) == 64)
                    {
                        //Error_Do_EEPROM_Action_WRERR
                        OnKD8Error(new KD8ErrorEventArgs("Error_Do_EEPROM_Action_WRERR: Address - " + address + " Seq - " + sequence));
                    }
                    if ((command[7] & 128) == 128)
                    {
                        //Error_Do_EEPROM_Action_BadOpCode/QueCnt
                        OnKD8Error(new KD8ErrorEventArgs("Error_Do_EEPROM_Action_BadOpCode/QueCnt: Address - " + address + " Seq - " + sequence));
                    }
                    #endregion
                }
                else
                {
                    //message is not from a KD8 board. Ignore
                }
            }
            else
            {
                //KD8 command opcode not recognized
                OnKD8Error(new KD8ErrorEventArgs("KD8 command opcode not recognized"));
            }
            return 0;
        }

        /// <summary>
        /// Create the checksum for the message packet given
        /// </summary>
        /// <param name="input">message packet</param>
        /// <returns></returns>
        private int CreateChecksum(byte[] input)
        {
            int total=0;
            foreach (byte b in input)
                total += b;
            return total;
        }

        /// <summary>
        /// Check the validity of the checksum received from a KD8 board.
        /// </summary>
        /// <param name="command">response message packet</param>
        /// <returns></returns>
        private bool CheckChecksum(byte[] command)
        {
            if (command[8] == ((command[1] + command[2] + command[3] + command[4] + command[5] + command[6] + command[7])%256))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Increment the communication sequence number
        /// </summary>
        private void IncSequenceNum()
        {
            SequenceNum++;
            SequenceNum = SequenceNum % 8;
        }

        public void closePort()
        {
            CommandPort.Close();
        }

        /// <summary>
        /// Handles valid responses from the KD8 board controller. Error messages from the KD8 are considered 'valid messages.'
        /// Other errors are not (such as an incomplete command).
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnKD8Response(KD8ResponseEventArgs e)
        {
            EventHandler<KD8ResponseEventArgs> handler = KD8Response;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handles errors which are cause by responses from the KD8 boards not being valid or any other reason which is not a specific error message
        /// from the KD8 board.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnKD8Error(KD8ErrorEventArgs e)
        {
            EventHandler<KD8ErrorEventArgs> handler = KD8Error;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class KD8ErrorEventArgs : System.EventArgs
    {
        public string Message;

        public KD8ErrorEventArgs(string message)
        {
            Message = message;
        }
    }

    public class KD8ResponseEventArgs : System.EventArgs
    {
        public string Message;

        public KD8ResponseEventArgs(string message)
        {
            Message = message;
        }
    }    
}
