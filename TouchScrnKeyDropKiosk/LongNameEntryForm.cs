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
    /// This class gives a generic Alpha Numeric Keyboard which can be used for data entry when both letters and numbers are needed.
    /// </summary>
    public partial class LongNameEntryForm : baseForm   
    {
        private int MaxTextSize; //maxium allowable size for text field

        private bool CapsOn; // cap state flag

        public bool Ok { get; private set; }

        public bool AllowBlankEntry { get; private set; }

        public bool UseSpaceBar 
        {
            set
            {
                bool spaceBar = value;

                if (spaceBar)
                {
                    this.buttonSpaceBar.Visible = true;
                }
                else
                {
                    this.buttonSpaceBar.Visible = false;
                }
            }
        }

        public bool UseAtSign
        {
            set
            {
                bool Atsign = value;

                if (Atsign)
                {
                    this.buttonAt.Visible = true;
                }
                else
                {
                    this.buttonAt.Visible = false;
                }
            }
        }

        public bool UsePeriod
        {
            set
            {
                bool period = value;

                if (period)
                {
                    this.buttonPeriod.Visible = true;
                }
                else
                {
                    this.buttonPeriod.Visible = false;
                }
            }
        }

        public bool UseUnderscore
        {
            set
            {
                bool underscore = value;

                if (underscore)
                {
                    this.buttonUnderscore.Visible = true;
                }
                else
                {
                    this.buttonUnderscore.Visible = false;
                }
            }
        }

        public string Description
        {
            get
            {
                return textBox1.Text;
            }
         
        }
        public string DialogTitle
        {
            set
            {
                //DialogTitle = value;
                this.label1.Text = value;
            }

        }
        public string InitialString
        {
            set
            {
                //for editing string
                this.textBox1.Text = value;
                this.textBox1.SelectionStart = 0;
                this.textBox1.SelectionLength = 0;
            }
        }

        public LongNameEntryForm(int maxTextSize, bool HideCharEntries, bool AllowNullEntry)
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            button_ShiftLock.Text = LanguageTranslation.CAPITALS_LOCK;
            Accept.Text = LanguageTranslation.ENTER;
            Space.Text = LanguageTranslation.CANCEL;
            Backspace.Text = LanguageTranslation.BACKSPACE;

            MaxTextSize = maxTextSize + 1;  //text less than max size
            CapsOn = false;
            Ok = false;

            if (HideCharEntries)
                textBox1.PasswordChar = '*';

            AllowBlankEntry = AllowNullEntry;
            this.buttonSpaceBar.Visible = false;
            this.labelCapsOnOff.Text = "Off";
        }

        public LongNameEntryForm(int maxTextSize, bool HideCharEntries, bool AllowNullEntry, string Title)
            : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            button_ShiftLock.Text = LanguageTranslation.CAPITALS_LOCK;
            Accept.Text = LanguageTranslation.ENTER;
            Space.Text = LanguageTranslation.CANCEL;
            Backspace.Text = LanguageTranslation.BACKSPACE;

            MaxTextSize = maxTextSize + 1;  //text less than max size
            CapsOn = false;
            Ok = false;

            if (HideCharEntries)
                textBox1.PasswordChar = '*';

            AllowBlankEntry = AllowNullEntry;
            this.buttonSpaceBar.Visible = false;
            this.labelCapsOnOff.Text = "Off";
            DialogTitle = Title;
        }

        #region Numeric Keys
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "1"; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "2"; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "3"; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "4"; }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "5"; }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "6"; }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "7"; }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "8"; }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "9"; }
        }

        private void button0_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) { textBox1.Text += "0"; }
        }
 #endregion

        #region Alpha Keys
        private void Q_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize) 
            {
                if (CapsOn)
                { textBox1.Text += "Q"; }
                else
                { textBox1.Text += "q"; }
            }
        }

        private void W_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "W"; }
                else
                { textBox1.Text += "w"; }
            }
        }

        private void E_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "E"; }
                else
                { textBox1.Text += "e"; }
            }
        }

        private void R_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "R"; }
                else
                { textBox1.Text += "r"; }
            }
        }

        private void T_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "T"; }
                else
                { textBox1.Text += "t"; }
            }
        }

        private void Y_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "Y"; }
                else
                { textBox1.Text += "y"; }
            }
        }

        private void U_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "U"; }
                else
                { textBox1.Text += "u"; }
            }
        }

        private void I_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "I"; }
                else
                { textBox1.Text += "i"; }
            }
        }

        private void O_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "O"; }
                else
                { textBox1.Text += "o"; }
            }
        }

        private void P_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "P"; }
                else
                { textBox1.Text += "p"; }
            }
        }

        private void A_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "A"; }
                else
                { textBox1.Text += "a"; }
            }
        }

        private void S_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "S"; }
                else
                { textBox1.Text += "s"; }
            }
        }

        private void D_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "D"; }
                else
                { textBox1.Text += "d"; }
            }
        }

        private void F_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "F"; }
                else
                { textBox1.Text += "f"; }
            }
        }

        private void G_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "G"; }
                else
                { textBox1.Text += "g"; }
            }
        }

        private void H_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "H"; }
                else
                { textBox1.Text += "h"; }
            }
        }

        private void J_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "J"; }
                else
                { textBox1.Text += "j"; }
            }
        }

        private void K_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "K"; }
                else
                { textBox1.Text += "k"; }
            }
        }

        private void L_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "L"; }
                else
                { textBox1.Text += "l"; }
            }
        }

        private void Z_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "Z"; }
                else
                { textBox1.Text += "z"; }
            }
        }

        private void X_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "X"; }
                else
                { textBox1.Text += "x"; }
            }
        }

        private void C_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "C"; }
                else
                { textBox1.Text += "c"; }
            }
        }

        private void V_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "V"; }
                else
                { textBox1.Text += "v"; }

            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "B"; }
                else
                { textBox1.Text += "b"; }

            }
        }

        private void N_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "N"; }
                else
                { textBox1.Text += "n"; }

            }
        }

        private void M_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                if (CapsOn)
                { textBox1.Text += "M"; }
                else
                { textBox1.Text += "m"; }

            }

        }
#endregion

        #region control keys

        private void Backspace_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
        }

        // now cancel
        private void Cancel_Click(object sender, EventArgs e)
        {
            Ok = false;
            Close();

        }

        private void Accept_Click(object sender, EventArgs e)
        {
            if (!AllowBlankEntry && Description == "")
            {
                Program.ShowErrorMessage(LanguageTranslation.NO_EMPTY_DATA, 4000);
                return;
            }
             
            Ok = true;
            Close();
        }
     

        private void button_ShiftLock_Click(object sender, EventArgs e)
        {
            //toggle CapsOn state flag
            if (CapsOn)
            {
                CapsOn = false;
                this.labelCapsOnOff.Text = LanguageTranslation.OFF;
            }
            else
            {
                CapsOn = true;
                this.labelCapsOnOff.Text = LanguageTranslation.ON;
            }
        }
        #endregion

        private void buttonSpaceBar_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {             
                { textBox1.Text += "-"; }
            }
        }

        private void buttonAt_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                { textBox1.Text += "@"; }
            }
        }

        private void buttonPeriod_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                { textBox1.Text += "."; }
            }
        }

        private void buttonUnderscore_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < MaxTextSize)
            {
                { textBox1.Text += "_"; }
            }
        }
    }
}
