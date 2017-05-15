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
    /// This form is designed to present the user with a question with a Yes or No answer. However, it can be configured to give any
    /// question which has only two possible answers.
    /// </summary>
    public partial class YesNoForm : baseForm
    {
        public bool YesResult { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message">This is the question to be asked</param>
        public YesNoForm(string Message)
            :base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            setFormMessage(Message);

            YesResult = true;
        }
        /// <summary>
        /// Changes what the answer for the default "yes" button is
        /// </summary>
        /// <param name="yesText"></param>
        public void ChangeTextYesButton(string yesText)
        {
            buttonYes.Text = yesText;
        }
        /// <summary>
        /// Changes what the answer for the default "no" button is
        /// </summary>
        /// <param name="yesText"></param>
        public void ChangeTextNoButton(string noText)
        {
            buttonNo.Text = noText;
        }

        /// <summary>
        /// This constructor allows the user to split their question between two lines and thus allow for longer questions
        /// </summary>
        /// <param name="Message1"></param>
        /// <param name="Message2"></param>
        public YesNoForm(String Message1, String Message2)
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            setFormMessage(Message1, Message2);

            YesResult = true;
        }
        private void buttonYes_Click(object sender, EventArgs e)
        {
            YesResult = true;
            Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            YesResult = false;
            Close();
        }

        /// <summary>
        /// This method safely sets the label in the middle of the screen (for when only one label is set) even if it is across threads
        /// </summary>
        /// <param name="Message"></param>
        internal void setFormMessage(string Message)
        {
            this.label2.Visible = false;
            this.label3.Visible = false;
            base.ThreadSafeLabelCenteringChange(this.labelMessage, Message);
            base.ThreadSafeLabelVerticalCenteringChange(this.labelMessage, Message);
        }

        /// <summary>
        /// This method safely sets the labels on top and bottom of the question area (for when both message lines are needed) even if it is across threads
        /// </summary>
        /// <param name="Message1"></param>
        /// <param name="Message2"></param>
        internal void setFormMessage(string Message1, string Message2)
        {
            this.labelMessage.Visible = false;
            base.ThreadSafeLabelCenteringChange(this.label2, Message1);
            base.ThreadSafeLabelCenteringChange(this.label3, Message2);
        }

        public void ChangeButtonFontSize(int size)
        {
            buttonYes.Font = new System.Drawing.Font("Arial Black", (float)size, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            buttonNo.Font = new System.Drawing.Font("Arial Black", (float)size, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }
    }
}
