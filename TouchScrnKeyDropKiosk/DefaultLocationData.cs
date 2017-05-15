using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This location data file is used in case the KIOSK_LOCATION config field doesn't match any previously configured options.
    /// </summary>
    public class DefaultLocationData:LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        public DefaultLocationData():base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.AccessCodeEntry, WhenToUse.Both, new string[] {LanguageTranslation.ENTER_ACCESS_CODE}));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CardSwipe, WhenToUse.Both));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.AskUserForTextMsgNumber, WhenToUse.Both));

            ConfigurableCodeAdminScreensList.Add(AdminScreenType.OpenSingleDoor);
            ConfigurableCodeAdminScreensList.Add(AdminScreenType.OpenAllDoors);
        }

        internal override string retrieveUserNameFromCard(string Magstrip1, string Magstrip2)
        {
            return Magstrip1.Split('^')[1].Split('/')[1] + " " + Magstrip1.Split('^')[1].Split('/')[0];
        }

        internal override string retrieveUserNumberFromCard(string Magstrip1, string Magstrip2)
        {
            return Magstrip1.TrimStart(new char[] { '%', 'b', 'B' }).Substring(12, 4);
        }

        /// <summary>
        /// Header - generates the coma separated header string to match CSV_Entry
        /// </summary>
        public override string  Header
        {
	        get 
	        {
                return "Date, Time, Box number, Access Code, Door opened, Card number, Card name";
	        }
	        internal set 
	        {
                Header = value; 
	        }
        }        

        public class DefaultTransactionData : LocationData.LocationTransactionData
        {
            #region get/set fields for property accessors
            string accesscode { get; set; }
            int locklocation { get; set; }
            int boxnumber { get; set; }
            bool dooropened { get; set; }
            string cardnumber { get; set; }
            string cardname { get; set; }
            bool returning { get; set; }
            string userid { get; set; }
            #endregion
            public override List<LocationTransactionData.TransactionDataObject> ObjectList { get; set; }
            public override string AccessCode { get { return accesscode; } set { accesscode = value; ObjectList[LocateIndexOfDataObjectByName("Access Code")].data = value; } }//Whatever piece of info is used to identify the key to be accessed
            public override int LockLocation { get { return locklocation; } set { locklocation = value; ObjectList[LocateIndexOfDataObjectByName("Lock")].data = value; } }//Location of lock based upon relay board numbering 
            public override DateTime transActionTime { get; set; }  //Time of transaction - when class created
            public override int BoxNumber { get { return boxnumber; } set { boxnumber = value; ObjectList[LocateIndexOfDataObjectByName("Box")].data = value; } }    // The box number - as setup for this project
            public override bool DoorOpened { get { return dooropened; } set { dooropened = value; ObjectList[LocateIndexOfDataObjectByName("Door Opened")].data = value; } }  // True if a correct access data given - does not indicate
            // that door was physically opened 
            public override string CardNumber { get { return cardnumber; } set { cardnumber = value; ObjectList[LocateIndexOfDataObjectByName("Card Number")].data = value; } }          // With card access - the last four digits of card number
            public override string CardName { get { return cardname; } set { cardname = value; ObjectList[LocateIndexOfDataObjectByName("Card Name")].data = value; } }           // Name of card holder - for ease of review transaction data
            public override bool ReturningKey { get { return returning; } set { returning = value; ObjectList[LocateIndexOfDataObjectByName("Return")].data = value; } }   //States if the transaction is taking or returning a key
            public override string UserID { get { return userid; } set { userid = value; ObjectList[LocateIndexOfDataObjectByName("UserID")].data = value; } }

            public DefaultTransactionData()
                : base()
            {
                AddAdditionalDataObjectsToList();
            }

            public override void ClearData()
            {
                base.ClearData();                
            }

            private void AddAdditionalDataObjectsToList()
            {
                
            }

            private int LocateIndexOfDataObjectByName(string name)
            {
                for (int i = 0; i < ObjectList.Count; i++)
                {
                    if (ObjectList[i].name == name)
                        return i;
                }
                return -1;
            }

            /// <summary>
            /// This method returns an exact data copy of the current object. However, it is 
            /// not the object itself so when this object is changed, the copy will not be.
            /// </summary>
            /// <returns></returns>
            public override LocationTransactionData HardCopy()
            {
                DefaultTransactionData copy = new DefaultTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;

                //The object list must be copied piece by piece or both objects will 
                //point to the same ObjectList.
                for (int i = 0; i < this.ObjectList.Count; i++)
                {
                    copy.ObjectList[i] = (TransactionDataObject)this.ObjectList[i].Clone();
                }
                return copy;
            }

            /// <summary>
            /// CSV_Entry - generate a comma separated data entry for a transaction
            /// </summary>
            /// <returns></returns>
            public override string CSV_Entry()
            {
                try
                {
                    return String.Format("{0},{1},{2:d},{3},{4:b},{5},{6}",
                                        Date, Time, BoxNumber, AccessCode, DoorOpened.ToString(), CardNumber, CardName); 
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    return "transaction recording error";
                }
            }

            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), CardNumber, CardName };
            }                       
        }
    }
}
