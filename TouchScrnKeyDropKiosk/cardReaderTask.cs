using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USB_Card_Scanner;

namespace KeyCabinetKiosk
{

    /// <summary>
    /// cardReaderTask - uses the card scanner - reads in data from card and then uses an event to pass the data
    ///                 out to the calling dialog
    /// </summary>
    class cardReaderTask  
    {

        private CardScan cardscan;   //uses the DLL that controls the scanner


        public delegate void ScanDataRecievedEventHandler(object sender, CreditCardReadEventArgs e);
        public static event ScanDataRecievedEventHandler ScanDataRecieved;
             
        public cardReaderTask()
        {           
            cardscan = new CardScan();

            cardscan.CreditCardControl.cardInserted += new AxotCreditCard.__creditCardControl_cardInsertedEventHandler(CreditCardControl_cardInserted);

            ScanDataRecieved += new ScanDataRecievedEventHandler(cardReaderTask_ScanDataRecieved);
        }

        public void StartScan()
        {
            cardscan.startScan();
        }

        void cardReaderTask_ScanDataRecieved(object sender, EventArgs e){} //nothing here
      
        /// <summary>
        /// CreditCardControl_cardInserted - event handler for card reader - event have track data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CreditCardControl_cardInserted(object sender, AxotCreditCard.__creditCardControl_cardInsertedEvent e)
        {
            try
            {
                //stop looking for card insertions
                //cardscan.stopScan();
                
                string Track1 = e.track1;
                string Track2 = e.track2;
                string Track3 = e.track3;

                CreditCardReadEventArgs args = null;

                cardData CardData = null;

                //Test for errors in the card read
                if ((Track1 == "" && Track2 == "" && Track3 == "") || (Track1 == "Error" || Track2 == "Error" || Track3 == "Error"))
                {
                    Program.logEvent("Bad Card Read:");
                    //Program.logEvent("  Track1: " + Track1);    uncomment for debug only -do not keep customer info in logs
                    //Program.logEvent("  Track2: " + Track2);
                    //Program.logEvent("  Track3: " + Track3);
                    
                    
                    //send message about bad read and then do a retry from customForm  

                    CardData = new keyCabinetCardData("", "", "");

                    args = new CreditCardReadEventArgs(CardData);
                    args.GoodRead = false;

                }
                else
                {

                    Program.logEvent("Good ID Card Read:");
                    //Program.logEvent("  Track1: " + Track1);   uncomment for debug only -do not keep customer info in logs
                    //Program.logEvent("  Track2: " + Track2);
                    //Program.logEvent("  Track3: " + Track3);

                    ////pull the card number and name out of the card tracks
                    CardData = new keyCabinetCardData(Track1, Track2, Track3);  //or substitute for specialized client

                    args = new CreditCardReadEventArgs(CardData);
                    args.GoodRead = true;
                }

              

                ScanDataRecieved.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Program.logEvent("cardReaderTask  - exception - " + ex.Message);

                CreditCardReadEventArgs args = null;
                cardData CardData = null;

                CardData = new keyCabinetCardData("", "", "");
                args = new CreditCardReadEventArgs(CardData);
                args.GoodRead = false;

                ScanDataRecieved.Invoke(this, args);
                //throw new Exception("cardReaderTask  - exception - " + ex.Message);
            }
        }

        /// <summary>
        /// StopScan - stops the scan - for use in cancelling a scan
        /// </summary>
        public void StopScan()
        {
            cardscan.stopScan();
        }            
    }

    /// <summary>
    /// CreditCardReadEventArgs - argument to hold the data used in a card scan
    ///                         - card data is base class - should be specialized per product
    /// </summary>
    public class CreditCardReadEventArgs : EventArgs
    {

        public bool GoodRead { get; set; }  //allow for  primary determination if read was good

        public cardData CardData {get; private set;} //base class - cast to desired class

        public CreditCardReadEventArgs(cardData readData)
        {
            CardData = readData;
        }
    }
}
