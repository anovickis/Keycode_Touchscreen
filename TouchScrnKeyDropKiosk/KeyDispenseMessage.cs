using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// KeyDispenseMessage - a encapsulation of the message sent out port to the relay control board
    ///                    - this class will throw exceptions
    ///                    - these messages are designed only for use with the RAJ 60 key control board.
    /// </summary>
    public class KeyDispenseMessage
    {
         private const int MESSAGE_SIZE = 8;
         private static byte[] keylocations;
         private static byte[] keyCheckSums;
      
         // look up arrays
        
//const char keyLocs[20]={'\xC0','\xC1','\xC2','\xC3','\xC4','\xC5','\xC6','\xC7','\xC8','\xC9','\xD0','\xD1','\xD2','\xD3','\xD4','\xD5','\xD6','\xD7','\xD8','\xD9'};
//const char keyCkSum[22]={'\x78','\x79','\x7A','\x7B','\x7C','\x7D','\x7E','\x7F','\x80','\x81','\x82','\x88','\x89','\x8A','\x8B','\x8C','\x8D','\x8E','\x8F','\x90','\x91','\x92'};

        private static byte[] keylocations20 = new byte[20]{ 0xC0,0xC1,0xC2,0xC3,0xC4,
                                                           0xC5,0xC6,0xC7,0xC8,0xC9,
                                                           0xD0,0xD1,0xD2,0xD3,0xD4,
                                                           0xD5,0xD6,0xD7,0xD8,0xD9};

        private static byte[] keylocations60 = new byte[60]{ 0x00,0x01,0x02,0x03,0x04, //Added 7/6/11 to allow for
                                                             0x05,0x06,0x07,0x08,0x09, //60 key dispenser functionality
                                                             0x0A,0x0B,0x0C,0x0D,0x0E, //These location values are only for use
                                                             0x10,0x11,0x12,0x13,0x14, //with the RAJ 60 key board (which is also 
                                                             0x15,0x16,0x17,0x18,0x19, //used for the 20 key board)
                                                             0x1A,0x1B,0x1C,0x1D,0x1E,
                                                             0x20,0x21,0x22,0x23,0x24,
                                                             0x25,0x26,0x27,0x28,0x29,
                                                             0x2A,0x2B,0x2C,0x2D,0x2E,
                                                             0x30,0x31,0x32,0x33,0x34,
                                                             0x35,0x36,0x37,0x38,0x39,
                                                             0x3A,0x3B,0x3C,0x3D,0x3E};

        private static byte[] keyChecSums20 = new byte[22]{ 0xA3,0xA4,0xA5,0xA6,0xA7,
                                                          0xA8,0xA9,0xAA,0xAB,0xAC,0xAD,
                                                          0xB3,0xB4,0xB5,0xB6,0xB7,
                                                          0xB8,0xB9,0xBA,0xBB,0xBC,0xBD};

        private static byte[] keyChecSums60 = new byte[64]{ 0xE3,0xE4,0xE5,0xE6,0xE7, //Added 7/6/11 to allow for
                                                            0xE8,0xE9,0xEA,0xEB,0xEC, //60 key dispenser functionality
                                                            0xED,0xEE,0xEF,0xF0,0xF1,0xF2, //These location values are only for use
                                                            0xF3,0xF4,0xF5,0xF6,0xF7,      //with the RAJ 60 key board (which is also 
                                                            0xF8,0xF9,0xFA,0xFB,0xFC,      //used for the 20 key board) 
                                                            0xFD,0xFE,0xFF,0x00,0x01,0x02,
                                                            0x03,0x04,0x05,0x06,0x07,
                                                            0x08,0x09,0x0A,0x0B,0x0C,
                                                            0x0D,0x0E,0x0F,0x10,0x11,0x12,
                                                            0x13,0x14,0x15,0x16,0x17,
                                                            0x18,0x19,0x1A,0x1B,0x1C,
                                                            0x1D,0x1E,0x1F,0x20,0x21,0x22};                                                
                                                          

         private byte[] message;  //this is the command message
         /// <summary>
         ///  commandBytes - return the byte array that is the command message
         /// </summary>
         public byte[] commandBytes  
         {
             get
             {
                 return message;
             }
         }
         /// <summary>
         /// commandSize - the number of bytes in the command message
         /// </summary>
         public int commandSize
         {
             get
             {
                 return message.Length;
             }
         }

         /// <summary>
         ///  MessageType - only two commands: Open or Close the relay
         /// </summary>
         public enum MessageType
         {
            OPEN_RELAY,
            CLOSE_RELAY   
         }
         /// <summary>
         ///   KeyDispenseMessage - create a command message to either open or close relay
         /// </summary>
         /// <param name="open_close"></param>
         /// <param name="location"></param>
         public KeyDispenseMessage(MessageType open_close, int location, byte seqnum)
         {
             try
             {
                 if (Program.SIXTY_KEY_DISPENSER)
                 {
                     keylocations = keylocations60;
                     keyCheckSums = keyChecSums60;
                 }
                 else
                 {
                     keylocations = keylocations20;
                     keyCheckSums = keyChecSums20;
                 }

                 if (location > keylocations.Length) //Changed 7/6/11
                 {
                     throw new Exception("location is out side range");
                 }

                 location--; //location offset into location array

                 byte loc = locationByte(location);

                 byte openClose = OpenOrClose(open_close);
                 byte chksum = CheckSum(location, open_close, seqnum); //Changed 7/6/11

                 ///  message structure
                 ///  b0 - FF - start of msg
                 ///  b1 - E2  -address - constant here
                 ///  b2 - location byte from look up table
                 ///  b3 - 01 -  NA
                 ///  b4 - D5 - sequence number - does not change
                 ///  b5 - 01  to open ---- 02 to close relay 
                 ///  b6 - 00 - NA
                 ///  b7 - checksum from a look up table
                 message = new byte[MESSAGE_SIZE] { 0xFF,
                                                0xE2,
                                                loc,         //location
                                                0x01,
                                                seqnum,
                                                openClose,   //open or Close relay command
                                                0x00,
                                                chksum};     //checksum
             }
             catch (Exception ex)
             {
                 string message = "KeyDispenseMessage exception at location: " + location.ToString() + " " + ex ;
                 Program.logError(message);
                 throw new Exception(message);
             }
         }

        /// <summary>
         ///  locationByte - uses the key locations look up array to find machine address for key hook location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
         private byte locationByte(int loc)
         {
             return keylocations[loc]; //Changed 7/6/11
         }

         /// <summary>
         ///  OpenOrClose  - returns machine code for an open or close command
         /// </summary>
         /// <param name="open_close"></param>
         /// <returns></returns>
         private byte OpenOrClose(MessageType open_close)
         {
             if (open_close == MessageType.OPEN_RELAY)
             {
                 return 1;
             }
             else if (open_close == MessageType.CLOSE_RELAY)
             {
                 return 2;
             }
             else
             {
                 throw new Exception(" unexpected message type: " + open_close.ToString());
             }
         }

         /// <summary>
         ///  CheckSum - uses the look up array to get the checksum 
         /// </summary>
         /// <param name="loc"></param>
         /// <param name="open_close"></param>
         /// <returns></returns>
         private byte CheckSum(int loc, MessageType open_close, byte seqnum)  //Added 7/6/11
         {
             try
             {
                 if (open_close == MessageType.OPEN_RELAY)
                 {

                     if (((Program.SIXTY_KEY_DISPENSER) && (loc < 15)) || ((!Program.SIXTY_KEY_DISPENSER) && (loc < 10)))
                     {
                         return (byte)((keyChecSums60[loc] + seqnum) % 256);
                     }
                     else if (((Program.SIXTY_KEY_DISPENSER) && (loc >= 15 && loc < 30)) || (!Program.SIXTY_KEY_DISPENSER))
                     {
                         return (byte)((keyChecSums60[loc + 1] + seqnum) % 256);
                     }

                     else if (loc >= 30 && loc < 45)
                     {
                         return (byte)((keyChecSums60[loc + 2] + seqnum) % 256);
                     }
                     else
                     {
                         return (byte)((keyChecSums60[loc + 3] + seqnum) % 256);
                     }

                 }
                 else if (open_close == MessageType.CLOSE_RELAY)
                 {
                     if (((Program.SIXTY_KEY_DISPENSER) && (loc < 15)) || ((!Program.SIXTY_KEY_DISPENSER) && (loc < 10)))
                     {
                         return (byte)((keyChecSums60[loc + 1] + seqnum) % 256);
                     }
                     else if (((Program.SIXTY_KEY_DISPENSER) && (loc >= 15 && loc < 30)) || (!Program.SIXTY_KEY_DISPENSER))
                     {
                         return (byte)((keyChecSums60[loc + 2] + seqnum) % 256);
                     }
                     else if (loc >= 30 && loc < 45)
                     {
                         return (byte)((keyChecSums60[loc + 3] + seqnum) % 256);
                     }
                     else
                     {
                         return (byte)((keyChecSums60[loc + 4] + seqnum) % 256);
                     }
                 }
                 else
                 {
                     throw new Exception(" unexpected message type: " + open_close.ToString());
                 }
             }
             catch (Exception ex)
             {
                 string message = "Check sum exception at location: " + loc.ToString() + " " + ex;
                 Program.logError(message);
                 throw new Exception(message);
             }

         }
        
         public override string ToString()
         {

             StringBuilder sb = new StringBuilder();

             foreach (byte b in commandBytes)
             {
                 sb.AppendFormat(" {0:x2}",b);
             }

             return sb.ToString();
         }
    }
}
//////////////////////////////////////////////////////////////////////////
////
////  the "c" code from the rabbit board solution - leave here for history
////
///////////////////////////////////////////////////////////////////////

///*** Beginheader */
//const char keyLocs[20]={'\xC0','\xC1','\xC2','\xC3','\xC4','\xC5','\xC6','\xC7','\xC8','\xC9','\xD0','\xD1','\xD2','\xD3','\xD4','\xD5','\xD6','\xD7','\xD8','\xD9'};
//const char keyCkSum[22]={'\x78','\x79','\x7A','\x7B','\x7C','\x7D','\x7E','\x7F','\x80','\x81','\x82','\x88','\x89','\x8A','\x8B','\x8C','\x8D','\x8E','\x8F','\x90','\x91','\x92'};
///*** endheader */
///
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

