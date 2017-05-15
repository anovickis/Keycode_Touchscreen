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
    public partial class BiometricEnrollmentForm : baseForm
    {
        string UserID;

        public BiometricEnrollmentForm(string id) 
            :base (Program.TIMEOUT_INTERVAL)
        {
            UserID = id;
            InitializeComponent();
            labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT;
            buttonClear.Text = LanguageTranslation.CLEAR_ENROLLED_FINGERPRINTS;
            buttonEnroll.Text = LanguageTranslation.ENROLL_FINGERPRINTS;
            buttonExit.Text = LanguageTranslation.EXIT;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Program.biometricMgr.ClearFingerprint(UserID);
            Program.biometricMgr.SaveFile();
            Program.ShowErrorMessage(LanguageTranslation.FINGERPRINTS_CLEARED, 3000);
            if (Program.biometricMgr.FindFingerprint(UserID) == null)
            {
                labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + UserID +
                " = 0";
            }
            else
            {
                labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + UserID +
                    " = " + Program.biometricMgr.FindFingerprint(UserID).Count;
            }
        }

        private void buttonEnroll_Click(object sender, EventArgs e)
        {
            TimeOutOff();
            DigitalPersonaBiometricsForm EnrollBiometrics = new DigitalPersonaBiometricsForm("Fingerprints.xml", DPBiometricsState.Enrolling, UserID, Program.BIOMETRIC_TIMEOUT, Program.biometricMgr);
            EnrollBiometrics.ShowDialog();
            if (Program.biometricMgr.FindFingerprint(UserID) == null)
            {
                labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + UserID +
                " = 0";
            }
            else
            {
                labelUserID.Text = LanguageTranslation.ENROLLED_FINGERPRINT_COUNT + UserID +
                    " = " + Program.biometricMgr.FindFingerprint(UserID).Count;
            }
            resetTimer();
        }
    }
}
