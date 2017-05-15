using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This enum is the list of possible screen modules which the customer has the ability to be configured to use
    /// </summary>
    public enum CustomerScreenType
    {
        BoxNumberEntry, AccessCodeEntry, BikeSelection, BikeOutOfStockQuestion, CardSwipe, MakeHttpPost, UserIDEntry, ReturningQuestion, IDScan, ReservationConfirmQuestion,
        MileageDamageFormQuestion, VehicleNumEntry, CarRefueledQuestion, CarCleanedQuestion, CarDamagedQuestion, OdometerEntry, ReservationNumberEntry, OperatorNumberEntry,
        AskUserForTextMsgNumber, BiometricVerification, BiometricIdentification, RetrieveRFIDTagNumber, HIDCardScan, SelectBioOrHid, ReservationAlphaNumberEntry
    }
    /// <summary>
    /// This enum is the list of possible options the admin screen can be set for. It currently is not being used, because all options
    /// are currently available to the administrator user
    /// </summary>
    public enum AdminScreenType
    {
        BiometricEnrollmentHID, BiometricEnrollmentKeyboard, OpenSingleDoor, OpenAllDoors, 
    }    
    /// <summary>
    /// This enum denotes the different kinds of messages which can be sent to a server which a customer has their own database on.
    /// </summary>
    public enum HTTPPostType
    {
        GetReservationInfo, KeyOut, KeyIn, KeyLoaded, CheckReservation, RFIDInventory, TransactionFail
    }

    public enum WhenToUse
    {
        Taking, Returning, Both
    }

    public abstract class LocationData
    {
        //This list contains what screens are to be run by the CustomerJob.cs class
        //and what order they are to be run in.
        abstract internal List<CustomerScreen> CustomerScreenOrderList { get; set; }
        abstract internal List<AdminScreenType> AdminScreenList { get; set; } //possibly not necessary for touchscreen version
        abstract internal List<AdminScreenType> ConfigurableCodeAdminScreensList { get; set; }
        abstract public string Header { get; internal set; }
        private List<CustomerScreen>.Enumerator CustomerScreenOrderListEnumerator { get; set; }

        public LocationData()
        {
            //Screens must be added by classes inheriting from this one
            CustomerScreenOrderList = new List<CustomerScreen>();
            CustomerScreenOrderListEnumerator = CustomerScreenOrderList.GetEnumerator();
            AdminScreenList = new List<AdminScreenType>();
            ConfigurableCodeAdminScreensList = new List<AdminScreenType>();
        }

        /// <summary>
        /// This method must be overridden by inheriting classes and should apply the 
        /// methodology used by the customer location for getting the user Name from the
        /// accepted user ID card.
        /// </summary>
        /// <param name="Magstrip">Magstrip card track from which to rip the user ID</param>
        /// <returns>The user ID obtained from the magstrip card track</returns>
        internal virtual string retrieveUserNameFromCard(string Magstrip1, string Magstrip2) { return Magstrip1; }

        /// <summary>
        /// This method must be overridden by inheriting classes and should apply the 
        /// methodology used by the customer location for getting the user Number from the
        /// accepted user ID card.
        /// </summary>
        /// <param name="Magstrip">Magstrip card track from which to rip the user ID</param>
        /// <returns>The user ID obtained from the magstrip card track</returns>
        internal virtual string retrieveUserNumberFromCard(string Magstrip1, string Magstrip2) { return Magstrip1; }

        /// <summary>
        /// This method must be overridden by inheriting classes and should apply the 
        /// methodology used by the customer location for getting their RFID tags for a 
        /// particular location
        /// </summary>
        /// <param name="location">The location for which you need the RFID tag. This may not be a necessary parameter for all customers</param>
        /// <returns>returns the RFID tag for 'location'</returns>
        internal virtual string retrieveRFIDTagNumber(int location, LocationTransactionData transdata) { return ""; }

        /// <summary>
        /// Because different locations can keep different amounts of an RFID tag for identification, each location must be allowed to have
        /// its own scanning method so that it can scan for its RFID tag information correctly.
        /// </summary>
        /// <param name="tagID">The tag ID to scan for</param>
        /// <returns></returns>
        internal virtual bool scanRFIDTagNumber(string tagID) { return true; }

        /// <summary>
        /// This method returns an enumerator for the CustomerScreenOrderList property
        /// </summary>
        /// <returns></returns>
        protected List<CustomerScreen>.Enumerator getListEnumerator()
        {
            return CustomerScreenOrderListEnumerator;
        }

        /// <summary>
        /// This method moves the given list enumerator forward one item
        /// </summary>
        /// <returns>Returns if the forward movement was successful</returns>
        protected bool nextCustomerScreen()
        {
            return CustomerScreenOrderListEnumerator.MoveNext();
        }

        /// <summary>
        /// This method returns the current screen that the customer job is on
        /// </summary>
        /// <param name="ListWalker">Enumerator being used to traverse the customer screens list</param>
        /// <returns>Returns the enum value for the current screen</returns>
        protected CustomerScreen currentCustomerScreen()
        {
            return CustomerScreenOrderListEnumerator.Current;
        }
        
        /// <summary>
        /// This method sets the CustomerScreenOrderListEnumerator to a new enumerator,
        /// thus resetting its position to the beginning.
        /// </summary>
        protected void resetListEnumerator()
        {
            CustomerScreenOrderListEnumerator = CustomerScreenOrderList.GetEnumerator();
        }

        /// <summary>
        /// This method is meant to be overridden by customers who need to access some online
        /// database for their transaction process.
        /// </summary>
        /// <param name="MethodType">Predetermined type of communication with server. Usually given as a 
        ///                          method name at the end of the url</param>
        /// <param name="extradata">This can be used for whatever other data might be needed by the
        ///                         server communication</param>
        /// <returns></returns>
        internal virtual bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3) { return true; }

        internal virtual bool makeHTTPPost(HTTPPostType MethodType, string extradata1, string extradata2, string extradata3, ref LocationData.LocationTransactionData transaction) { return true; }

        /// <summary>
        /// This method should allow the program to take the user-input odometer reading and go out to the reservation server and test
        /// to see if the driver drove more than the pre-defined (Program.MILEAGE_DIFFERENCE_ACCEPTABLE) amount, or if the odometer entry 
        /// is less than the amount entered by the last renter, both of which most likely indicates that they entered the wrong number.
        /// </summary>
        /// <param name="Odometer"></param>
        /// <returns></returns>
        internal virtual bool validateOdometerReading(LocationData.LocationTransactionData transaction) { return true; }

        /// <summary>
        /// This method is meant to contain any customer specific processing which must happen after all customer screens have been successfully 
        /// completed but before the door is open. Possible examples include getting the BoxNumber with the currently obtained data and 
        /// checking the validity of the currently obtained data.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <returns></returns>
        internal virtual bool preDoorOpeningProcessing(LocationTransactionData transaction) { return true; }

        /// <summary>
        /// This method is meant to contain any customer specific processing which must happen after all customer screens have been successfully 
        /// completed and after the door is open. Possible examples include scanning for an RFID tag upon the key's return.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <returns></returns>
        internal virtual bool postDoorOpeningProcessing(LocationTransactionData transaction) { return true; }

        /// <summary>
        /// This method gives the ability for the customer to show customizable content and have customizable retrieval
        /// of reservation data for the user to confirm.
        /// </summary>
        /// <returns>a bool which states whether or not the data retrieval was successful</returns>
        internal virtual bool showReservationData(ref LocationData.LocationTransactionData transaction, ref YesNoForm ReservationConfirmScreen) { return true; }

        /// <summary>
        /// This method allows for the customer to be able to remotely confirm their reservation. It is designed to be
        /// customized by each customer (or left alone if it is unnecessary) to send out to where the reservation data
        /// is stored and get a yes/no answer back whether or not the reservation is valid.
        /// </summary>
        /// <param name="reservationNum"></param>
        /// <returns></returns>
        internal virtual bool confirmReservation(ref LocationData.LocationTransactionData transaction) { return true; }

        /// <summary>
        /// This method is meant to run reminders to users/administrators to perform certain tasks like return keys or that they have a reservation to pick up.
        /// It is run by a timer which is resident in Program.cs which is run on a set interval. The reminders do not have to be run every time the timer expires,
        /// but there must be logic in the overridden method to control when they do run.
        /// </summary>
        internal virtual void runReminders() { return; }

        /// <summary>
        /// This class is the base class for transaction data. Each different location which has a
        /// DK3 kiosk will have a class defined which inherits from LocationData and includes
        /// a sub-class which inherits from LocationTransactionData. This class has the three
        /// basic bits of info that will be asked for at all locations. Extra information
        /// must be added into the inheriting classes.
        /// </summary>
        public abstract class LocationTransactionData
        {
            public abstract List<TransactionDataObject> ObjectList { get; set; }
            public abstract string AccessCode { get; set; }
            public abstract int LockLocation { get; set; }           //Location of lock based upon relay board numbering 
            public abstract DateTime transActionTime { get; set; }  //Time of transaction - when class created
            public abstract int BoxNumber { get; set; }              // The box number - as setup for this project
            public abstract bool DoorOpened { get; set; }            // True if a correct access data given - does not indicate
            public abstract string CardName { get; set; }
            public abstract string CardNumber { get; set; }
            public abstract bool ReturningKey { get; set; }
            public abstract string UserID { get; set; }
             
            /// <summary>
            /// LocationTransactionData
            /// </summary>
            public LocationTransactionData()
            {
                ObjectList = new List<TransactionDataObject>();
                AddInitialDataObjectsToList();
                AccessCode = "";
                LockLocation = -1;
                BoxNumber = -1;
                transActionTime = DateTime.Now;                
                DoorOpened = false;
                CardName = "";
                CardNumber = "";
                UserID = "";
            }

            abstract public string CSV_Entry();

            /// <summary>
            /// This method needs to return each of the data items which are relevant to the transaction for the customer. For instance
            /// if the customer doesnt use the "AccessCode" parameter, then it shouldn't be included in the return array. What is 
            /// returned should be a reflection of the transaction file "Header" which lists the titles for each column in the transaction file.
            /// </summary>
            /// <returns></returns>
            abstract public string[] TransactionData();

            private void AddInitialDataObjectsToList()
            {               
                //This order must always be followed so that the program knows where in
                //the list there items are.
                ObjectList.Add(new TransactionDataObject("Access Code", AccessCode, false, false));
                ObjectList.Add(new TransactionDataObject("Box", BoxNumber, false, false)); //this has a special spot on the receipt
                ObjectList.Add(new TransactionDataObject("Lock", LockLocation, false, false));
                ObjectList.Add(new TransactionDataObject("Card Name", CardName, false, false));//this has a special spot on the receipt
                ObjectList.Add(new TransactionDataObject("Card Number", CardNumber, false, false)); //this has a special spot on the receipt               
                ObjectList.Add(new TransactionDataObject("Door Opened", DoorOpened, false, false)); //this has a special spot on the receipt
                ObjectList.Add(new TransactionDataObject("Return", ReturningKey, false, false));
                ObjectList.Add(new TransactionDataObject("UserID", UserID, false, false));
            }

            // date and  time in form d/MM/yy HH:mm:ss
            public  string DateAndTime
            {
                get
                {
                    return String.Format("{0:MM/dd/yy HH:mm:ss}",transActionTime);
                }
            }

            // date and  time in form YYYY/MM/DD HH:mm:ss
            public string SQLiteDateAndTime
            {
                get
                {
                    return String.Format("{0:yyyy-MM-dd HH:mm:ss}", transActionTime);
                }
            }

            /// <summary>
            /// Date in form M-d-yyyy
            /// </summary>
            public string Date
            {
                get
                {
                    return String.Format("{0:MM-dd-yyyy}",transActionTime);
                }
            }

            // date and  time in form MM/dd/yy HH:mm:ss
            public string SQLiteDate
            {
                get
                {
                    return String.Format("{0:yyyy/MM/dd}", transActionTime);
                }
            }

            /// <summary>
            /// Time in form of short time to string
            /// </summary>
            public string Time
            {
                get
                {
                    return  transActionTime.ToShortTimeString();
                }
            }    
            
                                                   

            virtual public void ClearData()
            {                
                LockLocation = -1;
                BoxNumber = -1;
                DoorOpened = false;
                AccessCode = "";
                CardNumber = "";
                CardName = "";
                ReturningKey = false;
                UserID = "";
            }

            /// <summary>
            /// This method returns an exact data copy of the current object. However, it is 
            /// not the object itself so when this object is changed, the copy will not be.
            /// </summary>
            /// <returns></returns>
            abstract public LocationTransactionData HardCopy();

            public class TransactionDataObject :ICloneable
            {
                public string name { get; protected internal set; }
                public object data { get; internal set; }
                public bool UseOnceInReceipt { get; private set; }
                public bool UseInReceipt { get; private set; }

                public TransactionDataObject(string Name, object Data, bool OnlyOnceInReceipt, bool UseForReceipt)
                {
                    name = Name;
                    data = Data;
                    UseOnceInReceipt = OnlyOnceInReceipt;
                    UseInReceipt = UseForReceipt;
                }

                public object Clone()
                {
                    return (new TransactionDataObject(name, data, UseOnceInReceipt, UseInReceipt));
                }
            }
        }
    }

    public class CustomerScreen
    {
        public CustomerScreenType Type { get; private set; }
        public WhenToUse WhenIsScreenUsed { get; private set; } //this bool indicates if the screen should be used only if the user is returning a key.
        //If the customer does not ask the user if they are returning a key, set all screens to false for this bool
        public bool OptionalBool { get; private set; } //this bool is for use when some functionality is situational. It does not need to be set unless it will be used 
        //for sure. (For example, There are some screens which ask the user a question. Based on this bool you can either
        //cause the user to quit their transaction if they answer "no" to the question or you can allow them to say "no"
        //and continue on anyways, saving their answer to be logged/reported later.
        public string[] ScreenMessage { get; private set; }
        public CustomerScreen(CustomerScreenType type, WhenToUse usedinreturning)
        {
            Type = type;
            WhenIsScreenUsed = usedinreturning;
            OptionalBool = false;
            ScreenMessage = new string[] { " ", " ", " ", " " };
        }
        public CustomerScreen(CustomerScreenType type, WhenToUse usedinreturning, bool optionalbool)
        {
            Type = type;
            WhenIsScreenUsed = usedinreturning;
            OptionalBool = optionalbool;
            ScreenMessage = new string[] { " ", " ", " ", " " };
        }
        public CustomerScreen(CustomerScreenType type, WhenToUse usedinreturning, string[] message)
        {
            Type = type;
            WhenIsScreenUsed = usedinreturning;
            OptionalBool = false;
            ScreenMessage = message;
        }
        public CustomerScreen(CustomerScreenType type, WhenToUse usedinreturning, bool optionalbool, string[] message)
        {
            Type = type;
            WhenIsScreenUsed = usedinreturning;
            OptionalBool = optionalbool;
            ScreenMessage = message;
        }
    }
}
