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
    /// ChangeAdminPassWordForm - a simple dialog for changing the password\access code for the administrator
    /// </summary>
    public partial class ChangeAdminPassWordForm : baseForm
    {

        private const int ADMIN_PASSWORD_LOCATION = 0;

        private string AdminPassword
        {
            get
            {
                return Program.passwordMgr.FindPassword(ADMIN_PASSWORD_LOCATION).First<string>();
            }
            set
            {
                if (value.Length != Program.PASSWORD_SIZE)
                {
                    throw new Exception("ChangeAdminPassWordForm:AdminPassword - password not correct length");
                }
                if (!Program.passwordMgr.isUnique(value))
                {
                    throw new Exception("ChangeAdminPassWordForm:AdminPassword - password not unigue");
                }
            }
        }

        public ChangeAdminPassWordForm()
            :base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            buttonAdmin.Text = LanguageTranslation.ADMIN_PASSWORD;
            buttonExit.Text = LanguageTranslation.EXIT;
            this.textBoxAdmin.Text = AdminPassword;

            textBoxAdminConfig1.Text = LanguageTranslation.NOT_AVAILABLE; buttonAdminConfig1.Enabled = false;
            textBoxAdminConfig2.Text = LanguageTranslation.NOT_AVAILABLE; buttonAdminConfig2.Enabled = false;
            textBoxAdminConfig3.Text = LanguageTranslation.NOT_AVAILABLE; buttonAdminConfig3.Enabled = false;
            if (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 0)
            {
                buttonAdminConfig1.Text = Program.locationdata.ConfigurableCodeAdminScreensList[0].ToString();
                textBoxAdminConfig1.Text = Program.CONFIG_ADMIN_PWORD_1;
                buttonAdminConfig1.Enabled = true;
            }
            if (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 1)
            {
                buttonAdminConfig2.Text = Program.locationdata.ConfigurableCodeAdminScreensList[1].ToString();
                textBoxAdminConfig2.Text = Program.CONFIG_ADMIN_PWORD_2;
                buttonAdminConfig2.Enabled = true;
            }
            if (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 2)
            {
                buttonAdminConfig3.Text = Program.locationdata.ConfigurableCodeAdminScreensList[2].ToString();
                textBoxAdminConfig3.Text = Program.CONFIG_ADMIN_PWORD_3;
                buttonAdminConfig3.Enabled = true;
            }
        }
        
        /// <summary>
        /// buttonExit_Click - exit or close this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// IsPasswordUnique - checks to see that password has not been repeated
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsPasswordUnique(string password)
        {
            return Program.passwordMgr.isUnique(password);
        }
        /// <summary>
        /// ChangePassword - changes password at this location - does no tests for corred
        /// </summary>
        /// <param name="location"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool ChangePassword(int location, string password)
        {
            if (Program.passwordMgr.SetPassword(location, AdminPassword, password))
            {
                Program.passwordMgr.SaveFile();
                Program.logEvent("Admin Password Changed to " + password);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// AutoGeneratePassword - will create a random password that is unique
        /// </summary>
        /// <returns></returns>
        private string AutoGeneratePassword()
        {
            return Program.passwordMgr.autoGeneratePassword();
        }

        private void buttonAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                string result = getResult(AdminPassword);

                ChangePassword(ADMIN_PASSWORD_LOCATION, result);

                this.textBoxAdmin.Text = result;
            }
            catch (Exception ex)
            {
                Program.logDebug("ChangeAdminPassWordForm:buttonEnter_Click exception " + ex.Message);
                throw new Exception("ChangeAdminPassWordForm:buttonEnter_Click exception " + ex.Message);
            }
        }

        private void buttonAdminConfig1_Click(object sender, EventArgs e)
        {
            try
            {
                string result = getResult(Program.CONFIG_ADMIN_PWORD_1);

                Program.CONFIG_ADMIN_PWORD_1 = result;
                textBoxAdminConfig1.Text = result;
                CommonTasks.WriteValueToConfigurationFile("configurableadminpasswords", "adminpassword1", result);
                Program.logEvent("Configurable Admin Password 1 Set: " + result);
            }
            catch (Exception ex)
            {
                Program.logDebug("ChangeAdminPassWordForm:buttonAdminConfig1_Click exception " + ex.Message);
                throw new Exception("ChangeAdminPassWordForm:buttonAdminConfig1_Click exception " + ex.Message);
            }
        }

        private void buttonAdminConfig2_Click(object sender, EventArgs e)
        {
            try
            {
                string result = getResult(Program.CONFIG_ADMIN_PWORD_2);

                Program.CONFIG_ADMIN_PWORD_2 = result;
                textBoxAdminConfig2.Text = result;
                CommonTasks.WriteValueToConfigurationFile("configurableadminpasswords", "adminpassword2", result);
                Program.logEvent("Configurable Admin Password 2 Set: " + result);
            }
            catch (Exception ex)
            {
                Program.logDebug("ChangeAdminPassWordForm:buttonAdminConfig2_Click exception " + ex.Message);
                throw new Exception("ChangeAdminPassWordForm:buttonAdminConfig2_Click exception " + ex.Message);
            }
        }

        private void buttonAdminConfig3_Click(object sender, EventArgs e)
        {
            try
            {
                string result = getResult(Program.CONFIG_ADMIN_PWORD_3);

                Program.CONFIG_ADMIN_PWORD_3 = result;
                textBoxAdminConfig3.Text = result;
                CommonTasks.WriteValueToConfigurationFile("configurableadminpasswords", "adminpassword3", result);
                Program.logEvent("Configurable Admin Password 3 Set: " + result);
            }
            catch (Exception ex)
            {
                Program.logDebug("ChangeAdminPassWordForm:buttonAdminConfig3_Click exception " + ex.Message);
                throw new Exception("ChangeAdminPassWordForm:buttonAdminConfig3_Click exception " + ex.Message);
            }
        }

        private string getResult(string currentPassword)
        {            
            TimeOutOff();
            KeyPadForm keyNumberDlg = new KeyPadForm(LanguageTranslation.CHANGE_PASSWORD, false, Program.PASSWORD_SIZE, true, false);

            resetTimer();
            keyNumberDlg.ShowDialog();

            if (!keyNumberDlg.bOK)
            {
                Program.ShowErrorMessage(LanguageTranslation.NEW_PASSWORD_CANCELLED, 3000);
                return currentPassword;
            }
            string result = keyNumberDlg.Result;

            while (result.Length > Program.PASSWORD_SIZE)
            {
                Program.ShowErrorMessage(LanguageTranslation.PASSWORD_LENGTH_ERROR + Program.PASSWORD_SIZE + LanguageTranslation.PASSWORD_LENGTH_ERROR_2, 3000);
                
                keyNumberDlg.ShowDialog();

                result = keyNumberDlg.Result;
                if (!keyNumberDlg.bOK)
                {
                    Program.ShowErrorMessage(LanguageTranslation.NEW_PASSWORD_CANCELLED, 3000);
                    return currentPassword;
                }                    
            }
            return keyNumberDlg.Result;            
        }
    }
}
