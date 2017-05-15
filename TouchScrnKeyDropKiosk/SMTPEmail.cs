using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.IO;
//using System.Windows.Forms;

namespace SMTPEmail
{
    public class SMTPEmail
    {
        private int EmailTryCounter { get; set; }
        private int EmailTryCount { get; set; }
        private SmtpClient MailClient { get; set; }
        private MailMessage Message { get; set; }
        private IPHostEntry SmtpServerIP { get; set; }
        private Collection<Attachment> EmailAttachments { get; set; }

        public SMTPEmail()
        {
            MailClient = new SmtpClient();
          
            MailClient.Port = 25;
            MailClient.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
            EmailAttachments = new Collection<Attachment>();
        }

        public void setCredentials(string username, string password)
        {
            NetworkCredential cred = new System.Net.NetworkCredential(username, password, "Default");

            MailClient.Credentials = cred;
        }

        public  Attachment CreateAttachment(string fileName)
        {
            return  new Attachment(fileName);
        }

        public void addAttachment(Attachment NewAttachment)
        {
            EmailAttachments.Add(NewAttachment);
        }

        public void clearAttachments()
        {
            if (!(EmailAttachments == null))
                EmailAttachments.Clear();
        }

        public void sendEmail(string date, 
                             string time, 
                             string server_address, 
                             string from_address, 
                             string to_address, 
                             string email_subject, 
                             string email_body, 
                             bool username_password, 
                             bool SSL, 
                             string username, 
                             string password,
                             int trycount)
        {
            SmtpServerIP = Dns.GetHostEntry(server_address);
            MailClient.Host = SmtpServerIP.AddressList.GetValue(0).ToString();
            
            Message = new MailMessage();
            MailAddress from = new MailAddress(from_address);
            for (int i = 0; i < to_address.Split(';').Length; i++)
                Message.To.Add(new MailAddress(to_address.Split(';')[i]));

            Message.From = from;
            Message.Subject = email_subject;
            Message.SubjectEncoding = Encoding.UTF8;
            Message.Body = email_body;
            Message.BodyEncoding = Encoding.UTF8;

            if (!(EmailAttachments == null))
            {
                if (EmailAttachments.Count > 0)
                {
                    foreach (Attachment i in EmailAttachments)
                    {
                        Message.Attachments.Add(i);
                    }
                }
            }

            if (username_password)
                MailClient.Credentials = new System.Net.NetworkCredential(username, password);
            else
                MailClient.UseDefaultCredentials = true;

            if (SSL)
                MailClient.EnableSsl = true;
            else
                MailClient.EnableSsl = false;

            try
            {
                EmailTryCounter = 0;
                EmailTryCount = trycount;
                MailClient.Send(Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Sending of Email Failed.", new Exception(ex.Message));
            }
            KeyCabinetKiosk.Program.logEvent("email - success ");
            throw new EmailSuccessMessage("Email Success"); 
        }

        void smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
               // MessageBox.Show("Sending of Email Cancelled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //Mark in the monthly transaction file whether or not the email was successful
                //File.AppendAllText("Transactions/Monthly/" + DateTime.Now.Month + ".csv", "Email Cancelled\r\n", Encoding.Unicode);

                KeyCabinetKiosk.Program.logEvent("email cancelled " + e.ToString());
                //If the previous server IP did not work, use the next one. Try up to five different IPs if there are that many
                if (EmailTryCounter < EmailTryCount)
                {
                    EmailTryCounter++;
                    MailClient.Host = SmtpServerIP.AddressList.GetValue(EmailTryCounter % SmtpServerIP.AddressList.Length).ToString();
                    MailClient.SendAsync(Message, "Email Try" + EmailTryCounter);
                }
            }
            if (e.Error != null)
            {


                KeyCabinetKiosk.Program.logEvent("email -response " + e.Error.Message.ToString());
                //MessageBox.Show("Process Error: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                //If the previous server IP did not work, use the next one. Try up to five different IPs if there are that many
                if (EmailTryCounter < EmailTryCount)
                {
                    EmailTryCounter++;
                    MailClient.Host = SmtpServerIP.AddressList.GetValue(EmailTryCounter % SmtpServerIP.AddressList.Length).ToString();
                    MailClient.SendAsync(Message, "Email Try" + EmailTryCounter);
                }
            }
            else
            {
                KeyCabinetKiosk.Program.logEvent("email - sucess ");
                //MessageBox.Show("Email Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            clearAttachments();
        }
    }
    /// <summary>
    /// I figured it was easier to set up this message just as an exception rather
    /// than a delegate and listener message system.
    /// </summary>
    public class EmailSuccessMessage : Exception
    {
        public EmailSuccessMessage(string message)
            : base(message)
        {
        }
        public EmailSuccessMessage(string message, Exception innerexception)
            : base(message, innerexception)
        {
        }
    }
}
