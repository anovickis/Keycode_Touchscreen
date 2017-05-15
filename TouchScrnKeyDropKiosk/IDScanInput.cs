using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This form handles communication and info extraction for the SnapShell Id Camera. 
    /// </summary>
    public partial class IDScanInput : baseForm
    {
        bool numbercheck;
        bool namecheck;
        bool manualLicenseEntered;
        internal bool cancelled;
        SCANWLib.IdDataClass IDdata;
        SCANWLib.CBarCodeClass Barcodedata;
        string ScanType;
        SnapShell_Driver_Lic.Snapshell IDScanner;

        internal string firstname, lastname, licensenum;
        public IDScanInput(string scantype):base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            buttonCancel.Text = LanguageTranslation.CANCEL;
            buttonConfirm.Text = LanguageTranslation.CONFIRM;
            buttonManualEntry.Text = LanguageTranslation.MANUAL_ENTRY;

            IDScanner = new SnapShell_Driver_Lic.Snapshell();
            if (scantype.ToUpper() == "BARCODE") //put a message up on the screen asking the user to confirm their ID is placed
            {
                textBox1.Text = LanguageTranslation.SCAN_LICENSE_FACING_OUT;
            }
            else
            {
                textBox1.Text = LanguageTranslation.SCAN_LICENSE_FACING_IN;
            }
            buttonConfirm.Text = LanguageTranslation.SCAN;

            numbercheck = namecheck = false;
            cancelled = false;
            manualLicenseEntered = false;
            IDScanner.SnapshellMessage += new EventHandler<SnapShell_Driver_Lic.SnapshellMessageArgs>(IDScanner_SnapshellMessage);
            firstname = lastname = licensenum = "";
            ScanType = scantype;
            textBox1.GotFocus += new EventHandler(textBox1_GotFocus);
        }

        void textBox1_GotFocus(object sender, EventArgs e)
        {
            buttonConfirm.Focus();
        }

        /// <summary>
        /// This listener responds to ID scanner errors and then cancels out of this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void IDScanner_SnapshellMessage(object sender, SnapShell_Driver_Lic.SnapshellMessageArgs e)
        {
            //Program.ShowErrorMessage("ID Scanner Error\r\nContact Admin", 5000);
            Program.ShowErrorMessage(LanguageTranslation.ID_SCANNER_ERROR, 3000);
            Program.logEvent(e.Message);

            if (ScanType == "BARCODE" && !manualLicenseEntered)
            {
                LongNameEntryForm LicenseEntry = new LongNameEntryForm(20, false, false);
                LicenseEntry.DialogTitle = LanguageTranslation.ENTER_LICENSE_NUMBER;
                LicenseEntry.ShowDialog();

                manualLicenseEntered = true;
                if (pictureBox1.Image != null)
                    pictureBox1.Image.Dispose();

                if (!LicenseEntry.Ok)
                    buttonCancel_Click(this, null);
                else
                {
                    licensenum = '`' + LicenseEntry.Description;                    
                    this.Close();
                }
            }
            else
            {
                buttonCancel_Click(this, null);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cancelled = true;
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            this.Close();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            resetTimer();
            try
            {
                buttonManualEntry.Enabled = false;
                if (!namecheck && !numbercheck) //If you arent onto the Driver name or Driver number check, you are asking for
                {                               //confirmation that the user has placed their ID.
                    buttonCancel.Visible = false; buttonCancel.Update();
                    buttonConfirm.Visible = false; buttonConfirm.Update();
                    textBox1.Text = LanguageTranslation.SCANNING_WAIT; textBox1.Refresh(); 
                   
                    
                    IDScanner.ScanLicense("temp.jpg", Program.IMAGE_SCAN_RESOLUTION); //Scan the licence

                    textBox1.Text = LanguageTranslation.PROCESSING_ID;
                    //Bitmap licenseimage = new Bitmap("temp.jpg");
                    pictureBox1.Image = new Bitmap("temp.jpg");
                    textBox1.Refresh();                    

                    if (ScanType == "BARCODE")
                    {
                        Barcodedata = IDScanner.ProcessLicenseBarcode("temp.jpg");

                        if (Barcodedata == null)
                        {
                            Program.ShowErrorMessage(LanguageTranslation.BARCODE_SCANNING_ERROR, 5000);
                            Program.logEvent("Barcode scanning Error: Barcodedata is null");
                            Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Barcode scanning Error: Barcodedata is null", "", 0);

                            LongNameEntryForm LicenseEntry = new LongNameEntryForm(20, false, false);
                            LicenseEntry.DialogTitle = LanguageTranslation.ENTER_LICENSE_NUMBER;
                            LicenseEntry.ShowDialog();

                            if (!LicenseEntry.Ok)
                                buttonCancel_Click(this, null);
                            else
                            {
                                licensenum = '`' + LicenseEntry.Description;
                                if (pictureBox1.Image != null)
                                    pictureBox1.Image.Dispose();
                                this.Close();
                            }
                            return;
                        }
                        else if (Barcodedata.NameFirst == "" && Barcodedata.NameLast == "" && Barcodedata.license == "")
                        {
                            Program.ShowErrorMessage(LanguageTranslation.BARCODE_SCANNING_ERROR, 5000);
                            Program.logEvent("Barcode scanning Error: Barcodedata is Empty, likely because of license being wrong way on scanner or not on scanner at all");
                            Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Barcode scanning Error: Barcodedata is Empty, likely because of license being wrong way on scanner or not on scanner at all", "", 0);

                            if (!manualLicenseEntered)
                            {
                                LongNameEntryForm LicenseEntry = new LongNameEntryForm(20, false, false);
                                LicenseEntry.DialogTitle = LanguageTranslation.ENTER_LICENSE_NUMBER;
                                LicenseEntry.ShowDialog();

                                if (!LicenseEntry.Ok)
                                    buttonCancel_Click(this, null);
                                else
                                {
                                    licensenum = '`' + LicenseEntry.Description;
                                    if (pictureBox1.Image != null)
                                        pictureBox1.Image.Dispose();
                                    this.Close();
                                }
                            }
                            else
                            {
                                this.Close();
                            }
                            return;
                        }

                        firstname = Barcodedata.NameFirst;
                        lastname = Barcodedata.NameLast;
                        licensenum = Barcodedata.license;
                    }
                    else //if scan type is IMAGE
                    {
                        IDdata = IDScanner.ProcessLicense(); //Extract data from the license

                        if (IDdata == null)
                        {
                            //Program.ShowErrorMessage("Error While Processing ID:\r\nTransaction Cancelled", 5000);
                            Program.ShowErrorMessage(LanguageTranslation.PROCESSING_ID_ERROR, 4000);
                            buttonCancel_Click(this, null);
                            return;
                        }

                        firstname = IDdata.NameFirst;
                        lastname = IDdata.NameLast;
                        licensenum = IDdata.license;
                    }
                    
                    buttonCancel.Visible = true;
                    buttonConfirm.Visible = true;
                    textBox1.Focus();

                    buttonConfirm.Text = LanguageTranslation.CONFIRM;
                    textBox1.Text = LanguageTranslation.CONFIRM_NAME + firstname + "\r\n" + lastname;
                    namecheck = true;
                }
                else if (!numbercheck) //This confirms that the ID name is correct
                {
                    textBox1.Text = LanguageTranslation.CONFIRM_LICENSE + licensenum;
                    numbercheck = true;
                }
                else //this confirms that the ID number is correct
                {
                    Program.ShowErrorMessage(LanguageTranslation.ID_SCAN_CONFIRMED, 4000);
                    pictureBox1.Image.Dispose();
                    this.Close();
                }
            }
        
            catch (Exception ex)
            {
                Program.ShowErrorMessage(LanguageTranslation.SCANNING_ID_ERROR, 3000);
                Program.logEvent("IDScanInput Error: " + ex.Message);
                Program.SqlManager.ErrorDatabaseEntry("", "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "IDScanInput Error:" + ex.Message, "", 0);
                pictureBox1.Image.Dispose();
                buttonCancel_Click(this, null);
                return;
            }
        }

        /// <summary>
        /// NOT BEING USED FOR NOW DUE TO SECURITY CONCERNS. IT IS BELIEVED TO BE TOO EASY FOR USERS TO USE SOMEONE ELSE'S LICENSE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonManualEntry_Click(object sender, EventArgs e)
        {
            LongNameEntryForm EnterDriverFirstName = new LongNameEntryForm(20, false, false, LanguageTranslation.DRIVER_FIRST_NAME);
            EnterDriverFirstName.ShowDialog();

            if (!EnterDriverFirstName.Ok)
            {
                Program.ShowErrorMessage(LanguageTranslation.MANUAL_ID_CANCELLED, 3000);
                Program.logEvent("Manual Driver First Name Entry Cancelled");
                cancelled = true;
                this.Close();
            }

            LongNameEntryForm EnterDriverLastName = new LongNameEntryForm(20, false, false, LanguageTranslation.DRIVER_LAST_NAME);
            EnterDriverLastName.ShowDialog();

            if (!EnterDriverLastName.Ok)
            {
                Program.ShowErrorMessage(LanguageTranslation.MANUAL_ID_CANCELLED, 3000);
                Program.logEvent("Manual Driver Last Name Entry Cancelled");
                cancelled = true;
                this.Close();
            }

            LongNameEntryForm EnterDriverNumber = new LongNameEntryForm(20, false, false, LanguageTranslation.DRIVER_NUMBER);
            EnterDriverNumber.ShowDialog();

            if (!EnterDriverNumber.Ok)
            {
                Program.ShowErrorMessage(LanguageTranslation.MANUAL_ID_CANCELLED, 3000);
                Program.logEvent("Manual Driver Number Entry Cancelled");
                cancelled = true;
                this.Close();
            }

            firstname = EnterDriverFirstName.Description;
            lastname = EnterDriverLastName.Description;
            licensenum = EnterDriverNumber.Description;
            this.Close();
        }
    }
}
