using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace USB_Card_Scanner
{
    /// <summary>
    /// AUTHOR: Adam Bertsch
    /// 
    /// DATE:   9-21-08
    /// 
    /// DESIGN/PURPOSE: 
    /// This module is designed to handle the functionality of the MagTek card reader. In particular the 
    /// USB enable is turned on in the module so that the USB card readers will be recognized. This can be
    /// accessed if the serial version needs to be used, but this is not advised as the USB is much faster
    /// and USB ports are much easier to find.
    /// </summary>
    public class CardScan : Form
    {
        public AxotCreditCard.AxcreditCardControl CreditCardControl;
        public string LastTrack1 { get; private set; }
        public string LastTrack2 { get; private set; }
        public string LastTrack3 { get; private set; }

        public CardScan()
        {
            InitializeComponent();
            //CreditCardControl = new AxotCreditCard.AxcreditCardControl();
            CreditCardControl.otEnableUsb = true;
            CreditCardControl.cardInserted += new AxotCreditCard.__creditCardControl_cardInsertedEventHandler(CreditCardControl_cardInserted);
        }

        void CreditCardControl_cardInserted(object sender, AxotCreditCard.__creditCardControl_cardInsertedEvent e)
        {
            //LastTrack1 = e.track1;
            //LastTrack2 = e.track2;
            //LastTrack3 = e.track3;
            stopScan();
        }

        public void startScan()
        {
            CreditCardControl.otStart();
        }

        public void stopScan()
        {
            CreditCardControl.otStop();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardScan));
            this.CreditCardControl = new AxotCreditCard.AxcreditCardControl();
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardControl)).BeginInit();
            this.SuspendLayout();
            // 
            // CreditCardControl
            // 
            this.CreditCardControl.Enabled = true;
            this.CreditCardControl.Location = new System.Drawing.Point(12, 12);
            this.CreditCardControl.Name = "CreditCardControl";
            //this line taken out to prevent the need for a .resx file associated w/ this class
            //this.CreditCardControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("CreditCardControl.OcxState")));
            this.CreditCardControl.Size = new System.Drawing.Size(268, 242);
            this.CreditCardControl.TabIndex = 0;
            // 
            // CardScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.CreditCardControl);
            this.Name = "CardScan";
            this.Text = "CardScan";
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardControl)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
