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
    public partial class RFIDScanForm : baseForm
    {
        public bool ReturnBool { get; private set; }
        string TagID;
        public RFIDScanForm(string TagIDstring)
        {
            InitializeComponent();

            label1.Text = LanguageTranslation.SCANNING_FOR_RFID_TAG;
            label1.Update();
            ReturnBool = false;
            TagID = TagIDstring;
            this.Shown += new EventHandler(RFIDScanForm_Shown);
        }

        void RFIDScanForm_Shown(object sender, EventArgs e)
        {
            if (Program.RFID_READER_TYPE.ToUpper() == "THINGMAGIC")
            {
                if (Program.ThingMagicRFIDreader.IsReading)
                {
                    label1.Text = LanguageTranslation.PREPARING_RFID_SCANNER_WAIT;
                    ThreadSafeLabelCentering(label1);
                    label1.Update();
                }
                //Wait for the RFID read to be done reading.
                while (Program.ThingMagicRFIDreader.IsReading)
                    Thread.Sleep(1000);

                label1.Text = LanguageTranslation.SCANNING_FOR_RFID_TAG;
                ThreadSafeLabelCentering(label1);
                label1.Update();
                ReturnBool = Program.locationdata.scanRFIDTagNumber(TagID);               
                this.Close();
            }
            else if (Program.RFID_READER_TYPE.ToUpper() == "IMPINJ")
            {
                if (Program.ImpinjRFIDreader.IsReading)
                {
                    label1.Text = LanguageTranslation.PREPARING_RFID_SCANNER_WAIT;
                    ThreadSafeLabelCentering(label1);
                    label1.Update();
                }
                //Wait for the RFID read to be done reading.
                while (Program.ImpinjRFIDreader.IsReading)
                {
                    label1.Update();
                    Thread.Sleep(1000);
                }

                label1.Text = LanguageTranslation.SCANNING_FOR_RFID_TAG;
                ThreadSafeLabelCentering(label1);
                label1.Update();
                ReturnBool = Program.locationdata.scanRFIDTagNumber(TagID);
                Program.ImpinjRFIDreader.DoneReading();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}
