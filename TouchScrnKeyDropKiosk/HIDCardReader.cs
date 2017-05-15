using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace KeyCabinetKiosk
{
    public partial class HIDCardReader : baseForm
    {
        public bool Cancelled { get; private set; }
        public string Data { get; private set; }
        int DataLength;

        public HIDCardReader()
        {
            DataLength = Program.PASSWORD_SIZE;
            Cancelled = false;
            Data = "";
            InitializeComponent();

            label1.Text = LanguageTranslation.SCAN_HID_CARD;
            buttonCancel.Text = LanguageTranslation.CANCEL;
            buttonContinue.Text = LanguageTranslation.CONTINUE;

            this.textBoxHIDCardDisplay.KeyUp += new KeyEventHandler(textBoxHIDCardDisplay_KeyUp);
            this.buttonCancel.KeyUp += new KeyEventHandler(buttonCancel_KeyUp);
            this.KeyUp += new KeyEventHandler(HID_Card_Reader_KeyUp);
            this.textBoxHIDCardDisplay.Focus();
        }
               
        void textBoxHIDCardDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    Thread.Sleep(1000); //let the thread sleep for a second so that the user can see that their number showed up in the textbox
                    buttonContinue.Visible = true;                    
                    break;

                #region Digit Keys
                case Keys.D0:
                case Keys.NumPad0:
                    { if (Data.Length < DataLength) { Data += "0"; } break; }
                case Keys.D1:
                case Keys.NumPad1:
                    { if (Data.Length < DataLength) { Data += "1"; } break; }
                case Keys.D2:
                case Keys.NumPad2:
                    { if (Data.Length < DataLength) { Data += "2"; } break; }
                case Keys.D3:
                case Keys.NumPad3:
                    { if (Data.Length < DataLength) { Data += "3"; } break; }
                case Keys.D4:
                case Keys.NumPad4:
                    { if (Data.Length < DataLength) { Data += "4"; } break; }
                case Keys.D5:
                case Keys.NumPad5:
                    { if (Data.Length < DataLength) { Data += "5"; } break; }
                case Keys.D6:
                case Keys.NumPad6:
                    { if (Data.Length < DataLength) { Data += "6"; } break; }
                case Keys.D7:
                case Keys.NumPad7:
                    { if (Data.Length < DataLength) { Data += "7"; } break; }
                case Keys.D8:
                case Keys.NumPad8:
                    { if (Data.Length < DataLength) { Data += "8"; } break; }
                case Keys.D9:
                case Keys.NumPad9:
                    { if (Data.Length < DataLength) { Data += "9"; } break; }
                #endregion

                #region Alphas
                case Keys.A:
                    { if (Data.Length < DataLength) { Data += "a"; } break; }
                case Keys.B:
                    { if (Data.Length < DataLength) { Data += "b"; } break; }
                case Keys.C:
                    { if (Data.Length < DataLength) { Data += "c"; } break; }
                case Keys.D:
                    { if (Data.Length < DataLength) { Data += "d"; } break; }
                case Keys.E:
                    { if (Data.Length < DataLength) { Data += "e"; } break; }
                case Keys.F:
                    { if (Data.Length < DataLength) { Data += "f"; } break; }
                case Keys.G:
                    { if (Data.Length < DataLength) { Data += "g"; } break; }
                case Keys.H:
                    { if (Data.Length < DataLength) { Data += "h"; } break; }
                case Keys.I:
                    { if (Data.Length < DataLength) { Data += "i"; } break; }
                case Keys.J:
                    { if (Data.Length < DataLength) { Data += "j"; } break; }
                case Keys.K:
                    { if (Data.Length < DataLength) { Data += "k"; } break; }
                case Keys.L:
                    { if (Data.Length < DataLength) { Data += "l"; } break; }
                case Keys.M:
                    { if (Data.Length < DataLength) { Data += "m"; } break; }
                case Keys.N:
                    { if (Data.Length < DataLength) { Data += "n"; } break; }
                case Keys.O:
                    { if (Data.Length < DataLength) { Data += "o"; } break; }
                case Keys.P:
                    { if (Data.Length < DataLength) { Data += "p"; } break; }
                case Keys.Q:
                    { if (Data.Length < DataLength) { Data += "q"; } break; }
                case Keys.R:
                    { if (Data.Length < DataLength) { Data += "r"; } break; }
                case Keys.S:
                    { if (Data.Length < DataLength) { Data += "s"; } break; }
                case Keys.T:
                    { if (Data.Length < DataLength) { Data += "t"; } break; }
                case Keys.U:
                    { if (Data.Length < DataLength) { Data += "u"; } break; }
                case Keys.V:
                    { if (Data.Length < DataLength) { Data += "v"; } break; }
                case Keys.W:
                    { if (Data.Length < DataLength) { Data += "w"; } break; }
                case Keys.X:
                    { if (Data.Length < DataLength) { Data += "x"; } break; }
                case Keys.Y:
                    { if (Data.Length < DataLength) { Data += "y"; } break; }
                case Keys.Z:
                    { if (Data.Length < DataLength) { Data += "z"; } break; }
                #endregion
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }
        
        void HID_Card_Reader_KeyUp(object sender, KeyEventArgs e)
        {
            textBoxHIDCardDisplay_KeyUp(sender, e);
        }

        void buttonCancel_KeyUp(object sender, KeyEventArgs e)
        {
            textBoxHIDCardDisplay_KeyUp(sender, e);
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
