using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyCabinetKiosk
{
    public enum language
    {
        English = 0,
        Chinese = 1
    }
    class LanguageTranslation
    {
        internal static language language { get; set; }

        public LanguageTranslation()
        {
            language = language.English;
        }

        public LanguageTranslation(language UserLanguage)
        {
            language = UserLanguage;
        }

        /// <summary>
        /// "Unrecoverable Error in configuration file"
        /// </summary>
        internal static string CONFIGURATION_FILE_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIGURATION_FILE_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIGURATION_FILE_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID Error in configuration file"
        /// </summary>
        internal static string CONFIGURATION_RFID_ERROR_1
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIGURATION_RFID_ERROR_1;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIGURATION_RFID_ERROR_1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "WARNING:If you do not exit, additional program errors may occur.\r\nDo you want to Continue?"
        /// </summary>
        internal static string CONFIGURATION_RFID_ERROR_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIGURATION_RFID_ERROR_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIGURATION_RFID_ERROR_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Unrecoverable Error in  opening ports"
        /// </summary>
        internal static string CONFIGURATION_OPEN_PORT_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIGURATION_OPEN_PORT_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIGURATION_OPEN_PORT_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Unrecoverable Error running application"
        /// </summary>
        internal static string CONFIGURATION_APP_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIGURATION_APP_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIGURATION_APP_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Code"
        /// </summary>
        internal static string HIDDEN_EXIT_KEYPAD_MESSAGE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.HIDDEN_EXIT_KEYPAD_MESSAGE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.HIDDEN_EXIT_KEYPAD_MESSAGE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This user has no more\r\nusages available.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_USAGES_USER_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_USAGES_USER_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_USAGES_USER_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This box has no more\r\nusages available.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_USAGES_BOX_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_USAGES_BOX_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_USAGES_BOX_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This user's timeframe\r\nhas not arrived yet.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_TIMEFRAME_USER_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_TIMEFRAME_USER_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_TIMEFRAME_USER_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This box's timeframe\r\nhas not arrived yet.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_TIMEFRAME_BOX_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_TIMEFRAME_BOX_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_TIMEFRAME_BOX_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This user's timeframe\r\nhas already passed.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_TIMEFRAME_USER_ERROR_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_TIMEFRAME_USER_ERROR_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_TIMEFRAME_USER_ERROR_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "This box's timeframe\r\nhas already passed.\r\nDoor opening cancelled."
        /// </summary>
        internal static string DOOR_TIMEFRAME_BOX_ERROR_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DOOR_TIMEFRAME_BOX_ERROR_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DOOR_TIMEFRAME_BOX_ERROR_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Return Key And Close\r\nDoor Firmly When Done."
        /// </summary>
        internal static string RETURN_KEY_CLOSE_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RETURN_KEY_CLOSE_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RETURN_KEY_CLOSE_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Take Key And Close\r\nDoor Firmly When Done."
        /// </summary>
        internal static string TAKE_KEY_CLOSE_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TAKE_KEY_CLOSE_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TAKE_KEY_CLOSE_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Transaction Cancelled"
        /// </summary>
        internal static string TRANSACTION_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TRANSACTION_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TRANSACTION_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "There is no user\r\nassigned to this\r\naccess code."
        /// </summary>
        internal static string ACCESS_CODE_NO_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_CODE_NO_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_CODE_NO_USER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Invalid Password"
        /// </summary>
        internal static string INVALID_PASSWORD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INVALID_PASSWORD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INVALID_PASSWORD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Operation Timed Out"
        /// </summary>
        internal static string OPERATION_TIMED_OUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPERATION_TIMED_OUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPERATION_TIMED_OUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Take"
        /// </summary>
        internal static string TAKE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TAKE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TAKE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Return"
        /// </summary>
        internal static string RETURN
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RETURN;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RETURN;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter User ID"
        /// </summary>
        internal static string ENTER_USER_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_USER_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_USER_ID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "ID Scan Timed Out"
        /// </summary>
        internal static string ID_SCAN_TIMED_OUT		
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_SCAN_TIMED_OUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_SCAN_TIMED_OUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Confirm"
        /// </summary>
        internal static string CONFIRM
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIRM;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIRM;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Ok"
        /// </summary>
        internal static string OK
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OK;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OK;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Cancel"
        /// </summary>
        internal static string CANCEL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CANCEL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CANCEL;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Reservation Confirmation Timed Out"
        /// </summary>
        internal static string RESERVATION_CONFIRMATION_TIMEOUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RESERVATION_CONFIRMATION_TIMEOUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RESERVATION_CONFIRMATION_TIMEOUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Inspect And Report Car\r\nDamage On Form Prior\r\nTo Leaving"
        /// </summary>
        internal static string REPORT_CAR_DAMAGE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.REPORT_CAR_DAMAGE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.REPORT_CAR_DAMAGE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Continue"
        /// </summary>
        internal static string CONTINUE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONTINUE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONTINUE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Each vehicle contains a\r\nyellow folder with vehicle\r\nassigned mileage logs.\r\nDrivers are required to fill\r\nthe log out completely\r\nfor each driving day"
        /// </summary>
        internal static string FILL_OUT_MILEAGE_FORM
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.FILL_OUT_MILEAGE_FORM;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.FILL_OUT_MILEAGE_FORM;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nOperator\r\nNumber"
        /// </summary>
        internal static string ENTER_OPERATOR_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_OPERATOR_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_OPERATOR_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nVehicle\r\nNumber"
        /// </summary>
        internal static string ENTER_VEHICLE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_VEHICLE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_VEHICLE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Reservation Not Found\r\nTry Again or Please Call\r\nAdministrator"
        /// </summary>
        internal static string RESERVATION_NOT_FOUND
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RESERVATION_NOT_FOUND;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RESERVATION_NOT_FOUND;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Car\r\nOdometer\r\nReading"
        /// </summary>
        internal static string ENTER_ODOMETER_READING
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_ODOMETER_READING;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_ODOMETER_READING;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Multiple Users Match\r\nFingerprint. Try Again"
        /// </summary>
        internal static string MULTIPLE_FINGERPRINT_MATCHES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.MULTIPLE_FINGERPRINT_MATCHES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.MULTIPLE_FINGERPRINT_MATCHES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Would You Like To"
        /// </summary>
        internal static string ASK_TEXT_MESSAGE_1
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ASK_TEXT_MESSAGE_1;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ASK_TEXT_MESSAGE_1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Receive A Text Receipt?"
        /// </summary>
        internal static string ASK_TEXT_MESSAGE_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ASK_TEXT_MESSAGE_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ASK_TEXT_MESSAGE_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nPhone\r\nNumber"
        /// </summary>
        internal static string ENTER_PHONE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_PHONE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_PHONE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User text receipt/r/ntimed out"
        /// </summary>
        internal static string TEXT_MESSAGE_TIMEOUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TEXT_MESSAGE_TIMEOUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TEXT_MESSAGE_TIMEOUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User text receipt/r/ncancelled"
        /// </summary>
        internal static string TEXT_MESSAGE_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TEXT_MESSAGE_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TEXT_MESSAGE_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Invalid Location\r\nTry Again"
        /// </summary>
        internal static string INVALID_LOCATION
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INVALID_LOCATION;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INVALID_LOCATION;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "No RFID Tag Found\r\nFor Box "
        /// </summary>
        internal static string RFID_TAG_NOT_FOUND
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_TAG_NOT_FOUND;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_TAG_NOT_FOUND;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID Tag Found"
        /// </summary>
        internal static string RFID_TAG_FOUND
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_TAG_FOUND;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_TAG_FOUND;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID Tag Not Detected"
        /// </summary>
        internal static string RFID_TAG_NOT_DETECTED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_TAG_NOT_DETECTED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_TAG_NOT_DETECTED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Timed out\r\nTry Again"
        /// </summary>
        internal static string CARD_READ_TIMEOUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CARD_READ_TIMEOUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CARD_READ_TIMEOUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Swipe Cancelled\r\nTry Again"
        /// </summary>
        internal static string CARD_READ_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CARD_READ_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CARD_READ_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Card not read\r\nTry Again"
        /// </summary>
        internal static string CARD_READ_FAILED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CARD_READ_FAILED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CARD_READ_FAILED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Please Choose A Type\r\nOf Bike"
        /// </summary>
        internal static string BIKE_CHOOSE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BIKE_CHOOSE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BIKE_CHOOSE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Bike"
        /// </summary>
        internal static string BIKE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BIKE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BIKE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "EBike"
        /// </summary>
        internal static string EBIKE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EBIKE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EBIKE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Normal Bikes Are Out Of\r\nStock. Would You Like To\r\nRent An EBike?"
        /// </summary>
        internal static string OUT_OF_STOCK_NORMAL_BIKE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OUT_OF_STOCK_NORMAL_BIKE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OUT_OF_STOCK_NORMAL_BIKE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "EBikes Are Out Of\r\nStock. Would You Like To\r\nRent An Normal Bike?"
        /// </summary>
        internal static string OUT_OF_STOCK_EBIKE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OUT_OF_STOCK_EBIKE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OUT_OF_STOCK_EBIKE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Code Only"
        /// </summary>
        internal static string ACCESS_CODE_ONLY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_CODE_ONLY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_CODE_ONLY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Credit Card Only"
        /// </summary>
        internal static string CREDIT_CARD_ONLY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CREDIT_CARD_ONLY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CREDIT_CARD_ONLY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access and Card"
        /// </summary>
        internal static string ACCESS_AND_CARD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_AND_CARD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_AND_CARD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Invalid access type"
        /// </summary>
        internal static string INVALID_ACCESS_TYPE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INVALID_ACCESS_TYPE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INVALID_ACCESS_TYPE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Select Access Method"
        /// </summary>
        internal static string SELECT_ACCESS_METHOD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SELECT_ACCESS_METHOD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SELECT_ACCESS_METHOD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Current Access: "
        /// </summary>
        internal static string CURRENT_ACCESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CURRENT_ACCESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CURRENT_ACCESS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Exit"
        /// </summary>
        internal static string EXIT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EXIT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EXIT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Press Screen To Start"
        /// </summary>
        internal static string BUTTON_START
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BUTTON_START;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BUTTON_START;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Page Down"
        /// </summary>
        internal static string PAGE_DOWN
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PAGE_DOWN;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PAGE_DOWN;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Page Up"
        /// </summary>
        internal static string PAGE_UP
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PAGE_UP;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PAGE_UP;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Add User"
        /// </summary>
        internal static string ADD_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADD_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADD_USER;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Delete User"
        /// </summary>
        internal static string DELETE_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DELETE_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DELETE_USER;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Modify User"
        /// </summary>
        internal static string MODIFY_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.MODIFY_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.MODIFY_USER;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Add Another User"
        /// </summary>
        internal static string ADD_ANOTHER_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADD_ANOTHER_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADD_ANOTHER_USER;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Enter ID"
        /// </summary>
        internal static string ENTER_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_ID;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "ID Number not unique"
        /// </summary>
        internal static string ID_NUM_NOT_UNIQUE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_NUM_NOT_UNIQUE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_NUM_NOT_UNIQUE;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "User added"
        /// </summary>
        internal static string USER_ADDED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER_ADDED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER_ADDED;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Enter User ID to be Deleted"
        /// </summary>
        internal static string ENTER_USER_ID_DELETE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_USER_ID_DELETE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_USER_ID_DELETE;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "User Does Not Exist"
        /// </summary>
        internal static string USER_NOT_EXIST
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER_NOT_EXIST;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER_NOT_EXIST;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "ID #"
        /// </summary>
        internal static string ID_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Name"
        /// </summary>
        internal static string NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NAME;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Phone"
        /// </summary>
        internal static string PHONE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PHONE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PHONE;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Email"
        /// </summary>
        internal static string EMAIL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Enter ID of User to be Modified"
        /// </summary>
        internal static string ENTER_USER_ID_MODIFY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_USER_ID_MODIFY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_USER_ID_MODIFY;
                }
                else
                {
                    return null;
                }
            }
        }   
        
        /// <summary>
        /// "Cabinet Box Access"
        /// </summary>
        internal static string CABINET_BOX_ACCESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CABINET_BOX_ACCESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CABINET_BOX_ACCESS;
                }
                else
                {
                    return null;
                }
            }
        }   

            /// <summary>
        /// "Email Transaction Report"
        /// </summary>
        internal static string EMAIL_TRANSACTION_REPORT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_TRANSACTION_REPORT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_TRANSACTION_REPORT;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "USB Data Import"
        /// </summary>
        internal static string USB_TRANS_IMPORT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USB_TRANS_IMPORT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USB_TRANS_IMPORT;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Display Transactions"
        /// </summary>
        internal static string TRANSACTION_DISPLAY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TRANSACTION_DISPLAY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TRANSACTION_DISPLAY;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Display RFID Info"
        /// </summary>
        internal static string RFID_INFO_DISPLAY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_INFO_DISPLAY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_INFO_DISPLAY;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "Administrative Tasks"
        /// </summary>
        internal static string ADMIN_TASKS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADMIN_TASKS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADMIN_TASKS;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "transaction files copied"
        /// </summary>
        internal static string TRANSACTIONS_COPIED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TRANSACTIONS_COPIED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TRANSACTIONS_COPIED;
                }
                else
                {
                    return null;
                }
            }
        }   

            /// <summary>
        /// "transaction file\r\ncopy error"
        /// </summary>
        internal static string TRANSACTIONS_COPY_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TRANSACTIONS_COPY_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TRANSACTIONS_COPY_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }   

        /// <summary>
        /// "USB memory stick\r\nnot detected"
        /// </summary>
        internal static string USB_NOT_DETECTED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USB_NOT_DETECTED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USB_NOT_DETECTED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Importing Access Code\r\ndata or User data?"
        /// </summary>
        internal static string IMPORT_QUESTION
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.IMPORT_QUESTION;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.IMPORT_QUESTION;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User"
        /// </summary>
        internal static string USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access"
        /// </summary>
        internal static string ACCESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS;
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// "Number of Enrolled Fingerprints For User ID: "
        /// </summary>
        internal static string ENROLLED_FINGERPRINT_COUNT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENROLLED_FINGERPRINT_COUNT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENROLLED_FINGERPRINT_COUNT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Clear Enrolled Fingerprints"
        /// </summary>
        internal static string CLEAR_ENROLLED_FINGERPRINTS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CLEAR_ENROLLED_FINGERPRINTS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CLEAR_ENROLLED_FINGERPRINTS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enroll New Fingerprint"
        /// </summary>
        internal static string ENROLL_FINGERPRINTS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENROLL_FINGERPRINTS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENROLL_FINGERPRINTS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Fingerprints Cleared"
        /// </summary>
        internal static string FINGERPRINTS_CLEARED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.FINGERPRINTS_CLEARED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.FINGERPRINTS_CLEARED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "View Box Info"
        /// </summary>
        internal static string BOX_INFO
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BOX_INFO;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BOX_INFO;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set Box Access"
        /// </summary>
        internal static string BOX_ACCESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BOX_ACCESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BOX_ACCESS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Change Admin Password"
        /// </summary>
        internal static string CHANGE_ADMIN_PWORD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CHANGE_ADMIN_PWORD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CHANGE_ADMIN_PWORD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Service Manager Number"
        /// </summary>
        internal static string ADMIN_PHONE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADMIN_PHONE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADMIN_PHONE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Change Admin Email"
        /// </summary>
        internal static string ADMIN_EMAIL_ADDRESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADMIN_EMAIL_ADDRESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADMIN_EMAIL_ADDRESS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Change Admin TextMsg Number"
        /// </summary>
        internal static string ADMIN_TEXT_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADMIN_TEXT_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADMIN_TEXT_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Open Box Door"
        /// </summary>
        internal static string OPEN_BOX_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_BOX_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_BOX_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Open All Doors"
        /// </summary>
        internal static string OPEN_ALL_DOORS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_ALL_DOORS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_ALL_DOORS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Method"
        /// </summary>
        internal static string ACCESS_METHOD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_METHOD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_METHOD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User Management"
        /// </summary>
        internal static string USER_MANAGEMENT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER_MANAGEMENT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER_MANAGEMENT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Biometric Enrollment"
        /// </summary>
        internal static string BIOMETRIC_ENROLLMENT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BIOMETRIC_ENROLLMENT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BIOMETRIC_ENROLLMENT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Invalid box number\r\nTry Again"
        /// </summary>
        internal static string INVALID_BOX_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INVALID_BOX_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INVALID_BOX_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Use\"Change Admin Password\" Try Again"
        /// </summary>
        internal static string USE_CHANGE_ADMIN_PWORD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USE_CHANGE_ADMIN_PWORD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USE_CHANGE_ADMIN_PWORD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Service Manager Phone Number"
        /// </summary>
        internal static string SERVICE_MGR_PHONE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SERVICE_MGR_PHONE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SERVICE_MGR_PHONE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter New Admin Email"
        /// </summary>
        internal static string NEW_ADMIN_EMAIL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NEW_ADMIN_EMAIL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NEW_ADMIN_EMAIL;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Admin Text Message Number"
        /// </summary>
        internal static string NEW_ADMIN_TXT_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NEW_ADMIN_TXT_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NEW_ADMIN_TXT_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter User ID For Biometric Enrolling"
        /// </summary>
        internal static string ID_BIOMETRIC_ENROLLING
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_BIOMETRIC_ENROLLING;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_BIOMETRIC_ENROLLING;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "N/A"
        /// </summary>
        internal static string NOT_AVAILABLE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NOT_AVAILABLE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NOT_AVAILABLE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Change\r\nPassword"
        /// </summary>
        internal static string CHANGE_PASSWORD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CHANGE_PASSWORD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CHANGE_PASSWORD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "new password cancelled"
        /// </summary>
        internal static string NEW_PASSWORD_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NEW_PASSWORD_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NEW_PASSWORD_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "passwords must be\r\n"
        /// </summary>
        internal static string PASSWORD_LENGTH_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PASSWORD_LENGTH_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PASSWORD_LENGTH_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\ncharacters"
        /// </summary>
        internal static string PASSWORD_LENGTH_ERROR_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PASSWORD_LENGTH_ERROR_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PASSWORD_LENGTH_ERROR_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Admin Password"
        /// </summary>
        internal static string ADMIN_PASSWORD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ADMIN_PASSWORD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ADMIN_PASSWORD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nBox\r\nNumber"
        /// </summary>
        internal static string ENTER_BOX_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_BOX_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_BOX_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Unfortunately we have\r\nbeen unable to verify\r\n your reservation\r\nPlease call administrator"
        /// </summary>
        internal static string RESERVATION_VERIFICATION_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RESERVATION_VERIFICATION_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RESERVATION_VERIFICATION_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nAccess\r\nCode"
        /// </summary>
        internal static string ENTER_ACCESS_CODE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_ACCESS_CODE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_ACCESS_CODE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan"
        /// </summary>
        internal static string SCAN
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Press Scan to Begin\r\nFingerprint "
        /// </summary>
        internal static string SCAN_FINGERPRINT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_FINGERPRINT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_FINGERPRINT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Please Scan A New\r\nFinger For Enrollment"
        /// </summary>
        internal static string NEW_SCAN_FINGERPRINT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NEW_SCAN_FINGERPRINT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NEW_SCAN_FINGERPRINT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Please Scan The\r\nSame Finger Again\r\nFor Enrollment\r\nScan Count = "
        /// </summary>
        internal static string AGAIN_SCAN_FINGERPRINT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.AGAIN_SCAN_FINGERPRINT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.AGAIN_SCAN_FINGERPRINT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Fingerprints Found\r\nWith Another ID.\r\nClear Old Prints?"
        /// </summary>
        internal static string CLEAR_FINGERPRINT_QUESTION
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CLEAR_FINGERPRINT_QUESTION;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CLEAR_FINGERPRINT_QUESTION;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Old Prints Cleared"
        /// </summary>
        internal static string OLD_PRINTS_CLEARED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OLD_PRINTS_CLEARED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OLD_PRINTS_CLEARED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Saving Fingerprint\r\nData"
        /// </summary>
        internal static string SAVING_FINGERPRINTS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SAVING_FINGERPRINTS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SAVING_FINGERPRINTS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan An Enrolled\r\nFinger On The\r\nBiometric Scanner"
        /// </summary>
        internal static string SCAN_ENROLLED_FINGERPRINT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_ENROLLED_FINGERPRINT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_ENROLLED_FINGERPRINT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Place Enrolled\r\nFinger On\r\nBiometric Scanner"
        /// </summary>
        internal static string PLACE_ENROLLED_FINGER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PLACE_ENROLLED_FINGER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PLACE_ENROLLED_FINGER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan Timed Out"
        /// </summary>
        internal static string SCAN_TIMED_OUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_TIMED_OUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_TIMED_OUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Fingerprint Match\r\nSuccessful"
        /// </summary>
        internal static string FINGERPRINT_MATCHED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.FINGERPRINT_MATCHED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.FINGERPRINT_MATCHED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Fingerprint Match\r\nFailed"
        /// </summary>
        internal static string FINGERPRINT_FAILED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.FINGERPRINT_FAILED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.FINGERPRINT_FAILED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Clear Scan Table"
        /// </summary>
        internal static string CLEAR_SCAN_TABLE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CLEAR_SCAN_TABLE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CLEAR_SCAN_TABLE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID Scan"
        /// </summary>
        internal static string RFID_SCAN
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_SCAN;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_SCAN;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Number Of Seconds To Scan"
        /// </summary>
        internal static string SCAN_LENGTH
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_LENGTH;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_LENGTH;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Box Number"
        /// </summary>
        internal static string BOX_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BOX_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BOX_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Code"
        /// </summary>
        internal static string ACCESS_CODE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_CODE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_CODE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Card Number"
        /// </summary>
        internal static string CARD_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CARD_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CARD_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Other Data"
        /// </summary>
        internal static string OTHER_DATA
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OTHER_DATA;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OTHER_DATA;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan #"
        /// </summary>
        internal static string SCAN_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Tag ID"
        /// </summary>
        internal static string TAG_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TAG_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TAG_ID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Read Count"
        /// </summary>
        internal static string READ_COUNT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.READ_COUNT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.READ_COUNT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID Data Error"
        /// </summary>
        internal static string RFID_DATA_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_DATA_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_DATA_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Must scan for at least\r\none second"
        /// </summary>
        internal static string SCAN_LENGTH_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_LENGTH_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_LENGTH_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scanning..."
        /// </summary>
        internal static string SCANNING
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCANNING;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCANNING;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data Update Thread Error"
        /// </summary>
        internal static string DATA_UPDATE_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_UPDATE_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_UPDATE_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data View Set Thread Error"
        /// </summary>
        internal static string DATA_VIEW_THREAD_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_VIEW_THREAD_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_VIEW_THREAD_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Email Successful\r\n\r\nThank You."
        /// </summary>
        internal static string EMAIL_SUCCESSFUL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_SUCCESSFUL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_SUCCESSFUL;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Text Successful\r\n\r\nThank You."
        /// </summary>
        internal static string TEXT_SUCCESSFUL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TEXT_SUCCESSFUL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TEXT_SUCCESSFUL;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Transaction History\r\nSuccessful\r\nThank You."
        /// </summary>
        internal static string EMAIL_TRANSACTIONS_SUCCESSFUL
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_TRANSACTIONS_SUCCESSFUL;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_TRANSACTIONS_SUCCESSFUL;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Text Failed.\r\nLocal Copy Stored.\r\nContact Admin.\r\nThank You"
        /// </summary>
        internal static string TEXT_FAILED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TEXT_FAILED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TEXT_FAILED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Email Failed.\r\nLocal Copy Stored.\r\nContact Admin.\r\nThank You"
        /// </summary>
        internal static string EMAIL_FAILED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_FAILED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_FAILED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Transaction History\r\nFailed\r\nContact Admin.\r\nThank You"
        /// </summary>
        internal static string EMAIL_TRANSACTIONS_FAILED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_TRANSACTIONS_FAILED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_TRANSACTIONS_FAILED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan HID Card Now"
        /// </summary>
        internal static string SCAN_HID_CARD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_HID_CARD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_HID_CARD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Manual Entry"
        /// </summary>
        internal static string MANUAL_ENTRY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.MANUAL_ENTRY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.MANUAL_ENTRY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\nPlace your license on the scanner, photo facing out, back of license against the glass."
        /// </summary>
        internal static string SCAN_LICENSE_FACING_OUT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_LICENSE_FACING_OUT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_LICENSE_FACING_OUT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\nPlace your license on the scanner, photo facing in, front of license against the glass."
        /// </summary>
        internal static string SCAN_LICENSE_FACING_IN
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_LICENSE_FACING_IN;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_LICENSE_FACING_IN;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "ID Scanner Error"
        /// </summary>
        internal static string ID_SCANNER_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_SCANNER_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_SCANNER_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Driver's License Number"
        /// </summary>
        internal static string ENTER_LICENSE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_LICENSE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_LICENSE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\n\r\nSCANNING ID\r\n\r\nPLEASE WAIT..."
        /// </summary>
        internal static string SCANNING_WAIT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCANNING_WAIT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCANNING_WAIT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\nPROCESSING ID...\r\n\r\nPlease Remove ID Now"
        /// </summary>
        internal static string PROCESSING_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PROCESSING_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PROCESSING_ID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Barcode Scanning Error:\r\nNo Barcode Data Scanned"
        /// </summary>
        internal static string BARCODE_SCANNING_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BARCODE_SCANNING_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BARCODE_SCANNING_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Error While Processing ID"
        /// </summary>
        internal static string PROCESSING_ID_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PROCESSING_ID_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PROCESSING_ID_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\nIs This Name Correct?\r\n"
        /// </summary>
        internal static string CONFIRM_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIRM_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIRM_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\n\r\nIs License Number Correct?\r\n"
        /// </summary>
        internal static string CONFIRM_LICENSE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CONFIRM_LICENSE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CONFIRM_LICENSE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "ID Scan Confirmed\r\nPlease Remove License"
        /// </summary>
        internal static string ID_SCAN_CONFIRMED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ID_SCAN_CONFIRMED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ID_SCAN_CONFIRMED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Error While Scanning ID:\r\nTransaction Cancelled"
        /// </summary>
        internal static string SCANNING_ID_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCANNING_ID_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCANNING_ID_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Driver's First Name"
        /// </summary>
        internal static string DRIVER_FIRST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DRIVER_FIRST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DRIVER_FIRST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Driver's Last Name"
        /// </summary>
        internal static string DRIVER_LAST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DRIVER_LAST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DRIVER_LAST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Driver's License Number"
        /// </summary>
        internal static string DRIVER_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DRIVER_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DRIVER_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Manual ID Entry Cancelled"
        /// </summary>
        internal static string MANUAL_ID_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.MANUAL_ID_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.MANUAL_ID_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Insert USB Drive"
        /// </summary>
        internal static string INSERT_USB_DRIVE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INSERT_USB_DRIVE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INSERT_USB_DRIVE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Importing..."
        /// </summary>
        internal static string IMPORTING
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.IMPORTING;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.IMPORTING;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data File Does Not Exist."
        /// </summary>
        internal static string DATA_FILE_NOT_EXIST
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_FILE_NOT_EXIST;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_FILE_NOT_EXIST;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data File Contains Errors."
        /// </summary>
        internal static string DATA_FILE_HAS_ERRORS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_FILE_HAS_ERRORS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_FILE_HAS_ERRORS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data File Contains Errors\r\nOr Does Not Exist."
        /// </summary>
        internal static string DATA_FILE_ERRORS_NOT_EXIST
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_FILE_ERRORS_NOT_EXIST;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_FILE_ERRORS_NOT_EXIST;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Import Complete\r\nRemove USB Drive."
        /// </summary>
        internal static string IMPORT_COMPLETE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.IMPORT_COMPLETE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.IMPORT_COMPLETE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Choose .csv or .xml File for Importing"
        /// </summary>
        internal static string CHOOSE_IMPORT_FILE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CHOOSE_IMPORT_FILE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CHOOSE_IMPORT_FILE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Press Button to Modify Box "
        /// </summary>
        internal static string PRESS_BUTTON_MODIFY_BOX
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PRESS_BUTTON_MODIFY_BOX;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PRESS_BUTTON_MODIFY_BOX;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// " Information"
        /// </summary>
        internal static string INFORMATION
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INFORMATION;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INFORMATION;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Restrictions"
        /// </summary>
        internal static string ACCESS_RESTRICTIONS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_RESTRICTIONS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_RESTRICTIONS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "RFID tag"
        /// </summary>
        internal static string RFID_TAG
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_TAG;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_TAG;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Open Door"
        /// </summary>
        internal static string OPEN_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Apply Changes"
        /// </summary>
        internal static string APPLY_CHANGES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.APPLY_CHANGES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.APPLY_CHANGES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Limited Num. of Uses"
        /// </summary>
        internal static string LIMITED_USES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.LIMITED_USES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.LIMITED_USES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Time Period"
        /// </summary>
        internal static string TIME_PERIOD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TIME_PERIOD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TIME_PERIOD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Time & Limited Uses"
        /// </summary>
        internal static string TIME_PERIOD_LIMITED_USES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.TIME_PERIOD_LIMITED_USES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.TIME_PERIOD_LIMITED_USES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Always On"
        /// </summary>
        internal static string ALWAYS_ON
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ALWAYS_ON;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ALWAYS_ON;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nCode To\r\nChange"
        /// </summary>
        internal static string ENTER_CODE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_CODE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_CODE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Cancelled"
        /// </summary>
        internal static string CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Code Not Found"
        /// </summary>
        internal static string ACCESS_CODE_NOT_FOUND
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_CODE_NOT_FOUND;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_CODE_NOT_FOUND;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nNew\r\nCode"
        /// </summary>
        internal static string 	ENTER_NEW_CODE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_NEW_CODE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_NEW_CODE;
                }
                else
                {
                    return null;
                }
            }
        }  

        /// <summary>
        /// "new number cancelled"
        /// </summary>
        internal static string NEW_NUMBER_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NEW_NUMBER_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NEW_NUMBER_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter RFID Tag Number"
        /// </summary>
        internal static string ENTER_RFID_TAG
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_RFID_TAG;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_RFID_TAG;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "new RFID\r\nnumber cancelled"
        /// </summary>
        internal static string RFID_NUMBER_CANCELLED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.RFID_NUMBER_CANCELLED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.RFID_NUMBER_CANCELLED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nCard\r\nNumber"
        /// </summary>
        internal static string ENTER_CARD_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_CARD_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_CARD_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Box location not set"
        /// </summary>
        internal static string BOX_LOCATION_NOT_SET
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BOX_LOCATION_NOT_SET;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BOX_LOCATION_NOT_SET;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Empty Data Not Accepted"
        /// </summary>
        internal static string NO_EMPTY_DATA
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.NO_EMPTY_DATA;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.NO_EMPTY_DATA;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Data Must Be"
        /// </summary>
        internal static string DATA_LENGTH_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_LENGTH_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_LENGTH_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "\r\nCharacters Long"
        /// </summary>
        internal static string DATA_LENGTH_ERROR_2
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATA_LENGTH_ERROR_2;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATA_LENGTH_ERROR_2;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Caps"
        /// </summary>
        internal static string CAPITALS_LOCK
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CAPITALS_LOCK;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CAPITALS_LOCK;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter"
        /// </summary>
        internal static string ENTER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "BckSpc"
        /// </summary>
        internal static string BACKSPACE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BACKSPACE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BACKSPACE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "On"
        /// </summary>
        internal static string ON
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ON;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ON;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Off"
        /// </summary>
        internal static string OFF
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OFF;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OFF;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Limit + Time Period
        /// </summary>
        internal static string LIMIT_PLUS_TIME_PERIOD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.LIMIT_PLUS_TIME_PERIOD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.LIMIT_PLUS_TIME_PERIOD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Access Restriction Type"
        /// </summary>
        internal static string ACCESS_RESTRICTION_TYPE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ACCESS_RESTRICTION_TYPE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ACCESS_RESTRICTION_TYPE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Start Time"
        /// </summary>
        internal static string START_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.START_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.START_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "End Time"
        /// </summary>
        internal static string END_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.END_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.END_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Limited Number of Uses"
        /// </summary>
        internal static string LIMITED_NUMBER_OF_USES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.LIMITED_NUMBER_OF_USES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.LIMITED_NUMBER_OF_USES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set Start Time"
        /// </summary>
        internal static string SET_START_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SET_START_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SET_START_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set End Time"
        /// </summary>
        internal static string SET_END_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SET_END_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SET_END_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set Limited Uses"
        /// </summary>
        internal static string SET_LIMITED_USES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SET_LIMITED_USES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SET_LIMITED_USES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set Access Restrictions for Box Number "
        /// </summary>
        internal static string SET_BOX_NUMBER_ACCESS_RESTRICTIONS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SET_BOX_NUMBER_ACCESS_RESTRICTIONS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SET_BOX_NUMBER_ACCESS_RESTRICTIONS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Set Access Restrictions for User Number "
        /// </summary>
        internal static string SET_USER_ACCESS_RESTRICTIONS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SET_USER_ACCESS_RESTRICTIONS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SET_USER_ACCESS_RESTRICTIONS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Start Time"
        /// </summary>
        internal static string ENTER_START_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_START_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_START_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Invalid Time"
        /// </summary>
        internal static string INVALID_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.INVALID_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.INVALID_TIME;
                }
                else
                {
                    return null;
                }
            }
        }



        /// <summary>
        /// "Enter\r\nEnd\r\nTime"
        /// </summary>
        internal static string ENTER_END_TIME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_END_TIME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_END_TIME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nLimited\r\n Uses"
        /// </summary>
        internal static string ENTER_LIMITED_USES
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_LIMITED_USES;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_LIMITED_USES;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Start date is later\r\nthan end date"
        /// </summary>
        internal static string START_DATE_LATER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.START_DATE_LATER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.START_DATE_LATER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User ID"
        /// </summary>
        internal static string USER_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER_ID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "First Name"
        /// </summary>
        internal static string FIRST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.FIRST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.FIRST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Last Name"
        /// </summary>
        internal static string LAST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.LAST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.LAST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Email Address"
        /// </summary>
        internal static string EMAIL_ADDRESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_ADDRESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_ADDRESS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Phone Number"
        /// </summary>
        internal static string PHONE_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PHONE_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PHONE_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Department"
        /// </summary>
        internal static string DEPARTMENT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DEPARTMENT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DEPARTMENT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Group"
        /// </summary>
        internal static string GROUP
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.GROUP;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.GROUP;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "User Type"
        /// </summary>
        internal static string USER_TYPE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.USER_TYPE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.USER_TYPE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Save"
        /// </summary>
        internal static string SAVE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SAVE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SAVE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Press Button To Modify User Information"
        /// </summary>
        internal static string PRESS_MODIFY_USER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PRESS_MODIFY_USER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PRESS_MODIFY_USER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter New ID"
        /// </summary>
        internal static string ENTER_NEW_ID
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_NEW_ID;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_NEW_ID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter First Name"
        /// </summary>
        internal static string ENTER_FIRST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_FIRST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_FIRST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Last Name"
        /// </summary>
        internal static string ENTER_LAST_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_LAST_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_LAST_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Email Address"
        /// </summary>
        internal static string ENTER_EMAIL_ADDRESS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_EMAIL_ADDRESS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_EMAIL_ADDRESS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Email Address Not\r\nFormatted Properly"
        /// </summary>
        internal static string EMAIL_ADDRESS_NOT_FORMATTED
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.EMAIL_ADDRESS_NOT_FORMATTED;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.EMAIL_ADDRESS_NOT_FORMATTED;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Department Name"
        /// </summary>
        internal static string ENTER_DEPARTMENT_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_DEPARTMENT_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_DEPARTMENT_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter Group Name"
        /// </summary>
        internal static string ENTER_GROUP_NAME
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_GROUP_NAME;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_GROUP_NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Open Door Delay"
        /// </summary>
        internal static string OPEN_DOOR_DELAY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_DOOR_DELAY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_DOOR_DELAY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Starting Door Number"
        /// </summary>
        internal static string START_DOOR_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.START_DOOR_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.START_DOOR_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Ending Door Number"
        /// </summary>
        internal static string END_DOOR_NUMBER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.END_DOOR_NUMBER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.END_DOOR_NUMBER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Open Doors"
        /// </summary>
        internal static string OPEN_DOORS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_DOORS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_DOORS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nDoor\r\nDelay"
        /// </summary>
        internal static string ENTER_DOOR_DELAY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_DOOR_DELAY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_DOOR_DELAY;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nStart\r\nDoor"
        /// </summary>
        internal static string ENTER_START_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_START_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_START_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Box number does\r\nnot exist"
        /// </summary>
        internal static string BOX_NUMBER_NOT_EXIST
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.BOX_NUMBER_NOT_EXIST;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.BOX_NUMBER_NOT_EXIST;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Starting Box Number\r\nIs Greater Than Ending\r\nBox Number"
        /// </summary>
        internal static string START_BOX_GREATER_END_BOX
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.START_BOX_GREATER_END_BOX;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.START_BOX_GREATER_END_BOX;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Enter\r\nEnd\r\nDoor"
        /// </summary>
        internal static string ENTER_END_DOOR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.ENTER_END_DOOR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.ENTER_END_DOOR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Opening Box "
        /// </summary>
        internal static string OPENING_BOX
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPENING_BOX;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPENING_BOX;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Error opening and closing relay - location: "
        /// </summary>
        internal static string OPEN_RELAY_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.OPEN_RELAY_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.OPEN_RELAY_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scanning for RFID Tag..."
        /// </summary>
        internal static string SCANNING_FOR_RFID_TAG
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCANNING_FOR_RFID_TAG;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCANNING_FOR_RFID_TAG;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Preparing RFID\r\nScanner Please Wait..."
        /// </summary>
        internal static string PREPARING_RFID_SCANNER_WAIT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.PREPARING_RFID_SCANNER_WAIT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.PREPARING_RFID_SCANNER_WAIT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Swipe Card"
        /// </summary>
        internal static string SWIPE_CARD
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SWIPE_CARD;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SWIPE_CARD;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan Attempts:"
        /// </summary>
        internal static string SCAN_ATTEMPTS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SCAN_ATTEMPTS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SCAN_ATTEMPTS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Scan error:\r\nTry again"
        /// </summary>
        internal static string CARD_SWIPE_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CARD_SWIPE_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CARD_SWIPE_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Database Error.\r\nTry Again or Please Call\r\n"
        /// </summary>
        internal static string DATABASE_ERROR
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.DATABASE_ERROR;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.DATABASE_ERROR;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Select Date"
        /// </summary>
        internal static string SELECT_DATE
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SELECT_DATE;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SELECT_DATE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Cabinet Door Transactions"
        /// </summary>
        internal static string CABINET_DOOR_TRANSACTIONS
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.CABINET_DOOR_TRANSACTIONS;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.CABINET_DOOR_TRANSACTIONS;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Sending Email Receipt"
        /// </summary>
        internal static string SENDING_EMAIL_RECEIPT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SENDING_EMAIL_RECEIPT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SENDING_EMAIL_RECEIPT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Sending Text Message Receipt"
        /// </summary>
        internal static string SENDING_TXTMSG_RECEIPT
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SENDING_TXTMSG_RECEIPT;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SENDING_TXTMSG_RECEIPT;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Communicating with\r\nServer..."
        /// </summary>
        internal static string COMMUNICATING_WITH_SERVER
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.COMMUNICATING_WITH_SERVER;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.COMMUNICATING_WITH_SERVER;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// "Sending Transaction\r\nHistory Report"
        /// </summary>
        internal static string SENDING_TRANSACTION_HISTORY
        {
            get
            {
                if (language == language.English)
                {
                    return Language_EN.SENDING_TRANSACTION_HISTORY;
                }
                else if (language == language.Chinese)
                {
                    return Language_CH.SENDING_TRANSACTION_HISTORY;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
