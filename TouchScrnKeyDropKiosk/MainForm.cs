using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KeyCabinetKiosk
{

    enum returnTypes
    {
        CANCEL,
        NOMATCH,
        OK
    }

    /// <summary>
    /// MainForm - the initial dialog - always open until app is closed. 
    ///         has a hidden and developer only exit buttons.
    ///         all "jobs" either customer or admin should "start and end" here
    /// </summary>
    /// 
    public partial class MainForm : baseForm
    {
        private int OddEvenCountForTesting = 0;
        private const int BADACCESSCODEATTEMPTMAXCOUNT = 3;   //allow 3 attempts
        internal LocationData.LocationTransactionData transData;

        public MainForm()
            :base()  //never should time out
        {
            InitializeComponent();
            SetMainformTitle();
            buttonStart.Text = LanguageTranslation.BUTTON_START;
            if (!Program.DEV_STATION_SETUP)
            {
                this.buttonDevExit.Visible = false;
            }
            
        }

        private void panelHiddenExit_MouseClick(object sender, MouseEventArgs e)
        {
            //TESTING Program.ReminderTimer_Elapsed(null, null);
            KeyPadForm AdminPad = new KeyPadForm(LanguageTranslation.HIDDEN_EXIT_KEYPAD_MESSAGE, true, 6, true, true);
            AdminPad.ShowDialog();

            if (AdminPad.Result == Program.EXIT_CODE)
            {
                Program.logEvent("Program Exit");
                Program.programExit();
                Close();
            }
            else if (Program.passwordMgr.FindPassword(0).Contains(AdminPad.Result))
            {
                AdminTasks();//Allows users to enter the admin screen through the user ID entry screen
                             //transaction cancels when admin screen exited.
            }
            else if ((AdminPad.Result == Program.CONFIG_ADMIN_PWORD_1) && (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 0))
            {
                Program.logEvent("Config Admin Screen 1 Accessed");
                CabinetAccessForm adminTaskDlg = new CabinetAccessForm(Program.locationdata.ConfigurableCodeAdminScreensList[0]);
                adminTaskDlg.ShowDialog();

                Program.blank.Visible = false;
            }
            else if ((AdminPad.Result == Program.CONFIG_ADMIN_PWORD_2) && (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 1))
            {
                Program.logEvent("Config Admin Screen 2 Accessed");
                CabinetAccessForm adminTaskDlg = new CabinetAccessForm(Program.locationdata.ConfigurableCodeAdminScreensList[1]);
                adminTaskDlg.ShowDialog();

                Program.blank.Visible = false;
            }
            else if ((AdminPad.Result == Program.CONFIG_ADMIN_PWORD_3) && (Program.locationdata.ConfigurableCodeAdminScreensList.Count > 2))
            {
                Program.logEvent("Config Admin Screen 3 Accessed");
                CabinetAccessForm adminTaskDlg = new CabinetAccessForm(Program.locationdata.ConfigurableCodeAdminScreensList[2]);
                adminTaskDlg.ShowDialog();

                Program.blank.Visible = false;
            }
        }

        private void buttonDevExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartAction();
        }

        private void panelMessage_Click(object sender, EventArgs e)
        {
            StartAction();
        }

        private void panelFullScreenTouch_Click(object sender, EventArgs e)
        {
            StartAction();
        }

        /// <summary>
        /// StartAction - this contains all of the jobs admin and customers - all jobs begin and end here
        /// </summary>
        private void StartAction()
        {
            try
            {
                ////OddEvenCountForTesting++;TESTING ONLY

                //This is where the transaction data object is cast based upon the location of the kiosk
                if (Program.KIOSK_LOCATION.ToUpper() == "ONTARIO")
                    transData = new OntarioLocationData.OntarioTransactionData();
                else if (Program.KIOSK_LOCATION.ToUpper() == "NEBRASKA")
                    transData = new NebraskaLocationData.NebraskaTransactionData();
                else if (Program.KIOSK_LOCATION.ToUpper() == "OSU")
                    transData = new OregonStateULocationData.OregonStateUTransactionData();
                else if (Program.KIOSK_LOCATION.ToUpper() == "MONTANA")
                    transData = new MontanaLocationData.MontanaTransactionData();
                else if (Program.KIOSK_LOCATION.ToUpper() == "WAWATER" || Program.KIOSK_LOCATION.ToUpper() == "CHEVIN")
                    transData = new WAWaterLocationData.WAWaterTransactionData();
                else if (Program.KIOSK_LOCATION.ToUpper() == "DAIMLER")
                    transData = new DaimlerChinaLocationData.DaimlerChinaTransactionData();
                else
                    transData = new DefaultLocationData.DefaultTransactionData();
                                
                //CustomerTasks takes care of all screens a customer may go thru. They must all return true in order to move on to open the box door
                if (CustomerTasks())
                {
                    if (!Program.locationdata.preDoorOpeningProcessing(transData))
                        return;

                    accessTimeType currentBoxTimeType;
                    int limitedUses = 0;
                    DateTime starttime, endtime;
                    //check to see if Users time/numerical restraints override the box's
                    if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                    {
                        currentBoxTimeType = Program.userMgr.FindUserTimeAccessType(transData.UserID);
                        limitedUses = Program.userMgr.FindUserLimitedUsesCount(transData.UserID);
                        starttime = Program.userMgr.FindUserTimeframeStart(transData.UserID);
                        endtime = Program.userMgr.FindUserTimeframeEnd(transData.UserID);
                    }
                    else
                    {
                        //check and see if there are any time/numerical restaints on this box
                        currentBoxTimeType = Program.passwordMgr.FindAccessTimeType(transData.BoxNumber);
                        limitedUses = Program.passwordMgr.FindLimitedNumberofUses(transData.BoxNumber);
                        starttime = Program.passwordMgr.FindStartTime(transData.BoxNumber);
                        endtime = Program.passwordMgr.FindEndTime(transData.BoxNumber);
                    }

                    if ((currentBoxTimeType == accessTimeType.LIMITED_USE) || (currentBoxTimeType == accessTimeType.TIME_PERIOD_LIMITED_USE))
                    {
                        if (limitedUses <= 0)
                        {
                            //No more uses left. Door wont open    
                            if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_USAGES_USER_ERROR, 5000);
                                Program.logEvent("User " + transData.UserID + " has no more usages available. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "User " + transData.UserID + " has no more usages available. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            else
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_USAGES_BOX_ERROR, 5000);
                                Program.logEvent("Box number " + transData.BoxNumber + " has no more usages available. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Box number " + transData.BoxNumber + " has no more usages available. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            return;
                        }
                        else
                        {
                            if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                            {
                                Program.userMgr.ChangeUserLimitedUses(transData.UserID, limitedUses - 1);
                                Program.userMgr.SaveFile();
                            }
                            else
                            {
                                Program.passwordMgr.SetLimitedNumberofUses(transData.BoxNumber, Program.passwordMgr.FindLimitedNumberofUses(transData.BoxNumber) - 1);
                                Program.passwordMgr.SaveFile();
                            }
                        }
                    }
                    if ((currentBoxTimeType == accessTimeType.TIME_PERIOD) || (currentBoxTimeType == accessTimeType.TIME_PERIOD_LIMITED_USE))
                    {
                        if (DateTime.Now < starttime)
                        {
                            //Action has not been taken within necessary usage timeframe
                            if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_TIMEFRAME_USER_ERROR, 5000);
                                Program.logEvent("User ID " + transData.UserID + " usage timeframe has not arrived yet. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "User ID " + transData.UserID + " usage timeframe has not arrived yet. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            else
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_TIMEFRAME_BOX_ERROR, 5000);
                                Program.logEvent("Box number " + transData.BoxNumber + " usage timeframe has not arrived yet. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Box number " + transData.BoxNumber + " usage timeframe has not arrived yet. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            Program.logEvent("Early Timeframe is: " + endtime.ToString());
                            return;
                        }
                        else if (DateTime.Now > endtime)
                        {
                            //Action has not been taken within necessary usage timeframe

                            if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_TIMEFRAME_USER_ERROR_2, 5000);
                                Program.logEvent("User ID " + transData.UserID + " usage timeframe has already passed. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "User ID " + transData.UserID + " usage timeframe has already passed. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            else
                            {
                                Program.ShowErrorMessage(LanguageTranslation.DOOR_TIMEFRAME_BOX_ERROR_2, 5000);
                                Program.logEvent("Box number " + transData.BoxNumber + " usage timeframe has already passed. Door opening cancelled");
                                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Box number " + transData.BoxNumber + " usage timeframe has already passed. Door opening cancelled", "Box Number " + transData.BoxNumber, 0);
                            }
                            Program.logEvent("Passed Timeframe is: " + endtime.ToString());
                            return;
                        }
                    }

                    Program.pm.OpenAndCloseRelay(transData.BoxNumber);                    
                    //CommonTasks.OpenKeyBox(transData.BoxNumber, transData.ReturningKey);
                    transData.DoorOpened = true;
                    
                    if (transData.ReturningKey)
                    {
                        Program.logEvent("Box " + transData.BoxNumber + " was opened by user to return a key");
                        Program.ShowErrorMessage(LanguageTranslation.RETURN_KEY_CLOSE_DOOR, 4000);
                    }
                    else
                    {
                        Program.logEvent("Box " + transData.BoxNumber + " was opened by user to take a key");
                        Program.ShowErrorMessage(LanguageTranslation.TAKE_KEY_CLOSE_DOOR, 4000);
                    }

                    Program.locationdata.postDoorOpeningProcessing(transData);

                    Program.logTransaction(transData.CSV_Entry()); // log entry  

                    if ((Program.ENABLE_EMAIL) && (Program.emailMgr.GenerateReport(transData, ReportType.Email))) //email report
                    {
                        if (Program.emailMgr.SendEmail(ReportType.Email))
                        {
                            Program.logEvent("Email sent");
                        }
                        else
                        {
                            Program.logEvent("Email error");
                        }
                    }

                    if ((Program.ENABLE_TEXTMSG) && (Program.emailMgr.GenerateReport(transData, ReportType.TextMsg))) //text report
                    {
                        if (Program.emailMgr.SendEmail(ReportType.TextMsg))
                        {
                            Program.logEvent("Text sent");
                        }
                        else
                        {
                            Program.logEvent("Text error");
                        }
                        
                        foreach(CustomerScreen cs in Program.locationdata.CustomerScreenOrderList)
                        {
                            if (cs.Type == CustomerScreenType.AskUserForTextMsgNumber)
                            {
                                if (CheckReturningUseStatus(cs)) //skip screen if used only in returning
                                    continue;                //and you're not returning

                                DoUserTextMsgNumberEntry();
                            }
                        }
                    }
                }
                else
                {
                    if (!Program.locationdata.makeHTTPPost(HTTPPostType.TransactionFail, "", "", "", ref transData))
                        Program.logEvent("Transaction Fail Message Failed");

                    Program.logEvent("Transaction Cancelled");
                    Program.ShowErrorMessage(LanguageTranslation.TRANSACTION_CANCELLED, 2000);
                }
               
                Program.blank.Visible = false;
                Program.DeleteOldLogs();
            }
            catch (Exception ex)
            {
                Program.blank.Visible = false;

                Program.logEvent("EnterKeyCodeForm - keycode entry exception " + ex.Message);
            }
        }

        private bool CustomerTasks()
        {
            bool ReturnBool = true;
            int ObjectListCounter = 8; //the object list by default has 7 objects in it, which can be accessed via their parameter names. 
                                       //Objects which are customer specific start at the eigth object (array index 7).
            foreach (CustomerScreen s in Program.locationdata.CustomerScreenOrderList)
            {
                if (s.Type == CustomerScreenType.AccessCodeEntry || s.Type == CustomerScreenType.ReservationNumberEntry || s.Type == CustomerScreenType.ReservationAlphaNumberEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoAccessCodeEntry(s);
                    if (ReturnBool == false)
                    {
                        Program.logEvent("Access Code Cancelled");
                        return false;
                    }

                    Program.logEvent("Access Code Entered: " + transData.AccessCode);

                    //////
                    //////Don't know if this is how we want to have it long term, but right now each Access code needs to be unique to a User as well as a Box
                    //////
                    if (Program.USERS_ACCESS_RESTRICTIONS_OVERRIDE && Program.USERS_ENABLE)
                    {
                        string ID = Program.userMgr.FindUserIDByAccessCode(transData.AccessCode);
                        if (ID != "")
                        {
                            transData.UserID = ID;
                            transData.BoxNumber = int.Parse(Program.userMgr.FindUserBoxNumber(ID));
                        }
                        else
                        {
                            Program.ShowErrorMessage(LanguageTranslation.ACCESS_CODE_NO_USER, 4000);
                            Program.logEvent("No User Assigned to Access Code");
                            Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "No User Assigned to Access Code", "", 0);
                            return false;
                        }
                    }
                    else
                    {
                        if (s.Type == CustomerScreenType.AccessCodeEntry)
                        {
                            int keyNumber = Program.passwordMgr.FindLocationForCode(transData.AccessCode);

                            if (keyNumber == -1)
                            {
                                Program.ShowErrorMessage(LanguageTranslation.INVALID_PASSWORD, 3000);
                                return false;
                            }

                            transData.BoxNumber = keyNumber;
                            Program.logEvent("Box Number Identified: " + transData.BoxNumber);
                            transData.LockLocation = keyNumber;
                        }

                        else
                        {
                            ErrorForm commwithserver = Program.ShowMessage(LanguageTranslation.COMMUNICATING_WITH_SERVER);
                            //if the reservation cannot be confirmed on the remote server
                            if (!Program.locationdata.confirmReservation(ref transData))
                            {
                                //Error messages should be put into confirmReservation() code
                                commwithserver.Dispose();
                                return false;
                            }
                            commwithserver.Dispose();
                        }
                    }
                }
                else if (s.Type == CustomerScreenType.UserIDEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoUserIDEntry();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.BikeOutOfStockQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoBikeOutOfStockQuestion();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.BikeSelection)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoBikeSelection(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.BoxNumberEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoBoxNumberEntry();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.CardSwipe)
                {
                    if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_BOTH || Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY)
                    {
                        ReturnBool = DoCardSwipe();
                        if (ReturnBool == false)
                            return false;

                        if (Program.GLOBAL_ACCESS_TYPE == Program.GlobalAccessType.ALL_CARD_ONLY)
                            transData.BoxNumber = Program.passwordMgr.FindLocationForCard(transData.CardNumber);
                    }
                    else
                        continue;
                }
                else if (s.Type == CustomerScreenType.ReturningQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoReturningQuestion(s);
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.IDScan)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoIDScan(ObjectListCounter);
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.ReservationConfirmQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoReservationConfirmationQuestion();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.MileageDamageFormQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoMileageDamageFormQuestion();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.CarRefueledQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoRefueledQuestion(s, ref ObjectListCounter);
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.CarDamagedQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoDamagedQuestion(s, ref ObjectListCounter);
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.CarCleanedQuestion)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoCarCleanedQuestion(s, ref ObjectListCounter);
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.OdometerEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                 //and you're not returning

                    ReturnBool = DoOdometerReading(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.VehicleNumEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoVehicleNumberEntry(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.OperatorNumberEntry)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoOperatorNumberEntry(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.RetrieveRFIDTagNumber)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                //and you're not returning

                    ReturnBool = DoRetrieveRFIDNumber(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.BiometricIdentification)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                   //and you're not returning

                    ReturnBool = DoBiometricIdentification();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.BiometricVerification)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                   //and you're not returning

                    ReturnBool = DoBiometricVerification();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.HIDCardScan)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                   //and you're not returning

                    ReturnBool = DoHIDCardScan();
                    if (ReturnBool == false)
                        return false;
                }
                else if (s.Type == CustomerScreenType.SelectBioOrHid)
                {
                    if (CheckReturningUseStatus(s)) //skip screen if used only in returning
                        continue;                   //and you're not returning

                    ReturnBool = DoSelectBioOrHID(ObjectListCounter);
                    ObjectListCounter++;
                    if (ReturnBool == false)
                        return false;
                }
                else
                {
                    //Error. Screen not recognized.
                    Program.logEvent("The current screen in the customer list was not found as a viable option: " + s.Type.ToString());
                    Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "The current screen in the customer list was not found as a viable option: " + s.Type.ToString(), "", 0);
                }
            }
            return true;
        }

        private bool DoSelectBioOrHID(int ObjectListCount)
        {
            YesNoForm SelectMethod = new YesNoForm("Select ID Method\r\nTo Use");
            SelectMethod.ChangeTextYesButton("Biometric");
            SelectMethod.ChangeTextNoButton("HID");
            SelectMethod.ShowDialog();


            if (SelectMethod.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Select ID Method Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Select ID Method Timed Out", "", 0);
                return false;
            }

            if (SelectMethod.YesResult)
            {
                if (DoBiometricIdentification())
                {

                    transData.ObjectList[ObjectListCount].data = "BIOMETRIC";
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (DoHIDCardScan())
                {
                    transData.ObjectList[ObjectListCount].data = "CARD";
                }                    
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool DoHIDCardScan()
        {
            HIDCardReader HIDScanner = new HIDCardReader();
            HIDScanner.ShowDialog();

            if (HIDScanner.Cancelled)
            {
                Program.logEvent("HID Card Scan cancelled");
                return false;
            }
            if (HIDScanner.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("HID Card Scan Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "HID Card Scan Timed Out", "", 0);
                return false;
            }

            transData.CardNumber = HIDScanner.Data;
            Program.logEvent("HID Card Number: " + HIDScanner.Data);
            return true;
        }

        private bool DoReturningQuestion(CustomerScreen screen)
        {
            YesNoForm returning = new YesNoForm(screen.ScreenMessage[0], screen.ScreenMessage[1]);
            returning.ChangeTextYesButton(LanguageTranslation.TAKE);
            returning.ChangeTextNoButton(LanguageTranslation.RETURN);
            returning.ShowDialog();

            if (returning.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Returning Question Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Returning Question Timed Out", "", 0);
                return false;
            }

            transData.ReturningKey = !returning.YesResult; //The 'yes' button entails not returning a key
            if (transData.ReturningKey)
                Program.logEvent("User indicated they were returning a key");
            else
                Program.logEvent("User indicated they were taking a key");
            return true;
        }

        private bool DoUserIDEntry()
        {
            LongNameEntryForm userid = new LongNameEntryForm(10, false, false);
            userid.DialogTitle = LanguageTranslation.ENTER_USER_ID;
            userid.ShowDialog();

            if (userid.Ok)
            {
                transData.AccessCode = userid.Description;
                Program.logEvent("User ID Entered: " + transData.AccessCode);

                if (Program.passwordMgr.FindPassword(0).Contains(userid.Description))
                {
                    AdminTasks();//Allows users to enter the admin screen through the user ID entry screen
                    return false;//transaction cancels when admin screen exited.
                }
                return true;
            }
            else
                return false;
        }

        private bool DoAccessCodeEntry(CustomerScreen screen)
        {
            bool retbool = false;
            string retstring = "";
            if ((screen.Type == CustomerScreenType.AccessCodeEntry) || (screen.Type == CustomerScreenType.ReservationNumberEntry))
            {
                KeyPadForm access = new KeyPadForm(screen.ScreenMessage[0], false, Program.PASSWORD_SIZE, true, false);
                access.ShowDialog();

                retbool = access.bOK;
                retstring = access.Result;
                
            }
            else if (screen.Type == CustomerScreenType.ReservationAlphaNumberEntry)
            {
                LongNameEntryForm alphaaccess = new LongNameEntryForm(Program.PASSWORD_SIZE, false, false, screen.ScreenMessage[0]);
                alphaaccess.ShowDialog();

                retbool = alphaaccess.Ok;
                retstring = alphaaccess.Description;
            }

            if (retbool)
            {
                if (Program.passwordMgr.FindPassword(0).Contains(retstring))
                {
                    AdminTasks();//Allows users to enter the admin screen through the user ID entry screen
                    return false;//transaction cancels when admin screen exited.
                }

                transData.AccessCode = retstring;
                Program.logEvent("Access Code Entered: " + transData.AccessCode);
                return true;
            }
            else
            {
                Program.ShowErrorMessage(LanguageTranslation.TRANSACTION_CANCELLED, 3000);
                return false;
            }
        }

        /// <summary>
        /// This method Scans a driver's license and extracts information from it.
        /// A specialized set of screens is used to access the Snapshell API and perform this operation.
        /// </summary>
        /// <param name="ObjectListCount"></param>
        /// <returns></returns>
        private bool DoIDScan(int ObjectListCount)
        {
            //transData.ObjectList[ObjectListCount].data = "SWANSON, HANNAH D";
            //transData.ObjectList[ObjectListCount + 1].data = "H13285444";
            //return true; //TESTING ONLY

            IDScanInput inputscreen = new IDScanInput(Program.IMAGE_SCAN_TYPE.ToUpper());
            inputscreen.ShowDialog();

            //if (File.Exists("temp.jpg"))
            //    File.Delete("temp.jpg");

            if (inputscreen.cancelled)
            {
                Program.logEvent("ID Scan Cancelled");
                return false;
            }
            if (inputscreen.TimedOut)
            {
                Program.ShowErrorMessage("ID Scan Timed Out",5000);
                Program.logEvent(LanguageTranslation.ID_SCAN_TIMED_OUT);
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "ID Scan Timed Out", "", 0);
                return false;
            }

            //do stuff with license data
            transData.ObjectList[ObjectListCount].data = inputscreen.firstname + " " + inputscreen.lastname;//DataObjectList[?] = IDdata.Name;
            transData.ObjectList[ObjectListCount + 1].data = inputscreen.licensenum;//DataObjectList[?] = IDdata.Id;  
            Program.logEvent("ID Name Scanned: " + inputscreen.firstname + " " + inputscreen.lastname);
            Program.logEvent("ID Number Scanned: " + inputscreen.licensenum);
            return true;
        }

        /// <summary>
        /// This method asks a nebraska user to confirm their reservation information. Presumably, this could be extended to other customers
        /// as well
        /// </summary>
        /// <returns></returns>
        private bool DoReservationConfirmationQuestion()
        {
            YesNoForm resConfirm = new YesNoForm("");
            resConfirm.TimeOutOff();

            if (!Program.locationdata.showReservationData(ref transData, ref resConfirm))//This method needs to overrided for locations which are using this Screen.
                return false;

            resConfirm.resetTimer();
            resConfirm.ChangeTextYesButton(LanguageTranslation.CONFIRM);
            resConfirm.ChangeTextNoButton(LanguageTranslation.CANCEL);
            resConfirm.ShowDialog();

            if (resConfirm.TimedOut)
            {
                Program.logEvent("User confirmation of reservation timed out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "User confirmation of reservation timed out", "", 0);
                Program.ShowErrorMessage(LanguageTranslation.RESERVATION_CONFIRMATION_TIMEOUT, 5000);
                return false;
            }

            if (resConfirm.YesResult)
            {
                Program.logEvent("Box Number for reservation is " + transData.BoxNumber);
                return true;
            }
            else
            {
                Program.logEvent("Reservation Rejected");
                return false;
            }
        }

        /// <summary>
        /// This method tells the user to take a mileage and car damage form before leaving and fill them out. 
        /// It uses a generic confirmation question screen. Two separate screens are needed. One for the mileage form
        /// and one for the damage form. Both must be confirmed before moving on.
        /// </summary>
        /// <returns></returns>
        private bool DoMileageDamageFormQuestion()
        {
            YesNoForm damage = new YesNoForm(LanguageTranslation.REPORT_CAR_DAMAGE);
            damage.ChangeTextYesButton(LanguageTranslation.CONTINUE);
            damage.ChangeTextNoButton(LanguageTranslation.CANCEL);
            damage.ShowDialog();

            if (damage.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Mileage/Damage Form Confirmation Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Mileage/Damage Form Confirmation Timed Out", "", 0);
                return false;
            }

            if (damage.YesResult)
            {
                Program.logEvent("Car damage form confirmed");
                YesNoForm mileage = new YesNoForm(LanguageTranslation.FILL_OUT_MILEAGE_FORM);
                mileage.ChangeTextYesButton(LanguageTranslation.CONTINUE);
                mileage.ChangeTextNoButton(LanguageTranslation.CANCEL);
                mileage.ShowDialog();

                if (mileage.TimedOut)
                {
                    Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                    Program.logEvent("Mileage/Damage Form Confirmation Timed Out");
                    Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Mileage/Damage Form Confirmation Timed Out", "", 0);
                    return false;
                }

                if (mileage.YesResult)
                {
                    Program.logEvent("Car mileage form confirmed");
                    return true;
                }
                else
                {
                    Program.logEvent("Car mileage form rejected");
                    return false;
                }
            }
            else
            {
                Program.logEvent("Car damage form rejected");
                return false;
            }
        }

        /// <summary>
        /// This method allows the user to enter an operator number. It uses a generic data entry screen.
        /// </summary>
        /// <param name="ObjectListCount"></param>
        /// <returns></returns>
        private bool DoOperatorNumberEntry(int ObjectListCount)
        {
            KeyPadForm Operator = new KeyPadForm(LanguageTranslation.ENTER_OPERATOR_NUMBER, false, 9, true, false);
            Operator.ShowDialog();

            if (Operator.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Operator Number Entry Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Operator Number Entry Timed Out", "", 0);
                return false;
            }
            if (!Operator.bOK)
            {
                Program.logEvent("Operator Number Entry Cancelled");
                return false;
            }

            transData.ObjectList[ObjectListCount].data = Operator.Result;
            Program.logEvent("Operator Number Entered: " + Operator.Result);

            return true;
        }

        /// <summary>
        /// This method allows the user to enter a vehicle number. It uses a generic data entry screen.
        /// </summary>
        /// <param name="ObjectListCount"></param>
        /// <returns></returns>
        private bool DoVehicleNumberEntry(int ObjectListCount)
        {
            KeyPadForm vehicle = new KeyPadForm(LanguageTranslation.ENTER_VEHICLE_NUMBER, false, Program.VEHICLE_NUMBER_LENGTH, true, false);
            vehicle.ShowDialog();

            if (vehicle.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Vehicle Number Entry Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Vehicle Number Entry Timed Out", "", 0);
                return false;
            }
            if (!vehicle.bOK)
            {
                Program.logEvent("Vehicle Number Entry Cancelled");
                return false;
            }

            transData.ObjectList[ObjectListCount].data = vehicle.Result;
            Program.logEvent("Vehicle Number Entered: " + vehicle.Result);

            if (Program.KIOSK_LOCATION.ToUpper() == "NEBRASKA")
            {
                string reservation = Program.NebResMgr.FindReservationByVehicleNumber(vehicle.Result);
                if (reservation == null)
                {
                    Program.ShowErrorMessage(LanguageTranslation.RESERVATION_NOT_FOUND,5000);
                    Program.logEvent("No reservation found for current driver's number");
                    Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "No reservation found for current driver's number", "", 0);
                    return false;
                }
                else
                {
                    transData.BoxNumber = Program.NebResMgr.FindReturnKeyNumber(reservation);
                }
            }
            return true;
        }

        /// <summary>
        /// This method asks the user if they have refueled the car they are returning.
        /// It uses the generic user confirmation question.
        /// </summary>
        /// <returns></returns>
        private bool DoRefueledQuestion(CustomerScreen screen, ref int ObjectListCount)
        {
            YesNoForm fuel = new YesNoForm(screen.ScreenMessage[0], screen.ScreenMessage[1]);
            fuel.ShowDialog();

            if (fuel.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Refueled Question Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Refueled Question Timed Out", "", 0);
                return false;
            }

            if (screen.OptionalBool) //If OptionalBool is set to true for this screen it implies that the user wants to store the answer to the question
            {
                transData.ObjectList[ObjectListCount].data = fuel.YesResult;
                ObjectListCount++;
            }

            if (fuel.YesResult)
            {
                Program.logEvent("Refuelling Confirmed");
                return true;
            }
            else
            {
                Program.logEvent("Refuelling Denied");

                if (screen.OptionalBool) //This allows the user to answer NO to the question and not have the transaction be cancelled
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Asks the user if they have cleaned out the car they rented. Uses the generic confirmation question screen.
        /// </summary>
        /// <returns></returns>
        private bool DoCarCleanedQuestion(CustomerScreen screen, ref int ObjectListCount)
        {
            YesNoForm cleaned = new YesNoForm(screen.ScreenMessage[0], screen.ScreenMessage[1]);
            cleaned.ShowDialog();

            if (cleaned.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Car Cleaned Question Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Car Cleaned Question Timed Out", "", 0);
                return false;
            }

            if (screen.OptionalBool) //If OptionalBool is set to true for this screen it implies that the user wants to store the answer to the question
            {
                transData.ObjectList[ObjectListCount].data = cleaned.YesResult;
                ObjectListCount++;
            }

            if (cleaned.YesResult)
            {
                Program.logEvent("Car cleaning confirmed.");
                return true;
            }
            else
            {
                Program.logEvent("Car cleaning denied.");

                if (screen.OptionalBool) //This allows the user to answer NO to the question and not have the transaction be cancelled
                    return true;

                return false;
            }
        }

        /// <summary>
        /// This method asks the user if the car they are returning has been damaged.
        /// It uses the generic user confirmation question.
        /// </summary>
        /// <returns></returns>
        private bool DoDamagedQuestion(CustomerScreen screen, ref int ObjectListCount)
        {
            YesNoForm damage = new YesNoForm(screen.ScreenMessage[0], screen.ScreenMessage[1]);
            damage.ShowDialog();

            if (damage.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                Program.logEvent("Car Damage Question Timed Out");
                Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Car Damage Question Timed Out", "", 0);
                return false;
            }

            if (screen.OptionalBool) //If OptionalBool is set to true for this screen it implies that the user wants to store the answer to the question
            {
                transData.ObjectList[ObjectListCount].data = damage.YesResult;
                ObjectListCount++;
            }

            if (damage.YesResult)
            {
                Program.logEvent("Car Damage Confirmed");
                return true;
            }
            else
            {
                Program.logEvent("Car Damage Denied");

                if (screen.OptionalBool) //This allows the user to answer NO to the question and not have the transaction be cancelled
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Asks the user for the current odometer reading on the car they returned. No tenths of a mile necessary.
        /// Generic data entry screen is used
        /// </summary>
        /// <param name="ObjectListCount"></param>
        /// <returns></returns>
        private bool DoOdometerReading(int ObjectListCount)
        {
            string odometerreading = "";
            do
            {
                KeyPadForm OdometerReading = new KeyPadForm(LanguageTranslation.ENTER_ODOMETER_READING, false, 7, true, false);
                OdometerReading.ShowDialog();

                if (OdometerReading.TimedOut)
                {
                    Program.ShowErrorMessage(LanguageTranslation.OPERATION_TIMED_OUT, 5000);
                    Program.logEvent("Odometer Entry Timed Out");
                    Program.SqlManager.ErrorDatabaseEntry(transData.AccessCode, "", DateTime.Now.ToString(), Program.KIOSK_ID, 0, "Odometer Entry Timed Out", "", 0);
                    return false;
                }
                if (!OdometerReading.bOK)
                {
                    Program.logEvent("Odometer Entry Cancelled");
                    return false;
                }

                odometerreading = OdometerReading.Result;
                transData.ObjectList[ObjectListCount].data = odometerreading;
            } while (!Program.locationdata.validateOdometerReading(transData)); //used if there are restrictions on what odometer readings are accepted.

            Program.logEvent("Odometer Reading: " + odometerreading);
            return true;
        }

        private bool DoBiometricVerification()
        {
            //If there are no fingerprints associated with the current access code, run fingerprint enrollment
            if (Program.biometricMgr.FindFingerprint(transData.AccessCode) == null)
            {
                DigitalPersonaBiometricsForm EnrollBiometrics = new DigitalPersonaBiometricsForm("Fingerprints.xml", DPBiometricsState.Enrolling, transData.AccessCode, Program.BIOMETRIC_TIMEOUT, Program.biometricMgr);
                for (int i = 0; i < 3; i++)
                {
                    EnrollBiometrics.ShowDialog();
                    if (EnrollBiometrics.Cancelled || EnrollBiometrics.TimedOut)
                        return false;
                }
            }

            DigitalPersonaBiometricsForm VerifyBiometrics = new DigitalPersonaBiometricsForm("Fingerprints.xml", DPBiometricsState.Verification, transData.AccessCode, Program.BIOMETRIC_TIMEOUT, Program.biometricMgr);

            for (int i = 0; i < 3; i++)
            {
                VerifyBiometrics.ShowDialog();

                if (VerifyBiometrics.Cancelled || VerifyBiometrics.TimedOut)
                    return false;
                if (VerifyBiometrics.Success)
                    return true;
            }
            return false;
        }

        private bool DoBiometricIdentification()
        {
            DigitalPersonaBiometricsForm IdentifyBiometrics = new DigitalPersonaBiometricsForm("Fingerprints.xml", DPBiometricsState.Identification, "", Program.BIOMETRIC_TIMEOUT, Program.biometricMgr);

            for (int i = 0; i < 3; i++)
            {
                IdentifyBiometrics.ShowDialog();

                if (IdentifyBiometrics.Cancelled || IdentifyBiometrics.TimedOut)
                    return false;
                if (IdentifyBiometrics.Success)
                {
                    if (IdentifyBiometrics.IdentificationIDs.Count == 1)
                    {
                        transData.CardNumber = IdentifyBiometrics.IdentificationIDs[0];
                        return true;
                    }
                    else
                    {
                        Program.ShowErrorMessage(LanguageTranslation.MULTIPLE_FINGERPRINT_MATCHES, 4000);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ask the user if they would like a text message receipt. If they say yes, then get their phone number from them and send the message
        /// </summary>
        private void DoUserTextMsgNumberEntry()
        {
            YesNoForm AskAboutTextMsg = new YesNoForm(LanguageTranslation.ASK_TEXT_MESSAGE_1, LanguageTranslation.ASK_TEXT_MESSAGE_2);
            AskAboutTextMsg.ShowDialog();

            if (AskAboutTextMsg.YesResult)
            {
                Program.logEvent("User said they wanted a text receipt");
                string[] MobileCarriersList = new string[] { "Verizon", "AT&&T", "Sprint", "T-Mobile", "All Tell", "Boost", "U.S. Cellular", "Virgin" };
                EightOptionSelect SelectCarrier = new EightOptionSelect("Select Your Mobile Carrier", MobileCarriersList);
                SelectCarrier.ShowDialog();
                
                #region Select Mobile Carrier from List
                string MobileCarrier = "";
                string MobileCarrierRelaySuffix = "";
                switch (SelectCarrier.Result)
                {
                    case SelectionResult.Button1:
                    {
                        MobileCarrier = MobileCarriersList[0];
                        MobileCarrierRelaySuffix = "@vtext.com";
                        break;
                    }
                    case SelectionResult.Button2:
                    {
                        MobileCarrier = MobileCarriersList[1];
                        MobileCarrierRelaySuffix = "@txt.att.net";
                        break;
                    }
                    case SelectionResult.Button3:
                    {
                        MobileCarrier = MobileCarriersList[2];
                        MobileCarrierRelaySuffix = "@messaging.sprintpcs.com";
                        break;
                    }
                    case SelectionResult.Button4:
                    {
                        MobileCarrier = MobileCarriersList[3];
                        MobileCarrierRelaySuffix = "@tmomail.net";
                        break;
                    }
                    case SelectionResult.Button5:
                    {
                        MobileCarrier = MobileCarriersList[4];
                        MobileCarrierRelaySuffix = "@message.alltel.com";
                        break;
                    }
                    case SelectionResult.Button6:
                    {
                        MobileCarrier = MobileCarriersList[5];
                        MobileCarrierRelaySuffix = "@myboostmobile.com";
                        break;
                    }
                    case SelectionResult.Button7:
                    {
                        MobileCarrier = MobileCarriersList[6];
                        MobileCarrierRelaySuffix = "@email.uscc.net";
                        break;
                    }
                    case SelectionResult.Button8:
                    {
                        MobileCarrier = MobileCarriersList[7];
                        MobileCarrierRelaySuffix = "@vmobl.com";
                        break;
                    }
                }
                #endregion

                Program.logEvent("User selected " + MobileCarrier +" as their mobile carrier");                
                KeyPadForm EnterTextNumber = new KeyPadForm(LanguageTranslation.ENTER_PHONE_NUMBER, false, 10, false, false);
                EnterTextNumber.ShowDialog();
                
                if (EnterTextNumber.bOK)
                {
                    Program.emailMgr.GenerateReport(transData, ReportType.TextMsg);
                    Program.emailMgr.SendEmail(ReportType.TextMsg, EnterTextNumber.Result + MobileCarrierRelaySuffix);
                }
                else if (EnterTextNumber.TimedOut)
                {
                    Program.logEvent("User text receipt timed out");
                    Program.ShowErrorMessage(LanguageTranslation.TEXT_MESSAGE_TIMEOUT, 3000);
                }
                else
                {
                    Program.logEvent("User text receipt cancelled");
                    Program.ShowErrorMessage(LanguageTranslation.TEXT_MESSAGE_CANCELLED, 3000);
                }
            }
        }

        private bool DoBoxNumberEntry()
        {
            string boxNumber = CommonTasks.GetLocationFromKeypad();
            Program.logEvent("Box Number Entered " + boxNumber);
            Program.blank.Visible = true;

            if (String.IsNullOrEmpty(boxNumber))
            {
                Program.ShowErrorMessage(LanguageTranslation.INVALID_LOCATION, 3000);

                Program.blank.Visible = false;

                return false;
            }
            //check to see if this is a valid location
            int loc = -1;  //initialize as invalid

            if (CommonTasks.ValidBoxNumber(boxNumber))
            {
                transData.BoxNumber = int.Parse(boxNumber);
                loc = CommonTasks.FindLocationNumber(boxNumber); //location number is 1-Max
                transData.LockLocation = loc;
            }

            // if this was not a valid box number - then location will be -1
            if (loc == -1)
            {
                Program.ShowErrorMessage(LanguageTranslation.INVALID_LOCATION, 3000);

                Program.blank.Visible = false;

                return false;
            }
            return true;
        }

        private bool DoRetrieveRFIDNumber(int ObjectListCount)
        {
            bool scanSuccess = false;
            string ret = Program.locationdata.retrieveRFIDTagNumber(transData.BoxNumber, transData);
            if (ret == "")
            {
                Program.ShowErrorMessage(LanguageTranslation.RFID_TAG_NOT_FOUND + transData.BoxNumber.ToString(), 3000);
                return false;
            }                
            else
            {
                transData.ObjectList[ObjectListCount].data = ret;
                ObjectListCount++;

                //Only scan for the tag if the user is taking a key. Otherwise scan for it later in locationdata.postDoorOpeningProcessing
                if (!transData.ReturningKey)
                {
                    RFIDScanForm scanform = new RFIDScanForm(ret);
                    scanform.ShowDialog();

                    scanSuccess = scanform.ReturnBool;
                    if (scanSuccess)
                    {
                        Program.ShowErrorMessage(LanguageTranslation.RFID_TAG_FOUND, 3000);
                        Program.logEvent("RFID Tag " + ret + " FOUND.");
                    }
                    else
                    {
                        Program.ShowErrorMessage(LanguageTranslation.RFID_TAG_NOT_DETECTED, 3000);
                        Program.logEvent("RFID Tag " + ret + " NOT FOUND.");
                    }

                    transData.ObjectList[ObjectListCount].data = scanSuccess.ToString();
                }
                return true;
            }
        }

        private bool DoCardSwipe()
        {
            ScanCardForm scanDlg = new ScanCardForm();

            scanDlg.ShowDialog();

            Program.blank.Visible = false;

            if (scanDlg.TimedOut)
            {
                Program.ShowErrorMessage(LanguageTranslation.CARD_READ_TIMEOUT, 3000);
                return false;
            }
            if (scanDlg.Cancelled)
            {
                Program.ShowErrorMessage(LanguageTranslation.CARD_READ_CANCELLED, 3000);
                return false;
            }

            if (scanDlg.GoodData)
            {
                //name and last four digits 
                string cardNumber = scanDlg.CardNumber;
                string cardName = scanDlg.CardName;
                //compare cardNumber with location card number

                //string savedNumber = CreditCardLast4DigitsByLocation(loc);

                transData.CardName = cardName;
                transData.CardNumber = cardNumber;

                Program.logEvent("Card Swipe Number: " + cardNumber);
                Program.logEvent("Card Swipe Name: " + cardName);
                return true;
            }
            else
            {
                Program.ShowErrorMessage(LanguageTranslation.CARD_READ_FAILED, 3000);
                Program.logEvent("Card Read Failed");
                return false;
            }
        }

        private bool DoBikeSelection(int objectlistcounter)
        {
            if (transData.ReturningKey)
                return true;

            YesNoForm bikeselect = new YesNoForm(LanguageTranslation.BIKE_CHOOSE,""); //extra parameter pushes down the first message on the screen
            bikeselect.ChangeTextYesButton(LanguageTranslation.BIKE);
            bikeselect.ChangeTextNoButton(LanguageTranslation.EBIKE);
            bikeselect.ShowDialog();

            if (bikeselect.YesResult)
            {
                transData.ObjectList[objectlistcounter].data = BikeType.Bike;
                Program.logEvent("User indicated they wanted to rent a normal bike");
            }
            else
            {
                transData.ObjectList[objectlistcounter].data = BikeType.EBike;
                Program.logEvent("User indicated they wanted to rent an Ebike");
            }

            return true;
        }

        private bool DoBikeOutOfStockQuestion()
        {

            ////
            ////MAKE CALL TO SERVER WITH USER ID AND BIKE TYPE HERE
            ////IF RETURNING KEY, BIKE TYPE IS NOT NEEDED
            ////

            ////
            ////IF USER IS OK CONTINUE
            ////ELSE 
            //Program.ShowErrorMessage("User ID Not Valid", 3000);
            //Program.ShowErrorMessage("Please Sign Up for Bike\r\nShare At www.bikeshare.com", 5000);
            //return false;
                        
            ////
            ////IF TAKING AND BIKE IS AVAILABLE
            ////return true;
            ////ELSE
            ////RUN CODE BELOW

            ////IF RETURNING 
            ////SET BOX NUMBER BASED ON SERVER RESPONSE
            ////return true;

            string message1;
            int index = 0;

            for (int i = 0; i < transData.ObjectList.Count; i++)
            {
                if (transData.ObjectList[i].name == "BikeType")
                {
                    index = i;
                    break;
                }
            }

            if (!transData.ReturningKey)
            {
                BikeType currenttype = (BikeType)transData.ObjectList[index].data;

                ////TESTING ONLY START
                ////Randomly choose a door for opening.
                ////Odd numbered doors are Normal Bikes, Even numbered doors are Ebikes.
                ////
                transData.BoxNumber = TestingRandomBoxNumber(currenttype);

                //This just simulates a positive response from the server for bike availability every other time a transaction is run.
                if (OddEvenCountForTesting % 2 == 0)
                    return true;

                ////TESTING ONLY END
                Program.logEvent("User told that their bike type is out of stock");
                if (currenttype == BikeType.Bike)
                {
                    message1 = LanguageTranslation.OUT_OF_STOCK_NORMAL_BIKE;
                }
                else
                {
                    message1 = LanguageTranslation.OUT_OF_STOCK_EBIKE;
                }

                YesNoForm bikeoutofstock = new YesNoForm(message1/*, message2*/);
                bikeoutofstock.ShowDialog();

                if (bikeoutofstock.YesResult)
                {
                    ////TO BE DONE LATER:
                    ////CONTACT BIKE SERVER WITH NEW CHECKOUT INFORMATION
                    ////
                    ////IF NEW BIKE TYPE IS AVAILABLE CONTINUE BELOW
                    ////ELSE 
                    //Program.ShowErrorMessage("There are currently no bikes\r\navailable at this location", 4000);
                    //return false;
                    ////
                    if (currenttype == BikeType.Bike)
                    {
                        transData.ObjectList[index].data = BikeType.EBike;
                        Program.logEvent("User selected EBike Type");
                        ////TESTING ONLY START
                        transData.BoxNumber = TestingRandomBoxNumber(BikeType.EBike);
                        ////TESTING ONLY END
                    }
                    else
                    {
                        transData.ObjectList[index].data = BikeType.Bike;
                        Program.logEvent("User selected Normal Bike type");
                        ////TESTING ONLY START
                        transData.BoxNumber = TestingRandomBoxNumber(BikeType.Bike);
                        ////TESTING ONLY END
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ////TESTING ONLY START
                transData.ObjectList[index].data = (BikeType)(OddEvenCountForTesting % 2);
                transData.BoxNumber = TestingRandomBoxNumber(BikeType.Bike);
                Program.logEvent("Random " + (BikeType)(transData.ObjectList[index].data) + " Box Number " + transData.BoxNumber + " Opened");
                return true;
                ////TESTING ONLY END
            }
        }

        /// <summary>
        /// AdminTasks - these are all the admin tasks -changing access codes,copying to dongle, viewing lists
        /// </summary>
        public void AdminTasks()
        {
            Program.logEvent("Admin Screen Accessed");
            AdminTaskChoiceForm adminTaskDlg = new AdminTaskChoiceForm();
            adminTaskDlg.ShowDialog();

            Program.blank.Visible = false;
        }

        private bool CheckReturningUseStatus(CustomerScreen s)
        {
            //if this screen is set to only be used when returning a key and you
            //aren't returning a key, then skip it.
            if (s.WhenIsScreenUsed == WhenToUse.Returning && transData.ReturningKey == false)
                return true;
            //if this screen is set to only be used when taking a key and you are
            //returning a key then skip it.
            else if (s.WhenIsScreenUsed == WhenToUse.Taking && transData.ReturningKey == true)
                return true;
            else
                return false;
        }
        
        private void SetMainformTitle()
        {            
            labelMessageTitle.Text = Program.MAINSCREEN_TITLE1 + "\r\n" + Program.MAINSCREEN_TITLE2;

            labelMessageTitle.Location = new System.Drawing.Point(((labelMessageTitle.Parent.Width - labelMessageTitle.Width) / 2), labelMessageTitle.Location.Y);
            labelMessageTitle.Location = new System.Drawing.Point(labelMessageTitle.Location.X, (labelMessageTitle.Parent.Height - labelMessageTitle.Height) / 2);
        }
        
        /// <summary>
        /// This method is used for Ontario only and gives out a random odd or even number 1-32 based on the bike type
        /// </summary>
        /// <param name="BikeType"></param>
        /// <returns></returns>
        private int TestingRandomBoxNumber(BikeType BikeType)
        {
            ////Randomly choose a door for opening.
            ////Odd numbered doors are Normal Bikes, Even numbered doors are Ebikes.
            ////
            Random RandomBox = new Random();
            bool done = false; int random=0;
            while (!done)
            {
                random = RandomBox.Next(1, Program.NUMBER_RELAYS);
                if (((BikeType == BikeType.Bike) && (random % 2 == 1)) || ((BikeType == BikeType.EBike) && (random % 2 == 0)))
                {
                    done = true;
                }
            }
            return random;
        }
    }
}
