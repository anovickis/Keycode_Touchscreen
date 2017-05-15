using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{
    /// <summary>
    ///  PortManager - provides a singleton interface to the ports to the rest of application
    ///              use this interface to send messages to the Relay Control Board and the LCD display
    ///             
    /// </summary>

    class PortManager
    {
         private SolenoidControlBoardPort SCBPort;   // the relay control port


         private static bool initialized { get; set; }  //ports have been opened

          /// <summary>
          ///  singleton implementation
          /// </summary>
          /// 
         private static readonly PortManager portManger = new PortManager(); //static member
        
         static PortManager()    // the static constructor
         { 

             initialized = false;
         }

         private PortManager() {}  //no public constructor - as defined by singleton pattern

        /// <summary>
        ///  GetInstance - use this method to get a reference to this static object
        /// </summary>
        /// <returns></returns>
         public static PortManager GetInstance()
         {
             return portManger;
         }


        /// <summary>
        ///  InitializePorts - opens both ports or fails and throws exception
        ///             this must succeed for the application to work
        /// </summary>
        /// <param name="RCB_PortName"></param>
        /// 
         public void InitializePorts(string SCB_PortName)
         {
             try
             {
                // LCDPort = new LCD_Port(LCD_PortName);
                 if (Program.RELAY_CONTROL_BOARD_TYPE.ToUpper() == "WEEDER")
                     SCBPort = new WeederControlBoardPort(SCB_PortName);
                 else if (Program.RELAY_CONTROL_BOARD_TYPE.ToUpper() == "RAJ")
                     SCBPort = new RelayControlBoardPort(SCB_PortName);
                 else if (Program.RELAY_CONTROL_BOARD_TYPE.ToUpper() == "KD8")
                     SCBPort = new RAJ_KD8_Board_Port(SCB_PortName);
                 else
                     throw new ArgumentException("Config Value: RELAY_CONTROL_BOARD_TYPE is not valid");

                 initialized = true;
             }
             catch (Exception ex)
             {
                 throw new Exception("failure in opening ports - " + ex.Message);
             }
         }

        /// <summary>
        /// ClosePorts - closes both ports
        /// </summary>
         public void ClosePorts()
         {
             if (SCBPort != null)
             {
                 SCBPort.ClosePort();
             }
             
         }

         /// <summary>
         /// OpenAndCloseRelay - sends a message to Relay Control Board to open a relay and then close the relay
         ///                    the location here a based upon the logical location 1-20
         /// </summary>
         /// <param name="loc"></param>
         public void OpenAndCloseRelay(int loc)
         {
             try
             {
                SCBPort.OpenAndCloseRelay(loc);
             }
             catch(Exception ex)
             {
                 Program.logError("Error opening and closing relay - location: " + loc.ToString() + "\r\n" + ex.Message);
                 Program.ShowErrorMessage(LanguageTranslation.OPEN_RELAY_ERROR + loc.ToString(), 4000);
             }
         }
    }
}
