using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThingMagic;
using System.Threading;
using System.Data.SQLite;

namespace KeyCabinetKiosk
{
    public class MontanaLocationData:LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        public MontanaLocationData():base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.CardSwipe, WhenToUse.Both));
            //CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.BoxNumberEntry, WhenToUse.Both));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReturningQuestion, WhenToUse.Both, new string[] { "Are You Taking", "Or Returning A Key?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.RetrieveRFIDTagNumber, WhenToUse.Both));
        }

        internal override string retrieveUserNameFromCard(string Magstrip1, string Magstrip2)
        {
            return ""; //no names embedded into cards
        }

        internal override string retrieveUserNumberFromCard(string Magstrip1, string Magstrip2)
        {
            return Magstrip2.Substring(6, 10); 
        }

        internal override string retrieveRFIDTagNumber(int location, LocationTransactionData transdata)
        {                        
            return Program.passwordMgr.FindGenericData(location);
        }

        internal override bool scanRFIDTagNumber(string tagID)
        {
            TagReadData[] data;
            for (int i = 0; i < 30; i++)
            {
                data = Program.ThingMagicRFIDreader.ReadTags(1000);
                foreach (TagReadData d in data)
                {
                    if (d.EpcString.EndsWith(tagID))
                        return true;
                }
            }
            Program.ThingMagicRFIDreader.DoneReading();

            return false;
        }

        //Used to deal with the RFID scanning thread which will run for approx 30 seconds after transaction ends to make sure the RFID tag is either
        //present or not depending on whether the key was taken or returned.
        internal override bool preDoorOpeningProcessing(LocationData.LocationTransactionData transaction)
        {
            //Check to see if the box number entered by the user matches the one for their card on file
            //int truebox = Program.passwordMgr.FindLocationForCard(transaction.CardNumber);
            //if (truebox != transaction.BoxNumber)
            //{
            //    Program.logEvent("Box number " + transaction.BoxNumber + " Entered by the user does not match box number " + truebox + " for card number " + transaction.CardNumber + " in keypassword.xml");
            //    Program.ShowErrorMessage("Invalid Box\r\nNumber", 3000);
            //    return false;
            //}

            if (!transaction.ReturningKey)
            {
                bool returnBool = transaction.ReturningKey;
                int RFIDTagindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagNum"; });
                //Package all needed data for the thread into a single string
                string ThreadDataString = returnBool.ToString() + "," + transaction.ObjectList[RFIDTagindex].data.ToString();

                Thread afterTransScanThread = new Thread(MontanaLocationData.AfterTransactionScanThread);
                afterTransScanThread.Start(ThreadDataString);
            }
            return true;
        }

        //Used to run the initial scan for the RFID tag after the door is opened so that the tag can be seen by the antennae
        //Used to deal with the RFID scanning thread which will run for approx 30 seconds after transaction ends to make sure the RFID tag is either
        //present or not depending on whether the key was taken or returned.
        internal override bool postDoorOpeningProcessing(LocationData.LocationTransactionData transaction)
        {
            if (transaction.ReturningKey)
            {
                int RFIDTagindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagNum"; });
                int RFIDDetectedindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagDetected"; });

                RFIDScanForm scanform = new RFIDScanForm(transaction.ObjectList[RFIDTagindex].data.ToString());
                scanform.ShowDialog();
                
                bool scanSuccess = scanform.ReturnBool;
                if (scanSuccess)
                    Program.ShowErrorMessage("RFID Tag Found", 3000);
                else
                    Program.ShowErrorMessage("RFID Tag Not Detected", 3000);

                transaction.ObjectList[RFIDDetectedindex].data = scanSuccess.ToString();

                bool returnBool = transaction.ReturningKey;
                //Package all needed data for the thread into a single string
                string ThreadDataString = returnBool.ToString() + "," + transaction.ObjectList[RFIDTagindex].data.ToString();

                Thread afterTransScanThread = new Thread(MontanaLocationData.AfterTransactionScanThread);
                afterTransScanThread.Start(ThreadDataString);
            }
            return true;
        }

        /// <summary>
        /// This method is used as a seperate thread for scanning for an RFID tag after the transaction is over.
        /// </summary>
        /// <param name="Datastring">This is essentially a serialized string. The first character is a bool representing whether the user was
        ///                          taking or returning a key and the rest of the string is the last section of the RFID Tag EPC string.</param>
        public static void AfterTransactionScanThread(object Datastring)
        {
            bool Returning = bool.Parse(Datastring.ToString().Split(',')[0]);
            string RFIDTag = Datastring.ToString().Split(',')[1];
            TagReadData[] ScanData;

            try
            {
                ////Stage 1 Scan
                int StageOneScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ThingMagicRFIDreader.ReadTags(1000);
                    foreach (TagReadData d in ScanData)
                    {
                        if (d.EpcString.EndsWith(RFIDTag))
                            StageOneScanCount++;
                    }
                }

                ////Stage 2 Scan
                int StageTwoScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ThingMagicRFIDreader.ReadTags(1000);
                    foreach (TagReadData d in ScanData)
                    {
                        if (d.EpcString.EndsWith(RFIDTag))
                            StageTwoScanCount++;
                    }
                }

                ////Stage 3 Scan
                int StageThreeScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ThingMagicRFIDreader.ReadTags(1000);
                    foreach (TagReadData d in ScanData)
                    {
                        if (d.EpcString.EndsWith(RFIDTag))
                            StageThreeScanCount++;
                    }
                }
                Program.ThingMagicRFIDreader.DoneReading();

                ////////Write something to the transaction file and log file about this.
                Program.logEvent("After Trans Scans for Tag:" + RFIDTag + " in Stage 1,2,3 Respectively are " + StageOneScanCount + "," + StageTwoScanCount + "," + StageThreeScanCount);

                if (Returning)
                {
                    if ((StageOneScanCount > 0) && (StageTwoScanCount > 3) && (StageThreeScanCount > 6))
                    {
                        Program.logEvent("Tag: " + RFIDTag + " Return Scan Successful");
                        Program.logTransaction("Tag: " + RFIDTag + " Return Scan Successful " + StageOneScanCount + "," + StageTwoScanCount + "," + StageThreeScanCount);
                    }
                    else
                    {
                        Program.logEvent("Tag: " + RFIDTag + " Return Scan Unsuccessful");
                        Program.logTransaction("Tag: " + RFIDTag + " Return Scan Unsuccessful " + StageOneScanCount + "," + StageTwoScanCount + "," + StageThreeScanCount);
                    }
                }
                else
                {
                    if ((StageOneScanCount < 10) && (StageTwoScanCount < 5) && (StageThreeScanCount < 3))
                    {
                        Program.logEvent("Tag: " + RFIDTag + " Removal Scan Successful");
                        Program.logTransaction("Tag: " + RFIDTag + " Removal Scan Successful " + StageOneScanCount + "," + StageTwoScanCount + "," + StageThreeScanCount);
                    }
                    else
                    {
                        Program.logEvent("Tag: " + RFIDTag + " Removal Scan Unsuccessful");
                        Program.logTransaction("Tag: " + RFIDTag + " Removal Scan Unsuccessful " + StageOneScanCount + "," + StageTwoScanCount + "," + StageThreeScanCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.logEvent("ScanThreadError: " + ex.Message);
            }
        }

        internal override void runReminders()
        {
            //Run Reminder at 6pm and 7pm Mon-Fri
            Program.logEvent("run reminders-DateTime.Now--" + DateTime.Now.ToString());
            if ((DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0)) && (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, (Program.REMINDER_INTERVAL / 60000), 0)) && //convert reminder interval to minutes
                  (DateTime.Today.DayOfWeek != DayOfWeek.Saturday) && (DateTime.Today.DayOfWeek != DayOfWeek.Sunday)) 
            {
                Program.logEvent("Reminders for 6PM");

                //Open SQLite Database connection
                //Look for any entries which have today's date and to not have a return datetime
                foreach (string cardno in RetrieveLateIDNumbers())
                {
                    //Send email/text to users which havent returned their keys
                    SendTxt(cardno);
                    SendEmail(cardno);
                }                
            }

            if ((DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0)) && (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, (Program.REMINDER_INTERVAL / 60000), 0)) && //convert reminder interval to minutes
                  (DateTime.Today.DayOfWeek != DayOfWeek.Saturday) && (DateTime.Today.DayOfWeek != DayOfWeek.Sunday)) 
            {
                Program.logEvent("Reminders for 7PM");

                //Open SQLite Database connection
                //Look for any entries which have today's date and to not have a return datetime
                foreach (string cardno in RetrieveLateIDNumbers())
                {                    
                    //Send email/text to users which havent returned their keys
                    SendTxt(cardno);
                    SendEmail(cardno);
                }                
            }
        }

        private void SendTxt(string userID)
        {
            if (userID != "")
            {
                Program.logEvent("Sending text for University of Montana User " + userID);
                Program.emailMgr.GenerateTextMsg("SAFEPAK REMINDER: Please return your keys to the Safepak Key Cabinet");
                Program.emailMgr.SendEmail(ReportType.BackgroundTextMsg, Program.userMgr.FindUserPhoneNumber(userID) + "@vtext.com");
                Thread.Sleep(1000); //Give the thread a rest so it doesn't try to send out emails too fast.
            }
        }

        private void SendEmail(string userID)
        {
            if (userID != "")
            {
                Program.logEvent("Sending Email for University of Montana User " + userID);
                Program.emailMgr.GenerateTextMsg("SAFEPAK REMINDER: Please return your keys to the Safepak Key Cabinet");
                Program.emailMgr.SendEmail(ReportType.BackgroundEmail, Program.userMgr.FindUserEmailAddress(userID));
                Thread.Sleep(1000); //Give the thread a rest so it doesn't try to send out emails too fast.
            }
        }
        
        private List<string> RetrieveLateIDNumbers()
        {
            ///
            ///Retrieve unfinished transactions from LOCAL SQLite DATABASE
            ///
            List<string> CardNumbers = new List<string>();
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

                    cmd.CommandText = @"SELECT * FROM RES_KEY WHERE (ID_CARD_NO_IN is null or ID_CARD_NO_IN = '')";
                    SQLiteDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        DateTime Datetime = Convert.ToDateTime(r["OUT_DATETIME"]);

                        //Select Card records which have a Date of today, yesterday or two days ago. This way records which have been accidentally created will not continue to send reminders forever
                        if ((Datetime.Date == DateTime.Today) || (Datetime.Date == DateTime.Today - new TimeSpan(1, 0, 0, 0)) || (Datetime.Date == DateTime.Today - new TimeSpan(2, 0, 0, 0)))
                            CardNumbers.Add(Convert.ToString(r["ID_CARD_NO_OUT"]));                        
                    }

                    dbConn.Close();
                }
                Program.logEvent("Local SQLite Database Transaction Inserted Successfully");

                if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
            }
            return CardNumbers;
        }


        /// <summary>
        /// Header - generates the coma separated header string to match CSV_Entry
        /// </summary>
        public override string  Header
        {
	        get 
	        {
                return "Date, Time, Box number, Door opened, Card number, Card name, Returning, Tag Number, Tag Detected";
	        }
	        internal set 
	        {
                Header = value; 
	        }
        }        

        public class MontanaTransactionData : LocationData.LocationTransactionData
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
            string tagnum { get; set; }
            bool tagdetected { get; set; }
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
            public string RFIDTagNumber { get { return (string)ObjectList[LocateIndexOfDataObjectByName("TagNum")].data; } set { tagnum = value; ObjectList[LocateIndexOfDataObjectByName("TagNum")].data = value; } } //The RFID Tag number associated with the boxnumber
            public bool RFIDTagDetected { get { return bool.Parse(ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data.ToString()); } set { tagdetected = value; ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data = value; } } //states if the tag detection process was successful (based upon whether the tag was leaving or returning)

            public MontanaTransactionData()
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
                ObjectList.Add(new TransactionDataObject("TagNum", tagnum, true, true));
                ObjectList.Add(new TransactionDataObject("TagDetected", tagdetected, true, true));
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
                MontanaTransactionData copy = new MontanaTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;
                copy.RFIDTagNumber = this.RFIDTagNumber;
                copy.RFIDTagDetected = this.RFIDTagDetected;

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
                RFIDTagNumber = ObjectList[LocateIndexOfDataObjectByName("TagNum")].data.ToString();
                RFIDTagDetected = bool.Parse(ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data.ToString());

                if (Program.ENABLE_SQLITE)
                {
                    if (!ReturningKey)
                        InsertDBTransaction();
                    else
                        UpdateDBTransaction();
                } 

                try
                {
                    return String.Format("{0},{1},{2:d},{3:b},{4},{5},{6:b},{7},{8:b}",
                                        Date, Time, BoxNumber, DoorOpened.ToString(), CardNumber, CardName, ReturningKey, RFIDTagNumber, RFIDTagDetected);
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    return "transaction recording error";
                }
            }

            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), CardNumber, CardName, RFIDTagNumber, RFIDTagDetected.ToString()};
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
                                       KEY_BOX_NO,  
                                       ID_CARD_NO_OUT,
                                       RFID_TAG_NO,
                                       RFID_TAG_OUT_DETECTED,                                       
                                       KIOSK_ID
                                      )
                                   VALUES
                                      (
                                       @KioskDateOut,
                                       @BoxNum,
                                       @CardNum,
                                       @RFIDTagNo,
                                       @RFIDTagOutDetect,                                       
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
                            ParameterName = "@CardNum",
                            Value = this.CardNumber,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@RFIDTagNo",
                            Value = this.RFIDTagNumber,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@RFIDTagOutDetect",
                            Value = this.RFIDTagDetected,
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
                                       ID_CARD_NO_IN          = @CardNoIn,
                                       RFID_TAG_IN_DETECTED   = @TagDetectIn                                       
                                    WHERE
                                       ID_CARD_NO_OUT         = @CardNoIn
                                        AND
                                       OUT_DATETIME           = (SELECT max(OUT_DATETIME) FROM RES_KEY WHERE ID_CARD_NO_OUT = @CardNoIn)
                                       "; //This query should update the newest transaction for CardNoIn

                                           //////Alternatively perhaps we should select the oldest transaction for CardNoIn which hasn't been finished
                                       //     ID_CARD_NO_OUT         = @CardNoIn
                                       //       AND
                                       //     OUT_DATETIME           = (SELECT min(OUT_DATETIME) FROM RES_KEY WHERE ID_CARD_NO_IN = '')

                        //parameterized update - more flexibility on parameter creation

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {// SQLite date format is: yyyy-MM-dd HH:mm:ss
                            ParameterName = "@ReturnDate",
                            Value = this.SQLiteDateAndTime,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@CardNoIn",
                            Value = this.CardNumber,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@TagDetectIn",
                            Value = this.RFIDTagDetected,
                        });
                        
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KeyBoxNo",
                            Value = this.BoxNumber
                        });

                        cmd.ExecuteNonQuery();
                    }
                    Program.logEvent("Local SQLite Database Transaction Updated Successfully");

                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
            }
        }
    }
}
