using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Data.SQLite;

namespace KeyCabinetKiosk
{
    class OregonStateULocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; } 

        public OregonStateULocationData():base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReturningQuestion, WhenToUse.Both, new string[] { "Are You Taking", "Or Returning A Key?" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReservationNumberEntry, WhenToUse.Both, new string [] {"Enter\r\nReservation\r\nNumber"}));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.OperatorNumberEntry, WhenToUse.Taking));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReservationConfirmQuestion, WhenToUse.Taking));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.OdometerEntry, WhenToUse.Returning));
        }
        
        /// <summary>
        /// Header - generates the coma separated header string to match CSV_Entry
        /// </summary>
        public override string Header
        {
            get
            {
                return "Date, Time, Box Number, Door Opened, Reservation Number, Operator Number, Odometer Reading";
            }
            internal set
            {
                Header = value;
            }
        }

        internal override bool confirmReservation(ref LocationData.LocationTransactionData transaction)
        {
            int intReturnValue = 0;
            SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
            SqlParameterCollection Parameters = comm.Parameters;
            SqlParameterCollection ReturnParameters;
            try
            {
                if (!transaction.ReturningKey)
                {
                    //Add Input Values
                    Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, transaction.AccessCode, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskDateOut", ParameterDirection.Input, String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now), SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskNumOut", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4, '0'), SqlDbType.VarChar));
                    //Add Return Value
                    Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                    ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_TestResvNum", Parameters);
                    if (ReturnParameters == null) //Error msg already given by SQLManager
                        return false;

                    //set the return code (of the stored procedure) to the results sent back.
                    intReturnValue = (int)ReturnParameters["Return_Val"].Value;

                    switch (intReturnValue)
                    {
                        case 1:
                            //Success
                            return true;
                        case 2:
                            //Failure: Reservation does not exist
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Reservation Confirmation Error: Reservation Does not exist");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Confirmation Error: Reservation Does not exist", "", 0);
                            return false;
                        case 3:
                            //Failure: Current Date is not correct for your reservation
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Reservation Confirmation Error: The pickup date for the reservation is not the current date");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Confirmation Error: The pickup date for the reservation is not the current date", "", 0);
                            return false;
                        case 4:
                            //Failure: Reservation is for a different key cabinet
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Reservation Confirmation Error: The Reservation is not for this kiosk location");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Confirmation Error: The Reservation is not for this kiosk location", "", 0);
                            return false;
                    }
                }
                else
                {
                    //Add Input Values
                    Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, transaction.AccessCode, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskDateIn", ParameterDirection.Input, String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now), SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskNumIn", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4, '0'), SqlDbType.VarChar));
                    //Add Return Value
                    Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                    ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_TestResvRtn", Parameters);
                    if (ReturnParameters == null) //Error msg already given by SQLManager
                        return false;

                    //set the return code (of the stored procedure) to the results sent back.
                    intReturnValue = (int)ReturnParameters["Return_Val"].Value;

                    switch (intReturnValue)
                    {
                        case 1:
                            //Success
                            if (transaction.ReturningKey)
                            {
                                YesNoForm yesno = new YesNoForm("");
                                return getBoxNum(ref transaction, ref yesno);
                            }

                            return true;
                        case 15:
                            //Failure: Reservation does not exist
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Reservation Return Confirmation Error: Reservation Does not exist");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Return Confirmation Error: Reservation Does not exist", "", 0);
                            return false;
                        case 16:
                            //Failure: Reservation is for a different key cabinet
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Reservation Return Confirmation Error: The Reservation is not for this kiosk location");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Reservation Return Confirmation Error: The Reservation is not for this kiosk location", "", 0);
                            return false;
                    }
                }
            }            
            catch (Exception ex)
            {
                //An application exception was thrown - process/display exception.
                Program.logEvent("GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message);
                Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message, "", 0);
                return false;
            }
            return true;
        }

        private bool getBoxNum(ref LocationTransactionData transaction, ref YesNoForm ReservationConfirmScreen)
        {
            //
            //Put new stored procedure into place
            //Confirm Operator Number
            //Set BoxNumber
            //Display Destination & Return Time
            //
            int intReturnValue = 0;
            OregonStateUTransactionData transData = (OregonStateUTransactionData)transaction;
            SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
            SqlParameterCollection Parameters = comm.Parameters;
            SqlParameterCollection ReturnParameters;

            try
            {
                //Add inputs
                Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, transData.AccessCode, SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("OperNum", ParameterDirection.Input, transData.OperatorNumber, SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("KioskDateOut", ParameterDirection.Input, String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now), SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("KioskNumOut", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4,'0'), SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("KeyNumMax", ParameterDirection.Input, Program.NUMBER_RELAYS, SqlDbType.Int));
                Parameters.Add(Program.SqlManager.CreateParameter("KeyMode", ParameterDirection.Input, transaction.ReturningKey, SqlDbType.Int));
                //Add Outputs & Return
                Parameters.Add(Program.SqlManager.CreateParameter("KeyNumOut", ParameterDirection.Output, 0, SqlDbType.Int));
                Parameters.Add(Program.SqlManager.CreateParameter("DestCity", ParameterDirection.Output, "", SqlDbType.VarChar, 20));
                Parameters.Add(Program.SqlManager.CreateParameter("DueDate", ParameterDirection.Output, "", SqlDbType.VarChar, 20));
                Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_GetBoxNum", Parameters);
                if (ReturnParameters == null) //Error msg already given by SQLManager
                    return false;

                    //set the return code (of the stored procedure) to the results sent back.
                    intReturnValue = (int)ReturnParameters["Return_Val"].Value;
                    Program.logEvent("Show Reservation Data: Return Value - " + intReturnValue);
                    switch (intReturnValue)
                    {
                        case 1:
                            //Success                            
                            Program.logEvent("Show Reservation Data: Box Number - " + (int)ReturnParameters["KeyNumOut"].Value);                            
                            transaction.BoxNumber = (int)ReturnParameters["KeyNumOut"].Value;
                            Program.logEvent("Destination City: " + (string)ReturnParameters["DestCity"].Value);
                            Program.logEvent("Due Date: " + (string)ReturnParameters["DueDate"].Value);

                            if (!transaction.ReturningKey)
                            {
                                //return time prep
                                string dt = (string)ReturnParameters["DueDate"].Value;
                                string[] datetime = dt.Replace("  ", " ").Split(' ');
                                Program.logEvent("ScreenPrint Datetime0:" + datetime[0]);
                                Program.logEvent("ScreenPrint Datetime1:" + datetime[1]);
                                Program.logEvent("ScreenPrint Datetime2:" + datetime[2]);
                                Program.logEvent("ScreenPrint Datetime3:" + datetime[3]);

                                ReservationConfirmScreen.setFormMessage("Trip To:\r\n" + (string)ReturnParameters["DestCity"].Value + "\r\nDue Back:\r\n" + datetime[0] + '-' + datetime[1] + '-' + datetime[2].Remove(0, 2) + ' ' + datetime[3].Remove(datetime[3].Length - 2, 2));
                                Program.logEvent("Reservation Confirmation In Progress");
                            }
                            return true;

                        case 5:
                            //Failure - 
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - can't find data - perhaps RES_MAIN table reservation row does not exist");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - can't find data - perhaps RES_MAIN table reservation row does not exist", "", 0);
                            return false;

                        case 6:
                            //Failure - 
                            Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - Key # not in range");
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - Key # not in range", "", 0);
                            return false;                    
                }
            }
            catch (Exception ex)
            {
                //An application exception was thrown - process/display exception.
                Program.logEvent("GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message);
                Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message, "", 0);
                return false;
            }
            return true;
        }

        internal override bool showReservationData(ref LocationTransactionData transaction, ref YesNoForm ReservationConfirmScreen)
        {
            return getBoxNum(ref transaction, ref ReservationConfirmScreen);
        }
    
        internal override bool validateOdometerReading(LocationData.LocationTransactionData transaction)
        {
            int intReturnValue = 0;
            OregonStateUTransactionData thistransaction = new OregonStateUTransactionData();
            thistransaction = (OregonStateUTransactionData)transaction;
            SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
            SqlParameterCollection Parameters = comm.Parameters;
            SqlParameterCollection ReturnParameters;

            try
            {
                //Add Input Params
                Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, thistransaction.AccessCode, SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("OperNum", ParameterDirection.Input, thistransaction.OperatorNumber, SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("KioskDateIn", ParameterDirection.Input, String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now), SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("KioskNumIn", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4, '0'), SqlDbType.VarChar));
                Parameters.Add(Program.SqlManager.CreateParameter("MileageEnd", ParameterDirection.Input, thistransaction.Odometer, SqlDbType.Int));
                Parameters.Add(Program.SqlManager.CreateParameter("MileageDiff", ParameterDirection.Input, Program.MILEAGE_DIFFERENCE_ALLOWED, SqlDbType.Int));
                //Add Return Value
                Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_TestMileageIn", Parameters);

                if (ReturnParameters == null) //Error msg already given by SQLManager
                    return false;

                //set the return code (of the stored procedure) to the results sent back.
                intReturnValue = (int)ReturnParameters["Return_Val"].Value;

                switch (intReturnValue)
                {
                    case 1:
                        //Success
                        Program.logEvent("Odometer Reading Verified Successfully");
                        return true;
                    case 10:
                        //Failure
                        Program.ShowErrorMessage("Sorry Your Entry Did\r\nNot Find A Match\r\nPlease Re-Enter Or\r\nCall " + Program.SERVICE_MANAGER_NUMBER, 5000);
                        Program.logEvent("Failure - can't find transaction data row - perhaps RES_KEY table reservation row does not exist");
                        Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - can't find transaction data row - perhaps RES_KEY table reservation row does not exist", "", 0);
                        return false;
                    case 11:
                        //Failure here is now deemed unnecessary. A note is made of it and we move on. 4/26/13
                        Program.logEvent("Odometer Reading: Was too far based on MILEAGE_DIFFERENCE_ALLOWED or it was less than or equal to the previous odometer reading");
                        //MessageStrings.OdometerError();
                        //Thread.Sleep(4000);
                        //Program.logEvent("Odometer Reading Rejected: Either it was too far based on MILEAGE_DIFFERENCE_ALLOWED or it was less than or equal to the previous odometer reading");
                        Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Odometer Reading Rejected: Either it was too far based on MILEAGE_DIFFERENCE_ALLOWED or it was less than or equal to the previous odometer reading", "", 0);
                        return true;                    
                }
            }
            catch (Exception ex)
            {
                //An application exception was thrown - process/display exception.
                Program.logEvent("GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message);
                Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + "ReservationConfirmation: " + ex.Message, "", 0);
                return false;
            }
            return false;
        }
        
        public class OregonStateUTransactionData : LocationData.LocationTransactionData
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
            string operatornum { get; set; }
            int odometer { get; set; }
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
            public string OperatorNumber { get { return ObjectList[LocateIndexOfDataObjectByName("Operator")].data.ToString(); } set { operatornum = value; ObjectList[LocateIndexOfDataObjectByName("Operator")].data = value; } }
            public int Odometer { get { return int.Parse(ObjectList[LocateIndexOfDataObjectByName("Odometer")].data.ToString()); } set { odometer = value; ObjectList[LocateIndexOfDataObjectByName("Odometer")].data = value; } } 

            public OregonStateUTransactionData()
                : base()
            {
                AddAdditionalDataObjectsToList();
                OperatorNumber = "";
            }

            public override void ClearData()
            {
                base.ClearData();                
            }

            private void AddAdditionalDataObjectsToList()
            {
                ObjectList.Add(new TransactionDataObject("Operator", operatornum, false, true));
                ObjectList.Add(new TransactionDataObject("Odometer", odometer, false, true));
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
                OregonStateUTransactionData copy = new OregonStateUTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.OperatorNumber = this.OperatorNumber;
                copy.Odometer = this.Odometer;
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
                if (ReturningKey)
                {
                    //Update existing row
                    UpdateDBTransaction();
                }
                else
                {
                    //Insert 
                    InsertDBTransaction();
                }

                try
                {
                    return String.Format("{0},{1},{2:d},{3},{4},{5},{6},{7}",
                                        Date, Time, BoxNumber, DoorOpened.ToString(), AccessCode, OperatorNumber, ReturningKey, Odometer);
                }
                catch (Exception ex)
                {
                    Program.logEvent("error creating csv entry" + ex.Message);
                    Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "error creating csv entry" + ex.Message, "", Odometer);
                    return "transaction recording error";
                }
            }

            /// <summary>
            /// Returns an array of the data which is applicable to an OSU transaction
            /// </summary>
            /// <returns></returns>
            public override string[] TransactionData()
            {
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), AccessCode, OperatorNumber, Odometer.ToString(), ReturningKey.ToString() };
            }

            private void InsertDBTransaction()
            {
                ///
                ///INSERT TRANSACTION INTO REMORE SQL SERVER DATABASE
                ///

                #region SQL SERVER
                int intReturnValue = 0;
                SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
                SqlParameterCollection Parameters = comm.Parameters;
                SqlParameterCollection ReturnParameters;

                try
                {
                    //Input Parameters
                    Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, this.AccessCode, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("OperNum", ParameterDirection.Input, this.OperatorNumber, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskDateOut", ParameterDirection.Input, this.DateAndTime, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskNumOut", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4, '0'), SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KeyNumMax", ParameterDirection.Input, Program.NUMBER_RELAYS, SqlDbType.Int));
                    //Return Value
                    Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                    ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_InsRES_KEY", Parameters);
                                        
                    //set the return code (of the stored procedure) to the results sent back.
                    intReturnValue = (int)ReturnParameters["Return_Val"].Value;

                    switch (intReturnValue)
                    {                            
                        case 1:
                            //Success
                            Program.logEvent("Transaction Data Successfully Inserted into Database");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Transaction Data Successfully Inserted into Database", "", Odometer);
                            break;
                        case 7:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - can't save data - perhaps server is down.");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - can't save data - perhaps server is down.", "", Odometer);
                            break;
                        case 8:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - Key # is not in range(CAST(EQ_MAIN.PARKING_STALL AS int)).");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - Key # is not in range(CAST(EQ_MAIN.PARKING_STALL AS int)).", "", Odometer);
                            break;
                        case 9:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - vehicle number not found (RES_MAIN.MISC_CMT_4).");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - vehicle number not found (RES_MAIN.MISC_CMT_4).", "", Odometer);
                            break;                        
                    }
                }
                catch (Exception ex)
                {
                    //An application exception was thrown - process/display exception.
                    Program.logEvent("GENERAL EXCEPTION:" + "Transaction Insertion: " + ex.Message);
                    Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + "Transaction Insertion: " + ex.Message, "", Odometer);
                    Program.ShowErrorMessage("Database Error\r\nTransaction Data Saved\r\nPlease Call\r\n" + Program.SERVICE_MANAGER_NUMBER, 5000);
                }
                #endregion

                ///
                ///INSERT TRANSACTION INTO LOCAL SQLite DATABASE
                ///
                #region SQLite 

                if (!Program.ENABLE_SQLITE)
                    return;

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
                        //    '56650'     , '501889889'  ,       38318         ,    '2012-12-20'  , 'RETURNED'          3        '2012-12-20'    , 'COTTAGE GROVE','OR'	  '011563'
                        //RESV_RESERV_NO   OPER_OPER_NO   METER_1_READING_OUT   REQUIRED_DATETIME   STATUS          KeyBoxNum  DUE_DATETIME_ORIG   DESTINATION_CITY      EQ_EQUIP_NO


                        cmd.CommandText = @"INSERT INTO RES_KEY 
                                      (
                                       RESV_RESERV_NO, 
                                       OPER_OPER_NO, 
                                       OUT_DATETIME, 
                                       EQ_EQUIP_NO,
                                       KIOSK_OUT_NO,
                                       KEY_BOX_OUT_NO,
                                       INS_ERROR_FLAG
                                      )
                                   VALUES
                                      (
                                       @ResvNum,
                                       @OperNum,
                                       @KioskDateOut,
                                       @EquipNum,
                                       @KioskNum,
                                       @KeyNum,
                                       @ErrorFlag
                                      )";


                        //parameterized insert - more flexibility on parameter creation

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@ResvNum",
                            Value = this.AccessCode,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@OperNum",
                            Value = this.OperatorNumber,
                        });

                        // SQLite date format is: yyyy-MM-dd HH:mm:ss
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KioskDateOut",
                            Value = this.SQLiteDateAndTime,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@EquipNum",
                            Value = " ",
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KioskNum",
                            Value = Program.KIOSK_ID.PadLeft(4,'0'),
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KeyNum",
                            Value = this.BoxNumber,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@ErrorFlag",
                            Value = intReturnValue
                        });

                        cmd.ExecuteNonQuery();
                    }
                    Program.logEvent("Local SQLite Database Transaction Inserted Successfully");

                    if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                }
                #endregion
            }

            private void UpdateDBTransaction()
            {
                ///
                ///UPDATE TRANSACTION INTO REMOTE SQL SERVER DATABASE
                ///
                #region SQL SERVER 

                int intReturnValue = 0;
                int intOutput1 = 0;
                SqlCommand comm = new SqlCommand(); //This is needed because there is no public contructor for SqlParameterCollection
                SqlParameterCollection Parameters = comm.Parameters;
                SqlParameterCollection ReturnParameters;

                try
                {
                    //Input Parameters
                    Parameters.Add(Program.SqlManager.CreateParameter("ResvNum", ParameterDirection.Input, this.AccessCode, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("OperNum", ParameterDirection.Input, this.OperatorNumber, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskDateIn", ParameterDirection.Input, this.DateAndTime, SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("KioskNumIn", ParameterDirection.Input, Program.KIOSK_ID.PadLeft(4, '0'), SqlDbType.VarChar));
                    Parameters.Add(Program.SqlManager.CreateParameter("MileageEnd", ParameterDirection.Input, this.Odometer, SqlDbType.Int));
                    Parameters.Add(Program.SqlManager.CreateParameter("KeyNumMax", ParameterDirection.Input, Program.NUMBER_RELAYS, SqlDbType.Int));
                    //Return Values
                    Parameters.Add(Program.SqlManager.CreateParameter("KeyNumIn", ParameterDirection.Output, 0, SqlDbType.Int));
                    Parameters.Add(Program.SqlManager.CreateParameter("Return_Val", ParameterDirection.ReturnValue, 0, SqlDbType.Int));

                    ReturnParameters = Program.SqlManager.SqlStoredProcedure("SPA_ReturnKey", Parameters);

                    intOutput1 = (int)ReturnParameters["KeyNumIn"].Value;

                    //set the return code (of the stored procedure) to the results sent back.
                    intReturnValue = (int)ReturnParameters["Return_Val"].Value;

                    switch (intReturnValue)
                    {
                        case 1:
                            //Success
                            Program.logEvent("Transaction Data Successfully Updated into Database");
                            break;
                        case 12:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - can't save data - perhaps server is down.");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - can't save data - perhaps server is down.", "", Odometer);
                            break;
                        case 13:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - RES_KEY table INSERT failed.");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - RES_KEY table INSERT failed.", "", Odometer);
                            break;
                        case 14:
                            //Failure
                            Program.ShowErrorMessage("Database Error\r\nTry Again Or\r\nPlease Call " + Program.SERVICE_MANAGER_NUMBER, 5000);
                            Program.logEvent("Failure - Key# not in range (CAST(em.PARKING_STALL as int)).");
                            Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Failure - Key# not in range (CAST(em.PARKING_STALL as int)).", "", Odometer);
                            break;
                        
                    } 
                }
                catch (Exception ex)
                {
                    //An application exception was thrown - process/display exception.
                    Program.logEvent("GENERAL EXCEPTION:" + "Transaction Update: " + ex.Message);
                    Program.SqlManager.ErrorDatabaseEntry(AccessCode, OperatorNumber, DateTime.Now.ToString(), Program.KIOSK_ID, 0, "GENERAL EXCEPTION:" + "Transaction Update: " + ex.Message, "", Odometer);
                    Program.ShowErrorMessage("Database Error\r\nTransaction Data Saved\r\nPlease Call\r\n" + Program.SERVICE_MANAGER_NUMBER, 5000);
                }
                #endregion

                ///
                ///UPDATE TRANSACTION INTO LOCAL SQLite DATABASE
                ///
                #region SQLite 

                if (!Program.ENABLE_SQLITE)
                    return;

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
                        //    '56650'     , '501889889'  ,       38318         ,    '2012-12-20'  , 'RETURNED'          3        '2012-12-20'    , 'COTTAGE GROVE','OR'	  '011563'
                        //RESV_RESERV_NO   OPER_OPER_NO   METER_1_READING_OUT   REQUIRED_DATETIME   STATUS          KeyBoxNum  DUE_DATETIME_ORIG   DESTINATION_CITY      EQ_EQUIP_NO


                        cmd.CommandText = @"UPDATE RES_KEY
                                    SET
                                       KIOSK_IN_NO            = @KioskNumIn,
                                       KEY_BOX_IN_NO          = @KeyNumIn,
                                       RETURN_DATETIME        = @ReturnDate,
                                       METER_1_READING_OUT    = @MileageBegin,
                                       METER_1_READING_IN     = @MileageEnd
                                    WHERE
                                       RESV_RESERV_NO         = @ResvNum
                                       ";


                        //parameterized update - more flexibility on parameter creation

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KioskNumIn",
                            Value = Program.KIOSK_ID.PadLeft(4, '0'),
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@KeyNumIn",
                            Value = this.BoxNumber,
                        });

                        // SQLite date format is: yyyy-MM-dd HH:mm:ss
                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@ReturnDate",
                            Value = this.SQLiteDateAndTime,
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@MileageBegin",
                            Value = " ",
                        });

                        cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        {
                            ParameterName = "@MileageEnd",
                            Value = this.Odometer,
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
                #endregion
            }
        }
    }
}