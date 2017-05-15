using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using RAJ_KD8_Board;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This class handles the higher level communication with Allen Siefken's KD8 relay board. 
    /// </summary>
    class RAJ_KD8_Board_Port:SolenoidControlBoardPort
    {
        override protected SerialPort port { get; set; }
        RAJ_KD8_Board.RAJ_KD8_Board ControlBoard;

        public RAJ_KD8_Board_Port(string Portname)
        {
            try
            {
                //  19200, no parity, 
                port = new SerialPort(Portname, 19200, Parity.None);

                if (port == null)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port null");
                }
                port.Open();

                if (port.IsOpen == false)
                {
                    throw new Exception(" - Solenoid Control Board primary serial port failed to open");
                }

                ControlBoard = new RAJ_KD8_Board.RAJ_KD8_Board(port);
                ControlBoard.KD8Error += new EventHandler<KD8ErrorEventArgs>(ControlBoard_KD8Error);
                ControlBoard.KD8Response += new EventHandler<KD8ResponseEventArgs>(ControlBoard_KD8Response);
            }
            catch (Exception e)
            {
                StringBuilder message = new StringBuilder(Portname);
                message.Append(": ");
                message.Append(e.Message);

                throw new Exception(message.ToString());
            }
        }

        /// <summary>
        /// Listener for valid responses from the KD8 (KD8 errors which are sent by the controller board
        /// are considered valid messages).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ControlBoard_KD8Response(object sender, KD8ResponseEventArgs e)
        {
            Program.logEvent(e.Message);
        }

        /// <summary>
        /// Lisener for error messages which come from the KD8 driver class. These errors are not given by the controller board
        /// but rather are caused by bad communications and the like.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ControlBoard_KD8Error(object sender, KD8ErrorEventArgs e)
        {
            Program.logEvent(e.Message);
            Program.logError(e.Message);
        }

        /// <summary>
        /// ClosePort - close the KD8 serial port
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
                Program.logError("Error closing KD8 digital io port " + ex.Message);
            }
        }

        /// <summary>
        /// Sends the open relay command. The fourth parameter on the command ('offtime') is actually uneccessary because the relay is only doing
        /// one repetition. If there were multiple firings of the relay, the offtime would be the time between firings.
        /// </summary>
        /// <param name="location"></param>
        public override void OpenAndCloseRelay(int location)
        {
            ControlBoard.OpenRelay(location, 1, 500, 500);
        }
    }
}
