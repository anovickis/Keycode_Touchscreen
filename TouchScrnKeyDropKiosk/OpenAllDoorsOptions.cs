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
    public partial class OpenAllDoorsOptions : baseForm
    {
        int DoorDelay;
        int Startbox;
        int Endbox;

        public OpenAllDoorsOptions()
        {
            InitializeComponent();

            ButtonOpenDoorDelay.Text = LanguageTranslation.OPEN_DOOR_DELAY;
            buttonStartDoor.Text = LanguageTranslation.START_DOOR_NUMBER;
            buttonEndDoor.Text = LanguageTranslation.END_DOOR_NUMBER;
            buttonOpenDoors.Text = LanguageTranslation.OPEN_DOORS;
            buttonExit.Text = LanguageTranslation.EXIT;

            DoorDelay = 1500;
            Startbox = 1;
            Endbox = Program.NUMBER_RELAYS;

            textBoxOpenDoorDelay.Text = DoorDelay.ToString();
            textBoxStartingDoorNumber.Text = Startbox.ToString();
            textBoxEndingDoorNumber.Text = Endbox.ToString();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonOpenDoorDelay_Click(object sender, EventArgs e)
        {
            KeyPadForm delay = new KeyPadForm(LanguageTranslation.ENTER_DOOR_DELAY, false, 4, true, false);
            delay.ShowDialog();
            if (delay.Result == "")
                return;

            DoorDelay = int.Parse(delay.Result);
            textBoxOpenDoorDelay.Text = delay.Result;
        }

        private void buttonStartDoor_Click(object sender, EventArgs e)
        {
            KeyPadForm start = new KeyPadForm(LanguageTranslation.ENTER_START_DOOR, false, 3, true, false);
            start.ShowDialog();
            if (start.Result == "")
                return;

            int temp = int.Parse(start.Result);
            if (temp < 1 || temp > Program.NUMBER_RELAYS)
            {
                Program.ShowErrorMessage(LanguageTranslation.BOX_NUMBER_NOT_EXIST, 3000);
                return;
            }
            if (temp > Endbox)
            {
                Program.ShowErrorMessage(LanguageTranslation.START_BOX_GREATER_END_BOX, 4000);
                return;
            }

            Startbox = int.Parse(start.Result);
            textBoxStartingDoorNumber.Text = start.Result;
        }

        private void buttonEndDoor_Click(object sender, EventArgs e)
        {
            KeyPadForm end = new KeyPadForm(LanguageTranslation.ENTER_END_DOOR, false, 3, true, false);
            end.ShowDialog();
            if (end.Result == "")
                return;

            int temp = int.Parse(end.Result);
            if (temp < 1 || temp > Program.NUMBER_RELAYS)
            {
                Program.ShowErrorMessage(LanguageTranslation.BOX_NUMBER_NOT_EXIST, 3000);
                return;
            }
            if (temp < Startbox)
            {
                Program.ShowErrorMessage(LanguageTranslation.START_BOX_GREATER_END_BOX, 4000);
                return;
            }

            Endbox = int.Parse(end.Result);
            textBoxEndingDoorNumber.Text = end.Result;
        }

        private void buttonOpenDoors_Click(object sender, EventArgs e)
        {
            Program.logEvent("Admin is opening all doors from " + Startbox + " to " + Endbox);
            BlankForm blank = new BlankForm();
            blank.Visible = true;
            for (int i = Startbox; i <= Endbox; i++)
            {
                Program.ShowErrorMessage(LanguageTranslation.OPENING_BOX + i, DoorDelay);
                if (i < Program.passwordMgr.AllBoxLocations.Length)
                {
                    Program.pm.OpenAndCloseRelay(i);
                    Program.logEvent("Box# " + i.ToString() + " Opened");
                }
            }
            Program.locationdata.makeHTTPPost(HTTPPostType.KeyLoaded, "", "", "");
            blank.Close();
            Program.logEvent("All doors open process complete");
        }
    }
}
