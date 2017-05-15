using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This class is used as a generic keypad form which can take in many types of data. There is a currency setting which allows the output text
    /// to look like a currency field (keeps track of the decimal and such). It can be used for password entry too as one of the constructors has a bool
    /// which will '*' out the output for secrecy of entry.
    /// </summary>

    public enum GenericDataEntryType
    {
        Normal, Date, Time, Currency
    }

    public partial class KeyPadForm : baseForm
    {
        public virtual string Result
        {
            get
            {
                return Data;
            }
            protected set
            {
                Data = value;
            }

        }
        public bool bOK { get; private set; }
        public GenericDataEntryType DataDisplayType;

        private int MaxLength;
        private bool AcceptShortData;
        private bool AcceptNoData;
        private string Data;
        private KeyPadForm() { }


        public KeyPadForm(string MessageLabel, bool HideCharEntries, int maxInputLength, bool acceptshortdata, bool acceptnodata)
            : base(Program.TIMEOUT_INTERVAL)
        {

            InitializeComponent();

            panel_color_accent.BackColor = System.Drawing.SystemColors.ButtonFace; // default color

            base.threadSafeLabelChanging(label1, MessageLabel);

            base.ThreadSafeLabelCentering(label1);

            Cancel_button.Text = LanguageTranslation.CANCEL;
            Accept_button.Text = LanguageTranslation.OK;
            Backspace.Text = LanguageTranslation.BACKSPACE;

            textBoxAdmin.Visible = false;
            if (HideCharEntries)
                textBox1.PasswordChar = '*';

            textBox1.MaxLength = maxInputLength;
            MaxLength = maxInputLength;
            AcceptShortData = acceptshortdata;
            AcceptNoData = acceptnodata;

            bOK = false;
            DataDisplayType = GenericDataEntryType.Normal;
            Data = "";
        }

        public KeyPadForm(string MessageLabel, bool HideCharEntries, int maxInputLength, bool acceptshortdata, bool acceptnodata, GenericDataEntryType ddt)
            : base(Program.TIMEOUT_INTERVAL)
        {

            InitializeComponent();

            panel_color_accent.BackColor = System.Drawing.SystemColors.ButtonFace; // default color

            base.threadSafeLabelChanging(label1, MessageLabel);

            base.ThreadSafeLabelCentering(label1);

            Cancel_button.Text = LanguageTranslation.CANCEL;
            Accept_button.Text = LanguageTranslation.OK;
            Backspace.Text = LanguageTranslation.BACKSPACE;

            textBoxAdmin.Visible = false;
            if (HideCharEntries)
                textBox1.PasswordChar = '*';

            textBox1.MaxLength = maxInputLength;
            MaxLength = maxInputLength;
            AcceptShortData = acceptshortdata;
            AcceptNoData = acceptnodata;

            bOK = false;
            DataDisplayType = ddt;
            Data = "";
            DisplayNumber();
        }

        public KeyPadForm(System.Drawing.Color AccentBackground, string MessageLabel, bool HideCharEntries, int maxInputLength)
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();

            panel_color_accent.BackColor = AccentBackground;
            base.threadSafeLabelChanging(label1, MessageLabel);

            Cancel_button.Text = LanguageTranslation.CANCEL;
            Accept_button.Text = LanguageTranslation.OK;
            Backspace.Text = LanguageTranslation.BACKSPACE;

            textBoxAdmin.Visible = false;
            if (HideCharEntries)
                textBox1.PasswordChar = '*';

            textBox1.MaxLength = maxInputLength;
            MaxLength = maxInputLength;

           
            bOK = false;
            DataDisplayType = GenericDataEntryType.Normal;
            Data = "";
        }
        
        public void SetKeyPadMessage(string message)
        {
            base.threadSafeLabelChanging(label1, message);
        }
        public void SetMaxCharacters(int maxInputLength)
        {
           textBox1.MaxLength = maxInputLength;
           MaxLength = maxInputLength; 
        }
        public void SetInitialTextValue(string value)
        {
            textBox1.Text = value;
        }
        public void ClearText()
        {
            textBox1.Text = "";
        }
        private void Accept_button_Click(object sender, EventArgs e)
        {
            if (!AcceptNoData && Result == "")
            {
                Program.ShowErrorMessage(LanguageTranslation.NO_EMPTY_DATA, 4000);
                return;
            }
            else if (!AcceptShortData && (Result.Length < MaxLength))
            {
                Program.ShowErrorMessage(LanguageTranslation.DATA_LENGTH_ERROR + " " + MaxLength.ToString() + LanguageTranslation.DATA_LENGTH_ERROR_2, 4000);
                return;
            }

            bOK = true;
            Data = textBox1.Text;
        
            this.Close();
        }
        private void Cancel_button_Click(object sender, EventArgs e)
        {
            bOK = false;

            textBox1.Text = "";
            this.Close();
        }

        private void NumberButton(string addedNumber)
        {            
            DoTimerReset();
            if (Data.Length < MaxLength)
            { Data += addedNumber; }
            DisplayNumber();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NumberButton("1"); DoTimerReset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NumberButton("2"); DoTimerReset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NumberButton("3"); DoTimerReset();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NumberButton("4"); DoTimerReset();
        } 

        private void button5_Click(object sender, EventArgs e)
        {
            NumberButton("5"); DoTimerReset();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NumberButton("6"); DoTimerReset();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            NumberButton("7"); DoTimerReset();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            NumberButton("8"); DoTimerReset();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            NumberButton("9"); DoTimerReset();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            NumberButton("0"); DoTimerReset();
        }

        public void DisplayNumber()
        {
            switch (DataDisplayType)
            {
                case GenericDataEntryType.Normal:
                {
                    textBox1.Text = Data;
                    break;
                }
                case GenericDataEntryType.Currency:
                {
                    string display = Data.PadLeft(MaxLength, '0');
                    display = display.Insert(Data.Length - 3, ".");
                    display = display.Insert(0, "$");
                    textBox1.Text = display;
                    break;
                }
                case GenericDataEntryType.Date:
                    string display2 = Data.PadLeft(8, '0');
                    display2 = display2.Insert(2, "/");
                    display2 = display2.Insert(5, "/");
                    textBox1.Text = display2;
                    break;
                case GenericDataEntryType.Time:
                    string display3 = Data.PadLeft(4, '0');
                    display3 = display3.Insert(2, ":");
                    textBox1.Text = display3;
                    break;
             }     
        }

        public virtual void Backspace_Click(object sender, EventArgs e)
        {
            if (Data.Length == 0)
            {
                return;
            }
            else
            {
                string temp = Data;
                Data = temp.Remove(Data.Length - 1);
            }
            DisplayNumber();
        }

        private void DoTimerReset()
        {
            this.resetTimer(); //dialog timer
        } 
    }
}
