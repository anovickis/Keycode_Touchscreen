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
    /// This form allows the administrator to change the config file element GLOBAL_ACCESS_TYPE
    /// </summary>
    public partial class AccessModeSelectionForm : baseForm
    {
        public AccessModeSelectionForm()
           : base(Program.TIMEOUT_INTERVAL)
        {
            InitializeComponent();
            labelPanelMessage.Text = LanguageTranslation.SELECT_ACCESS_METHOD;
            labelName.Text = LanguageTranslation.CURRENT_ACCESS;
            buttonExit.Text = LanguageTranslation.EXIT;
            buttonSimple.Text = LanguageTranslation.ACCESS_CODE_ONLY;
            buttonCard.Text = LanguageTranslation.CREDIT_CARD_ONLY;
            buttonBoth.Text = LanguageTranslation.ACCESS_AND_CARD;
            ThreadSafeLabelCentering(labelPanelMessage);

            DisplayCurrentType();
        }

        /// <summary>
        /// Change Access Type to Access Code Only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSimple_Click(object sender, EventArgs e)
        {
            SetAccessType(Program.GlobalAccessType.ALL_SIMPLE);

            DisplayCurrentType();
            resetTimer();
        }

        /// <summary>
        /// Change Access Type to Card Reader Only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCard_Click(object sender, EventArgs e)
        {
            SetAccessType(Program.GlobalAccessType.ALL_CARD_ONLY);

            DisplayCurrentType();
            resetTimer();
        }

        /// <summary>
        /// Change Access Type to Both Access Code and Card Reader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBoth_Click(object sender, EventArgs e)
        {
            SetAccessType(Program.GlobalAccessType.ALL_BOTH);

            DisplayCurrentType();
            resetTimer();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetAccessType(Program.GlobalAccessType type)
        {
            Program.GLOBAL_ACCESS_TYPE = type;

            CommonTasks.WriteValueToConfigurationFile("globals", "globalAccessType",((int) type).ToString());
            Program.logEvent("Access Type changed to " + type.ToString());
        }
        
        private void DisplayCurrentType()
        {
            switch (Program.GLOBAL_ACCESS_TYPE)
            {
                case Program.GlobalAccessType.ALL_SIMPLE :

                    this.labelAccessType.Text = LanguageTranslation.ACCESS_CODE_ONLY;
                    break;

                case Program.GlobalAccessType.ALL_CARD_ONLY:

                    this.labelAccessType.Text = LanguageTranslation.CREDIT_CARD_ONLY;

                    break;

                case Program.GlobalAccessType.ALL_BOTH:

                    this.labelAccessType.Text = LanguageTranslation.ACCESS_AND_CARD;

                    break;

                default:

                    this.labelAccessType.Text = LanguageTranslation.INVALID_ACCESS_TYPE;

                    break;
            }
        }     
    }
}
