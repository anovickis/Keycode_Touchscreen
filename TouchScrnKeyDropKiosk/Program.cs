using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using ThingMagicRFIDM6eMicro;
using ImpinjRFID;

namespace KeyCabinetKiosk
{


    /// <summary>
    /// 
    /// TouchScrnKeyDropKiosk - derives from KD20PC_Keydispenser and Deposit Kiosk3
    /// 
    /// KD20PC_Keydispenser - A Windows XP base program for dispensing keys using the Relay Control Board
    ///                     program is configured via KD20_config.xml. Uses keyPasswords.xml as a datebase 
    ///                     for the look up "key code" (or passwords) that allow for a relay to open and close
    ///                     a selenoid to drop a key. 
    ///                     There are 3 modes of operation 
    ///                     - A customer task mode which involves only entering a
    ///                     key code which then drops a key.
    ///                     - Admin tassword task mode - which is entered by entering the admin mode "password" as
    ///                     a key code. This value is stored in the KD20_config.xml file. This task will then look f
    ///                     for a correct 'pin code' which allow the user to enter administrative mode. This 'pin code'
    ///                     password is stored at location 0 in the key passwords database.
    ///                     - Admin task mode - this mode allows an administrator to view current keycodes at any
    ///                     location, or to change the key codes.
    ///                     - The database requires that the location number and the password - keycode be both be
    ///                     unique as both are used as indexs for lookups. The program has checks to see that this
    ///                     integretity is maintained. 
    ///                     - The program is designed for 20 location keys - this hard coded, but program should
    ///                     be extensible to additional locations
    ///                     - There is a debug log which can be turned on or off via the configuration file
    ///                     
    ///   written by  - John Stevens
    ///               - version 1.1.0.0  - 10/26/2010
    ///                     
    ///               - history
    ///                 
    ///                 Modification
    ///                 Date:   7/8/2011
    ///                 Author: Adam Bertsch
    ///                 Purpose:Allowed the keydispenser to activate 60 solenoid locks. if "SixtyKeyVersion" in the config
    ///                         file is set to "true" then the program will allow and expect 60 key locations. If it is 
    ///                         false, the program will only allow and expect 20 key locations. The non-configurable 
    ///                         global "NUMBER_RELAYS" is no longer set when it is defined. It is set immediately after
    ///                         "SixtyKeyVersion" is read from the config file so that the value can be set based upon
    ///                         a 20 or 60 key kiosk version.
    ///                 Modification 
    ///                 Date: 7/13/2011
    ///                 Author: John Stevens
    ///                 Purpose: Redesigned the user interface to work with a touch screen instead of with a 
    ///                         keypad and LCD panel- user interface taken from DepositKiosk3 and the relay
    ///                         code from KD20PC_Keydispenser. This is a new solution,but a continuation of both
    ///                         of these older designs
    ///                         
    ///             - version 3.0.0.0  8/5/2011
    ///                  John Stevens
    ///                  this version is for the key cabinet kiosk
    ///                  This is very similiar in function to the touch screen "touchscrnkiosk" which is its' starting 
    ///                  point. 
    ///                     - it uses a card reader as a possible ID method
    ///                     - it adds a transaction record
    ///                     - it uses a USB memory stick to transfer key access location data
    ///                     - it is designed to work with the  "key cabinet" product
    ///                     - it is designed to have 35 locations
    ///                     - location numbering is via relocation array - to match key space door and solenoid location
    ///                     
    ///         
    /// </summary>

    static class Program
    {
        #region global variables

        public static XmlConfig.XmlConfigDoc xml { get; private set; }
        public static XmlConfig.XmlConfigNode root { get; private set; }
        public static XmlConfig.XmlConfigNode globals { get; private set; }
        public static XmlConfig.XmlConfigNode emailnode { get; private set; }
        public static XmlConfig.XmlConfigNode textnode { get; private set; }
        public static XmlConfig.XmlConfigNode rfidnode { get; private set; }
        public static XmlConfig.XmlConfigNode sqlitenode { get; private set; }
        public static XmlConfig.XmlConfigNode biometricnode { get; private set; }
        public static XmlConfig.XmlConfigNode usersnode { get; private set; }
        public static XmlConfig.XmlConfigNode portnode { get; private set; }
        public static XmlConfig.XmlConfigNode idscannode { get; private set; }
        public static XmlConfig.XmlConfigNode configadminpasswords { get; private set; }

        // these are system globals
        public static string PROGRAM_VERSION { get; private set; }
        public static string KIOSK_ID { get; private set; }
        public static string KIOSK_LOCATION { get; private set; }
        
        public static int PASSWORD_SIZE { get; private set; }           //number of digits for access codes
        public static int NUMBER_CREDIT_CARD_DIGITS { get; private set; }
        public static int VEHICLE_NUMBER_LENGTH { get; private set; }  //number of digits allowed for vehicle ID numbers
        public static int GENERIC_DATA_FIELD_LENGTH { get; private set; }
        public static int MILEAGE_DIFFERENCE_ALLOWED { get; private set; }
        public static bool KD_DEBUG { get; private set; }       //for debugging mode KD_DEBUG not DEBUG - to avoid confusion   
        public static string EXIT_CODE { get; private set; }
        public static string ADMIN_PASSMODE { get; private set; }       //this activates admin mode - is not a "Pin Code"
        public static bool SIXTY_KEY_DISPENSER { get; private set; }    //indicates that the number of keys is greater than 20
        public static string RELAY_CONTROL_BOARD_TYPE { get; private set; }
        public static int REMINDER_INTERVAL { get; private set; }       //how often the program checks for reminders which need to be set off
        public static int TIMEOUT_INTERVAL { get; private set; }        // timeout for dialogs 
        public static string MAINSCREEN_TITLE1 { get; private set; }
        public static string MAINSCREEN_TITLE2 { get; private set; }
        public static string WINDOW_BACKGROUND_COLOR { get; private set; } //RGB value for the background color of baseform
        public static string SERVICE_MANAGER_NUMBER { get; set; }       //displayed in error box after multiple failed entries
        public static int OPEN_DOOR_INTERVAL { get; private set; }      //how long to hold door open
        public static string RESERVATION_DATABASE_CONNECTION_STRING { get; private set; } //ADODB.net connection string for the reservation database
        public static string CUSTOMER_DATA_SERVER { get; private set; } //name of the server that the customer has set up for server data communications
        
        //ports
        public static string RCB_PORT { get; private set; }     //relay board port
        public static string RFID_PORT { get; private set; }    //RFID reader port

        //configurable admin passwords
        public static string CONFIG_ADMIN_PWORD_1 { get; internal set; }
        public static string CONFIG_ADMIN_PWORD_2 { get; internal set; }
        public static string CONFIG_ADMIN_PWORD_3 { get; internal set; }

        //idscanning
        public static int IMAGE_SCAN_RESOLUTION { get; private set; }   //scan resolution in dpi for Snapshell images
        public static string IMAGE_SCAN_TYPE { get; private set; }      //tells whether using the front image or back barcode for gathering data

        // email
        public static string SMTP_SERVER { get; private set; }
        public static string SMTP_USERNAME { get; private set; }
        public static string SMTP_PASSWORD { get; private set; }
        public static bool SMTP_AUTHENTICATION { get; private set; }
        public static string FROM_ADDRESS { get; private set; }
        public static string TO_ADDRESS { get; set; }
        public static bool ENABLE_EMAIL { get; private set; }

        //TextMsg
        public static bool ENABLE_TEXTMSG { get; private set; }
        public static string TEXTTO_ADDRESS { get; set; }

        //RFID
        public static bool ENABLE_RFID { get; private set; }
        public static string RFID_READER_TYPE { get; private set; }
        public static int RFID_TOTAL_READ_TIME { get; private set; }
        public static int RFID_READ_ON_TIME { get; private set; }
        public static int RFID_READ_OFF_TIME { get; private set; }
        public static int RFID_READ_POWER { get; private set; }
        public static int RFID_INVENTORY_READ_TIME { get; private set; }
        public static bool IMPINJ_ANTENNA1_ENABLE { get; private set; }
        public static int IMPINJ_ANTENNA1_POWER { get; private set; }
        public static bool IMPINJ_ANTENNA2_ENABLE { get; private set; }
        public static int IMPINJ_ANTENNA2_POWER { get; private set; }
        public static bool IMPINJ_ANTENNA3_ENABLE { get; private set; }
        public static int IMPINJ_ANTENNA3_POWER { get; private set; }
        public static bool IMPINJ_ANTENNA4_ENABLE { get; private set; }
        public static int IMPINJ_ANTENNA4_POWER { get; private set; }

        //SQLite
        public static bool ENABLE_SQLITE { get; private set; }
        public static string SQLITE_DATABASE_NAME { get; private set; }
        public static int SQLITE_MAX_RECORDS { get; private set; }

        //Biometrics
        public static bool BIOMETRIC_ENABLE { get; private set; }
        public static int BIOMETRIC_TIMEOUT { get; private set; }
        public static int BIOMETRIC_FALSE_POSITIVE_RATIO { get; private set; }

        //Users
        public static bool USERS_ENABLE { get; private set; }
        public static bool USERS_ACCESS_RESTRICTIONS_OVERRIDE { get; private set; }
        public static int USER_ID_LENGTH { get; private set; }

        //not configurable
        private static int MONTHS_LOGS = 3;
        public static int LOCATION_SIZE = 3;   // number of possible digits for location
        private static string CONFIGURATION_FILE_NAME = "KD20_config.xml";
        private static string PASSWORD_FILE_NAME = "keypasswords.xml";
        public static int NUMBER_RELAYS;
        
        // developer flags
        public static bool DEV_DEBUG = true;
        public static bool DEV_STATION_SETUP = true;  // controls basic window configuration of application

        //Window Background Color Components
        public static int WindowBackRed { get; private set; }
        public static int WindowBackGreen { get; private set; }
        public static int WindowBackBlue { get; private set; }

        public static KeyPasswordManager passwordMgr;  //this will throw exception upon error - by design
        public static UserManager userMgr; //used to track and modify user data
        public static PortManager pm;
        public static LocationData locationdata;

        public static EmailerManager emailMgr; // this will throw exception upon erro - by design
        public static SQLManager SqlManager;

        public static BiometricDataManager biometricMgr; //used to track and modify fingerprint data
        
        public static M6eMicroRFID ThingMagicRFIDreader; //used to read RFID tags for tracking of keys.
        public static ImpinjSpeedwayRFID ImpinjRFIDreader; 
        //Nebraska Only Definition
        public static NebraskaReservationManager NebResMgr; //only used for Univ of Nebraska to keep track of their reservations.
        public static SnapShell_Driver_Lic.Snapshell InitIDScanner; //used to initialize ID scanner. Not actually used to scan with. That object is created for each scan.

        public enum GlobalAccessType
        {
            ALL_SIMPLE = 1,
            ALL_CARD_ONLY = 2,
            ALL_BOTH = 3,
            NO_GLOBAL = 4
        }
        public static GlobalAccessType GLOBAL_ACCESS_TYPE;

        #endregion

        public static BlankForm blank;
        public static MainForm main;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WINDOW_BACKGROUND_COLOR = "255,255,255"; //must be set temporarily so that loading form can be initialized
            WindowBackRed = 255;
            WindowBackGreen = 255;
            WindowBackBlue = 255;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ErrorForm loading = ShowMessage("");

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            if (!Directory.Exists("Transactions"))
            {
                Directory.CreateDirectory("Transactions");
            }       

            #region Program Initialization
            try
            {
                #region Config file values
                loading.SetLabelText("Loading Config\r\nFile Values..."); 
                xml = new XmlConfig.XmlConfigDoc(CONFIGURATION_FILE_NAME);
                root = xml.GetNode("root");
                globals = root.GetNode("globals");
                portnode = root.GetNode("ports");
                idscannode = root.GetNode("idscan");
                emailnode = root.GetNode("email");
                textnode = root.GetNode("textmsg");
                rfidnode = root.GetNode("rfid");
                sqlitenode = root.GetNode("sqlite");
                biometricnode = root.GetNode("biometric");
                usersnode = root.GetNode("users");
                configadminpasswords = root.GetNode("configurableadminpasswords");

                //globals
                KD_DEBUG = GetBoolValue("UseDebugLog", globals);
                DEV_STATION_SETUP = GetBoolValue("DevSetup", globals);
                PROGRAM_VERSION = GetStringValue("Version", globals);
                KIOSK_ID = GetStringValue("KioskID", globals);
                KIOSK_LOCATION = GetStringValue("KioskLocation", globals);    
                
                EXIT_CODE = GetStringValue("ExitCode", globals);
                RELAY_CONTROL_BOARD_TYPE = GetStringValue("RelayControlBoardType", globals);
                MAINSCREEN_TITLE1 = GetStringValue("MainscreenTitle1", globals); 
                MAINSCREEN_TITLE2 = GetStringValue("MainscreenTitle2", globals);
                WINDOW_BACKGROUND_COLOR = GetStringValue("WindowBackColor", globals);
                SERVICE_MANAGER_NUMBER = GetStringValue("serviceManagerPhone", globals);
                REMINDER_INTERVAL = GetIntValue("reminderTimer", globals);
                TIMEOUT_INTERVAL = GetIntValue("timeOutInterval", globals);
                GLOBAL_ACCESS_TYPE = (GlobalAccessType)GetIntValue("globalAccessType", globals);              
                PASSWORD_SIZE = GetIntValue("passWordSize", globals);
                NUMBER_CREDIT_CARD_DIGITS = GetIntValue("cardNumberSize", globals);
                VEHICLE_NUMBER_LENGTH = GetIntValue("vehicleNumLength", globals);
                GENERIC_DATA_FIELD_LENGTH = GetIntValue("genericDataFieldLength", globals);
                MILEAGE_DIFFERENCE_ALLOWED = GetIntValue("mileageDifferenceAllowed", globals);
                NUMBER_RELAYS = GetIntValue("numberOfRelays", globals);
                RESERVATION_DATABASE_CONNECTION_STRING = GetStringValue("ReservationDatabaseConnection", globals);
                CUSTOMER_DATA_SERVER = GetStringValue("customerdataserver", globals);
                OPEN_DOOR_INTERVAL = GetIntValue("openDoorInterval", globals);

                //Window Background Color
                WindowBackRed = int.Parse(WINDOW_BACKGROUND_COLOR.Split(',')[0]);
                WindowBackGreen = int.Parse(WINDOW_BACKGROUND_COLOR.Split(',')[1]);
                WindowBackBlue = int.Parse(WINDOW_BACKGROUND_COLOR.Split(',')[2]);

                //ports
                string tempPort = GetStringValue("RCB_port", portnode);
                RCB_PORT = ValidPortName(tempPort);
                RFID_PORT = GetStringValue("RFID_port", portnode);

                //configurable admin passwords
                CONFIG_ADMIN_PWORD_1 = GetStringValue("adminpassword1", configadminpasswords);
                CONFIG_ADMIN_PWORD_2 = GetStringValue("adminpassword2", configadminpasswords);
                CONFIG_ADMIN_PWORD_3 = GetStringValue("adminpassword3", configadminpasswords);

                //idscan
                IMAGE_SCAN_RESOLUTION = GetIntValue("imagescanresolution", idscannode);
                IMAGE_SCAN_TYPE = GetStringValue("imagescantype", idscannode);

                //Text Msg
                ENABLE_TEXTMSG = GetBoolValue("textEnable", textnode);
                TEXTTO_ADDRESS = GetStringValue("textToAddress", textnode);

                //Email
                SMTP_SERVER = GetStringValue("smtpServer", emailnode);
                SMTP_USERNAME = GetStringValue("smtpUsername", emailnode);
                SMTP_PASSWORD = GetStringValue("smtpPassword", emailnode);
                SMTP_AUTHENTICATION = GetBoolValue("smtpAuthentication", emailnode);
                FROM_ADDRESS = GetStringValue("emailFromAddress", emailnode);
                TO_ADDRESS = GetStringValue("emailToAddress", emailnode);
                ENABLE_EMAIL = GetBoolValue("emailEnable", emailnode);

                //RFID
                ENABLE_RFID = GetBoolValue("RFID_Enable", rfidnode);
                RFID_READER_TYPE = GetStringValue("RFID_Reader", rfidnode);
                RFID_READ_ON_TIME = GetIntValue("RFID_ReadOnTime", rfidnode);
                RFID_READ_OFF_TIME = GetIntValue("RFID_ReadOffTime", rfidnode);
                RFID_TOTAL_READ_TIME = GetIntValue("RFID_TotalReadTime", rfidnode);
                RFID_READ_POWER = GetIntValue("RFID_ReadPower", rfidnode);
                RFID_INVENTORY_READ_TIME = GetIntValue("RFID_InventoryTime", rfidnode);
                IMPINJ_ANTENNA1_ENABLE = GetBoolValue("ImpinjAntenna1Enable", rfidnode);
                IMPINJ_ANTENNA1_POWER = GetIntValue("ImpinjAntenna1Power", rfidnode);
                IMPINJ_ANTENNA2_ENABLE = GetBoolValue("ImpinjAntenna2Enable", rfidnode);
                IMPINJ_ANTENNA2_POWER = GetIntValue("ImpinjAntenna2Power", rfidnode);
                IMPINJ_ANTENNA3_ENABLE = GetBoolValue("ImpinjAntenna3Enable", rfidnode);
                IMPINJ_ANTENNA3_POWER = GetIntValue("ImpinjAntenna3Power", rfidnode);
                IMPINJ_ANTENNA4_ENABLE = GetBoolValue("ImpinjAntenna4Enable", rfidnode);
                IMPINJ_ANTENNA4_POWER = GetIntValue("ImpinjAntenna4Power", rfidnode);

                //SQLite
                ENABLE_SQLITE = GetBoolValue("EnableSQLiteDatabase", sqlitenode);
                SQLITE_DATABASE_NAME = GetStringValue("SQLiteDatabaseName", sqlitenode);
                SQLITE_MAX_RECORDS = GetIntValue("MaxSQLiteRecords", sqlitenode);

                //Biometrics
                BIOMETRIC_ENABLE = GetBoolValue("BiometricEnable", biometricnode);
                BIOMETRIC_TIMEOUT = GetIntValue("BiometricTimeout", biometricnode);
                BIOMETRIC_FALSE_POSITIVE_RATIO = GetIntValue("BiometricFalsePositiveRatio", biometricnode);

                //Users
                USERS_ENABLE = GetBoolValue("Users_Enable", usersnode);
                USERS_ACCESS_RESTRICTIONS_OVERRIDE = GetBoolValue("Users_Access_Restrictions_Override", usersnode);
                USER_ID_LENGTH = GetIntValue("UsersIDLength", usersnode);
                #endregion

                #region Software Initialization
                if (NUMBER_RELAYS > 20)
                {
                    SIXTY_KEY_DISPENSER = true;
                }

                loading.SetLabelText("Loading key data"); 
                // read in the key location and password file
                passwordMgr = new KeyPasswordManager(PASSWORD_FILE_NAME);

                loading.SetLabelText("Loading email\r\nmanager"); 
                //create the email manager
                emailMgr = new EmailerManager(SMTP_SERVER, FROM_ADDRESS, TO_ADDRESS, TEXTTO_ADDRESS);

                //create the user manager
                if (USERS_ENABLE)
                {
                    loading.SetLabelText("Loading user data"); 
                    userMgr = new UserManager("Users.xml");
                }
 
                //create the SQLcommandClass
                SqlManager = new SQLManager();

                #endregion

                #region Hardware Initialization
                //create Biometric Reader connection
                if (BIOMETRIC_ENABLE)
                {
                    loading.SetLabelText("Loading biometric\r\ndata"); 
                    biometricMgr = new BiometricDataManager("Fingerprints.xml");
                }
            }
            catch (Exception e)
            {
                string message = LanguageTranslation.CONFIGURATION_FILE_ERROR + "\r\n" + e.Message;
                MessageBox.Show(message);

                Program.logError(message);  //try to log error
                Program.programExit();   //exit program - unreliable program due to unknown configuration problem
            }

            try
            {
                //create RFID reader connection
                if (ENABLE_RFID)
                {
                    if (RFID_READER_TYPE.ToUpper() == "IMPINJ")
                    {
                        loading.SetLabelText("Impinj Reader\r\nConnecting");                        
                        ImpinjRFIDreader = new ImpinjSpeedwayRFID(RFID_PORT);
                    }
                    else
                    {
                        loading.SetLabelText("M6e Micro Reader\r\nConnecting");
                        ThingMagicRFIDreader = new M6eMicroRFID(RFID_PORT, 115200, RFID_READ_ON_TIME, RFID_READ_OFF_TIME, RFID_READ_POWER, RFID_TOTAL_READ_TIME);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = LanguageTranslation.CONFIGURATION_RFID_ERROR_1 + "\r\n " + ex.Message + "\r\n" + LanguageTranslation.CONFIGURATION_RFID_ERROR_2;
                DialogResult result = MessageBox.Show(message, "RFID Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Program.logError(message);  //try to log error
                //if (result == DialogResult.No) //This ability to exit the program was removed 2/2/15 due to my concerns that if the impinj readers did not connect for some 
                //{                              //reason during a daily morning reboot, a user could exit the program, access the box opening software and steal keys.
                //    Program.programExit();
                //}
            }
                #endregion

            #endregion

            #region port initialization

            try
            {
                loading.SetLabelText("Initializing Serial\r\nPorts"); 
                pm = PortManager.GetInstance();
                pm.InitializePorts(RCB_PORT);
            }
            catch (Exception e)
            {
                string message = LanguageTranslation.CONFIGURATION_OPEN_PORT_ERROR + "\r\n" + e.Message;
                MessageBox.Show(message);

                Program.logError(message);  //try to log error
                Program.programExit();   //exit program - unreliable program due to unknown configuration problem
            }

            #endregion

            try
            {
                //Set REMINDER_INTERVAL to 0 if there is no need for reminders.
                if (REMINDER_INTERVAL > 0)
                {
                    loading.SetLabelText("Initializing Reminders"); 
                    System.Timers.Timer ReminderTimer = new System.Timers.Timer(REMINDER_INTERVAL);
                    ReminderTimer.Elapsed += new System.Timers.ElapsedEventHandler(ReminderTimer_Elapsed);
                    ReminderTimer.Start();
                }

                loading.SetLabelText("Initializing Customer\r\nConfiguration");
                if (KIOSK_LOCATION.ToUpper() == "ONTARIO")
                    locationdata = new OntarioLocationData();
                else if (KIOSK_LOCATION.ToUpper() == "NEBRASKA")
                {
                    NebResMgr = new NebraskaReservationManager("NebraskaReservations.xml");
                    locationdata = new NebraskaLocationData();
                    InitIDScanner = new SnapShell_Driver_Lic.Snapshell();
                }
                else if (KIOSK_LOCATION.ToUpper() == "OSU")
                    locationdata = new OregonStateULocationData();
                else if (KIOSK_LOCATION.ToUpper() == "MONTANA")
                    locationdata = new MontanaLocationData();
                else if (KIOSK_LOCATION.ToUpper() == "WAWATER")
                    locationdata = new WAWaterLocationData();
                else if (KIOSK_LOCATION.ToUpper() == "HUDSON")
                    locationdata = new HudsonLocationData();
                else if (KIOSK_LOCATION.ToUpper() == "DAIMLER")
                {
                    locationdata = new DaimlerChinaLocationData();
                    LanguageTranslation.language = language.Chinese;
                }
                else if (Program.KIOSK_LOCATION.ToUpper() == "CHEVIN") //if ChevinLocationData is used WaWaterTransactionData will be used with it
                {
                    locationdata = new ChevinLocationData();
                }
                else
                    locationdata = new DefaultLocationData();

                main = new MainForm();
                logEvent("Version: " + PROGRAM_VERSION);
                DeleteOldLogs();
                loading.Dispose();
                blank = new BlankForm();
                Program.blank.Visible = false;
                Program.blank.SendToBack();

                Program.logEvent("Program Started");
                Application.Run(main);
            }
            catch (Exception e)
            {
                string message = LanguageTranslation.CONFIGURATION_APP_ERROR + "\r\n " + e.Message;
                MessageBox.Show(message);

                Program.logError(message);  //try to log error
                Program.programExit();   //exit program - unreliable program due to unknown configuration problem
            }
        }

        internal static void ReminderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            locationdata.runReminders();
        }

        /// <summary>
        ///  DeleteOldLogs - checks for old logs and then deletes them
        /// </summary>
        public static void DeleteOldLogs()
        {
            //Delete logs which are more than MONTHS_LOGS - 1 month(s) old (i.e. keep the current months logs and
            //MONTHS_LOGS - 1 month(s) previous to that.
            try
            {
                int DeleteMe = ((DateTime.Today.Month + (12 - Program.MONTHS_LOGS)) % 12);

                string MonthToDelete = DeleteMe.ToString();

                if (File.Exists("Logs/" + MonthToDelete + "error.log"))
                {
                    File.Delete("Logs/" + MonthToDelete + "error.log");
                    Program.logDebug("Old error log file deleted");
                }

                if (File.Exists("Logs/" + MonthToDelete + "debug.log"))
                {
                    File.Delete("Logs/" + MonthToDelete + "debug.log");
                    Program.logDebug("Old debug log file deleted");
                }

                if (File.Exists("Logs/" + MonthToDelete + "event.log"))
                {
                    File.Delete("Logs/" + MonthToDelete + "event.log");
                    Program.logDebug("Old event log file deleted");
                }

                if (File.Exists("Transactions/" + MonthToDelete + "transaction.csv"))
                {
                    File.Delete("Transactions/" + MonthToDelete + "transaction.csv");
                    Program.logDebug("Old transaction file deleted");
                }
            }
            catch (Exception ex)
            {
                Program.logError("log deletion failure " + ex.Message);
            }
        }

        /// <summary>
        /// This function is used to output program information to log files in the "log" directory which is directly below the current
        /// working directory of the .exe
        /// </summary>
        /// <param name="description"></param>
        public static void logError(string description)
        {
            if (String.IsNullOrEmpty(description)) { return; }
            try
            {
                File.AppendAllText("Logs/" + DateTime.Today.Month.ToString() + "error.log", DateTime.Now.ToString() + " " + description + "\r\n");
            }
            catch (Exception)
            { } // nothing todo if this error log fails
        }

        /// <summary>
        /// logDebug - for developers - a verbose log - not expected to be used during production usage
        /// </summary>
        /// <param name="description"></param>
        public static void logDebug(string description)
        {
            if (!KD_DEBUG) { return; }  // debug log is turned off

            if (String.IsNullOrEmpty(description)) { return; }

            try
            {
                File.AppendAllText("Logs/" + DateTime.Today.Month.ToString() + "debug.log", DateTime.Now.ToString() + " " + description + "\r\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("Logs/error.log", DateTime.Now.ToString() + " " + ex.Message + "\r\n");
            }
        }

        /// <summary>
        /// This function is used to output program information to log files in the "log" directory which is directly below the current
        /// working directory of the .exe
        /// </summary>
        /// <param name="description"></param>
        public static void logEvent(string description)
        {
            if (String.IsNullOrEmpty(description)) { return; }
            try
            {
                File.AppendAllText("Logs/" + DateTime.Today.Month.ToString()
                    + "event.log", DateTime.Now.ToString() + " " + description + "\r\n");
            }
            catch (Exception e)
            {
                File.AppendAllText("Logs/error.log", e.Message + "\r\n"
                    + DateTime.Now.ToString() + " " + description + "\r\n");
            }
        }

        public static void logTransaction(string transactionCSVData)
        {
            if(String.IsNullOrEmpty(transactionCSVData)) { return; }
            try
            {
                string filename = "Transactions/" + DateTime.Today.Month.ToString() + "transaction.csv";
                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, Program.locationdata.Header + "\r\n");
                }
                File.AppendAllText(filename , transactionCSVData + "\r\n" );
            }
            catch (Exception e)
            {
                File.AppendAllText("Logs/error.log", e.Message + "\r\n"
                    + transactionCSVData + "\r\n");
            }
        }

        /// <summary>
        /// Displays an error message on a dialog which then closes after displayTimeMs milliseconds
        /// </summary>
        /// 
        /// <param name="message"></param>
        /// <param name="displayTimeMs"></param>
        public static void ShowErrorMessage(string message, int displayTimeMs)
        {
            ErrorForm errorForm = new ErrorForm(message, displayTimeMs);
            errorForm.ShowDialog();
        }

        public static ErrorForm ShowMessage(string message)
        {
            ErrorForm messageform = new ErrorForm(message);
            messageform.Show();
            messageform.Update();

            return messageform;
        }

        private static string ValidPortName(string comPort)
        {
            if (String.IsNullOrEmpty(comPort))
            {
                return "NONE";
            }
            string temp = comPort.ToUpper();

            if ((temp == "COM1") ||
                (temp == "COM2") ||
                (temp == "COM3") ||
                (temp == "COM4") ||
                (temp == "COM5") ||
                (temp == "COM6") ||
                (temp == "COM7") ||
                (temp == "COM8") ||

                (temp == "NONE"))
            {
                return temp;
            }
            else
            {
                Program.logError("ValidPortName- invalid portname - " + comPort);
                return "NONE";
            }
        }


        /// <summary>
        /// Exits the program and closes the process
        /// </summary>
        public static void programExit()
        {
            PortManager ports = PortManager.GetInstance();
            ports.ClosePorts();

            UI_settings.Restore();

            logEvent("Program Quit");
            Environment.Exit(0);
        }

           private static bool GetBoolValue(string valueName, XmlConfig.XmlConfigNode node )
        {
            try
            {
                return  (bool.Parse(node.GetValue(valueName)));
            }
            catch(Exception ex)
            {
                throw new Exception(" value: " + valueName + ex.Message);
            }
        }

        private static int GetIntValue(string valueName, XmlConfig.XmlConfigNode node)
        {
            try
            {
                return (int.Parse(node.GetValue(valueName)));
            }
            catch (Exception ex)
            {
                throw new Exception(" value: " + valueName + ex.Message);
            }
        }

        private static byte GetByteValue(string valueName, XmlConfig.XmlConfigNode node)
        {
            try
            {
                return (byte.Parse(node.GetValue(valueName)));
            }
            catch (Exception ex)
            {
                throw new Exception(" value: " + valueName + ex.Message);
            }
        }


        private static decimal GetDecimalValue(string valueName, XmlConfig.XmlConfigNode node)
        {
            try
            {
                return (decimal.Parse(node.GetValue(valueName)));
            }
            catch (Exception ex)
            {
                throw new Exception(" value: " + valueName + ex.Message);
            }
        }

        private static string GetStringValue(string valueName, XmlConfig.XmlConfigNode node)
        {
            try
            {
                return (node.GetValue(valueName));
            }
            catch (Exception ex)
            {
                throw new Exception(" value: " + valueName + ex.Message);
            }
        }
    }
}
