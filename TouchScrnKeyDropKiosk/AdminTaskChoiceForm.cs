using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyCabinetKiosk
{
    public partial class AdminTaskChoiceForm : baseForm
    {
        /// <summary>
        /// AdminTaskChoiceForm - a base dialog for all of the adminstrative tasks
        /// 
        /// </summary>
        private const int LOCATION_MAX_DIGITS_SIZE = 2; //maximium number of characters, each a digit for location

        public AdminTaskChoiceForm()
            :base(Program.TIMEOUT_INTERVAL)    
        {
            InitializeComponent();
            buttonDataImport.Text = LanguageTranslation.USB_TRANS_IMPORT;
            buttonTransactionEmail.Text = LanguageTranslation.EMAIL_TRANSACTION_REPORT;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonRFIDInfo.Text = LanguageTranslation.RFID_INFO_DISPLAY;
            buttonTransactionDisplay.Text = LanguageTranslation.TRANSACTION_DISPLAY;
            buttonLocation.Text = LanguageTranslation.CABINET_BOX_ACCESS;
            label1.Text = LanguageTranslation.ADMIN_TASKS;
            ThreadSafeLabelCentering(label1);

            if (!Program.ENABLE_RFID)
            {
                buttonTransactionDisplay.Size = new System.Drawing.Size(500, 65);
                buttonTransactionDisplay.Location = new Point(145, 434);
                buttonRFIDInfo.Visible = false;
            }
        }

        /// <summary>
        /// buttonLocation_Click - shows a secondary administrative dialog - that does all access operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLocation_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            CabinetAccessForm cabinetAccessDlg = new CabinetAccessForm();

            cabinetAccessDlg.ShowDialog();
            resetTimer();
        }

        /// <summary>
        /// buttonTransactionEmail_Click - Used to be buttonDongleCopy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTransactionEmail_Click(object sender, EventArgs e)
        {
            Program.emailMgr.GenerateTransactionHistoryReport();
            Program.emailMgr.SendEmail(ReportType.TransactionHistory);
        }
        /// <summary>
        /// buttonDongleCopy_Click - copy all transaction files to a USB memory stick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void buttonDongleCopy_Click(object sender, EventArgs e)
        //{
        //    string dongleDir = @"E:\";  //jas todo move to config
        //    copyToUSB copyUSB = new copyToUSB(dongleDir);

        //    if (copyUSB.USB_Present())
        //    {
        //        if (copyUSB.CopyTransactionsToUSB())
        //        {
        //            Program.ShowErrorMessage(LanguageTranslation.TRANSACTIONS_COPIED, 3000);
        //            Program.logEvent("Transaction files copied to USB");
        //        }
        //        else
        //        {
        //            Program.ShowErrorMessage(LanguageTranslation.TRANSACTIONS_COPY_ERROR, 3000);
        //            Program.logEvent("Transaction file copy error");
        //        }
        //    }
        //    else
        //    {
        //        Program.ShowErrorMessage(LanguageTranslation.USB_NOT_DETECTED, 3000);
        //        Program.logEvent("USB memory stick not detected");
        //    }
        //}

        /// <summary>
        /// buttonTransactionDisplay_Click - display transaction in a new dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTransactionDisplay_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            TransactionsDisplayForm transactionDlg = new TransactionsDisplayForm();
            transactionDlg.ShowDialog();
            resetTimer();
        }

        /// <summary>
        /// buttonExit_Click - exit this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonRFIDInfo_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            try
            {
                DisplayAllInfoForm displayDlg = new DisplayAllInfoForm(Displaytype.rfidinfo);

                displayDlg.ShowDialog();
                Program.logEvent("DisplayAllRFIDInfoForm Accessed");
            }
            catch (Exception ex)
            {
                Program.logError("Admin dialogs - view all RFID Info  exception " + ex.Message);
            }
            resetTimer();
        }

        private void buttonDataImport_Click(object sender, EventArgs e)
        {
            Program.logEvent("User has selected to import data files");
            ImportFromUSBForm Importer = new ImportFromUSBForm();

            YesNoForm BoxesorAdmin = new YesNoForm(LanguageTranslation.IMPORT_QUESTION);
            BoxesorAdmin.ChangeButtonFontSize(30);
            BoxesorAdmin.ChangeTextYesButton(LanguageTranslation.USER);
            BoxesorAdmin.ChangeTextNoButton(LanguageTranslation.ACCESS);

            BoxesorAdmin.ShowDialog();
            if (BoxesorAdmin.YesResult)
            {
                Program.logEvent("User has selected to import User data files");
                Importer.ImportType = ImportType.User;
            }
            else if (!BoxesorAdmin.YesResult)
            {
                Program.logEvent("User has selected to import Access Code data files");
                Importer.ImportType = ImportType.AccessCode;
            }
            Importer.Arrived = false;
            Importer.ShowDialog();
        }        
    }
}

  