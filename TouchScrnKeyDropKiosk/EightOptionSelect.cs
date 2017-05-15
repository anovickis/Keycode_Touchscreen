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
    public enum SelectionResult
    {
        Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8
    }
        
    public partial class EightOptionSelect : baseForm
    {
        public SelectionResult Result { get; private set; }
        private Button[] ScreenButtons { get; set; }

        public EightOptionSelect()
        {
            InitializeComponent();
        }

        public EightOptionSelect(string title)
        {
            InitializeComponent();
            base.threadSafeLabelChanging(label1, title);
        }

        public EightOptionSelect(string title, string[] buttontitles)
        {
            InitializeComponent();
            base.threadSafeLabelChanging(label1, title);

            ScreenButtons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8 };
            for (int i = 0; i < 8; i++)
            {
                if (buttontitles.Length > i)
                {
                    ScreenButtons[i].Text = buttontitles[i];
                }
            }
        }

        public void SetButtonText(Button b, string text)
        {
            b.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button1;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button2;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button3;
            Close();
        }        

        private void button4_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button4;
            Close();
        }    

        private void button5_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button5;
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button6;
            Close();
        }
    
        private void button7_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button7;
            Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Result = SelectionResult.Button8;
            Close();
        }
    }
}
