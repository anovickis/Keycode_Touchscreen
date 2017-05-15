using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// KeyPassword - is XML data class for a given keypassword - holds all information for a given location
    /// </summary>
    public enum accessType
    {
        SIMPLE = 1,
        CARD = 2,
        BOTH = 3
    }

    public enum accessTimeType
    {
        ALWAYS_ON = 1,
        TIME_PERIOD = 2,
        LIMITED_USE = 3,
        TIME_PERIOD_LIMITED_USE = 4
    }

    /// <summary>
    /// KeyPassword - individual password location -POD serializable
    ///               can throw exceptions
    /// </summary>
    public class KeyPassword
    {
        private List<string> Passwords;

        [XmlElementAttribute()]
        public int  loc { get; set; }            // key location - unique ID

        [XmlArrayItem("password", typeof(string))]
        public List<string> PasswordList        //xml interface for serialization
        {
            get { return Passwords; }       // password number as string"
            set { Passwords = value; }
        }

        [XmlElementAttribute()]
        public string cardNumber { get; set; }     // last 4 digits of credit card as string "0000" if not used

        [XmlElementAttribute()]
        public int accessIntType { get; set; }   // access type as int - cast to use as enum

        [XmlElementAttribute()]
        public string genericData { get; set; }   // this string field can be filled with whatever data should be associated with a keybox

        [XmlElementAttribute()]
        public int accessTimeIntType { get; set; } //time of access which is allowed (i.e. one-time use, always allowed, time frame allowed

        [XmlElementAttribute()]
        public int LimitedUseCount { get; set; }   // If Multi_TIME_USE selected, then

        [XmlElementAttribute()]
        public DateTime startAccessTimeframe { get; set; }

        [XmlElementAttribute()]
        public DateTime endAccessTimeframe { get; set; }

         /// <summary>
         ///  KeyPassword - no parameter constructor needed for streaming object
         /// </summary>
        public KeyPassword()
        {
            startAccessTimeframe = DateTime.Today;
            endAccessTimeframe = DateTime.Today;
        }

        public KeyPassword(string location, string accesscode, string cardnumber, string nonglobalaccesstype, string data, string timedAccessType, int usecount, string startTime, string endTime)
        {
            loc = int.Parse(location); PasswordList = new List<string>(); cardNumber = cardnumber; accessIntType = int.Parse(nonglobalaccesstype);
            accessTimeIntType = int.Parse(timedAccessType); LimitedUseCount = usecount; genericData = data;

            startAccessTimeframe = DateTime.Parse(startTime);
            endAccessTimeframe = DateTime.Parse(endTime);
        }

         public KeyPassword(string location, string accesscode)
         {
             loc = int.Parse(location); PasswordList = new List<string>(); PasswordList.Add(accesscode); cardNumber = "0000"; accessIntType = 1; genericData = "";
             accessTimeIntType = 0; LimitedUseCount = 0;
             startAccessTimeframe = DateTime.Today;
             endAccessTimeframe = DateTime.Today;
         }

         public KeyPassword(string location, string accesscode, string cardnumber, string nonglobalaccesstype, string data)
         {
             loc = int.Parse(location); PasswordList = new List<string>(); cardNumber = cardnumber; accessIntType = int.Parse(nonglobalaccesstype);
             accessTimeIntType = 0; LimitedUseCount = 0; genericData = data;

             startAccessTimeframe = DateTime.Today;
             endAccessTimeframe = DateTime.Today;
         }
    }  // end class

    
    /// <summary>
    /// KeyPassWordList - a list of KeyPassword Object; - serializable
    ///                  searchs for key passwords are implemented by iterating through the list
    ///                  looking for a match. Due to small size of the list this is deemed sufficiently
    ///                  efficient data lookup.
    /// </summary>
    public class KeyPassWordList
    {
        private List<KeyPassword> keypasswords;
        
        [XmlArrayItem("key_password", typeof(KeyPassword))]
        public List<KeyPassword> keyPasswordList        //xml interface for serialization
        {
            get { return keypasswords; }
            set { keypasswords = value; }    
        }

        /// <summary>
        /// Count - the number of passwords objects in the list
        /// </summary>
        public int Count
        {
            get
            {
                return keypasswords.Count;
            }
        }

        /// <summary>
        ///  KeyPassWordList - default no paramater constructor needed to stream in data
        /// </summary>
        public KeyPassWordList()
        {
            keypasswords = new List<KeyPassword>();
        }

        /// <summary>
        /// AddKeyPassword - add a new key password location object
        ///                - not used by application - only test harness
        /// </summary>
        /// <param name="keypassword"></param>
        public void AddKeyPassword(KeyPassword keypassword)
        {
            keypasswords.Add(keypassword);
        }

        /// <summary>
        /// Clears out all of the passwords for each key without saving them to file
        /// </summary>
        public void TemporaryClearKeyPasswords()
        {
            string[] admin = new string[keypasswords[0].PasswordList.Count];
            keypasswords[0].PasswordList.CopyTo(admin);
            foreach (KeyPassword kp in keypasswords)
            {                
                kp.PasswordList.Clear();                
            }
            keypasswords[0].PasswordList = admin.ToList<string>();
        }

        public void AddPasswordToKey(int loc, string password)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == loc)
                {
                    kp.PasswordList.Add(password);
                }
            }
        }

        /// <summary>
        /// SerializeToXmlFile - stream the key pass word list to the XML file
        /// </summary>
        /// <param name="xmlfile"></param>
        public void SerializeToXmlFile(string xmlfile)
        {
            TextWriter tr = new StreamWriter(xmlfile);
            XmlSerializer sr = new XmlSerializer(typeof(KeyPassWordList));

            sr.Serialize(tr, this);

            tr.Close();

        }

        /// <summary>
        /// FindPasswordByLoc - finds and returns the password at the given location
        ///           - returns null if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public List<string> FindPasswordByLoc(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.PasswordList;
                }
            }
            return null;
        }

        /// <summary>
        /// FindAccessByLocation  - finds and returns the access type at a given location
        ///                       - return SIMPLE if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public accessType FindAccessByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return (accessType)kp.accessIntType;
                }
            }
            return accessType.SIMPLE;
        }

        /// <summary>
        /// FindCardNumByLocation - finds and returns the card number (last 4 digits) at a given
        ///                         location
        ///                         -returns empty string if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public string FindCardNumByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.cardNumber;
                }
            }
            return "";
        }

        /// <summary>
        /// FindLocationByCardNum - finds and returns the first location for the given card number
        ///                         -returns -1 if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public int FindLocationByCardNum(string cardnum)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.cardNumber == cardnum)
                {
                    return kp.loc;
                }
            }
            return -1;
        }

        /// <summary>
        /// FindNumberofUsesByLocation - finds and returns the number of remaining uses for a location
        ///                         -returns -1 if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public int FindLimitedUsesByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.LimitedUseCount;
                }
            }
            return -1;
        }

        /// <summary>
        /// FindGenericDataByLocation - finds and returns the genericData field at a given
        ///                         location
        ///                         -returns empty string if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public string FindGenericDataByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.genericData;
                }
            }
            return "";
        }

        /// <summary>
        /// FindStartTimeByLocation - finds and returns the start time which the box is available at a given
        ///                           location
        ///                         - returns Datetime.minValue if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public DateTime FindStartTimeByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.startAccessTimeframe;
                }
            }
            return new DateTime(1970, 1, 1);
        }

        /// <summary>
        /// FindEndTimeByLocation - finds and returns the end time which the box is available at a given
        ///                           location
        ///                         - returns Datetime.minValue if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public DateTime FindEndTimeByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return kp.endAccessTimeframe;
                }
            }
            return new DateTime(1970, 1, 1);
        }

        /// <summary>
        /// FindTimeTypeByLocation  - finds and returns the access time type at a given location
        ///                       - return ALWAYS ON if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public accessTimeType FindTimeTypeByLocation(int lc)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    return (accessTimeType)kp.accessTimeIntType;
                }
            }
            return accessTimeType.ALWAYS_ON;
        }

        /// <summary>
        /// FindByString - finds and returns the location for a given password
        ///              - returns -1 if not found
        /// </summary>
        /// <param name="pswd"></param>
        /// <returns></returns>
        public int FindByString(string pswd)
        {
            foreach (KeyPassword kp in keypasswords)
            {
                foreach (string p in kp.PasswordList)
                {
                    if (String.Equals(p, pswd))
                    {
                        return kp.loc;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// ChangeCardNumber - change the four digit card number for this location
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ChangeCardNumber(int lc, string CardNo)
        {

            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logDebug("ChangeCardNumber - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.cardNumber = CardNo;
                return true;
            }
        }

        /// <summary>
        /// ChangeGenericData - change the genericdata field for this location
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ChangeGenericData(int lc, string Data)
        {

            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logDebug("ChangeGenericData - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.genericData = Data;
                return true;
            }
        }

        /// <summary>
        /// ChangeNumberofUses - change the number of remaining uses for this box location
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ChangeLimitedNumberofUses(int lc, int uses)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("ChangeNumberofUses - no location found " + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.LimitedUseCount = uses;
                return true;
            }
        }

        /// <summary>
        /// ChangeStartTime - change the start time a box is available for this location
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ChangeStartTime(int lc, DateTime Starttime)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("ChangeStartTime - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.startAccessTimeframe = Starttime;
                return true;
            }
        }

        /// <summary>
        /// ChangeEndTime - change the end time a box is available for this location
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool ChangeEndTime(int lc, DateTime Endtime)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("ChangeEndTime - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.endAccessTimeframe = Endtime;
                return true;
            }
        }

        /// <summary>
        /// ChangeAccessType - replace the AccessType of the given location
        /// </summary>
        /// <param name="lc - location"></param>
        /// <param name="pswd- password"></param>
        /// <returns> true if password change is made</returns>
        public bool ChangeAccessType(int lc, accessType type)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("KeyPassword - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.accessIntType = (int)type;
                return true;
            }
        }

        /// <summary>
        /// ChangeTimeAccessType - replace the AccessTimeType of the given location
        /// </summary>
        /// <param name="lc - location"></param>
        /// <param name="pswd- password"></param>
        /// <returns> true if password change is made</returns>
        public bool ChangeTimeAccessType(int lc, accessTimeType type)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("KeyPassword - no location found" + lc.ToString());
                return false;   ///location not found - what happened here
            }
            else
            {
                keyPasswrd.accessTimeIntType = (int)type;
                return true;
            }
        }

        /// <summary>
        /// ChangePassword - replace the password and the given location
        /// </summary>
        /// <param name="lc - location"></param>
        /// <param name="pswd- password"></param>
        /// <returns> true if password change is make</returns>
        public bool ChangePassword(int lc, string currentpswd, string newpswd)
        {
            KeyPassword keyPasswrd = null;
            foreach (KeyPassword kp in keypasswords)
            {
                if (kp.loc == lc)
                {
                    keyPasswrd = kp;
                    break;
                }
            }
            if (keyPasswrd == null)
            {
                Program.logEvent("KeyPassword - no location found" + lc.ToString());
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "KeyPassword - no location found" + lc.ToString(), "", 0);
                return false;   ///location not found - what happened here
            }
            else
            {
                foreach (string p in keyPasswrd.PasswordList)
                {
                    if (currentpswd == p)
                    {
                        keyPasswrd.PasswordList.Remove(p);
                        keyPasswrd.PasswordList.Add(newpswd);
                        return true;
                    }
                }
                Program.logEvent("KeyPassword - PasswordList - 'currentpswd' not found for this location.");
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "KeyPassword - PasswordList - 'currentpswd' not found for this location.", "", 0);
                return false;
            }
        }
    }
}

