using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;
using System.Xml;
using System.Data.SQLite;
using XmlConfig;

namespace KeyCabinetKiosk
{
    class HudsonLocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        public HudsonLocationData():base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.SelectBioOrHid, WhenToUse.Both));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReservationAlphaNumberEntry, WhenToUse.Both, new string[] { "Enter Reservation #" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CarDamagedQuestion, WhenToUse.Returning, true, new string[] { "Is Vehicle In Safe", "Operating Condition?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CarCleanedQuestion, WhenToUse.Returning, true, new string[] { "Is Vehicle Clean &", "Free Of Trash?"}));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CarRefueledQuestion, WhenToUse.Returning, true, new string[] { "Is Vehicle Fuel Tank", "At Least Half Full?" }));

            AdminScreenList.Add(AdminScreenType.BiometricEnrollmentHID);

            ConfigurableCodeAdminScreensList.Add(AdminScreenType.OpenSingleDoor);
            ConfigurableCodeAdminScreensList.Add(AdminScreenType.OpenAllDoors);
            ConfigurableCodeAdminScreensList.Add(AdminScreenType.BiometricEnrollmentHID);
        }
        
        /// <summary>
        /// Header - generates the comma separated header string to match CSV_Entry
        /// </summary>
        public override string Header
        {
            get
            {
                return "Date, Time, Box number, Door opened, Reservation Number, HID Number, Identification Type, Returning, Damaged, Cleaned, Refueled, TransactionNum";
            }
            internal set
            {
                Header = value;
            }
        }

        internal override bool confirmReservation(ref LocationData.LocationTransactionData transaction)
        {
            //This allows the admin to set up access codes for each box and then use them to allow the users to override the reservation system locally.
            int box = Program.passwordMgr.FindLocationForCode(transaction.AccessCode);
            if (box != -1)
            {
                Program.logEvent("Local Access Code Overriding Reservation System");
                transaction.BoxNumber = box;
                Program.logEvent("Box Number = " + box.ToString());
                int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                Program.logEvent("transnumindex = " + transnumindex.ToString());
                transaction.ObjectList[transnumindex].data = "Code Override";

                return true;
            }
            //This uses the standing reservation system to retrieve the box number, reservation status and transaction number from the reservation server.
            else 
            {
                int IDTypeIndex =  transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "IdentificationType"; });
                if (!makeHTTPPost(HTTPPostType.CheckReservation, transaction.AccessCode, transaction.ObjectList[IDTypeIndex].data.ToString(), transaction.CardNumber, ref transaction))
                {
                    Program.ShowErrorMessage("Unfortunately we have\r\nbeen unable to verify\r\n your reservation\r\nPlease call 202-264-3803\r\nOR 202-264-3804", 10000);
                    return false;
                }
                else
                    return true;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MethodType"></param>
        /// <param name="extradata1"></param>
        /// <param name="extradata2"></param>
        /// <param name="extradata3">This parameter is used to hold four boolean pieces of data</param>
        /// <returns></returns>
        internal override bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3, ref LocationData.LocationTransactionData transaction)
        {
            string methodtype = "";
            NameValueCollection methodparams = new NameValueCollection();
            switch (MethodType)
            {
                case HTTPPostType.CheckReservation:
                    methodtype = "Reservation";
                    methodparams.Add("clientName", "DCWATER");
                    methodparams.Add("reservationNumber", extradata1.ToUpper()); //any alpha characters in the res number must be capitals
                    methodparams.Add("authMethod", extradata2);
                    methodparams.Add("HIDcode", extradata3);
                    methodparams.Add("kioskNumber", Program.KIOSK_ID);                    
                    break;
                case HTTPPostType.KeyOut:
                    methodtype = "TransactionComplete";
                    methodparams.Add("clientName", "DCWATER");
                    methodparams.Add("transactionNumber", extradata1.Split(',')[0]);
                    methodparams.Add("reservationNumber", extradata1.Split(',')[1].ToUpper());
                    methodparams.Add("status", "SUCCESS");
                    methodparams.Add("question1", extradata3.Substring(1, 1));
                    methodparams.Add("question2", extradata3.Substring(2, 1));
                    methodparams.Add("question3", extradata3.Substring(3, 1));
                    methodparams.Add("UDF1", "0");
                    methodparams.Add("UDF2", "0");
                    methodparams.Add("UDF3", "0");
                    methodparams.Add("UDF4", "0");
                    methodparams.Add("UDF5", "0");
                    methodparams.Add("UDF6", "0");
                    methodparams.Add("authMethod", extradata2.Split(',')[0]);
                    methodparams.Add("HIDcode", extradata2.Split(',')[1]);
                    break;
                case HTTPPostType.TransactionFail:
                    return true;
            }
            return PostHudsonData(methodparams, Program.CUSTOMER_DATA_SERVER, methodtype, ref transaction);
        }

        internal override bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3)
        {
            LocationTransactionData fakedata = new HudsonTransactionData();
            return makeHTTPPost(MethodType, extradata1, extradata2, extradata3, ref fakedata);
        }

        private bool PostHudsonData(NameValueCollection methodParameters, string serverName, string methodName, ref LocationData.LocationTransactionData transaction)
        {
            XmlConfig.XmlConfigDoc xml;
            XmlConfig.XmlConfigNode root;
            //SEND HTTP POST AND HANDLE RESPONSE 
            StreamWriter myWriter = null;
            string TransactionResult = "0", ResponseMessage, strPost = ""; //TransactionResult needs to be initialized to something so that you know for sure when it is empty
                                                                           //because that indicates a successful confirmation message.   
            foreach (string key in methodParameters.AllKeys)
            {
                strPost += ("&" + key + "=" + methodParameters[key]);
            } strPost = strPost.TrimStart('&');

            HttpWebRequest serverReq = (HttpWebRequest)WebRequest.Create(serverName + "/" + methodName);
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
                File.WriteAllText("temp1.xml", TransactionResult, Encoding.UTF8);
                xml = new XmlConfig.XmlConfigDoc("temp1.xml");
                XmlNamespaceManager xnsm = new XmlNamespaceManager(xml.xmlDoc.NameTable);
                xnsm.AddNamespace("ns", "Chevin.WebServices.SafePak");
                root = xml.GetNode("ns:Result", xnsm);
                
                //
                //Deal with transactionresult file               
                switch (methodName)
                {
                    case "Reservation":
                        if (root.GetValue("ns:ReturnCode", xnsm).ToString() == "1")
                        {
                            Program.logEvent("Reservation Number Accepted.");
                            //set the box number
                            transaction.BoxNumber = int.Parse(root.GetValue("ns:Box", xnsm));
                            Program.logEvent("Box Number = " + transaction.BoxNumber);
                            //set the taking/returning status
                            string action = root.GetValue("ns:action", xnsm).ToString();
                            if (action.ToUpper() == "COLLECTING")
                            {
                                transaction.ReturningKey = false;
                                Program.logEvent("Key has COLLECTING status");
                            }
                            else if (action.ToUpper() == "RETURNING")
                            {
                                transaction.ReturningKey = true;
                                Program.logEvent("Key has RETURNING status");
                            }


                            //set the given transaction number
                            int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                            Program.logEvent("transnumindex = " + transnumindex.ToString());
                            transaction.ObjectList[transnumindex].data = root.GetValue("ns:TransactionNumber", xnsm).ToString();
                            Program.logEvent("Trasnaction Number = " + transaction.ObjectList[transnumindex].data.ToString());
                        }
                        else
                        {
                            //error occurred
                            Program.logEvent("Check Reservation Error: " + root.GetValue("ns:Error", xnsm).ToString());
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Check Reservation Error: " + root.GetValue("ns:Error", xnsm).ToString(), "", 0);
                            return false;
                        }
                        break;
                    case "TransactionComplete":
                        //needs to be checked for the "OK" response
                        if (root.GetValue("ns:ReturnCode", xnsm).ToString() == "1")//(TransactionResult == "HTTP/1.1 200 OK")
                            Program.logEvent("Transaction Complete Message Accepted");
                        else
                        {
                            Program.logEvent("Transaction Complete Error: " + TransactionResult);
                            int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, transaction.ObjectList[transnumindex].data.ToString(), DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Transaction Complete Error: " + root.GetValue("ns:Error", xnsm).ToString(), "", 0);
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
                //if (File.Exists("temp1.xml"))
                //    File.Delete("temp1.xml");
            }

            return true;
        }

        public class HudsonTransactionData : LocationData.LocationTransactionData
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
            string identificationtype { get; set; }
            bool damaged { get; set; }
            bool cleaned { get; set; }
            bool refueled { get; set; } 
            string transactionnum { get; set; }           
            #endregion
            public override List<LocationTransactionData.TransactionDataObject> ObjectList { get; set; }
            public override string AccessCode { get { return accesscode; } set { accesscode = value; ObjectList[LocateIndexOfDataObjectByName("Access Code")].data = value; } }//Whatever piece of info is used to identify the key to be accessed
            public override int LockLocation { get { return locklocation; } set { locklocation = value; ObjectList[LocateIndexOfDataObjectByName("Lock")].data = value; } }//Location of lock based upon relay board numbering 
            public override DateTime transActionTime { get; set; }  //Time of transaction - when class created
            public override int BoxNumber { get { return boxnumber; } set { boxnumber = value; ObjectList[LocateIndexOfDataObjectByName("Box")].data = value; } }    // The box number - as setup for this project
            public override bool DoorOpened { get { return dooropened; } set { dooropened = value; ObjectList[LocateIndexOfDataObjectByName("Door Opened")].data = value; } }  // True if a correct access data given - does not indicate that door was physically opened 
            public override string CardNumber { get { return cardnumber; } set { cardnumber = value; ObjectList[LocateIndexOfDataObjectByName("Card Number")].data = value; } }          // With card access - the last four digits of card number
            public override string CardName { get { return cardname; } set { cardname = value; ObjectList[LocateIndexOfDataObjectByName("Card Name")].data = value; } }           // Name of card holder - for ease of review transaction data
            public override bool ReturningKey { get { return returning; } set {returning = value; ObjectList[LocateIndexOfDataObjectByName("Return")].data = value; } }
            public override string UserID { get { return userid; } set { userid = value; ObjectList[LocateIndexOfDataObjectByName("UserID")].data = value; } }
            public string IdentificationType { get { return identificationtype; } set { identificationtype = value; ObjectList[LocateIndexOfDataObjectByName("IdentificationType")].data = value; } }
            public bool CarDamaged { get { return damaged; } set { damaged = value; ObjectList[LocateIndexOfDataObjectByName("Damaged")].data = value; } }
            public bool CarCleaned { get { return cleaned; } set { cleaned = value; ObjectList[LocateIndexOfDataObjectByName("Cleaned")].data = value; } }
            public bool CarRefueled { get { return refueled; } set { refueled = value; ObjectList[LocateIndexOfDataObjectByName("Refueled")].data = value; } }
            public string TransactionNumber { get { return transactionnum; } set { transactionnum = value; ObjectList[LocateIndexOfDataObjectByName("Transaction")].data = value; } }
           
            public HudsonTransactionData(): base()
            {
                AddAdditionalDataObjectsToList();
            }

            public override void ClearData()
            {
                base.ClearData();
                CarDamaged = false;
                CarCleaned = false;
                CarRefueled = false;
                IdentificationType = "";
                TransactionNumber = "";
            }

            private void AddAdditionalDataObjectsToList()
            {
                ObjectList.Add(new TransactionDataObject("IdentificationType", identificationtype, false, true));
                ObjectList.Add(new TransactionDataObject("Damaged", damaged, false, true));
                ObjectList.Add(new TransactionDataObject("Cleaned", cleaned, false, true));
                ObjectList.Add(new TransactionDataObject("Refueled", refueled, false, true));
                ObjectList.Add(new TransactionDataObject("Transaction", transactionnum, false, true));
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
                HudsonTransactionData copy = new HudsonTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;
                copy.CarDamaged = this.CarDamaged;
                copy.CarCleaned = this.CarCleaned;
                copy.CarRefueled = this.CarRefueled;
                copy.IdentificationType = this.IdentificationType;
                copy.TransactionNumber = this.TransactionNumber;

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
                IdentificationType = ObjectList[LocateIndexOfDataObjectByName("IdentificationType")].data.ToString();
                TransactionNumber = ObjectList[LocateIndexOfDataObjectByName("Transaction")].data.ToString();
                CarDamaged = bool.Parse(ObjectList[LocateIndexOfDataObjectByName("Damaged")].data.ToString());
                CarCleaned = bool.Parse(ObjectList[LocateIndexOfDataObjectByName("Cleaned")].data.ToString());
                CarRefueled = bool.Parse(ObjectList[LocateIndexOfDataObjectByName("Refueled")].data.ToString());

                Program.locationdata.makeHTTPPost(HTTPPostType.KeyOut, TransactionNumber + "," + AccessCode, IdentificationType  + "," + CardNumber, packageQuestions());
                
                if (Program.ENABLE_SQLITE)
                {
                    if (!ReturningKey)
                        InsertDBTransaction();
                    else
                        UpdateDBTransaction();
                }                

                try
                {
                    return String.Format("{0},{1},{2:d},{3:b},{4},{5},{6},{7:b},{8:b},{9:b},{10:b},{11}",
                                        Date, Time, BoxNumber, DoorOpened, AccessCode, CardNumber, IdentificationType, ReturningKey, CarDamaged, CarCleaned, CarRefueled, TransactionNumber);
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "error creating csv entry" + ex.Message, "", 0);
                    return "transaction recording error";
                }
            }

            /// <summary>
            /// Returns an array of the data which is applicable to an Nebraska transaction
            /// </summary>
            /// <returns></returns>
            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), AccessCode, CardNumber, IdentificationType, ReturningKey.ToString(), CarDamaged.ToString(), CarCleaned.ToString(), CarRefueled.ToString(), TransactionNumber};
            }

            private void InsertDBTransaction()
            {
                ///
                ///INSERT TRANSACTION INTO LOCAL SQLite DATABASE
                ///

                string dbFile = @Program.SQLITE_DATABASE_NAME;

                string connString = string.Format(@"Data Source={0}; Pooling=false; FailIfMissing=false;", dbFile);

                using (SQLiteConnection dbConn = new System.Data.SQLite.SQLiteConnection(connString))
                {
                    dbConn.Open();
                    using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                    {
                        //                                           -------------------------------
                        //                                              Current KioskData.db data
                        //                                           -------------------------------
                        
                        cmd.CommandText = @"INSERT INTO RES_KEY 
                                      (
                                       OUT_DATETIME, 
                                       KEY_BOX_OUT_NO, 
                                       RESV_RESERV_NO, 
                                       TRANSACTION_OUT_NO,
                                       KIOSK_ID
                                      )
                                   VALUES
                                      (
                                       @KioskDateOut,
                                       @BoxNum,
                                       @ReservNum,
                                       @IdentificationType,
                                       @KioskId
                                      )";


                        //parameterized insert - more flexibility on parameter creation

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {// SQLite date format is: yyyy-MM-dd HH:mm:ss
                            ParameterName = "@KioskDateOut",
                            Value = this.SQLiteDateAndTime,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@BoxNum",
                            Value = this.BoxNumber,
                        });

                        
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@ReservNum",
                            Value = this.AccessCode,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@IdentificationType",
                            Value = this.IdentificationType,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KioskId",
                            Value = Program.KIOSK_ID,
                        });
                        
                        cmd.ExecuteNonQuery();
                    }
                    Program.logEvent("Local SQLite Database Transaction Inserted Successfully");

                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }

            private void UpdateDBTransaction()
            {
                ///
                ///UPDATE TRANSACTION INTO LOCAL SQLite DATABASE
                ///
                string dbFile = @Program.SQLITE_DATABASE_NAME;

                string connString = string.Format(@"Data Source={0}; Pooling=false; FailIfMissing=false;", dbFile);

                using (SQLiteConnection dbConn = new System.Data.SQLite.SQLiteConnection(connString))
                {
                    dbConn.Open();
                    using (System.Data.SQLite.SQLiteCommand cmd = dbConn.CreateCommand())
                    {
                        //                                           -------------------------------
                        //                                              Current KioskData.db data
                        //                                          -------------------------------
                        cmd.CommandText = @"UPDATE RES_KEY
                                    SET
                                       IN_DATETIME            = @ReturnDate,
                                       KEY_BOX_IN_NO          = @KeyBoxNumIn,
                                       TRANSACTION_IN_NO      = @TransactionNum,
                                       CAR_DAMAGED            = @CarDamaged,
                                       CAR_CLEANED            = @CarCleaned,
                                       CAR_REFUELED           = @CarRefueled
                                    WHERE
                                       RESV_RESERV_NO         = @ResvNum
                                       ";
                        
                        //parameterized update - more flexibility on parameter creation

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {// SQLite date format is: yyyy-MM-dd HH:mm:ss
                            ParameterName = "@ReturnDate",
                            Value = this.SQLiteDateAndTime,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KeyBoxNumIn",
                            Value = this.BoxNumber,
                        });
                                                
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@TransactionNum",
                            Value = this.IdentificationType,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@CarDamaged",
                            Value = this.CarDamaged,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@CarCleaned",
                            Value = this.CarCleaned,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@CarRefueled",
                            Value = this.CarRefueled,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@ResvNum",
                            Value = this.AccessCode
                        });

                        cmd.ExecuteNonQuery();
                    }
                    Program.logEvent("Local SQLite Database Transaction Updated Successfully");

                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }

            private string packageQuestions()
            {
                string questionpackage = "";
                if (ReturningKey)
                    questionpackage += "1";
                else
                    questionpackage += "0";

                if (CarDamaged)
                    questionpackage += "1";
                else
                    questionpackage += "0";

                if (CarCleaned)
                    questionpackage += "1";
                else
                    questionpackage += "0";

                if (CarRefueled)
                    questionpackage += "1";
                else
                    questionpackage += "0";

                return questionpackage;
            }
        }
    }
}
