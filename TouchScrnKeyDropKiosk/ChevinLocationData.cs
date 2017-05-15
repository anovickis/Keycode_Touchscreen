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
    ////
    ////CHEVINLOCATIONDATA is used for Chevin demo kiosks. The only differences currently between this and Wa Water is a small clause in confirmReservation() and the "clientname"
    ////that is sent in server communications is "DEMO" rather than "DCWATER"
    ////
    class ChevinLocationData : LocationData
    {
        internal override List<CustomerScreen> CustomerScreenOrderList { get; set; }
        internal override List<AdminScreenType> AdminScreenList { get; set; }
        internal override List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; }

        public ChevinLocationData() : base()
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

                ////
                ////BEGIN TEMPORARY CODE FOR 5/19 WA WATER DEMO
                ////

                if (File.Exists("./Transactions/" + DateTime.Today.Month.ToString() + "transaction.csv"))
                {
                    FileStream readfile = File.OpenRead("./Transactions/" + DateTime.Today.Month.ToString() + "transaction.csv");
                    StreamReader readstream = new StreamReader(readfile);
                    string newline;
                    int transactioncount = 0;
                    while (readstream.Peek() >= 0)
                    {
                        newline = readstream.ReadLine();
                        if (newline.Split(',')[4] == transaction.AccessCode)
                            transactioncount++;
                    }

                    if (transactioncount % 2 == 1)
                        transaction.ReturningKey = true;
                }
                ////
                ////END TEMPORARY CODE FOR 5/19 WA WATER DEMO
                ////
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
                    methodparams.Add("clientName", "PUBLIC");
                    methodparams.Add("reservationNumber", extradata1.ToUpper()); //any alpha characters in the res number must be capitals
                    methodparams.Add("authMethod", extradata2);
                    methodparams.Add("HIDcode", extradata3);
                    methodparams.Add("kioskNumber", Program.KIOSK_ID);                    
                    break;
                case HTTPPostType.KeyOut:
                    methodtype = "TransactionComplete";
                    methodparams.Add("clientName", "PUBLIC");
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
            return PostWAWaterData(methodparams, Program.CUSTOMER_DATA_SERVER, methodtype, ref transaction);
        }

        internal override bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3)
        {
            LocationTransactionData fakedata = new WAWaterLocationData.WAWaterTransactionData();
            return makeHTTPPost(MethodType, extradata1, extradata2, extradata3, ref fakedata);
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

        ////
        ////CHEVINLOCATIONDATA SHOULD USE WAWATERTRANSACTIONDATACLASS
        ////
    }
}
