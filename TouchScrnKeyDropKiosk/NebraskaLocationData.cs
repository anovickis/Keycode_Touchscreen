using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Timers;
using System.Xml.Serialization;
using System.Threading;

namespace KeyCabinetKiosk
{
    class NebraskaLocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        private DateTime GetUpdatesSince { get; set; }
        private System.Timers.Timer GetUpdatesTimer { get; set; }

        public NebraskaLocationData()
            : base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReturningQuestion, WhenToUse.Both, new string[] {"Are You Taking", "Or Returning A Key?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.IDScan, WhenToUse.Taking));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReservationConfirmQuestion, WhenToUse.Taking));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.MileageDamageFormQuestion, WhenToUse.Taking));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.VehicleNumEntry, WhenToUse.Returning));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CarRefueledQuestion, WhenToUse.Returning, new string[] { "Have You Removed All", "Trash And Personal Items\r\nFrom The Car?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CarCleanedQuestion, WhenToUse.Returning, new string[] { "Have You Refuelled", "Your Car Completely?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.OdometerEntry, WhenToUse.Returning));

            GetUpdatesSince = DateTime.Now;
            GetUpdatesTimer = new System.Timers.Timer(900000);
            GetUpdatesTimer.Elapsed += new ElapsedEventHandler(GetUpdatesTimer_Elapsed);
            GetUpdatesTimer.AutoReset = true; //needs to be true except for testing
            GetUpdatesTimer.Start();
            GetUpdatesTimer_Elapsed(this, null);
        }

        void GetUpdatesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            makeHTTPPost(HTTPPostType.GetReservationInfo, "", "", "");
        }

        internal override bool showReservationData(ref LocationData.LocationTransactionData transaction, ref YesNoForm ReservationConfirmScreen)
        {
            int Driverindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Driver Number"; });

            NebraskaReservationInfo reservation = Program.NebResMgr.FindNearestReservation(transaction.ObjectList[Driverindex].data.ToString().TrimStart('`'));

            if (reservation == null)
            {
                NebraskaReservationInfo reserv = performAccessCodeVerification("Reservation Not Found", ref transaction);
                if (reserv != null)
                {
                    transaction.ObjectList[Driverindex].data = transaction.ObjectList[Driverindex].data.ToString().TrimStart('`');
                    if (performSecondaryImageScan(ref transaction))
                    {
                        Program.logEvent("Soonest Reservation for current Driver Number is " + reserv.ReservationNumber);

                        DateTime pickuptime = DateTime.Parse(reserv.PickupTime);
                        DateTime returntime = DateTime.Parse(reserv.ReturnTime);
                        string line2 = "Pickup: " + pickuptime.Month + '/' + pickuptime.Day + " " + pickuptime.Hour + ":" + pickuptime.Minute.ToString().PadLeft(2, '0');
                        string line3 = "Return: " + returntime.Month + '/' + returntime.Day + " " + returntime.Hour + ":" + returntime.Minute.ToString().PadLeft(2, '0');
                        ReservationConfirmScreen.setFormMessage("Confirm Unit " + reserv.VehicleNumber + "\r\n" + line2 + "\r\n" + line3);
                        return true;
                    }
                }                
                return false;
            }
            else if (reservation.PickupTime == "2040-01-1 00:00:00")
            {
                Program.logEvent("No reservation within the allowed timeframe");
                Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "No reservation within the allowed timeframe", "", 0);

                NebraskaReservationInfo reserv = performAccessCodeVerification("Reservation Too Late\r\nOr Too Early", ref transaction);

                //if access code entered does not match any reservation, exit
                if (reserv == null)
                    return false;

                //if a reservation was found to match the alternate access code and the driver's license number was entered manually
                if (reserv != null && transaction.ObjectList[Driverindex].data.ToString().StartsWith("`"))
                {
                    transaction.ObjectList[Driverindex].data = transaction.ObjectList[Driverindex].data.ToString().TrimStart('`');
                    if (!performSecondaryImageScan(ref transaction))
                    {
                        return false;
                    }                    
                }   
                Program.logEvent("Soonest Reservation for current Driver Number is " + reserv.ReservationNumber);

                DateTime pickuptime = DateTime.Parse(reserv.PickupTime);
                DateTime returntime = DateTime.Parse(reserv.ReturnTime);
                string line2 = "Pickup: " + pickuptime.Month + '/' + pickuptime.Day + " " + pickuptime.Hour + ":" + pickuptime.Minute.ToString().PadLeft(2, '0');
                string line3 = "Return: " + returntime.Month + '/' + returntime.Day + " " + returntime.Hour + ":" + returntime.Minute.ToString().PadLeft(2, '0');
                ReservationConfirmScreen.setFormMessage("Confirm Unit " + reserv.VehicleNumber + "\r\n" + line2 + "\r\n" + line3);
                return true;
            }
            else //driver's reservation found and was in the correct time frame
            {
                Program.logEvent("Soonest Reservation for current Driver Number is " + reservation.ReservationNumber);

                //if the driver's license number was entered manually
                if (transaction.ObjectList[Driverindex].data.ToString().StartsWith("`"))
                {
                    transaction.ObjectList[Driverindex].data = transaction.ObjectList[Driverindex].data.ToString().TrimStart('`');
                    if (!performSecondaryImageScan(ref transaction))
                    {
                        return false;
                    }
                }

                DateTime pickuptime = DateTime.Parse(reservation.PickupTime);
                DateTime returntime = DateTime.Parse(reservation.ReturnTime);
                string line2 = "Pickup: " + pickuptime.Month + '/' + pickuptime.Day + " " + pickuptime.Hour + ":" + pickuptime.Minute.ToString().PadLeft(2, '0');
                string line3 = "Return: " + returntime.Month + '/' + returntime.Day + " " + returntime.Hour + ":" + returntime.Minute.ToString().PadLeft(2, '0');
                ReservationConfirmScreen.setFormMessage("Confirm Unit " + reservation.VehicleNumber + "\r\n" + line2 + "\r\n" + line3);

                transaction.BoxNumber = reservation.PickupBoxNumber;
                transaction.AccessCode = reservation.ReservationNumber;
                return true;               
            }            
        }

        private NebraskaReservationInfo performAccessCodeVerification(string message, ref LocationData.LocationTransactionData transaction)
        {
            YesNoForm accesscodeconfirm = new YesNoForm(message, "Enter Access Code?");
            accesscodeconfirm.ShowDialog();

            if (accesscodeconfirm.TimedOut)
            {
                Program.ShowErrorMessage("Transaction Timed Out", 5000);
                Program.logEvent("Access Code Confirmation Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Access Code Confirmation Timed Out", "", 0);
                return null;
            }
            if (accesscodeconfirm.YesResult)
            {
                KeyPadForm accesscode = new KeyPadForm("Enter\r\nAccess\r\nCode", false, 10, false, false);
                accesscode.ShowDialog();

                if (accesscode.TimedOut)
                {
                    Program.ShowErrorMessage("Transaction Timed Out", 5000);
                    Program.logEvent("Access Code Entry Timed Out");
                    Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Access Code Entry Timed Out", "", 0);
                    return null;
                }
                if (!accesscode.bOK)
                {
                    Program.logEvent("Access Code Entry Cancelled");
                    return null;
                }
                
                //Find Reservation By Access Code
                string str = Program.NebResMgr.FindReservationNumber(accesscode.Result);

                if (str != null)
                {
                    NebraskaReservationInfo reserv = Program.NebResMgr.FindReservationbyReservationNumber(str);
                    transaction.BoxNumber = reserv.PickupBoxNumber;
                    transaction.AccessCode = reserv.ReservationNumber;
                    int index = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Alternate Access Code"; });
                    transaction.ObjectList[index].data = accesscode.Result;
                    return reserv;
                }
                else
                {
                    Program.ShowErrorMessage("Reservation # not Found\r\n with Access Code", 5000);
                    Program.logEvent("Reservation Number not Found with Access Code:" + accesscode.Result);
                    Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Number not Found with Access Code:" + accesscode.Result, "", 0);

                    return null;
                }
            }
            else
            {
                Program.logEvent("Reservation Secondary Access Code Entry Cancelled");
                return null;
            }
            
        }

        private bool performSecondaryImageScan(ref LocationData.LocationTransactionData transaction)
        {
            //If the barcode scan failed and the access code is valid then prompt for an image scan just to get some data to give to the
            //admin. Don't check for if the info matches the reservation.
            if (Program.IMAGE_SCAN_TYPE.ToUpper() == "BARCODE")
            {
                //get license data storage indexes
                int nameindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Driver Name"; });
                int numindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Driver Number"; });

                IDScanInput inputscreen = new IDScanInput("IMAGE");
                inputscreen.ShowDialog();

                if (inputscreen.cancelled)
                {
                    Program.logEvent("ID Scan Cancelled");
                    return false;
                }
                if (inputscreen.TimedOut)
                {
                    Program.ShowErrorMessage("Transaction Timed Out", 5000);
                    Program.logEvent("ID Scan Timed Out");
                    Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "ID Scan Timed Out", "", 0);
                    return false;
                }

                //do stuff with license data
                transaction.ObjectList[nameindex].data = inputscreen.firstname + " " + inputscreen.lastname;
                transaction.ObjectList[numindex].data = inputscreen.licensenum;
                Program.logEvent("ID Name Scanned: " + inputscreen.firstname + " " + inputscreen.lastname);
                Program.logEvent("ID Number Scanned: " + inputscreen.licensenum);
                return true;
            }
            return false;
        }

        internal override bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3)
        {
            string methodtype = "", apiKey = "E1AC88E1-FB98-49DC-AD6B-0F0956465EFE";
            NameValueCollection methodparams = new NameValueCollection();
            switch (MethodType)
            {
                case HTTPPostType.GetReservationInfo:
                    methodtype = "GetAllReservationInfo";
                    methodparams.Add("apiKey", apiKey);
                    methodparams.Add("kioskID", Program.KIOSK_ID);
                    //omission of this field will return all reservations
                    //methodparams.Add("updatesSince", GetUpdatesSince.ToString());
                    break;
                case HTTPPostType.KeyIn:
                    methodtype = "KeyReturned";
                    methodparams.Add("apiKey", apiKey);
                    methodparams.Add("kioskID", Program.KIOSK_ID);
                    methodparams.Add("BoxNumber", extradata1);
                    methodparams.Add("VehicleNumber", extradata2);
                    methodparams.Add("odometerReading", extradata3);
                    break;
                case HTTPPostType.KeyOut:
                    methodtype = "KeyCheckedOut";
                    methodparams.Add("apiKey", apiKey);
                    methodparams.Add("kioskID", Program.KIOSK_ID);
                    methodparams.Add("BoxNumber", extradata1);
                    methodparams.Add("ReservationNumber", extradata2);
                    methodparams.Add("DriversLicense", extradata3);
                    break;
                case HTTPPostType.KeyLoaded:
                    methodtype = "KeysLoaded";
                    methodparams.Add("apiKey", apiKey);
                    methodparams.Add("kioskID", Program.KIOSK_ID);
                    break;
            }
            return GetNebraskaWebDatabaseInfo(methodparams, Program.CUSTOMER_DATA_SERVER/*scsapps.unl.edu*/, methodtype);
        }

        private bool GetNebraskaWebDatabaseInfo(NameValueCollection methodParameters, string serverName, string methodName)
        {
            //SEND HTTP POST AND HANDLE RESPONSE 
            StreamWriter myWriter = null;
            string TransactionResult = "0", ResponseMessage, strPost = ""; //TransactionResult needs to be initialized to something so that you know for sure when it is empty
            //because that indicates a successful confirmation message.   
            foreach (string key in methodParameters.AllKeys)
            {
                strPost += ("&" + key + "=" + methodParameters[key]);
            } strPost = strPost.TrimStart('&');

            HttpWebRequest serverReq = (HttpWebRequest)WebRequest.Create("https://" + serverName + "/MPERequest/ReservationService.asmx/" + methodName);
            serverReq.Method = "POST";
            serverReq.ContentLength = strPost.Length;
            serverReq.ContentType = "application/x-www-form-urlencoded";

            try
            {
                //send data
                myWriter = new StreamWriter(serverReq.GetRequestStream());
                myWriter.Write(strPost);
                myWriter.Close();

                //receive response
                HttpWebResponse serverResponse = (HttpWebResponse)serverReq.GetResponse();

                using (StreamReader sr = new StreamReader(serverResponse.GetResponseStream()))
                {
                    TransactionResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                //write the response to a file
                File.WriteAllText("temp1.xml", TransactionResult, Encoding.Unicode);

                //
                //Deal with transactionresult file
                switch (methodName)
                {
                    case "GetAllReservationInfo":
                        //need to add whatever reservations were returned to current list
                        Program.NebResMgr.AddReservationsToFile("temp1.xml");
                        GetUpdatesSince = DateTime.Now; //if successful
                        break;
                    case "KeyCheckedOut":
                        //needs to be checked for the "OK" response
                        if (TransactionResult == "")
                            Program.logEvent("Key Checked Out Message Accepted");
                        else
                        {
                            Program.logEvent("Key Checked Out Error: " + TransactionResult);
                            Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Key Checked Out Error: " + TransactionResult, "", 0);
                        }
                        break;
                    case "KeyReturned":
                        //needs to be checked for the "OK" response
                        if (TransactionResult == "")
                            Program.logEvent("Key Returned Message Accepted");
                        else
                        {
                            Program.logEvent("Key Returned Error: " + TransactionResult);
                            Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Key Returned Error: " + TransactionResult, "", 0);
                        }
                        break;
                    case "KeyLoaded":
                        //needs to be checked for the "OK" response
                        if (TransactionResult == "")
                            Program.logEvent("Keys Loaded Message Accepted");
                        else
                        {
                            Program.logEvent("Keys Loaded Error: " + TransactionResult);
                            Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Key Loaded Error: " + TransactionResult, "", 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ResponseMessage = ex.Message;
                Program.logEvent(DateTime.Now.ToString() + "  " + ex.Message);
                return false;
            }
            finally
            {
                if (myWriter != null)
                    myWriter.Close();
                if (File.Exists("temp1.xml"))
                    File.Delete("temp1.xml");
            }
            return true;
        }

        internal override bool preDoorOpeningProcessing(LocationTransactionData transaction)
        {
            if ((Program.NUMBER_RELAYS <=33 && transaction.BoxNumber == 33) || (Program.NUMBER_RELAYS <= 65 && transaction.BoxNumber == 65))
            {
                Program.ShowErrorMessage("Place Your Key In\r\nThe Drop Box", 7000);
            }
            return true;
        }

        /// <summary>
        /// Header - generates the coma separated header string to match CSV_Entry
        /// </summary>
        public override string Header
        {
            get
            {
                return "Date, Time, Box number, Door opened, Reservation number, Alternate Access Code, Lic number, Lic name, Returning Key, Odometer, VehicleNumber";
            }
            internal set
            {
                Header = value;
            }
        }

        public class NebraskaTransactionData : LocationTransactionData
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
            string idname { get; set; }
            string idnumber { get; set; }
            int odometer { get; set; }
            string vehiclenumber { get; set; }
            string alternateaccesscode { get; set; }
            #endregion
            public override List<LocationTransactionData.TransactionDataObject> ObjectList { get; set; }
            public override string AccessCode { get { return accesscode; } set { accesscode = value; ObjectList[LocateIndexOfDataObjectByName("Access Code")].data = value; } }//Whatever piece of info is used to identify the key to be accessed
            public override int LockLocation { get { return locklocation; } set { locklocation = value; ObjectList[LocateIndexOfDataObjectByName("Lock")].data = value; } }//Location of lock based upon relay board numbering 
            public override DateTime transActionTime { get; set; }  //Time of transaction - when class created
            public override int BoxNumber { get { return boxnumber; } set { boxnumber = value; ObjectList[LocateIndexOfDataObjectByName("Box")].data = value; } }    // The box number - as setup for this project
            public override bool DoorOpened { get { return dooropened; } set { dooropened = value; ObjectList[LocateIndexOfDataObjectByName("Door Opened")].data = value; } }  // True if a correct access data given - does not indicate that door was physically opened 
            public override string CardNumber { get { return cardnumber; } set { cardnumber = value; ObjectList[LocateIndexOfDataObjectByName("Card Number")].data = value; } }          // With card access - the last four digits of card number
            public override string CardName { get { return cardname; } set { cardname = value; ObjectList[LocateIndexOfDataObjectByName("Card Name")].data = value; } }           // Name of card holder - for ease of review transaction data
            public override bool ReturningKey { get { return returning; } set { returning = value; ObjectList[LocateIndexOfDataObjectByName("Return")].data = value; } }   //States if the transaction is taking or returning a key
            public override string UserID { get { return userid; } set { userid = value; ObjectList[LocateIndexOfDataObjectByName("UserID")].data = value; } }
            public string IDName { get { return (string)ObjectList[LocateIndexOfDataObjectByName("Driver Name")].data; } set { idname = value; ObjectList[LocateIndexOfDataObjectByName("Driver Name")].data = value; } }
            public string IDNumber { get { return (string)ObjectList[LocateIndexOfDataObjectByName("Driver Number")].data; } set { idnumber = value; ObjectList[LocateIndexOfDataObjectByName("Driver Number")].data = value; } }
            public int Odometer { get { return (int)ObjectList[LocateIndexOfDataObjectByName("Odometer")].data; } set { odometer = value; ObjectList[LocateIndexOfDataObjectByName("Odometer")].data = value; } }
            public string VehicleNumber { get { return (string)ObjectList[LocateIndexOfDataObjectByName("VehicleNumber")].data; } set { vehiclenumber = value; ObjectList[LocateIndexOfDataObjectByName("VehicleNumber")].data = value; } }
            public string AlternateAccessCode { get { return (string)ObjectList[LocateIndexOfDataObjectByName("Alternate Access Code")].data; } set { alternateaccesscode = value; ObjectList[LocateIndexOfDataObjectByName("Alternate Access Code")].data = value; } } //used as a secondary form of identification in case the ID Scanner fails

            public NebraskaTransactionData()
                : base()
            {
                AddAdditionalDataObjectsToList();
                AlternateAccessCode = "";
            }

            public override void ClearData()
            {
                base.ClearData();
            }

            private void AddAdditionalDataObjectsToList()
            {
                ObjectList.Add(new TransactionDataObject("Driver Name", idname, true, true));
                ObjectList.Add(new TransactionDataObject("Driver Number", idnumber, true, true));
                ObjectList.Add(new TransactionDataObject("VehicleNumber", vehiclenumber, false, true));
                ObjectList.Add(new TransactionDataObject("Odometer", odometer, false, true));
                ObjectList.Add(new TransactionDataObject("Alternate Access Code", alternateaccesscode, true, true));
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
                NebraskaTransactionData copy = new NebraskaTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.IDName = this.IDName;
                copy.IDNumber = this.IDNumber;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;
                copy.Odometer = this.Odometer;
                copy.VehicleNumber = this.VehicleNumber;
                copy.AlternateAccessCode = this.AlternateAccessCode;

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
                if (ReturningKey == true)
                {   //When returning a vehicle, because IDName and IDNumber are not used, but are stored first in ObjectList, they are what the data for VehicleNumber and Odometer are assigned to
                    VehicleNumber = IDName; IDName = "";
                    Odometer = int.Parse(IDNumber); IDNumber = "";
                    Program.locationdata.makeHTTPPost(HTTPPostType.KeyIn, BoxNumber.ToString(), VehicleNumber, Odometer.ToString());
                }
                else if (ReturningKey == false)
                {
                   Program.locationdata.makeHTTPPost(HTTPPostType.KeyOut, BoxNumber.ToString(), AccessCode, IDNumber);
                }

                //Update Reservation Info After Each Transaction
                Program.locationdata.makeHTTPPost(HTTPPostType.GetReservationInfo, "", "", "");

                try
                {
                    string ret = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                        Date, Time, BoxNumber, DoorOpened.ToString(), AccessCode, AlternateAccessCode, IDName, IDNumber, ReturningKey, Odometer, VehicleNumber);

                    ObjectList[LocateIndexOfDataObjectByName("Access Code")].name = "Reservation Number"; //This changes the name of the "access code" so that it shows up in the email receipt as "reservation number"

                    return ret;
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    Program.SqlManager.ErrorDatabaseEntry(AccessCode, IDNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "error creating csv entry" + ex.Message, IDName, int.Parse(VehicleNumber));
                    return "transaction recording error";
                }          
            }

            /// <summary>
            /// Returns an array of the data which is applicable to an Nebraska transaction
            /// </summary>
            /// <returns></returns>
            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), AccessCode, IDName, IDNumber, Odometer.ToString(), VehicleNumber, ReturningKey.ToString() };
            }
        }
    }   
    
    public class NebraskaReservationManager
    {
        public NebraskaReservationList ReservationList { get; private set; }
        private string xmlFileName;

        /// <summary>
        /// NebraskaReservationManager - constructor - reads the reservation file into a reservation list 
        ///                             that is use for all look ups
        /// </summary>
        /// <param name="fileName"></param>
        public NebraskaReservationManager(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new Exception("bad file name for Reservation xml file");
            }
            xmlFileName = fileName;
            LoadFromFile();
        }

        /// <summary>
        /// LoadFromFile- reads the "filename" xml file into a list structure - the keyPassWordList
        /// </summary>
        private void LoadFromFile()
        {
            try
            {
                LoadFromFile(xmlFileName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// LoadFromFile- reads the "filename" xml file into a list structure - the keyPassWordList
        /// </summary>
        internal void LoadFromFile(string filename)
        {
            //read in xml data from file
            TextReader tr = new StreamReader(filename);
            try
            {
                ReservationList = new NebraskaReservationList();

                XmlSerializer sr = new XmlSerializer(typeof(NebraskaReservationList));

                ReservationList = (NebraskaReservationList)sr.Deserialize(tr);

                CheckDataValid();

                UpdateKeypasswordDataWithReservationData();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                tr.Close();
            }
        }

        internal void AddReservationsToFile(string filename)
        {
            TextReader tr = new StreamReader(filename);
            try
            {
                NebraskaReservationList TempReservationList = new NebraskaReservationList();

                //read in xml data from file

                XmlSerializer sr = new XmlSerializer(typeof(NebraskaReservationList), "http://schemas.scs.unl.edu/WebServices/ReservationService");

                TempReservationList = (NebraskaReservationList)sr.Deserialize(tr);

                ReservationList.ReplaceReservationList(TempReservationList);

                CheckDataValid();

                Program.NebResMgr.SaveFile();
                Program.logEvent("Nebraska Reservations Updated Successfully");
                UpdateKeypasswordDataWithReservationData();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                tr.Close();

            }
        }

        private void UpdateKeypasswordDataWithReservationData()
        {
            NebraskaReservationInfo info;
            string CurrentReservationNumber;
            Program.passwordMgr.TemporaryClearKeyPasswords();
            for (int i = 0; i < ReservationList.Count; i++)
            {
                info = ReservationList.ArrayOfReservationInfo[i];
                //This will always set the first password in the list with the reservation number because there should never be more than one in the list and if there is
                //it would only be because of an error and should be ignored.
                //CurrentReservationNumber = Program.passwordMgr.FindPassword(info.PickupBoxNumber).First<string>();
                Program.passwordMgr.AddPasswordToKey(info.PickupBoxNumber, info.ReservationNumber + "P");
                Program.passwordMgr.AddPasswordToKey(info.ReturnBoxNumber, info.ReservationNumber + "R");
                //Program.passwordMgr.SetPassword(info.PickupBoxNumber, CurrentReservationNumber, info.ReservationNumber);
            }
            Program.passwordMgr.SaveFile();
        }

        /// <summary>
        ///  SaveFile - writes the Key Password list to the xml file
        /// </summary>
        public void SaveFile()
        {
            Thread.Sleep(500); //This is to give other processes time to let go of their handle on the file
            ReservationList.SerializeToXmlFile(xmlFileName);
        }

        /// <summary>
        /// CheckDataValid - do basic checks on the data to ensure that it is valid for operations
        ///                 required. Throw exception if not valid
        /// </summary>
        private void CheckDataValid()
        {
            for (int i = 0; i < ReservationList.Count; i++)
            {
                if ((ReservationList.ArrayOfReservationInfo[i].ReservationNumber == null) || (ReservationList.ArrayOfReservationInfo[i].ReservationNumber == ""))
                    throw new Exception("Reservation " + (i + 1).ToString() + " Doesn't Have A Reservation Number");

                if ((ReservationList.ArrayOfReservationInfo[i].DriversLicenseNumber == null) || (ReservationList.ArrayOfReservationInfo[i].DriversLicenseNumber == ""))
                    throw new Exception("Driver's # for Reservation # " + ReservationList.ArrayOfReservationInfo[i].ReservationNumber + " Doesn't exist");

                if ((!Program.passwordMgr.IsValidBoxNumber(ReservationList.ArrayOfReservationInfo[i].PickupBoxNumber.ToString())))
                    throw new Exception("Pickup Key Number for Reservation # " + ReservationList.ArrayOfReservationInfo[i].ReservationNumber + " Is Invalid");

                if ((!Program.passwordMgr.IsValidBoxNumber(ReservationList.ArrayOfReservationInfo[i].ReturnBoxNumber.ToString())))
                    throw new Exception("Return Key Number for Reservation # " + ReservationList.ArrayOfReservationInfo[i].ReservationNumber + " Is Invalid");
            }
        }

        public NebraskaReservationInfo FindNearestReservation(string DriversLicense)
        {
            return ReservationList.FindSoonestReservationByDriversLic(DriversLicense);
        }

        public NebraskaReservationInfo FindReservationbyReservationNumber(string reservation)
        {
            return ReservationList.FindReservationByReservationNumber(reservation);
        }

        public string FindReservationNumber(string AlternateAccessCode)
        {
            return ReservationList.FindReservationByAlternateAccessCode(AlternateAccessCode);
        }

        public string FindDriverNumber(string Reservation)
        {
            return ReservationList.FindDriverLicByReservation(Reservation);
        }

        public bool FindReturnOnly(string Reservation)
        {
            return ReservationList.FindReturnOnlyByReservation(Reservation);
        }

        public int FindPickupKeyNumber(string Reservation)
        {
            return ReservationList.FindPickupKeyNumByReservation(Reservation);
        }

        public int FindReturnKeyNumber(string Reservation)
        {
            return ReservationList.FindReturnKeyNumByReservation(Reservation);
        }

        public string FindReservationByVehicleNumber(string Vehicle)
        {
            return ReservationList.FindReservationByVehicleNum(Vehicle);
        }
    }

    public class NebraskaReservationInfo
    {
        [XmlElementAttribute()]
        public string DriversLicenseNumber { get; set; } // Reservation Holders DNL

        [XmlElementAttribute()]
        public string DriversName { get; set; } // Reservation Holders DNL

        [XmlElementAttribute()]
        public string ReservationNumber { get; set; }   //Reservation ID Number

        [XmlElementAttribute()]
        public int PickupBoxNumber { get; set; }     //key box which holds the reserved key to be picked up from

        [XmlElementAttribute()]
        public int ReturnBoxNumber { get; set; }  //key box which holds the reserved key to be returned to

        [XmlElementAttribute()]
        public string VehicleNumber { get; set; }  //vehicle number assigned to rental  

        [XmlElementAttribute()]
        public bool ReturnOnly { get; set; }   //Whether or not the car will be returned only      

        [XmlElementAttribute()]
        public string PickupTime { get; set; } //pickup datetime

        [XmlElementAttribute()]
        public string ReturnTime { get; set; } //return datetime

        [XmlElementAttribute()]
        public string AccessCode { get; set; } //Alternate Access Code used if license scanner does not scan driver# properly

        //empty constructor needed for streaming object
        public NebraskaReservationInfo() { }

        public NebraskaReservationInfo(string DNL, string DName, string ResNum, string PickupKeyNum, string ReturnKeyNum, string VehicleNum, string Return,
                                        string TakeTime, string GiveTime, string Access)
        {
            DriversLicenseNumber = DNL; DriversName = DName; ReservationNumber = ResNum;
            PickupBoxNumber = int.Parse(PickupKeyNum); ReturnOnly = bool.Parse(Return);
            ReturnBoxNumber = int.Parse(ReturnKeyNum); VehicleNumber = VehicleNum;
            PickupTime = TakeTime; ReturnTime = GiveTime; AccessCode = Access;
        }
    }

    public class NebraskaReservationList
    {
        private List<NebraskaReservationInfo> reservations;

        //[XmlArrayItem("ReservationInfo", typeof(NebraskaReservationInfo))]
        [XmlElement("ReservationInfo")] //XmlElement is used instead of XmlArrayItem so that an additional tag (with the array name) is not needed in the XML file.
        public List<NebraskaReservationInfo> ArrayOfReservationInfo //xml interface for serialization
        {
            get { return reservations; }
            set { reservations = value; }
        }

        /// <summary>
        /// Count - the number of reservation objects in the list
        /// </summary>
        public int Count
        {
            get
            {
                return reservations.Count;
            }
        }

        /// <summary>
        ///  ReservationList - default no paramater constructor needed to stream in data
        /// </summary>
        public NebraskaReservationList()
        {
            reservations = new List<NebraskaReservationInfo>();
        }

        /// <summary>
        /// AddReservation - add a new reservation object
        /// </summary>
        /// <param name="keypassword"></param>
        public void AddReservation(NebraskaReservationInfo reservation)
        {
            reservations.Add(reservation);
        }

        /// <summary>
        /// SerializeToXmlFile - stream the reservation list to the XML file
        /// </summary>
        /// <param name="xmlfile"></param>
        public void SerializeToXmlFile(string xmlfile)
        {
            TextWriter tr = new StreamWriter(xmlfile);
            XmlSerializer sr = new XmlSerializer(typeof(NebraskaReservationList));

            sr.Serialize(tr, this);

            tr.Close();

        }

        public NebraskaReservationInfo FindSoonestReservationByDriversLic(string DriverLic)
        {
            NebraskaReservationInfo res1 = null, res2 = null;
            foreach (NebraskaReservationInfo res in reservations)
            {
                if (res.DriversLicenseNumber.ToUpper() == DriverLic.ToUpper() && !res.ReturnOnly)
                {
                    if (!IsReservationTimeStillValid(res))
                    {
                        if (res1 == null)
                            res1 = new NebraskaReservationInfo("", "", "", "0", "0", "", "false", "2040-01-1 00:00:00", "", ""); //blank reservation info indicates found reservation but too early or too late.
                        continue;                                                                                 //a pickup time is still needed to compare against other reservations.
                    }

                    if (res1 == null || res1.PickupTime == "2040-01-1 00:00:00")
                        res1 = res;
                    else
                    {
                        res2 = res;
                        if (DateTime.Parse(res1.PickupTime) > DateTime.Parse(res2.PickupTime))
                            res1 = res2;
                    }
                }
            }
            return res1;
        }

        public NebraskaReservationInfo FindReservationByReservationNumber(string reservation)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == reservation)
                    return r;
            }
            return null;
        }

        /// <summary>
        /// Determines if the current time is within the pre-defined window (currently
        /// 2 hours before or 4 hours after the reservation's pickup time)
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool IsReservationTimeStillValid(NebraskaReservationInfo res)
        {
            DateTime restime = DateTime.Parse(res.PickupTime);
            DateTime curtime = DateTime.Now;

            if (restime.CompareTo(curtime) == 0) //if reservation time is right now
                return true;
            else if (restime.CompareTo(curtime) < 0) //if reservation time is earlier than current time
            {
                curtime = curtime.Subtract(new TimeSpan(4, 0, 0));
                if (restime.CompareTo(curtime) < 0)
                    return false;
            }
            else if (restime.CompareTo(curtime) > 0) //if reservation time is later than current time
            {
                curtime = curtime.Add(new TimeSpan(2, 0, 0));
                if (restime.CompareTo(curtime) > 0)
                    return false;
            }

            return true;
        }

        public string FindReservationByAlternateAccessCode(string Access)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.AccessCode == Access)
                {
                    return r.ReservationNumber;
                }
            }
            return null;
        }

        public int FindPickupKeyNumByReservation(string res)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == res)
                {
                    return r.PickupBoxNumber;
                }
            }
            return -1;
        }

        public int FindReturnKeyNumByReservation(string res)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == res)
                {
                    return r.ReturnBoxNumber;
                }
            }
            return -1;
        }

        public string FindVehicleNumByReservation(string res)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == res)
                {
                    return r.VehicleNumber;
                }
            }
            return null;
        }

        /// <summary>
        /// FindDriverLicByReservation - finds and returns the Driver's # for the given reservation
        ///                            - returns null if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public string FindDriverLicByReservation(string res)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == res)
                {
                    return r.DriversLicenseNumber;
                }
            }
            return null;
        }

        public string FindReservationByDriverLic(string lic)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.DriversLicenseNumber == lic)
                {
                    return r.ReservationNumber;
                }
            }
            return null;
        }

        public string FindReservationByVehicleNum(string Vehicle)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.VehicleNumber == Vehicle && r.ReturnOnly) //if the vehicle number matches and the flag for returning the car has been set by the web server.
                {                                               //This is to prevent problems when there are multiple reservations in the list with the same vehicle#
                    return r.ReservationNumber;
                }
            }
            return null;
        }

        /// <summary>
        /// FindReturnOnlyByReservation  - finds and returns whether or not the reservation
        ///                                is a return only transaction
        ///                             - return false if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public bool FindReturnOnlyByReservation(string res)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.ReservationNumber == res)
                {
                    return r.ReturnOnly;
                }
            }
            return false;
        }

        public DateTime FindPickupTimeByDriverLic(string lic)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.DriversLicenseNumber == lic)
                {
                    return DateTime.Parse(r.PickupTime);
                }
            }
            return new DateTime();
        }

        public DateTime FindReturnTimeByDriverLic(string lic)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.DriversLicenseNumber == lic)
                {
                    return DateTime.Parse(r.ReturnTime);
                }
            }
            return new DateTime();
        }

        public string FindVehicleNumByDriverLic(string lic)
        {
            foreach (NebraskaReservationInfo r in reservations)
            {
                if (r.DriversLicenseNumber == lic)
                {
                    return r.VehicleNumber;
                }
            }
            return null;
        }

        internal void ReplaceReservationList(NebraskaReservationList NewList)
        {
            reservations.Clear();
            for (int i = 0; i < NewList.Count; i++)
                this.AddReservation(NewList.ArrayOfReservationInfo[i]);
        }

        internal void ConcatinateReservationLists(NebraskaReservationList NewList)
        {
            for (int i = 0; i < NewList.Count; i++)
                this.AddReservation(NewList.ArrayOfReservationInfo[i]);
        }
    }
}
