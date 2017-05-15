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
    /// <summary>
    /// CabinetAccessForm - a dialog for all the administrative tasks involving access
    /// </summary>
    public partial class CabinetAccessForm : baseForm
    {


        private const int SERVICE_NUMBER_MAX_SIZE = 15;

        public CabinetAccessForm()
            :base(Program.TIMEOUT_INTERVAL)                   
        {
            InitializeComponent();

            buttonAccessType.Text = LanguageTranslation.ACCESS_METHOD;
            buttonAdminPass.Text = LanguageTranslation.CHANGE_ADMIN_PWORD;
            buttonBiometricEnrollment.Text = LanguageTranslation.BIOMETRIC_ENROLLMENT;
            buttonBoxAcces.Text = LanguageTranslation.BOX_ACCESS;
            buttonChangeAdminEmail.Text = LanguageTranslation.ADMIN_EMAIL_ADDRESS;
            buttonChangeTextMsgNumber.Text = LanguageTranslation.ADMIN_TEXT_NUMBER;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonOpenAllDoors.Text = LanguageTranslation.OPEN_ALL_DOORS;
            buttonOpenDoor.Text = LanguageTranslation.OPEN_BOX_DOOR;
            buttonSrvMngNumber.Text = LanguageTranslation.ADMIN_PHONE_NUMBER;
            buttonUserManagement.Text = LanguageTranslation.USER_MANAGEMENT;
            buttonViewAll.Text = LanguageTranslation.BOX_INFO;

            if (!Program.BIOMETRIC_ENABLE)
                buttonBiometricEnrollment.Visible = false;
            if (!Program.ENABLE_TEXTMSG)
                buttonChangeTextMsgNumber.Visible = false;
            if (!Program.ENABLE_EMAIL)
                buttonChangeAdminEmail.Visible = false;
            if (!Program.USERS_ENABLE)
                buttonUserManagement.Visible = false;
        }

        public CabinetAccessForm(AdminScreenType screen)
            :base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            buttonAccessType.Text = LanguageTranslation.ACCESS_METHOD;
            buttonAdminPass.Text = LanguageTranslation.CHANGE_ADMIN_PWORD;
            buttonBiometricEnrollment.Text = LanguageTranslation.BIOMETRIC_ENROLLMENT;
            buttonBoxAcces.Text = LanguageTranslation.BOX_ACCESS;
            buttonChangeAdminEmail.Text = LanguageTranslation.ADMIN_EMAIL_ADDRESS;
            buttonChangeTextMsgNumber.Text = LanguageTranslation.ADMIN_TEXT_NUMBER;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonOpenAllDoors.Text = LanguageTranslation.OPEN_ALL_DOORS;
            buttonOpenDoor.Text = LanguageTranslation.OPEN_BOX_DOOR;
            buttonSrvMngNumber.Text = LanguageTranslation.ADMIN_PHONE_NUMBER;
            buttonUserManagement.Text = LanguageTranslation.USER_MANAGEMENT;
            buttonViewAll.Text = LanguageTranslation.BOX_INFO;

            invisibleButtons();

            switch (screen)
            {
                case AdminScreenType.BiometricEnrollmentHID:
                {
                    buttonBiometricEnrollment.Visible = true;
                    break;
                }
                case AdminScreenType.OpenAllDoors:
                {
                    buttonOpenAllDoors.Visible = true;
                    break;
                }
                case AdminScreenType.OpenSingleDoor:
                {
                    buttonOpenDoor.Visible = true;
                    break;
                }
            }
        }

        /// <summary>
        /// buttonViewAll_Click - display all locations with access codes and expected card numbes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonViewAll_Click(object sender, EventArgs e)
        {
            TimeOutOff();
             try
             {
                 DisplayAllInfoForm displayDlg = new DisplayAllInfoForm();

                 displayDlg.ShowDialog();
                 Program.logEvent("DisplayAllInfoForm Accessed");
             }
             catch (Exception ex)
             {
                 Program.logError("Admin dialogs - view all tasks  exception " + ex.Message);
             }
             resetTimer();
        }
        /// <summary>
        /// buttonBoxAcces_Click - allows the user to view and change the access information
        ///                     this is different for the different types (simple, credit card or both)
        ///                     so different dialogs will be shown for the different types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBoxAccess_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            try
            {
                keyLocationChange();
                resetTimer();
            }
            catch (Exception ex)
            {
                Program.logEvent("DisplayAllInfoForm:DisplayAll exception " + ex.Message);

                throw new Exception("DisplayAllInfoForm:DisplayAll exception " + ex.Message);

            }
        }
        /// <summary>
        /// buttonOpenDoor_Click - will open a locked door for an admin, give access to the keys without
        ///                     using access codes or cards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenDoor_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            try
            {
                string boxNumber = CommonTasks.GetLocationFromKeypad();

                if (String.IsNullOrEmpty(boxNumber))
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_BOX_NUMBER, 3000);
                    return;
                }
                //check to see if this is a valid location
                int loc = -1;  //initialize as invalid

                if (CommonTasks.ValidBoxNumber(boxNumber))
                {

                    loc = CommonTasks.FindLocationNumber(boxNumber); //location number is 1-Max - box numbers are different sequence

                }

                // if this was not a valid box number - then location will be -1
                if (loc == -1)
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_BOX_NUMBER, 3000);
                    return;
                }
                else
                {
                    CommonTasks.OpenKeyBox(loc, false);
                    Program.logEvent("Box " + loc + " Opened by Admin");
                }
                resetTimer();
            }                
            catch (Exception ex)
            {
                Program.logEvent("Admin dialogs - Open door task  exception " + ex.Message);
            }
        }

        /// <summary>
        /// buttonAdminPass_Click - opens a 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdminPass_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            ChangeAdminPassWordForm adminChangeDlg = new ChangeAdminPassWordForm();

            adminChangeDlg.ShowDialog();
            resetTimer();
        }

        /// <summary>
        /// buttonExit_Click - exit dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// SimpleAccess - simple access matchs a saved access code - 6 digits with digits entered here
        /// </summary>
        private void keyLocationChange()
        {
             try
             {
                 string boxNumber = CommonTasks.GetLocationFromKeypad();
                 resetTimer();
                if (String.IsNullOrEmpty(boxNumber))
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_LOCATION,3000);
                    return;
                }
                //check to see if this is a valid location
                int loc = -1;  //initialize as invalid

                if (CommonTasks.ValidBoxNumber(boxNumber))
                {
                    loc = CommonTasks.FindLocationNumber(boxNumber); //location number is 1-Max - box numbers are different sequence                   
                }
              
                // if this was not a valid box number - then location will be -1
                if (loc == -1)
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_LOCATION, 3000);
                    return;   
                }
                // the admin task has password at location 0
                else if (loc == 0) // this the admin location - use different task to change admin password
                {
                    Program.ShowErrorMessage(LanguageTranslation.USE_CHANGE_ADMIN_PWORD, 3000);
                    return;                      
                }
                else
                {
                    TimeOutOff();
                    KeyLocationChangeTasksForm keyLocChangeDlg = new KeyLocationChangeTasksForm(loc);

                    keyLocChangeDlg.ShowDialog();
                    resetTimer();
                }            
             }
             catch (Exception ex)
             {
                 Program.logError("Admin dialogs - SimpleAccess -change access task  exception " + ex.Message);
             }
        }
        
        private void buttonSrvMngNumber_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            LongNameEntryForm numberNameDlg = new LongNameEntryForm(SERVICE_NUMBER_MAX_SIZE, false, false);

            numberNameDlg.DialogTitle = LanguageTranslation.SERVICE_MGR_PHONE_NUMBER;

            numberNameDlg.InitialString = Program.SERVICE_MANAGER_NUMBER;

            numberNameDlg.UseSpaceBar = true;

            numberNameDlg.ShowDialog();
            resetTimer();
            string result = numberNameDlg.Description;

            Program.SERVICE_MANAGER_NUMBER = result;

            CommonTasks.WriteValueToConfigurationFile("globals", "serviceManagerPhone", result);

        }

        private void buttonAccessType_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            AccessModeSelectionForm accessModeDlg = new AccessModeSelectionForm();

            accessModeDlg.ShowDialog();
            resetTimer();
        }

        private void buttonOpenAllDoors_Click(object sender, EventArgs e)
        {
            TimeOutOff();

            OpenAllDoorsOptions DoorOptions = new OpenAllDoorsOptions();
            DoorOptions.ShowDialog();

            resetTimer();
            return;
        }

        private void buttonChangeAdminEmail_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            LongNameEntryForm buttonChangeAdminEmail = new LongNameEntryForm(250, false, false, LanguageTranslation.NEW_ADMIN_EMAIL);
            buttonChangeAdminEmail.InitialString = Program.TO_ADDRESS;
            buttonChangeAdminEmail.UseAtSign = true;
            buttonChangeAdminEmail.UsePeriod = true;
            buttonChangeAdminEmail.ShowDialog();
            resetTimer();

            if (buttonChangeAdminEmail.Ok)
            {
                string result = buttonChangeAdminEmail.Description;

                Program.TO_ADDRESS = result;

                CommonTasks.WriteValueToConfigurationFile("email", "emailToAddress", result);    
            }
        }

        private void buttonChangeTextMsgNumber_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            LongNameEntryForm buttonChangeAdminTextMsg = new LongNameEntryForm(10, false, false, LanguageTranslation.NEW_ADMIN_TXT_NUMBER);
            buttonChangeAdminTextMsg.InitialString = Program.TEXTTO_ADDRESS.Split('@')[0];
            buttonChangeAdminTextMsg.ShowDialog();
            resetTimer();

            if (buttonChangeAdminTextMsg.Ok)
            {
                string result = buttonChangeAdminTextMsg.Description;

                Program.TEXTTO_ADDRESS = result;

                CommonTasks.WriteValueToConfigurationFile("textmsg", "textToAddress", result + "@vtext.com");
            }
        }

        private void buttonBiometricEnrollment_Click(object sender, EventArgs e)
        {
            string data; bool screenOK;
            TimeOutOff();
            if (Program.locationdata.AdminScreenList.Contains(AdminScreenType.BiometricEnrollmentHID))
            {
                HIDCardReader HIDEntry = new HIDCardReader();
                HIDEntry.ShowDialog();
                screenOK = !HIDEntry.Cancelled;
                data = HIDEntry.Data;
            }
            else
            {
                LongNameEntryForm IDEntry = new LongNameEntryForm(Program.PASSWORD_SIZE, false, false);
                IDEntry.DialogTitle = LanguageTranslation.ID_BIOMETRIC_ENROLLING; ;
                IDEntry.ShowDialog();
                screenOK = IDEntry.Ok;
                data = IDEntry.Description;
            }
            resetTimer();

            if (!screenOK)
                return;

            TimeOutOff();
            BiometricEnrollmentForm Enroll = new BiometricEnrollmentForm(data);


            if (Program.biometricMgr.FindFingerprint(data) == null)
            {
                Enroll.labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + data +
                " = 0";
            }
            else
            {
                Enroll.labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + data +
                    " = " + Program.biometricMgr.FindFingerprint(data).Count;
            }
            Enroll.ShowDialog();
            resetTimer();
        }

        private void buttonUserManagement_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            AddUsersForm ManageUsers = new AddUsersForm();
            ManageUsers.ShowDialog();
            resetTimer();
        }

        private void invisibleButtons()
        {
            buttonAccessType.Visible = false;
            buttonAdminPass.Visible = false;
            buttonBiometricEnrollment.Visible = false;
            buttonBoxAcces.Visible = false;
            buttonChangeAdminEmail.Visible = false;
            buttonChangeTextMsgNumber.Visible = false;
            buttonOpenAllDoors.Visible = false;
            buttonOpenDoor.Visible = false;
            buttonSrvMngNumber.Visible = false;
            buttonUserManagement.Visible = false;
            buttonViewAll.Visible = false;
        }
    }
}
