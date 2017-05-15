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
    /// KeyLocationChangeTasksForm - dialog for changing access codes - location selected 
    ///                 before this dialog is created
    /// </summary>
    public partial class KeyLocationChangeTasksForm : baseForm
    {
        private int KeyLocation = -1;
        private int boxNumber = -1 ;

        public KeyLocationChangeTasksForm(int Location)   // this is the location in database and lock number
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            buttonChangeAccessCode.Text = LanguageTranslation.ACCESS_CODE;
            buttonChangeCardNumber.Text = LanguageTranslation.CARD_NUMBER;
            buttonChangeAccessRestrictions.Text = LanguageTranslation.ACCESS_RESTRICTIONS;
            buttonChangeRFIDTag.Text = LanguageTranslation.RFID_TAG;
            buttonExit.Text = LanguageTranslation.CANCEL;
            buttonDrop.Text = LanguageTranslation.OPEN_DOOR;
            buttonApplyChanges.Text = LanguageTranslation.APPLY_CHANGES;

            boxNumber = CommonTasks.FindBoxNumber(Location);

            this.label1.Text = LanguageTranslation.PRESS_BUTTON_MODIFY_BOX + " " + boxNumber.ToString() + " " + LanguageTranslation.INFORMATION;
            KeyLocation = Location;

            buttonChangeCardNumber.Enabled = false;
            textBoxCardNumber.Enabled = false;
            buttonChangeRFIDTag.Visible = false;
            textBoxRFID.Visible = false;

            SetTextBoxes();
        }

        private void SetTextBoxes()
        {
            foreach (string s in Program.passwordMgr.FindPassword(boxNumber))
            {
                textBoxAccessCode.Text += s + ",";
            }
            textBoxAccessCode.Text = textBoxAccessCode.Text.TrimEnd(',');

            if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH)
            {
                buttonChangeCardNumber.Enabled = true;
                textBoxCardNumber.Enabled = true;
                textBoxCardNumber.Text = Program.passwordMgr.FindCardNumber(boxNumber);
            }
            else if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY)
            {
                buttonChangeAccessCode.Enabled = false;
                textBoxAccessCode.Enabled = false;
                textBoxAccessCode.Text = "";
                buttonChangeCardNumber.Enabled = true;
                textBoxCardNumber.Enabled = true;
                textBoxCardNumber.Text = Program.passwordMgr.FindCardNumber(boxNumber);
            }

            if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE)
            {
                if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.LIMITED_USE)
                    textBoxAccessRestrictions.Text = LanguageTranslation.LIMITED_USES;
                else if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.TIME_PERIOD)
                    textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD;
                else if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.TIME_PERIOD_LIMITED_USE)
                    textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD_LIMITED_USES;
                else
                    textBoxAccessRestrictions.Text = LanguageTranslation.ALWAYS_ON;
            }
            else 
            {
                buttonChangeAccessRestrictions.Enabled = false;
                textBoxAccessRestrictions.Enabled = false;
            }
            
            if (Program.ENABLE_RFID)
            {
                buttonChangeRFIDTag.Visible = true;
                textBoxRFID.Visible = true;
                textBoxRFID.Text = Program.passwordMgr.FindGenericData(boxNumber);
            }
        }

        /// <summary>
        /// buttonNewNumber_Click - change number of location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChangeAccessCode_Click(object sender, EventArgs e)
        {
            string PasswordtoChange;
            if (Program.passwordMgr.FindPassword(boxNumber).Count > 1)
            {
                TimeOutOff();
                KeyPadForm CurrentPwordEntry = new KeyPadForm(LanguageTranslation.ENTER_CODE, false, Program.PASSWORD_SIZE, true, false);
                CurrentPwordEntry.ShowDialog();
                resetTimer();

                if (!CurrentPwordEntry.bOK)
                {
                    Program.ShowErrorMessage(LanguageTranslation.CANCELLED, 3000);
                    return;
                }

                if (!(Program.passwordMgr.FindPassword(boxNumber).Contains(CurrentPwordEntry.Result)))
                {
                    Program.ShowErrorMessage(LanguageTranslation.ACCESS_CODE_NOT_FOUND, 3000);
                    return;
                }
                PasswordtoChange = CurrentPwordEntry.Result;
            }
            else
            {
                PasswordtoChange = Program.passwordMgr.FindPassword(boxNumber).First<string>();
            }
            TimeOutOff();
            KeyPadForm keyNumberDlg = new KeyPadForm(LanguageTranslation.ENTER_NEW_CODE, false, Program.PASSWORD_SIZE, true, false);
            keyNumberDlg.ShowDialog();
            resetTimer();

            if (!keyNumberDlg.bOK)
            {
                Program.ShowErrorMessage(LanguageTranslation.NEW_NUMBER_CANCELLED, 3000);
                return;
            }

            textBoxAccessCode.Text = textBoxAccessCode.Text.Replace(PasswordtoChange, keyNumberDlg.Result);
        }
        
        private void buttonChangeRFIDTag_Click(object sender, EventArgs e)
        {
            string rfidNumber = Program.passwordMgr.FindGenericData(KeyLocation);

            if (String.IsNullOrEmpty(rfidNumber))
            {
                rfidNumber = "000000";
            }

            TimeOutOff();
            LongNameEntryForm keyNumberDlg = new LongNameEntryForm(Program.GENERIC_DATA_FIELD_LENGTH, false, false, LanguageTranslation.ENTER_RFID_TAG);

            keyNumberDlg.ShowDialog();
            resetTimer();

            if (!keyNumberDlg.Ok)
            {
                Program.ShowErrorMessage(LanguageTranslation.RFID_NUMBER_CANCELLED, 3000);
                return;
            }

            textBoxRFID.Text = keyNumberDlg.Description;
        }

        private void buttonChangeCardNumber_Click(object sender, EventArgs e)
        {
            string cardNumber = Program.passwordMgr.FindCardNumber(boxNumber); ;

            if ((String.IsNullOrEmpty(cardNumber)) || (cardNumber.Length != Program.NUMBER_CREDIT_CARD_DIGITS))
            {
                cardNumber = "0000";
            }

            TimeOutOff();
            KeyPadForm keyNumberDlg = new KeyPadForm(LanguageTranslation.ENTER_CARD_NUMBER, false, Program.NUMBER_CREDIT_CARD_DIGITS, true, false);

            keyNumberDlg.ShowDialog();
            resetTimer();

            if (!keyNumberDlg.bOK)
            {
                Program.ShowErrorMessage(LanguageTranslation.NEW_NUMBER_CANCELLED, 3000);
                return;
            }

            textBoxCardNumber.Text = keyNumberDlg.Result;
        }

        private void buttonChangeAccessRestrictions_Click(object sender, EventArgs e)
        {
            ModifyAccessRestriction restrictions = new ModifyAccessRestriction(AccessRestrictionType.Keypassword, boxNumber.ToString());
            restrictions.ShowDialog();

            if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.LIMITED_USES;
            else if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.TIME_PERIOD)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD;
            else if (Program.passwordMgr.FindAccessTimeType(boxNumber) == accessTimeType.TIME_PERIOD_LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD_LIMITED_USES;
            else
                textBoxAccessRestrictions.Text = LanguageTranslation.ALWAYS_ON;          
        }    

        /// <summary>
        /// buttonDrop_Click - drop key at this location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDrop_Click(object sender, EventArgs e)
        {
            resetTimer();
            if (KeyLocation == -1)
            {
                Program.ShowErrorMessage(LanguageTranslation.BOX_LOCATION_NOT_SET,3000);
                return;
            }
            CommonTasks.OpenKeyBox(KeyLocation, false);
            Program.logEvent("Box " + KeyLocation + " Opened By Administrator");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
             
        private void buttonApplyChanges_Click(object sender, EventArgs e)
        {
            List<string> currentpasswords = new List<string>(Program.passwordMgr.FindPassword(boxNumber));
            int i = 0;
            if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH || Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_SIMPLE)
            {
                foreach (string s in currentpasswords)
                {
                    Program.passwordMgr.SetPassword(boxNumber, s, textBoxAccessCode.Text.Split(',')[i]);
                    i++;
                }
            }
            if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH || Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY)
                Program.passwordMgr.SetCardNumber(boxNumber, textBoxCardNumber.Text);
            if (Program.ENABLE_RFID)
                Program.passwordMgr.SetGenericData(boxNumber, textBoxRFID.Text);

            Program.passwordMgr.SaveFile();
            this.Close();
        }        

        /// <summary>
        /// NO LONGER NEEDED FOR THIS VERSION OF THE FORM
        /// </summary>
        //private void PositionButtons()
        //{
        //    Button[] Buttonstoshow = new Button[6];            
        //    int buttonindex = 0;

        //    if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_SIMPLE)
        //    {
        //        Buttonstoshow[0] = buttonChangeAccessCode;
        //        Buttonstoshow[1] = buttonViewAccessCode;
        //        buttonindex = 2;
        //    }
        //    else if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY)
        //    {
        //        Buttonstoshow[0] = buttonChangeCardNumber;
        //        Buttonstoshow[1] = buttonViewCurrentCardNumber;
        //        buttonindex = 2;
        //    }
        //    else if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH)
        //    {
        //        Buttonstoshow[0] = buttonChangeAccessCode;
        //        Buttonstoshow[1] = buttonViewAccessCode;
        //        Buttonstoshow[2] = buttonChangeCardNumber;
        //        Buttonstoshow[3] = buttonViewCurrentCardNumber;
        //        buttonindex = 4;
        //    }            

        //    if (Program.ENABLE_RFID)
        //    {
        //        Buttonstoshow[buttonindex] = buttonChangeRFIDTag;
        //        Buttonstoshow[buttonindex + 1] = buttonViewRFIDTag;
        //    }

        //    buttonsInvisible();
        //    //6 buttons
        //    if ((Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH) && Program.ENABLE_RFID)
        //    {
        //        Buttonstoshow[0].Visible = true;
        //        Buttonstoshow[1].Visible = true;
        //        Buttonstoshow[2].Visible = true;
        //        Buttonstoshow[3].Visible = true;
        //        Buttonstoshow[4].Visible = true;
        //        Buttonstoshow[5].Visible = true;

        //        Buttonstoshow[0].Location = new Point(40, 270);
        //        Buttonstoshow[1].Location = new Point(40, 335);
        //        Buttonstoshow[2].Location = new Point(40, 400);
        //        Buttonstoshow[3].Location = new Point(40, 465);
        //        Buttonstoshow[4].Location = new Point(410, 270);
        //        Buttonstoshow[5].Location = new Point(410, 335);

        //        Buttonstoshow[0].Size = Buttonstoshow[1].Size = Buttonstoshow[2].Size = Buttonstoshow[3].Size = Buttonstoshow[4].Size = Buttonstoshow[5].Size = new Size(350, 55);
        //        Buttonstoshow[0].Font = Buttonstoshow[1].Font = Buttonstoshow[2].Font = Buttonstoshow[3].Font = Buttonstoshow[4].Font = Buttonstoshow[5].Font = new Font("Arial Black", 22, FontStyle.Bold, GraphicsUnit.Pixel);
        //    }
        //    //4 buttons
        //    else if (((Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH) && (!Program.ENABLE_RFID)) ||
        //              (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY && (Program.ENABLE_RFID)) ||
        //              (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_SIMPLE && (Program.ENABLE_RFID)))
        //    {                
        //        Buttonstoshow[0].Visible = true;
        //        Buttonstoshow[1].Visible = true;
        //        Buttonstoshow[2].Visible = true;
        //        Buttonstoshow[3].Visible = true;

        //        Buttonstoshow[0].Location = new Point(90, 270);
        //        Buttonstoshow[1].Location = new Point(90, 335);
        //        Buttonstoshow[2].Location = new Point(90, 400);
        //        Buttonstoshow[3].Location = new Point(90, 465);
        //    }
        //    //2 buttons
        //    else if (((Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_SIMPLE) && (!Program.ENABLE_RFID)) ||
        //              (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY && (!Program.ENABLE_RFID)))
        //    {
        //        Buttonstoshow[0].Visible = true;
        //        Buttonstoshow[1].Visible = true;

        //        Buttonstoshow[0].Location = new Point(90, 270);
        //        Buttonstoshow[1].Location = new Point(90, 335);
        //    }
        //}

        /// <summary>
        /// NO LONGER NEEDED FOR THIS VERSION OF THE FORM
        /// </summary>
        //private void buttonsInvisible()
        //{
        //    buttonChangeAccessCode.Visible = false;
        //    buttonChangeCardNumber.Visible = false;
        //    buttonChangeRFIDTag.Visible = false;
        //    buttonViewAccessCode.Visible = false;
        //    buttonViewCurrentCardNumber.Visible = false;
        //    buttonViewRFIDTag.Visible = false;
        //}
    }
}
