using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{

    /// <summary>
    /// cardData - is a POD class which can be specialized for the different sites
    ///         depending upon how they read the card data on tracks1,2,3
    ///         may involve  a Lookup to see if card is valid, or some algorithm
    /// </summary>
    public abstract class cardData
    {
        protected string track1;
        protected string track2;
        protected string track3;

        public cardData(string Track1, string Track2, string Track3) 
        {
            track1 = Track1;
            track2 = Track2;
            track3 = Track2;
        }
        public abstract bool isValid();
        public abstract string getCardNumber();
        public abstract string getCardName();
    }

    /// <summary>
    /// keyCabinetCardData - card data and validation and transaction methods for program of key cabinets
    ///                     for this key cabinet program the last four digits of the credit card are used
    ///                     as access verification for opening the loc door.
    ///                     
    ///                     needed data
    ///                         - last four digits of the credit card number - transaction and access
    ///                         - name on card - transaction report
    ///                         - valid if name and number can be extracted
    /// </summary>
    public class keyCabinetCardData : cardData
    {
        private bool valid  = false;

        public keyCabinetCardData(string Track1, string Track2, string Track3)
            :base(Track1, Track2, Track3)      
        {
            //This bit of code does not necessarily apply to all cards. i.e. Montana ID cards have all of their data on track 2.
            //Commented out 11/27/13 Adam Bertsch
            //if (Track1.Length == 0)
            //{

            //    valid = false;
            //    return;
            //}
            valid = true;
        }
        public override bool isValid() { return valid; }

        public override string getCardNumber() { return Program.locationdata.retrieveUserNumberFromCard(track1, track2); }
        public override string getCardName() { return Program.locationdata.retrieveUserNameFromCard(track1, track2); }

    }

}

