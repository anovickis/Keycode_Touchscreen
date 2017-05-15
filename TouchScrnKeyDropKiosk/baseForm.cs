using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// baseForm - a base for all dialogs - holds some common activities including:
    ///             - an inactivity timeout timer - can be used or not
    ///             - a developer or production setup for a dialog
    ///             - several thread safe operations such as closing a dialog and centering
    ///                 a label
    ///                 
    /// </summary>
    public partial class baseForm : Form
    {
        public bool TimedOut { get; set; }

        int timeoutValue = 0;

        private System.Timers.Timer timerTimeOut;

        public bool DialogClosed { get; private set; }
        /// <summary>
        ///  baseForm constructor - with timeout not started or value set.
        /// </summary>
        public baseForm()
        {
            InitializeComponent();

            if (!Program.DEV_STATION_SETUP) 
            { 
                makeLikeParent();         
            }

            timerTimeOut = new System.Timers.Timer(500000000);  //arbitrary huge number

            TimedOut = false;

            DialogClosed = false;
        }

         /// <summary>
        ///  This a base class for the forms - it provides a timeout and a development setup for the windows
        /// </summary>
        public baseForm(int TimeoutValue)
        {
            InitializeComponent();

            if (!Program.DEV_STATION_SETUP) { makeLikeParent(); }

            TimedOut = false;

            timeoutValue = TimeoutValue;

            timerTimeOut = new System.Timers.Timer(TimeoutValue);
            timerTimeOut.Elapsed += new ElapsedEventHandler(timerTimeOut_Elapsed);
            timerTimeOut.SynchronizingObject = this;
            timerTimeOut.Start();
               
            DialogClosed = false;
        }

        public virtual void timerTimeOut_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerTimeOut.Stop();
                TimedOut = true;
                ThreadSafeCloseDialog();
            }
            catch(Exception)
            {
                //do nothing
            }
        }


        public void resetTimer()
        {
            this.timerTimeOut.Interval = timeoutValue;
            this.timerTimeOut.Stop();
            this.timerTimeOut.Start();
        }
        public void TimeOutOff()
        {
            this.timerTimeOut.Stop();
        }

        public virtual void DialogTimerSet(int timeout)
        {
            this.timerTimeOut.Interval = timeout;   
            this.timerTimeOut.Start();
        }


        private void makeLikeParent()
        {
            try
            {
                FormBorderStyle = FormBorderStyle.None;
                this.Location = new Point(0, 0);
                this.WindowState = FormWindowState.Maximized;
                this.Cursor = new Cursor("blank.cur");
            }
            catch (Exception ex)
            {
                Program.logEvent("Exception BaseForm:makeLikeParent - " + ex.Message);
            }
        }

        /// <summary>
        /// Closes the dialog across thread lines without causing a system crash
        /// </summary>
        public virtual void ThreadSafeCloseDialog()
        {
            if (this.InvokeRequired)
            {
                threadSafeClose c = new threadSafeClose(closeDialog);

                this.Invoke(c, null);
            }
            else
            {
                this.Close();
            }
        }
        delegate void threadSafeClose();
      
        void closeDialog()
        {
            this.Close();
        }

        /// <summary>
        /// Allows another thread to made labels of this form visible and invisible
        /// </summary>
        /// <param name="label"></param>
        /// <param name="visible"></param>
        public void threadSafeLabelVisible(System.Windows.Forms.Label label, bool visible)
        {
            if (DialogClosed)
            {
                return;
            }
            if (this.InvokeRequired)
            {
                SetLabelVisible i = new SetLabelVisible(setLableVisible);
                this.Invoke(i, new object[] { label, visible });
            }
            else
            {
                label.Visible = visible;
            }

        }
        delegate void SetLabelVisible(System.Windows.Forms.Label label, bool visible);

        private void setLableVisible(System.Windows.Forms.Label label, bool visible)
        {
            label.Visible = visible;
        }

        /// <summary>
        /// Allows another thread to change the text of a label in this form.
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="NewText"></param>
        public void threadSafeLabelChanging(System.Windows.Forms.Label Label, string NewText)
        {
            if (DialogClosed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                SetLabelText i = new SetLabelText(setLabelText);
                this.Invoke(i, new object[] { Label, NewText });
            }
            else
            {
                setLabelText(Label, NewText);
            }
        }


        delegate void SetLabelText(System.Windows.Forms.Label Label, string NewText);

        private void setLabelText(System.Windows.Forms.Label Label, string NewText)
        {
            Label.Text = NewText;
        }

        /// <summary>
        /// Allows another thread to center the selected label on the form
        /// </summary>
        /// <param name="label"></param>
        public void ThreadSafeLabelCentering(System.Windows.Forms.Label label)
        {
            if (this.InvokeRequired)
            {
                LabelCentering i = new LabelCentering(labelCentering);
                this.Invoke(i, new object[] { label });
            }
            else
            {
                labelCentering(label);
            }

        }
        delegate void LabelCentering(System.Windows.Forms.Label label);

        private void labelCentering(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);
        }

        /// <summary>
        /// This method allows another thread to center a label and change its text safely
        /// </summary>
        /// <param name="label"></param>
        /// <param name="NewText"></param>
        public void ThreadSafeLabelCenteringChange(System.Windows.Forms.Label label, string NewText)
        {
            if (this.InvokeRequired)
            {
                LabelCenteringChange i = new LabelCenteringChange(labelCenteringChange);
                this.Invoke(i, new object[] { label, NewText });
            }
            else
            {
                labelCenteringChange(label, NewText);
            }

        }
        delegate void LabelCenteringChange(System.Windows.Forms.Label label, string NewText);

        private void labelCenteringChange(System.Windows.Forms.Label label, string NewText)
        {
            label.Text = NewText;
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);           
        }


        public void ThreadSafeLabelVerticalCenteringChange(System.Windows.Forms.Label label, string NewText)
        {
            if (this.InvokeRequired)
            {
                LabelVertCenteringChange i = new LabelVertCenteringChange(labelVertCenteringChange);
                this.Invoke(i, new object[] { label, NewText });
            }
            else
            {
                labelVertCenteringChange(label, NewText);
            }
        }
        delegate void LabelVertCenteringChange(System.Windows.Forms.Label label, string NewText);

        private void labelVertCenteringChange(System.Windows.Forms.Label label, string NewText)
        {
            label.Text = NewText;
            label.Location = new System.Drawing.Point(label.Location.X,(label.Parent.Height - label.Height) / 2);           
        }
    }
}
