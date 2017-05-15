using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMTPEmail;
using System.IO;

namespace KeyCabinetKiosk
{
    enum ReportType
    {
        Email, TextMsg, BackgroundTextMsg, BackgroundEmail, TransactionHistory
    }

   /// <summary>
   /// This class handles all of the upper level calls for emailing receipts and transactions to customers
   /// </summary>
    class EmailerManager
    {
        //authSMTP account information
        private string userName = Program.SMTP_USERNAME;
        private string passWord = Program.SMTP_PASSWORD;
                                        //"kaxwrxc5ntxypt"

        private string smtpServer;
        private string emailFromAddress;
        private string emailToAddress;
        private string textToAddress;

        private string report;

        private SMTPEmail.SMTPEmail smtpEmailer;

        public EmailerManager(string smtpserver, string fromAddr, string toAddr, string txttoAddr)
        {
            try
            {
                if (String.IsNullOrEmpty(smtpserver)) { throw new Exception("string smtpserver - null or empty"); }
                if (String.IsNullOrEmpty(fromAddr)) { throw new Exception("string fromAddr - null or empty"); }
                if (String.IsNullOrEmpty(toAddr)) { throw new Exception("string toAddr - null or empty"); }

                smtpServer = smtpserver;
                emailFromAddress = fromAddr;
                emailToAddress = toAddr;
                textToAddress = txttoAddr;

                smtpEmailer = new SMTPEmail.SMTPEmail();

                //smtpEmailer.setCredentials(userName, passWord);

                report = "";
            }
            catch (Exception ex)
            {
                throw new Exception("Email manager constructor error - " + ex.Message);
            }
        }

        public void GenerateTextMsg(string msg)
        {
            report = msg;
        }

        public bool GenerateReport(LocationData.LocationTransactionData TransactionData, ReportType type)
        {
            bool firstpass = true;
            try
            {
                //
                //This needs to be rewritten in a generic manner using the Object List for the
                //current location.
                //
                //
                StringBuilder sb = new StringBuilder();
                
                sb.AppendLine("Key cabinet " + type.ToString() + " report");

                if (type == ReportType.Email)
                    sb.AppendLine();

                sb.AppendLine("Kiosk ID: " + Program.KIOSK_ID);
                if (type == ReportType.Email)
                    sb.AppendLine();

                if (TransactionData.AccessCode != "0")
                    sb.Append("Access Code: " + TransactionData.AccessCode);

                sb.AppendLine();

                if (firstpass) //this will catch all of the data bits which should only be used
                {              //once in each receipt 
                    firstpass = false;
                    foreach (LocationData.LocationTransactionData.TransactionDataObject t in TransactionData.ObjectList)
                    {
                        if (t.UseOnceInReceipt && t.UseInReceipt)
                            sb.AppendFormat("{0}: {1}\r\n", new string[] { t.name, t.data.ToString() });
                    }
                }
                if (type == ReportType.Email)
                    sb.AppendLine();
                //This will catch all data bits which need to be repeated for each box opening.
                //Currently the code does not allow for multiple openings per transaction so this won't do anything
                //but it is possible it will be used in the future.
                foreach (LocationData.LocationTransactionData.TransactionDataObject t in TransactionData.ObjectList)
                {
                    if (!t.UseOnceInReceipt && t.UseInReceipt)
                        sb.AppendFormat("{0}: {1}\r\n", t.name, t.data);
                }

                if (TransactionData.CardNumber != "")
                    sb.AppendFormat("Card Number {0} \r\n", TransactionData.CardNumber);

                if (TransactionData.CardName != "")
                    sb.AppendFormat("Card Name {0} \r\n", TransactionData.CardName);

                //find if any of the customer screens asked if they were taking or returning a key
                if (Program.locationdata.CustomerScreenOrderList.Find(delegate(CustomerScreen cs) { return cs.Type == CustomerScreenType.ReturningQuestion; }) != null)
                {   //if the program doesn't ask the user if they are returning a key, then the receipt shouldn't 
                    //say anything about it.
                    if (TransactionData.ReturningKey)
                    {
                        sb.AppendLine("Key Returned.");
                    }
                    else
                    {
                        sb.AppendLine("Key Taken.");
                    }
                }


                sb.AppendFormat("Box {0} ", TransactionData.BoxNumber);
                if (TransactionData.DoorOpened == true)
                {
                    sb.AppendLine("was opened");
                }
                else
                {
                    sb.AppendLine("failed to open");
                }
                sb.AppendFormat("At {0} on {1}\r\n", TransactionData.Time, TransactionData.Date);


                report = sb.ToString();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("EmailerManager:GenerateReport exception-" + ex.Message);
            }
        }

        public bool GenerateTransactionHistoryReport()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Key cabinet Transaction report");
                sb.AppendLine("Kiosk ID: " + Program.KIOSK_ID);
                sb.AppendLine();
                sb.AppendLine("Transaction .csv Files Attached");

                string[] TransactionFiles = Directory.GetFiles("./Transactions");

                foreach (string s in TransactionFiles)
                    smtpEmailer.addAttachment(smtpEmailer.CreateAttachment(s));

                report = sb.ToString();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("EmailerManager:GenerateTransactionReport exception-" + ex.Message);
            }
        }

        public bool SendEmail(ReportType type, string newToAddress)
        {
            bool returnbool;
            string temp;

            if (type == ReportType.Email)
            {
                temp = emailToAddress;
                emailToAddress = newToAddress;
                returnbool = SendEmail(type);
                emailToAddress = temp;
            }
            else
            {
                temp = textToAddress;
                textToAddress = newToAddress;
                returnbool = SendEmail(type);
                textToAddress = temp;
            }           

            return returnbool;
        }
        
        public bool SendEmail(ReportType type)
        {
            if (smtpEmailer == null) 
            {
                Program.logError("smtp emailer null");
                return false; 
            }
            if (string.IsNullOrEmpty(report))
            {
                Program.logError("report null or empty");
                return false; 
            }

            string date = DateTime.Now.Date.ToString();
            string time = DateTime.Now.TimeOfDay.ToString();
            string serverAddr = smtpServer;   // "mail.authsmtp.com";
            string fromAddr = emailFromAddress; // "john.stevens@kiosksnw.com";            
            string toAddr;
            if (type == ReportType.TextMsg || type == ReportType.BackgroundTextMsg)
                toAddr = textToAddress;
            else
                toAddr = emailToAddress;
            string emailSubjectHeader = "SafePak key cabinet";

            string username = userName; //local constant
            string password = passWord;  //local constant
            bool UsePassword = Program.SMTP_AUTHENTICATION;
            bool UseSSL = false;
            int tryCount = 2;

            ErrorForm SendingEmail = null;
            //if ((type == ReportType.TextMsg) || (type == ReportType.Email))
            if (type == ReportType.Email || type == ReportType.BackgroundEmail)
                SendingEmail = Program.ShowMessage(LanguageTranslation.SENDING_EMAIL_RECEIPT);
            else if (type == ReportType.TextMsg || type == ReportType.BackgroundTextMsg)
                SendingEmail = Program.ShowMessage(LanguageTranslation.SENDING_TXTMSG_RECEIPT);
            else if (type == ReportType.TransactionHistory)
                SendingEmail = Program.ShowMessage(LanguageTranslation.SENDING_TRANSACTION_HISTORY);
            try
            {
                smtpEmailer.sendEmail(date,                                             // string date
                                    time,                                               // string time
                                    serverAddr,                                         // string server address
                                    fromAddr,                                           // string from_address,
                                    toAddr,                                            // string to_address 
                                    emailSubjectHeader,                                // string email subject
                                    this.report,                                        // string email_body
                                    UsePassword,                                        // bool username_password
                                    UseSSL,                                             // bool SSL
                                    username,                                          // string username,                            
                                    password,                                          // string password,                            
                                    tryCount);                                          // int trycount    
            }
                
            catch (EmailSuccessMessage esm)
            {
                if (type == ReportType.TextMsg) 
                    Program.ShowErrorMessage(LanguageTranslation.TEXT_SUCCESSFUL, 5000);
                else if (type == ReportType.Email)
                    Program.ShowErrorMessage(LanguageTranslation.EMAIL_SUCCESSFUL, 5000);
                else if (type == ReportType.TransactionHistory)
                    Program.ShowErrorMessage(LanguageTranslation.EMAIL_TRANSACTIONS_SUCCESSFUL, 5000);

                Program.logEvent(type.ToString() + " Success");
                return true;
            }
            catch (Exception ex)
            {
                //If the sending of the email receipt fails a local copy is stored to be retrieved later.
                Program.logEvent("Receipt Failed. Local Copy Stored");
                Program.logEvent(ex.Message);
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Receipt Failed. Local Copy Stored" + ex.Message, "", 0);
                try { File.AppendAllText("Receipts\\" + DateTime.Today.Month.ToString() + ".log", "FAILED EMAIL" + DateTime.Now.ToString() + " " + this.report.ToString() + "\r\n"); }
                catch (Exception e)
                { File.AppendAllText("Logs\\error.log", e.Message + "\r\n" + DateTime.Now.ToString() + " " + this.report.ToString() + "\r\n"); }

                if (type == ReportType.TextMsg)
                    Program.ShowErrorMessage(LanguageTranslation.TEXT_FAILED, 7000);
                else if (type == ReportType.Email)
                    Program.ShowErrorMessage(LanguageTranslation.EMAIL_FAILED, 7000);
                else if (type == ReportType.TransactionHistory)
                    Program.ShowErrorMessage(LanguageTranslation.EMAIL_TRANSACTIONS_FAILED, 7000);
                
                return false;
            }
            finally
            {
                report = "";
                SendingEmail.Dispose();
            } 
            return true;
        }
    }
}
