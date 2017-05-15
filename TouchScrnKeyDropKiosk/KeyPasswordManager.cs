using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace KeyCabinetKiosk
{
    /// <summary>
    ///  The KeyPasswordManager is the interface to the key location and password (key code) data
    ///         this class does check the password strings for rules like length and numbers
    ///         only.
    ///         this class will throw exceptions 
    /// </summary>
    public class KeyPasswordManager
    {
        public KeyPassWordList keyPassWordList{get; set;}
        private string xmlFileName;
        
        private int[] BoxNumbers;

        /// <summary>
        /// KeyPasswordManager - constructor - reads the key password file into a key password list 
        ///                       that is use for all key code and location look ups
        /// </summary>
        /// <param name="fileName"></param>
        public KeyPasswordManager(string fileName)
        {
            if(String.IsNullOrEmpty(fileName))
            {
                throw new Exception("bad file name for KeyPassword xml file");
            }
            xmlFileName = fileName;
            LoadFromFile(xmlFileName);

            BoxNumbers = new int[501];
            for (int i=0; i <= 500; i++)
            {
                BoxNumbers[i] = i;
            }
        }

        public int[] AllBoxLocations
        {
            get
            {
                return BoxNumbers;
            }
        }

        /// <summary>
        /// LoadFromFile- reads the "filename" xml file into a list structure - the keyPassWordList
        /// </summary>
        private bool LoadFromFile(string FileName)
        {
            //read in xml data from file
            TextReader tr = new StreamReader(FileName);
            try
            {
                keyPassWordList = new KeyPassWordList();
                               
                XmlSerializer sr = new XmlSerializer(typeof(KeyPassWordList));

                keyPassWordList = (KeyPassWordList)sr.Deserialize(tr);

                CheckDataValid();
                return true;
            }
            catch(Exception  ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                tr.Close();
            }
        }

        public void TemporaryClearKeyPasswords()
        {
            keyPassWordList.TemporaryClearKeyPasswords();
        }

        public void AddPasswordToKey(int location, string newpassword)
        {
            keyPassWordList.AddPasswordToKey(location, newpassword);
        }

        /// <summary>
        /// ReloadFromFile - reads in xml data file and re-creates the key password list.
        /// </summary>
        public bool ReloadFromFile(string filename)
        {
            return LoadFromFile(filename);
        }

        /// <summary>
        ///  SaveFile - writes the Key Password list to the xml file
        /// </summary>
        public void SaveFile()
        {
            keyPassWordList.SerializeToXmlFile(xmlFileName);
        }


        /// <summary>
        /// CheckDataValid - do basic checks on the data to insure that it is valid for operations
        ///                 required. Throw exception if not valid
        /// </summary>
        private void CheckDataValid()
        {
            //check to see that size of list matches configured Program
            if (keyPassWordList.Count != Program.NUMBER_RELAYS + 1)  //plus 1 for admin password in loc 0
            {
                throw new Exception("key password list is the wrong size-" + keyPassWordList.Count.ToString());
            }
          
            List<string> tempPasswrds = new List<string>();

            //now inspect each password - not null, correct length, is unique, and that all locations are present
            //can all 20 locations be found - 1 to number relays

            // now just check for correct length
            for (int i = 0; i < Program.NUMBER_RELAYS + 1; i++)
            {
                List<string> passwords = FindPassword(i);
                foreach (string p in passwords)
                {
                    if (String.IsNullOrEmpty(p)) //found
                    {
                        throw new Exception("password null or empty - location: " + i.ToString());
                    }
                    if (p.Length > Program.PASSWORD_SIZE)   //correct size
                    {
                        throw new Exception("password incorrect length - location: " + i.ToString() + " size: " + p.Length.ToString());
                    }
                }

                ////uses lamda expression
                //if (tempPasswrds.Exists(val => val == password))
                //{
                //    throw new Exception("password not unique - - location: " + i.ToString() + " password " + password);
                //}

                //tempPasswrds.Add(password); //to this temporary list
           
            }  
            // check to see that credit card numbers are all just 4 digits
            // check for correct length
            for (int i = 1; i < Program.NUMBER_RELAYS + 1; i++)   //no card number needed for admin  0 slot
            {
                string cardNumber = FindCardNumber(i);

                if (String.IsNullOrEmpty(cardNumber)) //found
                {
                    throw new Exception("card number null or empty - location: " + i.ToString());
                }
                if (cardNumber.Length > Program.NUMBER_CREDIT_CARD_DIGITS)   //correct size
                {
                    throw new Exception("card number incorrect length - location: " + i.ToString() + " size: " + cardNumber.Length.ToString());
                }
            } 
        }

        /// <summary>
        ///  FindLocationForCode will search list for a match with a password and returns the location
        ///      if not found returns a -1
        /// </summary>
        /// <param name="pswd"></param>
        /// <returns></returns>
        public int FindLocationForCode(string pswd)
        {
            return keyPassWordList.FindByString(pswd);
        }

        /// <summary>
        ///  FindLocationForCard will search list for a match with a cardnumber and returns the location
        ///      if not found returns a -1
        /// </summary>
        /// <param name="pswd"></param>
        /// <returns></returns>
        public int FindLocationForCard(string card)
        {
            return keyPassWordList.FindLocationByCardNum(card);
        }

        public string CreditCardInfo(int loc)
        {
            return keyPassWordList.FindCardNumByLocation(loc);

        }
        /// <summary>
        /// IsValidBoxNumber - checks to see if box number input is in the list of box numbers
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsValidBoxNumber(string location)
        {
            if(String.IsNullOrEmpty(location)) { return false; }

            int loc = LocationForBoxNumber(location);

            return (loc==-1) ? false : true;
        }

        /// <summary>
        /// LocationForBoxNumber - returns the lock location number associated with box number
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public int LocationForBoxNumber(string location)
        {
            if (String.IsNullOrEmpty(location)) { return -1; }
            int box = -1;
            if (!int.TryParse(location, out box))
            {
                return -1;
            }
            for (int i = 0; i < BoxNumbers.Length; i++)
            {
                if (BoxNumbers[i] == box)
                {
                    return i;
                }
            }
            return -1;
        }

        public int BoxNumberForLocation(int location)
        {
            if (location > BoxNumbers.Length - 1) { return -1; } //outside of array
            return BoxNumbers[location];
        }



        /// <summary>
        /// LocalAccessType - uses the lock location number to lookup the access type for that location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public accessType LocalAccessType(int loc)
        {
            return keyPassWordList.FindAccessByLocation(loc);
        }


        /// <summary>
        ///         DO NOT USE IF PASSWORD IS NOT UNIQUE - AS WITH KEY CABINET
        /// 
        ///  FindPassword  will search list for a match with a location and returns the password
        ///     if location is not found a null object is returned
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public List<string> FindPassword(int loc)
        {
            return keyPassWordList.FindPasswordByLoc(loc);
        }

        /// <summary>
        /// FindCardNumber - Finds the 4 digit card number for a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public String FindCardNumber(int loc)
        {
            return keyPassWordList.FindCardNumByLocation(loc);
        }

        /// <summary>
        /// FindGenericData - Finds the genericData field for a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public String FindGenericData(int loc)
        {
            return keyPassWordList.FindGenericDataByLocation(loc);
        }

        /// <summary>
        /// FindNumberofUses - Finds the remaining number of uses for a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public int FindLimitedNumberofUses(int loc)
        {
            return keyPassWordList.FindLimitedUsesByLocation(loc);
        }

        /// <summary>
        /// FindStartTime - finds and returns the start time which the box is available at a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public DateTime FindStartTime(int loc)
        {
            return keyPassWordList.FindStartTimeByLocation(loc);
        }

        /// <summary>
        /// FindEndTime - finds and returns the end time which the box is available at a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public DateTime FindEndTime(int loc)
        {
            return keyPassWordList.FindEndTimeByLocation(loc);
        }

        /// <summary>
        /// FindAccessTimeType - finds and returns the accessTimeType which says what type of time access the box has at a given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public accessTimeType FindAccessTimeType(int loc)
        {
            return keyPassWordList.FindTimeTypeByLocation(loc);
        }

        /// <summary>
        ///  isUnique - search list for password match if one is found password is not unique
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool isUnique(string password)
        {
            return (-1 == FindLocationForCode(password)); //if FindLocation does not find it returns -1 and password is unique
        }


        /// <summary>
        ///  SetPassword - this will find the location based upon location. If not found returns false
        ///             then changes to the new password.
        ///             
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool SetPassword(int loc, string Currentpassword, string NewPassword)
        {
            ////ensure that this check has been made 
            //if(!isUnique(password))
            //{
            //    throw new Exception("password not unique - - location: " + loc.ToString() + " password " + password);
            //}
            return keyPassWordList.ChangePassword(loc, Currentpassword, NewPassword);
        }

        public bool SetCardNumber(int loc, string cardNumber)
        {

            return keyPassWordList.ChangeCardNumber(loc, cardNumber);
        }

        public bool SetGenericData(int loc, string data)
        {
            return keyPassWordList.ChangeGenericData(loc, data);
        }

        public bool SetLimitedNumberofUses(int loc, int uses)
        {
            return keyPassWordList.ChangeLimitedNumberofUses(loc, uses);
        }

        public bool SetAccessType(int loc, accessType type)
        {
            return keyPassWordList.ChangeAccessType(loc, type);
        }

        public bool SetAccessTimeType(int loc, accessTimeType type)
        {
            return keyPassWordList.ChangeTimeAccessType(loc, type);
        }

        public bool SetStartTime(int loc, DateTime StartTime)
        {
            return keyPassWordList.ChangeStartTime(loc, StartTime);
        }

        public bool SetEndTime(int loc, DateTime EndTime)
        {
            return keyPassWordList.ChangeEndTime(loc, EndTime);
        }

        /// <summary>
        /// autoGeneratePassword - creates a unique password for the list - not needed for key cabinets
        /// </summary>
        /// <returns></returns>
        public string autoGeneratePassword()
        {
            string passwordTemp = "";

            //protect against loop forever
            for (int i = 0; i < 100000; i++)
            {
                passwordTemp = RandomString(Program.PASSWORD_SIZE);

                if (passwordTemp.Length != Program.PASSWORD_SIZE)
                {
                    continue;
                }
                if (isUnique(passwordTemp))
                {
                    break;
                }
                System.Threading.Thread.Sleep(500);  //allow for new seed
            }
            return passwordTemp;
        }

         // <summary>
        /// Generates a random string with the given length   - 
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private string RandomString(int size)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random((int)DateTime.Now.Ticks);

            char[] possibleValues = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char ch;
            int index;
            for (int i = 0; i < size; i++)
            {
                try
                {
                    index = random.Next(possibleValues.Length - 1);
                    
                    ch = possibleValues[index];
                    
                    sb.Append(ch);
                }
                catch(Exception)
                {
                    break;
                }
            }           
            return sb.ToString();
        }
    }
}



