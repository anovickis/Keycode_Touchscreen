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
    /// ScanCardForm - This dialog prompts the user to swipe a credit card which is the read 
    ///             using the cardReaderTask. The data is read from the card stripes but only
    ///             the last 4 digits of the card and card name are available from this dialog
    ///             as information for determining that the card is valid. No data from the card
    ///             is presisted.
    /// </summary>
    public partial class ScanCardForm : baseForm
    {
        private cardReaderTask cardRead = new cardReaderTask();

        public bool Cancelled { get; private set; }   //user cancelled this dialog
        public bool GoodData { get; private set; }   // there was a good read of data - no check for being valid
        public String CardName { get; private set; }    // the name on the card
        public String CardNumber { get; private set; }  // the number on the card - only last four digits
        private int ScanAttempts { get; set; }

        private System.Windows.Forms.Timer startupTimer;  //used to start up card scan after dialog starts

        public ScanCardForm()
            :base(Program.TIMEOUT_INTERVAL)           // this dialog will timeout
        {
            InitializeComponent();

            buttonExit.Text = LanguageTranslation.CANCEL;
            label1.Text = LanguageTranslation.SWIPE_CARD;
            label2.Text = LanguageTranslation.SCAN_ATTEMPTS;

            Cancelled = false;
            GoodData = false;

            // this is the event from a good card read
            cardReaderTask.ScanDataRecieved += new cardReaderTask.ScanDataRecievedEventHandler(cardReaderTask_ScanDataRecieved);

            // used just to start read after dialog is up - for visual smoothness
            startupTimer = new System.Windows.Forms.Timer();
            startupTimer.Interval = 100;
            startupTimer.Tick += new EventHandler(startupTimer_Tick);
            startupTimer.Start();

            ScanAttempts = 0;
        }

        void startupTimer_Tick(object sender, EventArgs e)
        {
            startupTimer.Stop();
            cardRead.StartScan();
        }
        /// <summary>
        /// cardReaderTask_ScanDataRecieved - if the card has been scanned 
        ///                         CreditCardReadEventArgs - will have the card name
        ///                         if read is bad will start a new scan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cardReaderTask_ScanDataRecieved(object sender, CreditCardReadEventArgs e)
        {
            base.resetTimer();

            Program.logDebug("ScanCardForm - card data read");

            keyCabinetCardData data =(keyCabinetCardData) e.CardData;

            if ((e.GoodRead == false) || (data.isValid() == false))
            {
                GoodData = false;

                Program.logDebug("ScanCardForm : bad credit card read");

                //Program.ShowErrorMessage("Scan error\r\ntry again", 2000);
                this.label1.Text = LanguageTranslation.CARD_SWIPE_ERROR;
                this.label2.Visible = true;
                ScanAttempts++;
                this.label2.Text = LanguageTranslation.SCAN_ATTEMPTS + ScanAttempts.ToString();
                cardRead.StartScan(); // start a new scan
            }
            else
            {
                GoodData = true;

                CardName = data.getCardName();

                CardNumber = data.getCardNumber();

                this.Close();
            }
        }
        /// <summary>
        /// buttonExit_Click - exit dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            cardRead.StopScan();
            Cancelled = true;
            Close();
        }
        /// <summary>
        /// timerTimeOut_Elapsed - if dialog times out this event will be called
        ///                        base will close dialog and set flag for timeout occurring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void timerTimeOut_Elapsed(object sender, ElapsedEventArgs e)
        {
            cardRead.StopScan();

            base.timerTimeOut_Elapsed(sender, e);
        }
    }
}
