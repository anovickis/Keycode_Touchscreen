using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using Impinj.OctaneSdk;
using ImpinjRFID;

namespace KeyCabinetKiosk
{
    public enum Displaytype
    {
        boxinfo, rfidinfo
    }

    public partial class DisplayAllInfoForm : baseForm
    {
        /// <summary>
        /// DisplayAllInfoForm - dialog displays all locations and passwords in a text box
        /// </summary>
        private const uint SB_PAGEUP = 0x02;
        private const uint SB_PAGEDOWN = 0x03;
        private const int WM_VSCROLL = 277; // Vertical scroll

        [DllImport("user32")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        
        private DataTable table;
        private ThingMagic.TagReadData[] TMreaddata;
        private TagReport IPJreaddata;
        private List<RFIDTagScanHistory> RFIDTagHistories;
        private Color[] RFIDTagColorRating { get; set; }

        public DisplayAllInfoForm()
            :base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            buttonClearTable.Text = LanguageTranslation.CLEAR_SCAN_TABLE;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonPageDown.Text = LanguageTranslation.PAGE_DOWN;
            buttonPageUp.Text = LanguageTranslation.PAGE_UP;
            buttonScan.Text = LanguageTranslation.RFID_SCAN;
            label1.Text = LanguageTranslation.SCAN_LENGTH;

            threeDArrayCreateAndSort();

           // DisplayAll();
        }

        public DisplayAllInfoForm(Displaytype type)
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            buttonClearTable.Text = LanguageTranslation.CLEAR_SCAN_TABLE;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonPageDown.Text = LanguageTranslation.PAGE_DOWN;
            buttonPageUp.Text = LanguageTranslation.PAGE_UP;
            buttonScan.Text = LanguageTranslation.RFID_SCAN;
            label1.Text = LanguageTranslation.SCAN_LENGTH;

            if (type == Displaytype.boxinfo)
                threeDArrayCreateAndSort();

            if (type == Displaytype.rfidinfo)
            {
                RFIDTagHistories = new List<RFIDTagScanHistory>();
                RFIDTagColorRating = new Color[] { Color.FromArgb(255, 0, 0), Color.FromArgb(240, 50, 50), Color.FromArgb(230, 100, 100), Color.FromArgb(220, 150, 150), Color.FromArgb(210, 180, 180), 
                    Color.FromArgb(255, 255, 255), Color.FromArgb(180, 210, 180), Color.FromArgb(150, 220, 150), Color.FromArgb(100, 230, 100), Color.FromArgb(50, 240, 50), Color.FromArgb(0, 255, 0) };
                buttonScan.Visible = true;
                label1.Visible = true;
                numericUpDownScanLength.Visible = true;
                buttonClearTable.Visible = true;
            }            
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {           
            this.Close();
        }
     
        private void buttonPageUp_Click(object sender, EventArgs e)
        {
            resetTimer();
            try
            {
                int ViewableRows = 18;

                if (dataGridView1.RowCount > ViewableRows)
                {
                    PropertyInfo verticalOffset = dataGridView1.GetType().
                                  GetProperty("VerticalOffset", BindingFlags.NonPublic | BindingFlags.Instance);

                    verticalOffset.SetValue(this.dataGridView1, 0, null);
                }                 
            }
            catch (Exception ex)
            {
                throw new Exception("DisplayAllInfoForm:buttonPageUp_Click - scrolling error" + ex.Message);
            }
        }

        private void buttonPageDown_Click(object sender, EventArgs e)
        {
            resetTimer();
            try
            {
                int ViewableRows = 18;
                int rowHeight = 21;  // 15;

                if (dataGridView1.RowCount > ViewableRows)
                {
                    int adjustmentRange = dataGridView1.RowCount * rowHeight;

                    int currentOffset = dataGridView1.VerticalScrollingOffset;

                    if (adjustmentRange > currentOffset)
                    {
                        PropertyInfo verticalOffset = dataGridView1.GetType().
                            GetProperty("VerticalOffset", BindingFlags.NonPublic | BindingFlags.Instance);

                        verticalOffset.SetValue(this.dataGridView1, (currentOffset + 50), null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DisplayAllInfoForm:buttonPageDown_Click - scrolling error" + ex.Message);
            }
        }

        private void threeDArrayCreateAndSort()
        {
            table = new DataTable();
                      
            table.Columns.Add(LanguageTranslation.BOX_NUMBER,typeof(int));
            table.Columns.Add(LanguageTranslation.ACCESS_CODE,typeof(string));
            table.Columns.Add(LanguageTranslation.CARD_NUMBER,typeof(string));
            table.Columns.Add(LanguageTranslation.OTHER_DATA, typeof(string));

            int[] boxNumbers = Program.passwordMgr.AllBoxLocations;

            for (int i = 1; i <= Program.NUMBER_RELAYS; i++)
            {
                string passwords = "";

                foreach (string s in Program.passwordMgr.FindPassword(i))
                    passwords += s + ',';

                if (passwords.Length > 0)
                    passwords = passwords.Remove(passwords.Length - 1);
                table.Rows.Add(boxNumbers[i], passwords, Program.passwordMgr.FindCardNumber(i), Program.passwordMgr.FindGenericData(i));
            }

            dataGridView1.DataSource = table;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoResizeColumnHeadersHeight();

            dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending);            
        }

        private void RFIDCreateAndSort()
        {
            try
            {
                if (Program.RFID_READER_TYPE.ToUpper() == "THINGMAGIC")
                {
                    TMreaddata = Program.ThingMagicRFIDreader.ReadTags(2000);

                    DecrementAllRFIDTagHistories();
                    bool HistoryExists = false;
                    foreach (ThingMagic.TagReadData tag in TMreaddata)
                    {
                        foreach (RFIDTagScanHistory history in RFIDTagHistories)
                        {
                            if (tag.EpcString.Equals(history.TagID))
                            {
                                history.IncrementRating();
                                history.ReadCount = tag.ReadCount;
                                HistoryExists = true;
                                break;
                            }
                        }
                        if (!HistoryExists)
                            RFIDTagHistories.Add(new RFIDTagScanHistory(tag.EpcString, tag.ReadCount));
                    }
                }
                else if (Program.RFID_READER_TYPE.ToUpper() == "IMPINJ")
                {
                    IPJreaddata = Program.ImpinjRFIDreader.ReadTags(2000, Program.IMPINJ_ANTENNA1_ENABLE, Program.IMPINJ_ANTENNA1_POWER, Program.IMPINJ_ANTENNA2_ENABLE,
                        Program.IMPINJ_ANTENNA2_POWER, Program.IMPINJ_ANTENNA3_ENABLE, Program.IMPINJ_ANTENNA3_POWER, Program.IMPINJ_ANTENNA4_ENABLE, Program.IMPINJ_ANTENNA4_POWER);
                   
                    DecrementAllRFIDTagHistories();
                    bool HistoryExists = false;
                    foreach (Tag tag in IPJreaddata)
                    {
                        foreach (RFIDTagScanHistory history in RFIDTagHistories)
                        {
                            if (tag.Epc.ToString().Equals(history.TagID))
                            {
                                history.IncrementRating();
                                history.ReadCount = tag.TagSeenCount;
                                HistoryExists = true;
                                break;
                            }
                        }
                        if (!HistoryExists)
                            RFIDTagHistories.Add(new RFIDTagScanHistory(tag.Epc.ToString(), tag.TagSeenCount));
                    }
                }

                //Had to create a new table here each time because resetting the same table was not working. It was causing errors in DataViewSet class
                table = new DataTable();

                table.Columns.Add(LanguageTranslation.SCAN_NUMBER, typeof(string));
                table.Columns.Add(LanguageTranslation.TAG_ID, typeof(string));
                table.Columns.Add(LanguageTranslation.READ_COUNT, typeof(string));
                for (int i = 0; i < RFIDTagHistories.Count; i++)
                {
                    table.Rows.Add((i + 1).ToString().PadLeft(3, '0'), RFIDTagHistories[i].TagID, RFIDTagHistories[i].ReadCount);
                }                

                ThreadSafeDataViewSetting(table);
            }
            catch (Exception Ex)
            {
                Program.ShowErrorMessage(LanguageTranslation.RFID_DATA_ERROR, 4000);
            }
        }
        
        private void ScanThread()
        {
            for (int i = 0; i < (numericUpDownScanLength.Value / 2); i++)
            {              
                RFIDCreateAndSort();
                Thread.Sleep(200);
            }
            if (Program.RFID_READER_TYPE.ToUpper() == "THINGMAGIC")
                Program.ThingMagicRFIDreader.DoneReading();
            else if (Program.RFID_READER_TYPE.ToUpper() == "IMPINJ")
                Program.ImpinjRFIDreader.DoneReading();

            ThreadSafeButtonEnabling(buttonScan);
        }
              
        private void buttonScan_Click(object sender, EventArgs e)
        {
            resetTimer();

            if (numericUpDownScanLength.Value < 1)
            {
                Program.ShowErrorMessage(LanguageTranslation.SCAN_LENGTH_ERROR, 4000);
                return;
            }

            buttonScan.Enabled = false;
            buttonScan.Text = LanguageTranslation.SCANNING;

            Thread scanthread = new Thread(new ThreadStart(ScanThread));
            scanthread.Start();
        }

        private void buttonClearTable_Click(object sender, EventArgs e)
        {
            RFIDTagHistories.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Update();
        }

        #region ThreadSafeDataViewSetting
        /// <summary>
        /// Allows another thread to enter the data into the table on the form
        /// </summary>
        /// <param name="label"></param>
        public void ThreadSafeDataViewSetting(DataTable datatable)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    DataViewSetting i = new DataViewSetting(dataViewSet);
                    this.Invoke(i, new object[] { datatable });
                }
                else
                {
                    dataViewSet(datatable);
                }
            }
            catch (Exception ex)
            {
                Program.ShowErrorMessage(LanguageTranslation.DATA_UPDATE_ERROR, 4000);
            }

        }
        delegate void DataViewSetting(DataTable datatable);

        private void dataViewSet(DataTable datatable)
        {
            try
            {
                dataGridView1.DataSource = datatable;
                dataGridView1.Sort(this.dataGridView1.Columns[1], ListSortDirection.Ascending);
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++) //must be dataGridView1.Rows.Count -1 to account for the blank row at the end of the table
                {
                    int rate = GetRFIDTagHistoryRating(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    Color color = RFIDTagColorRating[rate];
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = color;
                }
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AutoResizeColumnHeadersHeight();

                //dataGridView1.Sort(this.dataGridView1.Columns[1], ListSortDirection.Ascending);
                //dataGridView1.Update();
            }
            catch (Exception ex)
            {
                Program.ShowErrorMessage(LanguageTranslation.DATA_VIEW_THREAD_ERROR, 4000);
            }
        }
        #endregion

        #region ThreadSafeButtonEnabling
        /// <summary>
        /// Allows another thread to set the scan button on the form back to enabled
        /// </summary>
        /// <param name="label"></param>
        public void ThreadSafeButtonEnabling(Button button)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ButtonEnablingDelegate i = new ButtonEnablingDelegate(ButtonEnabling);
                    this.Invoke(i, new object[] { button });
                }
                else
                {
                    ButtonEnabling(button);
                }
            }
            catch (Exception ex)
            {
                Program.ShowErrorMessage(LanguageTranslation.DATA_UPDATE_ERROR, 4000);
            }

        }
        delegate void ButtonEnablingDelegate(Button button);

        private void ButtonEnabling(Button button)
        {
            button.Text = LanguageTranslation.RFID_SCAN;
            button.Enabled = true;
        }
        #endregion

        private void DecrementAllRFIDTagHistories()
        {
            foreach (RFIDTagScanHistory h in RFIDTagHistories)
            {
                h.DecrementRating();
                h.ReadCount = 0;
            }
        }

        private int GetRFIDTagHistoryRating(string TagID)
        {
            foreach (RFIDTagScanHistory h in RFIDTagHistories)
            {
                if (TagID.Equals(h.TagID))
                    return h.ScanRating;
            }
            return -1;
        }        
    }

    public class RFIDTagScanHistory
    {
        public string TagID { get; private set; }
        public int ScanRating { get; private set; }
        public int ReadCount { get; internal set; }

        public RFIDTagScanHistory(string id, int reads)
        {
            TagID = id;
            ReadCount = reads;
            ScanRating = 5;
        }

        public void IncrementRating()
        {
            if (ScanRating < 10)
                ScanRating++;
            //if the scan rating is not 10 to begin with, it will need to be incremented twice to 
            //offset the decrementing of all histories at the beginning of each scan.
            if (ScanRating < 10)
                ScanRating++;
        }

        public void DecrementRating()
        {
            if (ScanRating > 0)
                ScanRating--;
        }
    }    
}


///// <summary>
//    /// DisplayAll - calls the display based upon a switch for type
//    /// </summary>
//    private void DisplayAll()
//    {
//        try
//        {

//            switch (Program.GLOBAL_ACCESS_TYPE)
//            {
//                case Program.GlobalAccessType.ALL_SIMPLE:

//                    //SimpleDisplay();
//                    break;

//                case Program.GlobalAccessType.ALL_CARD_ONLY:

//                    //CardDisplay();
//                    break;

//                case Program.GlobalAccessType.ALL_BOTH:

//                    //CardDisplay();
//                    break;

//                case Program.GlobalAccessType.NO_GLOBAL:

//                    ByTypeDisplay();
//                    break;

//                default:

//                    throw new Exception("AccessPermissionCheck: unknown accessType");
//            }
//         }
//        catch (Exception ex)
//        {
//            Program.logDebug("DisplayAllInfoForm:DisplayAll exception " + ex.Message);

//            throw new Exception("DisplayAllInfoForm:DisplayAll exception " + ex.Message);

//        }      
//    }
//        private void ByTypeDisplay()
//    {
//       throw new Exception("ByTypeDisplay - exception - unsupported type");

//    }


///// <summary>
/////  SimpleDisplay - display data for simple access code only configuration
///// </summary>
//private void SimpleDisplay()
//{
//    try
//    {
//      StringBuilder sb = new StringBuilder();

//        string blank4 = "    ";
//        string blank2 = "  ";

//        sb.AppendFormat("\t{0}Box Numbers\t\tAccess Code", blank4);

//        sb.AppendLine();

//        List<KeyPassword> locationsList = Program.passwordMgr.keyPassWordList.keyPasswordList;
//        int[] boxNumbers = Program.passwordMgr.AllBoxLocations;

//        foreach (KeyPassword loc in locationsList)
//        {
//            if (loc.loc == 0)
//            {
//                continue;
//            }
//            else
//            {
//                sb.AppendFormat("\t{0}{1}{2}\t\t\t{3}{4}\r\n", blank4, blank4, boxNumbers[loc.loc], blank2, loc.password);
//            }
//        }

//        this.textBoxDisplay.Text = sb.ToString();
//    }
//    catch (Exception ex)
//    {
//        Program.logDebug("DisplayAllInfoForm:SimpleDisplay exception " + ex.Message);

//        throw new Exception("DisplayAllInfoForm:SimpleDisplay exception " + ex.Message);

//    }
//}
///// <summary>
///// CardDisplay - display data for cards - last 4 digits as well as access code - used with user intervention
///// </summary>
//private void CardDisplay()
//{

//    try
//    {
//        StringBuilder sb = new StringBuilder();

//        string blank4 = "    ";
//        string blank2 = "  ";

//        sb.AppendFormat("{0}Box Numbers\tAccess Code\tCard last 4 numbers", blank4);

//        sb.AppendLine();

//        List<KeyPassword> locationsList = Program.passwordMgr.keyPassWordList.keyPasswordList;
//        int[] boxNumbers = Program.passwordMgr.AllBoxLocations;

//        foreach (KeyPassword loc in locationsList)
//        {
//            if (loc.loc == 0)
//            {
//                continue;
//            }
//            else
//            {                                                  
//                sb.AppendFormat("{0}{1}{2}{3}\t\t{4}{5}\t\t\t{6}\r\n"
//                                    ,blank4                 //0
//                                    ,blank4                 //1
//                                    ,blank2                 //2
//                                    ,boxNumbers[loc.loc]    //3
//                                    ,blank4                 //4 
//                                    ,loc.password           //5
//                                    ,loc.cardNumber);       //6

//            }
//        }

//        this.textBoxDisplay.Text = sb.ToString();
//    }
//    catch (Exception ex)
//    {
//        Program.logDebug("DisplayAllInfoForm:SimpleDisplay exception " + ex.Message);

//        throw new Exception("DisplayAllInfoForm:SimpleDisplay exception " + ex.Message);

//    }

//}
