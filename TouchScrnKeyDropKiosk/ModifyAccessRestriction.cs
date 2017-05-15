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
    public enum AccessRestrictionType
    {
        Keypassword, User
    }

    public partial class ModifyAccessRestriction : baseForm
    {
        AccessRestrictionType Screentype;
        string ObjectID;
        public ModifyAccessRestriction(AccessRestrictionType screentype, string IDnumber)
        {
            InitializeComponent();

            label2.Text = LanguageTranslation.ACCESS_RESTRICTION_TYPE;
            radioButtonAlwaysOn.Text = LanguageTranslation.ALWAYS_ON;
            radioButtonLimitedUses.Text = LanguageTranslation.LIMITED_USES;
            radioButtonTimePeriod.Text = LanguageTranslation.TIME_PERIOD;
            radioButtonLimitTimePeriod.Text = LanguageTranslation.LIMIT_PLUS_TIME_PERIOD;
            label3.Text = LanguageTranslation.START_TIME;
            label4.Text = LanguageTranslation.END_TIME;
            label5.Text = LanguageTranslation.LIMITED_NUMBER_OF_USES;
            buttonApplyChanges.Text = LanguageTranslation.APPLY_CHANGES;
            buttonExit.Text = LanguageTranslation.CANCEL;
            buttonLimitedUses.Text = LanguageTranslation.SET_LIMITED_USES;
            buttonSetEndTime.Text = LanguageTranslation.SET_END_TIME;
            buttonSetStartTime.Text = LanguageTranslation.SET_START_TIME;
            

            Screentype = screentype;
            ObjectID = IDnumber;

            if (Screentype == AccessRestrictionType.Keypassword)
                labelheadline.Text = LanguageTranslation.SET_BOX_NUMBER_ACCESS_RESTRICTIONS + IDnumber;
            else 
                labelheadline.Text = LanguageTranslation.SET_USER_ACCESS_RESTRICTIONS + IDnumber;

            HorizontalCenterLabel(labelheadline);
            SetAccessRestrictionType();
        }
        
        private void SetAccessRestrictionType()
        {
            accessTimeType timetype;
            if (Screentype == AccessRestrictionType.Keypassword)
            {
                timetype = Program.passwordMgr.FindAccessTimeType(int.Parse(ObjectID));
            }
            else
            {
                timetype = Program.userMgr.FindUserTimeAccessType(ObjectID);
            }

            if (timetype == accessTimeType.TIME_PERIOD_LIMITED_USE)
                radioButtonLimitTimePeriod.Checked = true;
            else if (timetype == accessTimeType.LIMITED_USE)
                radioButtonLimitedUses.Checked = true;
            else if (timetype == accessTimeType.TIME_PERIOD)
                radioButtonTimePeriod.Checked = true;
            else
                radioButtonAlwaysOn.Checked = true;
        }

        private void HorizontalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);
        }

        private void radioButtonAlwaysOn_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAlwaysOn.Checked == true)
            {
                datePickerStart.Enabled = false;
                datePickerEnd.Enabled = false;
                timePickerStart.Enabled = false;
                timePickerEnd.Enabled = false;
                limitUsesUpDown.Enabled = false;
                buttonLimitedUses.Enabled = false;
                buttonSetEndTime.Enabled = false;
                buttonSetStartTime.Enabled = false;
            }
        }

        private void radioButtonLimitedUses_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLimitedUses.Checked == true)
            {
                datePickerStart.Enabled = false;
                datePickerEnd.Enabled = false;
                timePickerStart.Enabled = false;
                timePickerEnd.Enabled = false;
                limitUsesUpDown.Enabled = true;
                buttonLimitedUses.Enabled = true;
                buttonSetEndTime.Enabled = false;
                buttonSetStartTime.Enabled = false;

                if (Screentype == AccessRestrictionType.Keypassword)
                {
                    limitUsesUpDown.Value = Program.passwordMgr.FindLimitedNumberofUses(int.Parse(ObjectID));
                }
                else
                {
                    limitUsesUpDown.Value = Program.userMgr.FindUserLimitedUsesCount(ObjectID);
                }
            }
        }

        private void radioButtonTimePeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonTimePeriod.Checked == true)
            {
                datePickerStart.Enabled = true;
                datePickerEnd.Enabled = true;
                timePickerStart.Enabled = true;
                timePickerEnd.Enabled = true;
                limitUsesUpDown.Enabled = false;
                buttonLimitedUses.Enabled = false;
                buttonSetEndTime.Enabled = true;
                buttonSetStartTime.Enabled = true;

                if (Screentype == AccessRestrictionType.Keypassword)
                {
                    datePickerStart.Value = Program.passwordMgr.FindStartTime(int.Parse(ObjectID));
                    datePickerEnd.Value = Program.passwordMgr.FindEndTime(int.Parse(ObjectID));
                    timePickerStart.Value = Program.passwordMgr.FindStartTime(int.Parse(ObjectID));
                    timePickerEnd.Value = Program.passwordMgr.FindEndTime(int.Parse(ObjectID));
                }
                else
                {
                    datePickerStart.Value = Program.userMgr.FindUserTimeframeStart(ObjectID);
                    datePickerEnd.Value = Program.userMgr.FindUserTimeframeEnd(ObjectID);
                    timePickerStart.Value = Program.userMgr.FindUserTimeframeStart(ObjectID);
                    timePickerEnd.Value = Program.userMgr.FindUserTimeframeEnd(ObjectID);
                }
            }
        }

        private void radioButtonLimitTimePeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLimitTimePeriod.Checked == true)
            {
                datePickerStart.Enabled = true;
                datePickerEnd.Enabled = true;
                timePickerStart.Enabled = true;
                timePickerEnd.Enabled = true;
                limitUsesUpDown.Enabled = true;
                buttonLimitedUses.Enabled = true;
                buttonSetEndTime.Enabled = true;
                buttonSetStartTime.Enabled = true;

                if (Screentype == AccessRestrictionType.Keypassword)
                {
                    datePickerStart.Value = Program.passwordMgr.FindStartTime(int.Parse(ObjectID));
                    datePickerEnd.Value = Program.passwordMgr.FindEndTime(int.Parse(ObjectID));
                    timePickerStart.Value = Program.passwordMgr.FindStartTime(int.Parse(ObjectID));
                    timePickerEnd.Value = Program.passwordMgr.FindEndTime(int.Parse(ObjectID));
                    limitUsesUpDown.Value = Program.passwordMgr.FindLimitedNumberofUses(int.Parse(ObjectID));
                }
                else
                {
                    datePickerStart.Value = Program.userMgr.FindUserTimeframeStart(ObjectID);
                    datePickerEnd.Value = Program.userMgr.FindUserTimeframeEnd(ObjectID);
                    timePickerStart.Value = Program.userMgr.FindUserTimeframeStart(ObjectID);
                    timePickerEnd.Value = Program.userMgr.FindUserTimeframeEnd(ObjectID);
                    limitUsesUpDown.Value = Program.userMgr.FindUserLimitedUsesCount(ObjectID);
                }
            }
        }

        private void buttonSetStartTime_Click(object sender, EventArgs e)
        {
            KeyPadForm setstart = new KeyPadForm(LanguageTranslation.ENTER_START_TIME, false, 4, true, true, GenericDataEntryType.Time);
            setstart.ShowDialog();

            if (setstart.bOK)
            {
                try
                {
                    timePickerStart.Value = new DateTime(2000, 1, 1, int.Parse(setstart.Result.Split(':')[0]), int.Parse(setstart.Result.Split(':')[1]), 0);
                }
                catch (Exception ex)
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_TIME, 3000);
                }
            }
            else
                Program.logEvent("Start Time Entry Cancelled");
        }

        private void buttonSetEndTime_Click(object sender, EventArgs e)
        {
            KeyPadForm setend = new KeyPadForm(LanguageTranslation.ENTER_END_TIME, false, 4, true, true, GenericDataEntryType.Time);
            setend.ShowDialog();

            if (setend.bOK)
            {
                try
                {
                    timePickerEnd.Value = new DateTime(2000, 1, 1, int.Parse(setend.Result.Split(':')[0]), int.Parse(setend.Result.Split(':')[1]), 0);
                }
                catch (Exception ex)
                {
                    Program.ShowErrorMessage(LanguageTranslation.INVALID_TIME, 3000);
                }
            }
            else
                Program.logEvent("End Time Entry Cancelled");
        }

        private void buttonLimitedUses_Click(object sender, EventArgs e)
        {
            KeyPadForm uses = new KeyPadForm(LanguageTranslation.ENTER_LIMITED_USES, false, 5, true, false);
            uses.ShowDialog();

            if (uses.bOK)
                limitUsesUpDown.Value = int.Parse(uses.Result);
            else
                Program.logEvent("Limited Time Entry Cancelled");
        }

        private void buttonApplyChanges_Click(object sender, EventArgs e)
        {
            if (Screentype == AccessRestrictionType.Keypassword)
            {
                if (radioButtonLimitedUses.Checked || radioButtonLimitTimePeriod.Checked)
                {
                    Program.passwordMgr.SetAccessTimeType(int.Parse(ObjectID), accessTimeType.LIMITED_USE); //Below TimeAccessType will be set correctly if radioButtonMultiUseandTimePeriod is Checked
                    Program.passwordMgr.SetLimitedNumberofUses(int.Parse(ObjectID), (int)limitUsesUpDown.Value);
                    Program.logEvent("Multiple Uses Being Set: " + limitUsesUpDown.Value.ToString());
                }
                if (radioButtonTimePeriod.Checked || radioButtonLimitTimePeriod.Checked)
                {
                    DateTime Start, End;
                    Start = new DateTime(datePickerStart.Value.Year, datePickerStart.Value.Month, datePickerStart.Value.Day,
                                                                   timePickerStart.Value.Hour, timePickerStart.Value.Minute, timePickerStart.Value.Second);
                    End = new DateTime(datePickerEnd.Value.Year, datePickerEnd.Value.Month, datePickerEnd.Value.Day,
                                                                   timePickerEnd.Value.Hour, timePickerEnd.Value.Minute, timePickerEnd.Value.Second);
                    if (Start > End)
                    {
                        Program.ShowErrorMessage(LanguageTranslation.START_DATE_LATER, 4000);
                        return;
                    }

                    Program.passwordMgr.SetAccessTimeType(int.Parse(ObjectID), accessTimeType.TIME_PERIOD); //Below TimeAccessType will be set correctly if radioButtonMultiUseandTimePeriod is Checked
                    Program.passwordMgr.SetStartTime(int.Parse(ObjectID), Start);
                    Program.passwordMgr.SetEndTime(int.Parse(ObjectID), End);
                    Program.logEvent("Start Timeframe Being Set: " + Start.ToString());
                    Program.logEvent("End Timeframe Being Set: " + End.ToString());
                }
                if (radioButtonLimitTimePeriod.Checked)
                {
                    Program.passwordMgr.SetAccessTimeType(int.Parse(ObjectID), accessTimeType.TIME_PERIOD_LIMITED_USE);
                }
                if (!radioButtonLimitTimePeriod.Checked && !radioButtonLimitedUses.Checked && !radioButtonTimePeriod.Checked)
                {
                    Program.passwordMgr.SetAccessTimeType(int.Parse(ObjectID), accessTimeType.ALWAYS_ON);
                    Program.logEvent("Always On Mode Set");
                }
            }
            else
            {
                if (radioButtonLimitedUses.Checked || radioButtonLimitTimePeriod.Checked)
                {
                    Program.userMgr.ChangeUserTimeAccessType(ObjectID, accessTimeType.LIMITED_USE); //Below TimeAccessType will be set correctly if radioButtonMultiUseandTimePeriod is Checked
                    Program.userMgr.ChangeUserLimitedUses(ObjectID, (int)limitUsesUpDown.Value);
                    Program.logEvent("Multiple Uses Being Set: " + limitUsesUpDown.Value.ToString());
                }
                if (radioButtonTimePeriod.Checked || radioButtonLimitTimePeriod.Checked)
                {
                    DateTime Start, End;
                    Start = new DateTime(datePickerStart.Value.Year, datePickerStart.Value.Month, datePickerStart.Value.Day,
                                                                   timePickerStart.Value.Hour, timePickerStart.Value.Minute, timePickerStart.Value.Second);
                    End = new DateTime(datePickerEnd.Value.Year, datePickerEnd.Value.Month, datePickerEnd.Value.Day,
                                                                   timePickerEnd.Value.Hour, timePickerEnd.Value.Minute, timePickerEnd.Value.Second);
                    if (Start > End)
                    {
                        Program.ShowErrorMessage(LanguageTranslation.START_DATE_LATER, 4000);
                        return;
                    }

                    Program.userMgr.ChangeUserTimeAccessType(ObjectID, accessTimeType.TIME_PERIOD); //Below TimeAccessType will be set correctly if radioButtonMultiUseandTimePeriod is Checked
                    Program.userMgr.ChangeUserTimeframestart(ObjectID, Start);
                    Program.userMgr.ChangeUserTimeframeend(ObjectID, End);
                    Program.logEvent("Start Timeframe Being Set: " + Start.ToString());
                    Program.logEvent("End Timeframe Being Set: " + End.ToString());
                }
                if (radioButtonLimitTimePeriod.Checked)
                {
                    Program.userMgr.ChangeUserTimeAccessType(ObjectID, accessTimeType.TIME_PERIOD_LIMITED_USE);
                }
                if (!radioButtonLimitTimePeriod.Checked && !radioButtonLimitedUses.Checked && !radioButtonTimePeriod.Checked)
                {
                    Program.userMgr.ChangeUserTimeAccessType(ObjectID, accessTimeType.ALWAYS_ON);
                    Program.logEvent("Always On Mode Set");
                }
            }
            this.Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Program.logEvent("Modify Access Restrictions Cancelled");
            this.Close();
        }
    }
}
