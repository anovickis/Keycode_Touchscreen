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
    public partial class ModifyUsersForm : baseForm
    {
        User UserToModify;
        internal bool Cancelled;

        public ModifyUsersForm(User usertomodify)
        {
            InitializeComponent();

            buttonID.Text = LanguageTranslation.USER_ID;
            buttonFirstName.Text = LanguageTranslation.FIRST_NAME;
            buttonLastName.Text = LanguageTranslation.LAST_NAME;
            buttonEmail.Text = LanguageTranslation.EMAIL_ADDRESS;
            buttonPhone.Text = LanguageTranslation.PHONE_NUMBER;
            buttonDepartment.Text = LanguageTranslation.DEPARTMENT;
            buttonGroup.Text = LanguageTranslation.GROUP;
            buttonAccessRestrictions.Text = LanguageTranslation.ACCESS_RESTRICTIONS;
            buttonUserType.Text = LanguageTranslation.USER_TYPE;
            buttonSave.Text = LanguageTranslation.SAVE;
            buttonCancel.Text = LanguageTranslation.CANCEL;
            label1.Text = LanguageTranslation.PRESS_MODIFY_USER;

            Cancelled = false;
            UserToModify = usertomodify;
            SetTextBoxes();            
        }
                
        private void SetTextBoxes()
        {
            textBoxID.Text = UserToModify.ID;
            textBoxFirstName.Text = UserToModify.FirstName;
            textBoxLastName.Text = UserToModify.LastName;
            textBoxEmail.Text = UserToModify.emailAddress;
            textBoxPhone.Text = UserToModify.phoneNumber;
            textBoxGroup.Text = UserToModify.Group;
            textBoxDepartment.Text = UserToModify.Department;            

            if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.LIMITED_USES;
            else if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.TIME_PERIOD)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD;
            else if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.TIME_PERIOD_LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD_LIMITED_USES;
            else
                textBoxAccessRestrictions.Text = LanguageTranslation.ALWAYS_ON;
            
            textBoxUserType.Text = ((userType)UserToModify.intType).ToString();
        }

        private void buttonID_Click(object sender, EventArgs e)
        {
            LongNameEntryForm IDForm = new LongNameEntryForm(Program.USER_ID_LENGTH, false, false);
            IDForm.UseSpaceBar = true;
            IDForm.DialogTitle = LanguageTranslation.ENTER_NEW_ID;
            IDForm.ShowDialog();

            if (IDForm.Ok)
            {
                Program.logEvent("User entered new user ID: " + IDForm.Description);

                if (!Program.userMgr.IsUnique(IDForm.Description))
                {
                    Program.logEvent("The user ID Number entered is not unique. User not added");
                    Program.ShowErrorMessage(LanguageTranslation.ID_NUM_NOT_UNIQUE, 3000);
                    return;
                }    
                textBoxID.Text = IDForm.Description;            
            }
            else
            {
                Program.logEvent("User cancelled ID change");
            }            
        }

        private void buttonFirstName_Click(object sender, EventArgs e)
        {
            LongNameEntryForm IDForm = new LongNameEntryForm(20, false, true);
            IDForm.DialogTitle = LanguageTranslation.ENTER_FIRST_NAME;   //just simple name at first
            IDForm.InitialString = "";
            IDForm.ShowDialog();

            if (IDForm.Ok)
            {
                Program.logEvent("User entered first name: " + IDForm.Description);
                textBoxFirstName.Text = IDForm.Description;
            }
            else
            {
                Program.logEvent("User cancelled first name change");
            }            
        }

        private void buttonLastName_Click(object sender, EventArgs e)
        {
            LongNameEntryForm IDForm = new LongNameEntryForm(20, false, true);
            IDForm.DialogTitle = LanguageTranslation.ENTER_LAST_NAME;   //just simple name at first
            IDForm.InitialString = "";
            IDForm.ShowDialog();

            if (IDForm.Ok)
            {
                Program.logEvent("User entered last name: " + IDForm.Description);
                textBoxLastName.Text = IDForm.Description;
            }
            else
            {
                Program.logEvent("User cancelled last name change");
            }            
        }

        private void buttonEmail_Click(object sender, EventArgs e)
        {
            LongNameEntryForm IDForm = new LongNameEntryForm(30, false, true);
            IDForm.DialogTitle = LanguageTranslation.ENTER_EMAIL_ADDRESS;   //just simple name at first
            IDForm.InitialString = "";
            IDForm.UsePeriod = true;
            IDForm.UseAtSign = true;
            IDForm.UseUnderscore = true;
            IDForm.ShowDialog();

            if (IDForm.Ok)
            {
                if (emailCheck(IDForm.Description))
                {
                    Program.logEvent("User entered Email Address: " + IDForm.Description);
                    textBoxEmail.Text = IDForm.Description;
                }
                else
                {
                    Program.logEvent("Email Address: " + IDForm.Description + " Not Formatted Properly");
                    Program.ShowErrorMessage(LanguageTranslation.EMAIL_ADDRESS_NOT_FORMATTED, 4000);
                }
            }
            else
            {
                Program.logEvent("User cancelled email address change");
            }            
        }

        private void buttonPhone_Click(object sender, EventArgs e)
        {
            KeyPadForm IDForm = new KeyPadForm(LanguageTranslation.ENTER_PHONE_NUMBER, false, 10, true, true);            
            IDForm.ShowDialog();

            if (IDForm.bOK)
            {
                Program.logEvent("User entered Email Address: " + IDForm.Result);
                textBoxPhone.Text = IDForm.Result;
            }
            else
            {
                Program.logEvent("User cancelled phone number change");
            }            
        }

        private void buttonUserType_Click(object sender, EventArgs e)
        {
            //Not implemented yet
        }

        private void buttonDepartment_Click(object sender, EventArgs e)
        {
            LongNameEntryForm DepartmentForm = new LongNameEntryForm(30, false, true);
            DepartmentForm.DialogTitle = LanguageTranslation.ENTER_DEPARTMENT_NAME;   //just simple name at first
            DepartmentForm.InitialString = "";
            DepartmentForm.ShowDialog();

            if (DepartmentForm.Ok)
            {
                Program.logEvent("User entered Department name: " + DepartmentForm.Description);
                textBoxDepartment.Text = DepartmentForm.Description;
            }
            else
            {
                Program.logEvent("User cancelled Department name change");
            }
        }

        private void buttonGroup_Click(object sender, EventArgs e)
        {
            LongNameEntryForm GroupForm = new LongNameEntryForm(30, false, true);
            GroupForm.DialogTitle = LanguageTranslation.ENTER_GROUP_NAME;   //just simple name at first
            GroupForm.InitialString = "";
            GroupForm.ShowDialog();

            if (GroupForm.Ok)
            {
                Program.logEvent("User entered Group name: " + GroupForm.Description);
                textBoxGroup.Text = GroupForm.Description;
            }
            else
            {
                Program.logEvent("User cancelled Group name change");
            }
        }

        private void buttonAccessRestrictions_Click(object sender, EventArgs e)
        {
            ModifyAccessRestriction restrictions = new ModifyAccessRestriction(AccessRestrictionType.User, UserToModify.ID);
            restrictions.ShowDialog();

            if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.LIMITED_USES;
            else if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.TIME_PERIOD)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD;
            else if ((accessTimeType)UserToModify.accessTimeIntType == accessTimeType.TIME_PERIOD_LIMITED_USE)
                textBoxAccessRestrictions.Text = LanguageTranslation.TIME_PERIOD_LIMITED_USES;
            else
                textBoxAccessRestrictions.Text = LanguageTranslation.ALWAYS_ON;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Program.userMgr.ChangeUserID(UserToModify.ID, textBoxID.Text);
            Program.userMgr.ChangeUserFirstName(UserToModify.ID, textBoxFirstName.Text);
            Program.userMgr.ChangeUserLastName(UserToModify.ID, textBoxLastName.Text);
            Program.userMgr.ChangeUserEmailAddress(UserToModify.ID, textBoxEmail.Text);
            Program.userMgr.ChangeUserPhoneNumber(UserToModify.ID, textBoxPhone.Text);
            Program.userMgr.ChangeUserDepartment(UserToModify.ID, textBoxDepartment.Text);
            Program.userMgr.ChangeUserGroup(UserToModify.ID, textBoxGroup.Text);

            Program.userMgr.SaveFile();
            this.Close();
        }
        
        private bool emailCheck(string email)
        {
            if ((email.Split('@').Length != 2) || //if there's more or less than one '@' sign
               ((email.Split('.')[email.Split('.').Length - 1] != "com") && //if the suffix is not .com .net or .edu
                (email.Split('.')[email.Split('.').Length - 1] != "net") &&
                (email.Split('.')[email.Split('.').Length - 1] != "edu")) ||
                (email.Split('@')[0].Length <= 0))
                return false;
            else
                return true;
        }        
    }
}
