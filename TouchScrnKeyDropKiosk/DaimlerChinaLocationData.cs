using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml;
using System.Data.SQLite;
using ImpinjRFID;
using Impinj.OctaneSdk;
using XmlConfig;

namespace KeyCabinetKiosk
{
    class DaimlerChinaLocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; }
        private System.Timers.Timer SendInventoryTimer { get; set; }
        private bool FirstInventory { get; set; }

        public DaimlerChinaLocationData(): base()
        {
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.ReservationAlphaNumberEntry, WhenToUse.Both, new string[] { "输入预订号码" }));
            CustomerScreenOrderList.Add(new CustomerScreen(CustomerScreenType.RetrieveRFIDTagNumber, WhenToUse.Both));

            //////BE SURE TO TURN BACK ON FOR FURTHER TESTING
            SendInventoryTimer = new System.Timers.Timer(1000);
            SendInventoryTimer.Elapsed += new System.Timers.ElapsedEventHandler(SendInventoryTimer_Elapsed);
            SendInventoryTimer.AutoReset = true; //needs to be true except for testing
            SendInventoryTimer.Start();
            FirstInventory = true;
            ////SendInventoryTimer_Elapsed(this, null);
        }

        //////BE SURE TO TEST FOR COLLISIONS WITH NORMAL TRANSACTIONS
        void SendInventoryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {   
            SendInventoryTimer.Interval = Program.REMINDER_INTERVAL; //after the initial scan, set the interval to happen every 10 minutes            
            int readtime; //inventory read time       
            if (Program.ImpinjRFIDreader.IsReading)
            {
                Program.logEvent("RFID Inventory is waiting for RFID reader");
            }
            //Wait for the RFID read to be done reading.
            while (Program.ImpinjRFIDreader.IsReading)
                Thread.Sleep(1000);

            if (!FirstInventory)         //The initial read that happens at program startup should only be for 
                readtime = Program.RFID_INVENTORY_READ_TIME; //10 seconds so it has less chance of immediately interfering with transactions.
            else
            {
                readtime = 10000;
                FirstInventory = false;
            }

            TagReport inventory = Program.ImpinjRFIDreader.ReadTags(readtime, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE,
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);
            List<Tag> UniqueInventory = OrganizeTagReads(inventory);

            Program.ImpinjRFIDreader.DoneReading();

            string InventoryString = "";
            if (UniqueInventory != null)
            {
                foreach (Tag t in UniqueInventory)
                {
                    string i = t.Epc.ToString().Replace(" ", "");
                    InventoryString += i.Substring(i.Length - 8) + ",";
                }
                InventoryString = InventoryString.Remove(InventoryString.Length - 1);
                Program.logEvent("RFID Inventory Sent with " + UniqueInventory.Count + " unique tags");
            }
            else
            {
                Program.logEvent("RFID Inventory Sent with 0 unique tags");
            }
           
            makeHTTPPost(HTTPPostType.RFIDInventory, InventoryString, "", "");            
        }
        
        /// <summary>
        /// Header - generates the comma separated header string to match CSV_Entry
        /// </summary>
        public override string Header
        {
            get
            {
                return "Date, Time, Box number, Door opened, Reservation Number, Returning, TransactionNum, Tag Number, Tag Detected";
            }
            internal set
            {
                Header = value;
            }
        }

        internal override bool confirmReservation(ref LocationData.LocationTransactionData transaction)
        {
            //This allows the admin to set up access codes for each box and then use them to allow the users to override the reservation system locally.
            //int box = Program.passwordMgr.FindLocationForCode(transaction.AccessCode);
            //if (box != -1)
            //{
            //    Program.logEvent("Local Access Code Overriding Reservation System");
            //    transaction.BoxNumber = box;
            //    Program.logEvent("Box Number = " + box.ToString());
            //    int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
            //    Program.logEvent("transnumindex = " + transnumindex.ToString());
            //    transaction.ObjectList[transnumindex].data = "Code Override";
            //    return true;
            //}
            //This uses the standing reservation system to retrieve the box number, reservation status and transaction number from the reservation server.
            //else 
            //{
                if (!makeHTTPPost(HTTPPostType.CheckReservation, transaction.AccessCode, "", "", ref transaction))
                {
                    Program.ShowErrorMessage(LanguageTranslation.RESERVATION_VERIFICATION_ERROR, 10000);
                    return false;
                }
                else
                    return true;
            //}
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
            //ErrorForm commwithserver = Program.ShowMessage("Communicating with\r\nServer...");
            string methodtype = "";
            NameValueCollection methodparams = new NameValueCollection();

            string securityToken = getDaimlerSecurityToken();
            if (securityToken == "")
            {
                Program.logEvent("Daimler Security Token Error");
                return false;
            }

            switch (MethodType)
            {
                case HTTPPostType.CheckReservation:
                    methodtype = "Reservation";
                    methodparams.Add("kioskLocation", Program.KIOSK_ID);
                    methodparams.Add("reservationNumber", extradata1.ToUpper()); //any alpha characters in the res number must be capitals                   
                    methodparams.Add("accessToken", securityToken);
                    break;
                case HTTPPostType.KeyOut:
                    methodtype = "TransactionComplete";
                    methodparams.Add("kioskLocation", Program.KIOSK_ID);
                    methodparams.Add("transactionNumber", extradata1);
                    methodparams.Add("reservationNumber", extradata2.ToUpper());
                    methodparams.Add("status", "SUCCESS");
                    methodparams.Add("accessToken", securityToken);
                    break;
                case HTTPPostType.TransactionFail:
                    int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                    string TNumber = transaction.ObjectList[transnumindex].data.ToString();
                    
                    methodtype = "TransactionComplete";
                    methodparams.Add("kioskLocation", Program.KIOSK_ID);
                    methodparams.Add("transactionNumber", TNumber);
                    methodparams.Add("reservationNumber", transaction.AccessCode.ToUpper());
                    methodparams.Add("status", "FAIL");
                    methodparams.Add("accessToken", securityToken);
                    break;
                case HTTPPostType.RFIDInventory:
                    methodtype = "RFIDInventory";
                    methodparams.Add("kioskLocation", Program.KIOSK_ID);
                    methodparams.Add("inventory", extradata1);
                    methodparams.Add("accessToken", securityToken);
                    break;                
            }
            bool ret = PostWAWaterData(methodparams, Program.CUSTOMER_DATA_SERVER, methodtype, ref transaction);
            //commwithserver.Dispose();
            return ret;
        }

        internal override bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3)
        {
            LocationTransactionData fakedata = new DaimlerChinaTransactionData();
            return makeHTTPPost(MethodType, extradata1, extradata2, extradata3, ref fakedata);
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            HttpWebRequest senderinfo = (HttpWebRequest)sender;
            if (senderinfo.Address.Host == Program.RESERVATION_DATABASE_CONNECTION_STRING.Split('/')[2])
                return true;
            else
                return false;
        }

        private string getDaimlerSecurityToken()
        {
            //SEND HTTP POST AND HANDLE RESPONSE 
            StreamWriter myWriter = null;
            string TransactionResult = "0", strPost = ""; //TransactionResult needs to be initialized to something so that you know for sure when it is empty
            
            strPost = "apiUid=kiosk&apiSecret=e10adc3949ba59abbe56e057f20f883e";

            HttpWebRequest serverReq = (HttpWebRequest)WebRequest.Create(Program.RESERVATION_DATABASE_CONNECTION_STRING);
            serverReq.Method = "POST";
            serverReq.ContentLength = strPost.Length;
            serverReq.ContentType = "application/x-www-form-urlencoded";

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

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
                //parse out token
                if (TransactionResult.Substring(2, 6) == "result")
                {

                    string token = TransactionResult.Replace('"', ' ').Replace(" ", "");
                    token = token.Split(':')[2];
                    token = token.Substring(0, 32);
                    return token;
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                Program.logEvent(ex.Message);
                return "";
            }
            finally
            {
                if (myWriter != null)
                    myWriter.Close();
            }
        }

        private bool PostWAWaterData(NameValueCollection methodParameters, string serverName, string methodName, ref LocationData.LocationTransactionData transaction)
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

            HttpWebRequest serverReq = (HttpWebRequest)WebRequest.Create(serverName + "/" + methodName + ".shtml");
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
                //XmlNamespaceManager xnsm = new XmlNamespaceManager(xml.xmlDoc.NameTable);
                //xnsm.AddNamespace("ns", "Daimler.WebServices.SafePak");
                root = xml.GetNode("Result");
                
                //
                //Deal with transactionresult file
                switch (methodName)
                {                    
                    case "Reservation":
                        if (root.GetValue("ReturnCode").ToString() == "1")
                        {
                            Program.logEvent("Reservation Number Accepted.");
                            //set the box number
                            transaction.BoxNumber = int.Parse(root.GetValue("Box"));
                            Program.logEvent("Box Number = " + transaction.BoxNumber);

                            //set the RFID number
                            int rfidnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagNum"; });
                            Program.logEvent("rfidnumindex = " + rfidnumindex.ToString());
                            transaction.ObjectList[rfidnumindex].data = root.GetValue("RfidTag").ToString();
                            Program.logEvent("RFID Tag Number = " + transaction.ObjectList[rfidnumindex].data.ToString());

                            //set the taking/returning status
                            string action = root.GetValue("Action").ToString();
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
                            transaction.ObjectList[transnumindex].data = root.GetValue("TransactionNumber").ToString();
                            Program.logEvent("Transaction Number = " + transaction.ObjectList[transnumindex].data.ToString());
                        }
                        else
                        {
                            //error occurred
                            Program.logEvent("Check Reservation Error: " + root.GetValue("Error").ToString());
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Check Reservation Error: " + root.GetValue("Error").ToString(), "", 0);
                            return false;
                        }
                        break;
                    case "TransactionComplete":
                        //needs to be checked for the "OK" response
                        if (root.GetValue("ReturnCode").ToString() == "1")//(TransactionResult == "HTTP/1.1 200 OK")
                            Program.logEvent("Transaction Complete Message Accepted");
                        else
                        {
                            Program.logEvent("Transaction Complete Error: " + root.GetValue("Error").ToString());
                            int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, transaction.ObjectList[transnumindex].data.ToString(), DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Transaction Complete Error: " + root.GetValue("Error").ToString(), "", 0);
                        }
                        break;
                    case "RFIDInventory":
                        //needs to be checked for the "OK" response
                        if (root.GetValue("ReturnCode").ToString() == "1")//(TransactionResult == "HTTP/1.1 200 OK")
                            Program.logEvent("RFID Inventory Accepted");
                        else
                        {
                            Program.logEvent("RFID Inventory Error: " + TransactionResult);
                            int transnumindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "Transaction"; });
                            Program.SqlManager.ErrorDatabaseEntry(transaction.AccessCode, transaction.ObjectList[transnumindex].data.ToString(), DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Transaction Complete Error: " + root.GetValue("Error").ToString(), "", 0);
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

        //Used to deal with the RFID scanning thread which will run for approx 30 seconds after transaction ends to make sure the RFID tag is either
        //present or not depending on whether the key was taken or returned.
        internal override bool preDoorOpeningProcessing(LocationData.LocationTransactionData transaction)
        {
            if (!transaction.ReturningKey)
            {
                bool returnBool = transaction.ReturningKey;
                int RFIDTagindex = transaction.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagNum"; });
                //Package all needed data for the thread into a single string
                string ThreadDataString = returnBool.ToString() + "," + transaction.ObjectList[RFIDTagindex].data.ToString();

                Thread afterTransScanThread = new Thread(DaimlerChinaLocationData.AfterTransactionScanThread);
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
                    Program.ShowErrorMessage(LanguageTranslation.RFID_TAG_FOUND, 3000);
                else
                    Program.ShowErrorMessage(LanguageTranslation.RFID_TAG_NOT_DETECTED, 3000);

                transaction.ObjectList[RFIDDetectedindex].data = scanSuccess.ToString();

                bool returnBool = transaction.ReturningKey;
                //Package all needed data for the thread into a single string
                string ThreadDataString = returnBool.ToString() + "," + transaction.ObjectList[RFIDTagindex].data.ToString();

                Thread afterTransScanThread = new Thread(DaimlerChinaLocationData.AfterTransactionScanThread);
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
            TagReport ScanData;

            try
            {
                if (Program.ImpinjRFIDreader.IsReading)
                {
                    Program.logEvent("AfterTransactionThread is waiting for RFID reader");
                }
                //Wait for the RFID read to be done reading.
                while (Program.ImpinjRFIDreader.IsReading)
                    Thread.Sleep(1000);

                ////Stage 1 Scan
                int StageOneScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ImpinjRFIDreader.ReadTags(1000, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE, 
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);

                    if (ScanData != null)
                    {
                        foreach (Tag d in ScanData.Tags)
                        {
                            string TagNoSpaces = d.Epc.ToString().Replace(" ", "");
                            if (TagNoSpaces.EndsWith(RFIDTag.ToUpper()))
                                StageOneScanCount++;
                        }
                    }
                }

                ////Stage 2 Scan
                int StageTwoScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ImpinjRFIDreader.ReadTags(1000, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE,
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);

                    if (ScanData != null)
                    {
                        foreach (Tag d in ScanData.Tags)
                        {
                            string TagNoSpaces = d.Epc.ToString().Replace(" ", "");
                            if (TagNoSpaces.EndsWith(RFIDTag.ToUpper()))
                                StageTwoScanCount++;
                        }
                    }
                }

                ////Stage 3 Scan
                int StageThreeScanCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    ScanData = Program.ImpinjRFIDreader.ReadTags(1000, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE,
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);

                    if (ScanData != null)
                    {
                        foreach (Tag d in ScanData.Tags)
                        {
                            string TagNoSpaces = d.Epc.ToString().Replace(" ", "");
                            if (TagNoSpaces.EndsWith(RFIDTag.ToUpper()))
                                StageThreeScanCount++;
                        }
                    }
                }
                Program.ImpinjRFIDreader.DoneReading();

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
                Program.ImpinjRFIDreader.DoneReading();
            }
        }

        internal override string retrieveRFIDTagNumber(int location, LocationTransactionData transdata)
        {
            int RFIDTagindex = transdata.ObjectList.FindIndex(0, delegate(LocationTransactionData.TransactionDataObject ltd) { return ltd.name == "TagNum"; });
            return transdata.ObjectList[RFIDTagindex].data.ToString();
        }

        internal override bool scanRFIDTagNumber(string tagID)
        {
            TagReport data;
            for (int i = 0; i < 30; i++)
            {
                data = Program.ImpinjRFIDreader.ReadTags(1000, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE,
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);
                if (data != null)
                {
                    foreach (Tag d in data.Tags)
                    {
                        string TagNoSpaces = d.Epc.ToString().Replace(" ", "");
                        if (TagNoSpaces.EndsWith(tagID.ToUpper()))
                            return true;
                    }
                }
            }
            return false;
        }

        static List<Tag> OrganizeTagReads(TagReport reads)
        {
            if (reads == null || reads.Tags.Count == 0)
                return null;

            reads.Tags = new HashSet<Tag>(reads.Tags, new TagEPCComparer()).ToList<Tag>();

            reads.Tags.Sort(delegate(Tag p1, Tag p2) { return p1.Epc.ToString().CompareTo(p2.Epc.ToString()); });

            return reads.Tags;
        }

        public class DaimlerChinaTransactionData : LocationData.LocationTransactionData
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
            public string RFIDTagNumber { get { return (string)ObjectList[LocateIndexOfDataObjectByName("TagNum")].data; } set { tagnum = value; ObjectList[LocateIndexOfDataObjectByName("TagNum")].data = value; } } //The RFID Tag number associated with the boxnumber
            public bool RFIDTagDetected { get { return bool.Parse(ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data.ToString()); } set { tagdetected = value; ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data = value; } } //states if the tag detection process was successful (based upon whether the tag was leaving or returning)
            public string TransactionNumber { get { return transactionnum; } set { transactionnum = value; ObjectList[LocateIndexOfDataObjectByName("Transaction")].data = value; } }

            public DaimlerChinaTransactionData()
                : base()
            {
                AddAdditionalDataObjectsToList();
                RFIDTagNumber = "";
                RFIDTagDetected = false;
                TransactionNumber = "";
            }

            public override void ClearData()
            {
                base.ClearData();
                
            }

            private void AddAdditionalDataObjectsToList()
            {
                ObjectList.Add(new TransactionDataObject("TagNum", tagnum, true, true));
                ObjectList.Add(new TransactionDataObject("TagDetected", tagdetected, true, true));
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
                DaimlerChinaTransactionData copy = new DaimlerChinaTransactionData();

                copy.AccessCode = this.AccessCode;
                copy.CardName = this.CardName;
                copy.CardNumber = this.CardNumber;
                copy.BoxNumber = this.BoxNumber;
                copy.DoorOpened = this.DoorOpened;
                copy.LockLocation = this.LockLocation;
                copy.ReturningKey = this.ReturningKey;
                copy.UserID = this.UserID;
                copy.TransactionNumber = this.TransactionNumber;
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
                TransactionNumber = ObjectList[LocateIndexOfDataObjectByName("Transaction")].data.ToString();
                RFIDTagNumber = ObjectList[LocateIndexOfDataObjectByName("TagNum")].data.ToString();
                RFIDTagDetected = bool.Parse(ObjectList[LocateIndexOfDataObjectByName("TagDetected")].data.ToString());

                Program.locationdata.makeHTTPPost(HTTPPostType.KeyOut, TransactionNumber, AccessCode, "");
                
                if (Program.ENABLE_SQLITE)
                {
                    if (!ReturningKey)
                        InsertDBTransaction();
                    else
                        UpdateDBTransaction();
                }                

                try
                {
                    return String.Format("{0},{1},{2:d},{3:b},{4},{5:b},{6},{7},{8:b}",
                                        Date, Time, BoxNumber, DoorOpened, AccessCode, ReturningKey, TransactionNumber, RFIDTagNumber, RFIDTagDetected);
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
                return new string[] { Date, Time, BoxNumber.ToString(), DoorOpened.ToString(), AccessCode, ReturningKey.ToString(), TransactionNumber};
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
                            
                        //cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        //{
                        //    ParameterName = "@CarDamaged",
                        //    Value = this.CarDamaged,
                        //});

                        //cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        //{
                        //    ParameterName = "@CarCleaned",
                        //    Value = this.CarCleaned,
                        //});

                        //cmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                        //{
                        //    ParameterName = "@CarRefueled",
                        //    Value = this.CarRefueled,
                        //});

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

            //private string packageQuestions()
            //{
            //    string questionpackage = "";
            //    if (ReturningKey)
            //        questionpackage += "1";
            //    else
            //        questionpackage += "0";

            //    if (CarDamaged)
            //        questionpackage += "1";
            //    else
            //        questionpackage += "0";

            //    if (CarCleaned)
            //        questionpackage += "1";
            //    else
            //        questionpackage += "0";

            //    if (CarRefueled)
            //        questionpackage += "1";
            //    else
            //        questionpackage += "0";

            //    return questionpackage;
            //}
        }
    }

    public class TagEPCComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            return x.Epc.ToString().Equals(y.Epc.ToString());
        }

        public int GetHashCode(Tag t)
        {
            int i = t.Epc.ToString().GetHashCode();
            //Console.WriteLine("EPC: {0}, Hash: {1}", t.Epc.ToString(), t.Epc.ToString().GetHashCode());
            return i;
        }
    }
}
