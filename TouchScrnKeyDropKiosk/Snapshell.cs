using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace SnapShell_Driver_Lic
{
    /// <summary>
    /// Library class which give access to the Scapshell API libraries. 
    /// </summary>
    class Snapshell
    {
        SCANWLib.SLibExClass scanner = new SCANWLib.SLibExClass();
        SCANWLib.IdDataClass license = new SCANWLib.IdDataClass();
        SCANWEXLib.IdDataClass license_ex = new SCANWEXLib.IdDataClass();
        SCANWLib.CImageClass image = new SCANWLib.CImageClass();
        SCANWEXLib.CImageClass image_ex = new SCANWEXLib.CImageClass();
        SCANWLib.CBarCodeClass barcode = new SCANWLib.CBarCodeClass();

        private string imagebuffer;
        private Image LicenseImage;

        public event EventHandler<SnapshellMessageArgs> SnapshellMessage;

        public Snapshell()
        {            
            init();
        }

        private void init()
        {
            scanner.InitLibrary("4KQ2NFWAGPC2Z46V"); //This is the registration code given to our company and shouldn't expire.
            license.InitLibrary("4KQ2NFWAGPC2Z46V");
            image.InitLibrary("4KQ2NFWAGPC2Z46V");
            barcode.InitLibrary("4KQ2NFWAGPC2Z46V");            
        }
        
        /// <summary>
        /// Scan the drivers license and create a picture file in the current working directory.
        /// </summary>
        /// <param name="filename"></param>
        public void ScanLicense(string filename, int resolution)
        {
            if (scanner.IsScannerValid == 1)
            {
                // Calibration
                if (scanner.IsNeedCalibration == 1)
                {
                    scanner.CalibrateScanner();
                    OnSnapshellMessage(new SnapshellMessageArgs("Please load card into the tray to scan"));
                }

                scanner.Resolution = (short)resolution;
                //scanner.ScannerColorScheme = 
                scanner.ScanHeight = -1;
                scanner.ScanWidth = -1;

                int result;
                // Scan to file
                if (true) //change to a variable
                {
                    result = scanner.ScanToFileEx(filename);
                }

                if (result < 0) //If there is an error
                {
                    OnSnapshellMessage(new SnapshellMessageArgs("Scanner class error " + result.ToString()/*CSSNCore.GetScannerClassError(result)*/));
                    return;
                }

                else
                {
                    result = image_ex.GetImageBufferData("JPG", out imagebuffer);
                    if (result < 0) //If there is an error
                    {
                        OnSnapshellMessage(new SnapshellMessageArgs("Image class error " + result.ToString()/*CSSNCore.GetImageClassError(result)*/));
                        return;
                    }
                    else
                    {
                        LicenseImage = GetImageFromBuffer(imagebuffer);
                    }
                }
            }
            else
            {
                OnSnapshellMessage(new SnapshellMessageArgs("Scanner not valid. Please check the connection to the scanner."));
            }
        }

        /// <summary>
        /// This method retrieves the ID image from the image buffer.
        /// </summary>
        /// <param name="imagebuffer"></param>
        /// <returns></returns>
        private Image GetImageFromBuffer(string imagebuffer)
        {
            byte[] imgBuffer;
            // If image buffer found process buffer
            if (imagebuffer != null)
            {
                // Change image buffer to byte[] to be  able to save in DataBase.
                char[] tempBuffer = imagebuffer.ToCharArray();
                byte[] imageDataBuffer = new byte[tempBuffer.Length];
                for (int i = 0; i < tempBuffer.Length; i++)
                {
                    imageDataBuffer[i] = (byte)tempBuffer[i];
                }
                imgBuffer = imageDataBuffer;
                MemoryStream ms = new MemoryStream(imgBuffer);
                Image returnImage = Image.FromStream(ms);
                ms.Close();
                return returnImage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method processes the ID picture by extracting the necessary data from it and returning an object which encapsulates
        /// all of that data.
        /// </summary>
        /// <returns></returns>
        public SCANWLib.IdDataClass ProcessLicense()
        {
            int stateID = -1; // State not found yet
            license_ex.RegionSet(GetRegionIntValue("United States"));

            // Auto detect crops and rotates the internal image for better OCR results

            stateID = license.AutoDetectState(string.Empty);
            if (stateID < 0)
            {
                OnSnapshellMessage(new SnapshellMessageArgs("Snapshell StateID Data class error " + stateID.ToString()/*(CSSNCore.GetIdDataClassError(stateID)*/));
                return null;
            }

            // Extract data using state ID template
            int result = license.ProcState(string.Empty, stateID);
            if (result < 0)
            {
                OnSnapshellMessage(new SnapshellMessageArgs("Snapshell ID Data class error " + result.ToString()/*CSSNCore.GetIdDataClassError(result)*/));
                return null;
            }

            // Check process result
            if (result >= 0)
            {
                // Set data in properties
                license.RefreshData();
                return license;
            }
            return null;
        }

        public SCANWLib.CBarCodeClass ProcessLicenseBarcode(string LicenseBackSideFilename)
        {
            int result = barcode.ProcImage(LicenseBackSideFilename);
            if (result < 0)
            {
                OnSnapshellMessage(new SnapshellMessageArgs("Snapshell Barcode class error " + result.ToString()/*CSSNCore.GetIdDataClassError(result)*/));
                return null;
            }

            // Check process result
            if (result >= 0)
            {
                // Set data in properties
                barcode.RefreshData();
                return barcode;
            }
            return null;
        }

        private int GetRegionIntValue(string region)
        {
            switch (region)
            {
                case "United States":
                    return 0;
                case "Australia":
                    return 4;
                case "Asia":
                    return 5;
                case "Canada":
                    return 1;
                case "America":
                    return 2;
                case "Europe":
                    return 3;
                case "Africa":
                    return 7;
                default:
                    return -1;
            }
        }

        protected virtual void OnSnapshellMessage(SnapshellMessageArgs e)
        {
            EventHandler<SnapshellMessageArgs> handler = SnapshellMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class SnapshellMessageArgs : System.EventArgs
    {
        public string Message;

        public SnapshellMessageArgs(string message)
        {
            Message = message;
        }
    }
}
