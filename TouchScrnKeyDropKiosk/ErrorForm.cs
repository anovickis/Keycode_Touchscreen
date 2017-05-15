using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace KeyCabinetKiosk
{
    public partial class ErrorForm : baseForm//Form   // not derived from base - no need for timer
    {
        /// <summary>
        ///  Class ErrorForm
        ///  Design:
        ///  This class creates a form to display a message which is centered on form.
        ///  The constructor starts a time out timer, displays the message. When the timer times 
        ///  out the form is closed
        ///  
        /// </summary>
       // private System.Windows.Forms.Timer timeOut; //closes the form
        private System.Timers.Timer timeOut2;

        private ErrorForm() { }  // default contructor private - do not use
        /// <summary>
        ///  This contructor will create the form, display "message" and
        ///  the delay time specified in "MilliTime" is reached
        ///  close the from
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="MilliTime"></param>
        public ErrorForm(string Message,int MilliTime)
            :base()
        {
            InitializeComponent();
            //this.BackColor = System.Drawing.SystemColors.Desktop;
            FormBorderStyle = FormBorderStyle.None;     
           
            SetLabelText(label1, Message);           

            timeOut2 = new System.Timers.Timer();
            timeOut2.Interval = MilliTime;
            timeOut2.Elapsed += new System.Timers.ElapsedEventHandler(timeOut_Tick);
            timeOut2.Start();
        }

        /// <summary>
        /// This constructor will create the form, display "message" and wait to be closed.
        /// It is meant to be used to show a message while other processes need to happen.
        /// </summary>
        /// <param name="Message"></param>
        public ErrorForm(string Message)
            : base()
        {
            InitializeComponent();
            //this.BackColor = System.Drawing.SystemColors.Desktop;
            FormBorderStyle = FormBorderStyle.None;

            SetLabelText(label1, Message);   
        }

        void timeOut_Tick(Object sender, EventArgs e)
        {
            ThreadSafeCloseDialog();
        }

        private void makeLikeParent()
        {
            FormBorderStyle = FormBorderStyle.None;
            this.Location = new Point(0, 0);
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            try
            {
                this.Cursor = new Cursor("blank.cur");
            }
            catch(Exception ex)
            {
                // log and then eat the exception
                Program.logEvent("Error in creating error form" + ex.Message);
            }
        }
        public override void ThreadSafeCloseDialog()
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


        private void SetLabelText(System.Windows.Forms.Label label, string text)
        {
            label.Text = text;
            HorizontalCenterLabel(label);
            VerticalCenterLabel(label);
            label.Update();
            this.Update();
        }

        public void SetLabelText(string text)
        {
            label1.Text = text;
            HorizontalCenterLabel(label1);
            VerticalCenterLabel(label1);
            label1.Update();
            this.Update();
        }

        private void HorizontalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);
        }

        private void VerticalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(label.Location.X, (label.Parent.Height - label.Height)/2);
                                                    
        }
        void closeDialog()
        {
            label1.Visible = false;
            this.Close();
        }        
    }
}


  
   