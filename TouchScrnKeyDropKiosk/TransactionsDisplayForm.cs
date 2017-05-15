using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// TransactionsDisplayForm - display all transactions (open a door) on a given day. Uses a "date picker" to
    ///     determine date. Initializes as todays date. Use a datagrid to display data
    /// </summary>
    public partial class TransactionsDisplayForm : baseForm
    {

        [DllImport("user32.dll")] //this dll import is necessary to be able to send mouse events to simulate clicking
        private static extern void mouse_event(
            UInt32 dwFlags, // motion and click options
            UInt32 dx, // horizontal position or change
            UInt32 dy, // vertical position or change
            UInt32 dwData, // wheel movement
            IntPtr dwExtraInfo // application-defined information
            );

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint uMsg, int wParam, int lParam);

        private DataTable Transactions;


        public TransactionsDisplayForm()
            :base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            buttonExit.Text = LanguageTranslation.EXIT;
            buttonPgDn.Text = LanguageTranslation.PAGE_DOWN;
            buttonPgUp.Text = LanguageTranslation.PAGE_UP;
            buttonSelectDate.Text = LanguageTranslation.SELECT_DATE;
            label1.Text = LanguageTranslation.CABINET_DOOR_TRANSACTIONS;
            
            try
            {
                Transactions = new DataTable();
                
                for (int i = 0; i < Program.locationdata.Header.Split(',').Length; i++)
                {
                    Transactions.Columns.Add(Program.locationdata.Header.Split(',')[i]);
                }

                dateTimePicker1.MinDate = new DateTime(2009, 9, 1);
                dateTimePicker1.MaxDate = new DateTime(2050, 1, 1);

                showData();
            }
            catch (Exception ex)
            {
                throw new Exception("TransactionsDisplayForm:constructor - exception - " + ex.Message);
            }
        }

        /// <summary>
        /// showData - reads data from transaction file and will then populate the display form
        /// </summary>
        private void showData()
        {
            try
            {
                List<string> Data = new List<string>(); //all file data read in as list of strings
                List<string> NewRow = new List<string>();

                //Clear current list of transactions
                Transactions.Clear();

                //Check the current month's file first


                string fileName = "Transactions/" + dateTimePicker1.Value.Month + "transaction.csv";

                if (File.Exists(fileName))
                {
                    Data.AddRange(File.ReadAllLines(fileName, Encoding.UTF8));
                }
                else
                {     
                    // do nothing here - else causes dialog to "loop"
                    return;
                }

                Data.Reverse();  //show the lastest data on top


                string next;
                for (int i = 0; i < Data.Count; i++)
                {
                    //If the current transaction was from the correct date then add it to the list
                    next = Data.ElementAt<string>(i).Split(',')[0];
                    if (next == (dateTimePicker1.Value.Date.Month.ToString().PadLeft(2, '0') + "-" + dateTimePicker1.Value.Date.Day.ToString().PadLeft(2, '0') + "-" + dateTimePicker1.Value.Date.Year))
                    {
                        for (int j = 0; j < Program.locationdata.Header.Split(',').Length; j++)
                        {
                            NewRow.Add(Data.ElementAt<string>(i).Split(',')[j]);
                        }
                        Transactions.Rows.Add(NewRow.ToArray());
                        NewRow.Clear();
                    }
                }

                dataGridView1.DataSource = Transactions;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AutoResizeColumnHeadersHeight();
            }
            catch (Exception ex)
            {            
                throw new Exception("TransactionsDisplayForm:showData - exception - " + ex.Message);
            }
        }

        /// <summary>
        /// buttonSelectDate_Click - will send "mouse event" to date picker to bring up 'calendar display'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectDate_Click(object sender, EventArgs e)
        {
            uint WM_LBUTTONDOWN = 0x0201;

            int x = dateTimePicker1.Width - 10;
            int y = dateTimePicker1.Height / 2;
            int lParam = x + y * 0x00010000;

            SendMessage(dateTimePicker1.Handle, WM_LBUTTONDOWN, 1, lParam);
            resetTimer();
        }

        /// <summary>
        /// buttonPgUp_Click - scroll data grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPgUp_Click(object sender, EventArgs e)
        {
            if((dataGridView1.FirstDisplayedScrollingRowIndex - 1) >= 0)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex - 1;
            }
            resetTimer();
        }
        /// <summary>
        /// buttonPgDn_Click - scroll data grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPgDn_Click(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex + 1;
            resetTimer();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// dateTimePicker1_ValueChanged - shows the data whenever the date time pickers' value changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            showData();
            resetTimer();
        }
    }
}
