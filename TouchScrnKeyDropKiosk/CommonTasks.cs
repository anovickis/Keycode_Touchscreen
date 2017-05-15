using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XmlConfig;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// CommonTasks - a class of static functions used in multiple classes
    /// </summary>
    static class CommonTasks
    {
       
        /// <summary>
        /// GetLocationFromKeypad - use the keypad to have user enter a number for cabinet keybox
        /// </summary>
        /// <returns></returns>
         public static string GetLocationFromKeypad()
        {

            KeyPadForm keypad = new KeyPadForm(LanguageTranslation.ENTER_BOX_NUMBER, true, Program.LOCATION_SIZE, true, false);

            keypad.ShowDialog();

            if (!keypad.bOK)
            {
                return "";  //was a cancel
            }

            return keypad.Result;

        }
        
        /// <summary>
        /// ValidBoxNumber - checks the list of box number to see if number passed in as string is in list
        /// </summary>
        /// <param name="BoxNumberAsString"></param>
        /// <returns></returns>
        public static bool ValidBoxNumber(string BoxNumberAsString)
        {
            try
            {
                return Program.passwordMgr.IsValidBoxNumber(BoxNumberAsString);
            }
            catch (Exception ex)
            {

                Program.logDebug("Entry exception -ValidBoxNumber  -" + BoxNumberAsString);
                Program.logError("Entry exception -ValidBoxNumber - " + ex.Message);

                return false;
            }
        }

        /// <summary>
        /// FindLocationNumber - retrieves the lock location number corresponding to the given box number
        ///                     returns - 1 if not found
        /// </summary>
        /// <param name="BoxNumberAsString"></param>
        /// <returns></returns>
        public static int FindLocationNumber(string BoxNumberAsString)
        {
            try
            {
                return Program.passwordMgr.LocationForBoxNumber(BoxNumberAsString);
            }
            catch (Exception ex)
            {
                Program.logDebug("Entry exception -FindLocationNumber  -" + BoxNumberAsString);
                Program.logError("Entry exception -FindLocationNumber - " + ex.Message);

                return -1;
            }
        }

        /// <summary>
        /// FindBoxNumber - given a location number return box number or -1 if not found
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static int FindBoxNumber(int location)
        {
            try
            {
                return Program.passwordMgr.BoxNumberForLocation(location);
            }
            catch (Exception ex)
            {
                Program.logDebug("Entry exception -FindBoxNumber  -" + location.ToString());
                Program.logError("Entry exception -FindBoxNumber - " + ex.Message);

                return -1;
            }
        }

        /// <summary>
        ///  dropKey -  sends message to Relay Control Board to drop a key at the given location
        /// </summary>
        /// <param name="loc"></param>
        public static void OpenKeyBox(int loc, bool takereturn)
        {
            Program.blank.Visible = true;
            PortManager port = PortManager.GetInstance();

            port.OpenAndCloseRelay(loc);

            Program.blank.Visible = false;
            if (takereturn)
                Program.ShowErrorMessage(LanguageTranslation.RETURN_KEY_CLOSE_DOOR, 3000);
            else
                Program.ShowErrorMessage(LanguageTranslation.TAKE_KEY_CLOSE_DOOR, 3000);

            Program.logEvent("ProcessEntry - OpenKeyBox location - " + loc.ToString());
        }
              
        public static void WriteValueToConfigurationFile(string node, string key, string value)
        {
            try
            {
                if ((key.Length == 0) || (value.Length == 0))
                {
                    return;
                }
                //make backup first
                File.Copy("KD20_config.xml", "KD20_config.xml.bak", true); //allows file to be overwritten

                XmlConfigDoc xml = new XmlConfigDoc("KD20_config.xml");

                XmlConfigNode root = xml.GetNode("root");
                XmlConfigNode writenode = root.GetNode(node);

                writenode.SetValue(key, value);
                xml.xmlDoc.Save("KD20_config.xml");
            }
            catch (Exception ex)
            {
                if (File.Exists("KD20_config.xml.bak"))
                {
                    File.Copy("KD20_config.xml.bak", "KD20_config.xml", true);
                }
                Program.logEvent("Error in writing values to config file" + ex.Message);
            }
        }
    }
}
