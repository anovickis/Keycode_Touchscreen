using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// copyToUSB - a class for coping all transactions files to a USB memory dongle
    /// </summary>
    class copyToUSB
    {
        private string USB_drivePath;
        private string USB_root;
        private string TRANSACTIONS_SOURCE_DIR = "Transactions";    //a relative directory
        private string TRANSACTION_SEARCH_PATTERN = "*transaction.csv"; //search pattern for matching all files to copy

        /// <summary>
        ///  constructor - takes the root of the USB drive
        /// </summary>
        /// <param name="USB_Location"></param>
        public copyToUSB(string USB_Location)
        {
            USB_root = USB_Location;
            USB_drivePath = Path.Combine(USB_Location,"Safepak transactions copied at " + DateTime.Now.ToString());
        }

        /// <summary>
        /// CopyTransactionsToUSB -does the copy all transaction files found- true if files copied - will overwrite
        /// </summary>
        /// <returns></returns>
        public bool CopyTransactionsToUSB()
        {
            try
            {
                if (!Directory.Exists(USB_drivePath))
                {
                    Directory.CreateDirectory(USB_drivePath);
                }
                String[] sourceFiles = Directory.GetFiles(TRANSACTIONS_SOURCE_DIR, TRANSACTION_SEARCH_PATTERN);
                foreach (String s in sourceFiles)
                {
                    string fileName = s.Substring(TRANSACTIONS_SOURCE_DIR.Length + 1);

                    File.Copy(s,(Path.Combine(USB_drivePath,fileName)), true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Program.logEvent("copyToUSB:CopyTransactionsToUSB - exception - " + ex.Message);
                Program.logEvent("Unable to copy files to USB memory stick");
                return false;
            }
        }
        /// <summary>
        /// USB_Present - tests to see if usb is present
        /// </summary>
        /// <returns></returns>
        public bool USB_Present()
        {
            try
            {
                return Directory.Exists(USB_root);
            }
            catch (Exception ex)
            {
                Program.logEvent("copyToUSB:USB_Present - exception - " + ex.Message);
                Program.logEvent("USB detection error");
                return false;
            }
        }
    }
}
