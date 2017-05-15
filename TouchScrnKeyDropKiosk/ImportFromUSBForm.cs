using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace KeyCabinetKiosk
{
    public partial class ImportFromUSBForm : baseForm
    {
        private DriveDetector USBDetector = null;
        public ImportType ImportType;
        private FileDialog FileChooser;
        private string DestinationFileName = "";
        public bool Arrived;
        public ImportFromUSBForm()
        {
            InitializeComponent();
            button_cancel.Text = LanguageTranslation.CANCEL;
            label.Text = LanguageTranslation.INSERT_USB_DRIVE;

            ImportType = ImportType.User;
            USBDetector = new DriveDetector();
            USBDetector.ProgramStarted = true;
            USBDetector.DeviceArrived += new DriveDetectorEventHandler(USBDetector_DeviceArrived);
            USBDetector.DeviceRemoved += new DriveDetectorEventHandler(USBDetector_DeviceRemoved);
            FileChooser = new OpenFileDialog();
            FileChooser.FileOk += new CancelEventHandler(FileChooser_FileOk);
            FileChooser.DefaultExt = ".xml";
            FileChooser.Title = LanguageTranslation.CHOOSE_IMPORT_FILE;
            FileChooser.Filter = "Data Files(*.XLS;*.XLSX;*.CSV)|*.XLS;*.XLSX;*.CSV|All files (*.*)|*.*";
            FileChooser.CheckFileExists = true;
            FileChooser.CheckPathExists = true;
            Arrived = false;
        }        

        void USBDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
        {
            if (Arrived)
                return;
            Arrived = true;

            Program.logEvent("USB device has been detected");
            SetLabelText(label, LanguageTranslation.IMPORTING);

            //open file choose dialog
            if (ImportType == ImportType.User)
            {
                FileChooser.InitialDirectory = Path.Combine(e.Drive, "Users.xml");
                DestinationFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "Users.xml");
            }
            else if (ImportType == ImportType.AccessCode)
            {
                FileChooser.InitialDirectory = Path.Combine(e.Drive, "keypasswords.xml");
                DestinationFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "keypasswords.xml");
            }
            FileChooser.ShowDialog();
        }

        void FileChooser_FileOk(object sender, CancelEventArgs e)
        {
            FileChooser.Dispose();
            if (e.Cancel)
                return;

            string ImportFileName = FileChooser.FileName;
            Program.logEvent("Import file name: " + ImportFileName);
            if (!File.Exists(ImportFileName))
            {
                Program.logEvent("Data file does not exist");
                Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_NOT_EXIST, 4000);
                resetCurrentWorkingDirectory();
                return;
            }

            #region Import Users
            if (ImportType == ImportType.User)
            {
                try
                {
                    Program.logEvent("Importing User Data");
                    if (Path.GetExtension(ImportFileName) == ".xml")
                    {
                        Program.logEvent("XML File identified");
                        if ((Program.userMgr.ReloadFromFile(ImportFileName)))
                        {
                            Program.logEvent("XML File Verified");
                            File.Copy(ImportFileName, DestinationFileName, true);
                            Program.logEvent("XML File Copied");
                        }
                        else
                        {
                            Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_HAS_ERRORS, 4000);
                            Program.logEvent("Error with file during user import");
                        }
                    }
                    else if (Path.GetExtension(ImportFileName) == ".csv")
                    {
                        Program.logEvent("CSV File identified");
                        usersList list = new usersList();
                        //read in list from .csv into List
                        StreamReader reader = new StreamReader(ImportFileName);
                        string currentline; string[] currentlinecomponents;

                        currentline = reader.ReadLine(); //read the first line and ignore it
                        while (!reader.EndOfStream)
                        {
                            currentline = reader.ReadLine();
                            
                            if (currentline.Trim() != "" && currentline != null)
                            {   
                                currentlinecomponents = currentline.Split(',');
                                if (currentlinecomponents.Length == 5)
                                {
                                    list.AddUser(new User(currentlinecomponents[0].Trim(), currentlinecomponents[1].Trim(),
                                        currentlinecomponents[2].Trim(), currentlinecomponents[3].Trim(),
                                        currentlinecomponents[4].Trim()));
                                }
                                else if (currentlinecomponents.Length == 7)
                                {
                                    list.AddUser(new User(currentlinecomponents[0].Trim(), currentlinecomponents[1].Trim(),
                                        currentlinecomponents[2].Trim(), currentlinecomponents[3].Trim(),
                                        currentlinecomponents[4].Trim(), currentlinecomponents[5].Trim(),
                                        currentlinecomponents[6].Trim()));
                                }
                                else if (currentlinecomponents.Length == 14)
                                {
                                    list.AddUser(new User(currentlinecomponents[0].Trim(), currentlinecomponents[1].Trim(),
                                        currentlinecomponents[2].Trim(), currentlinecomponents[3].Trim(),
                                        currentlinecomponents[4].Trim(), currentlinecomponents[5].Trim(),
                                        currentlinecomponents[6].Trim(), int.Parse(currentlinecomponents[7].Trim()),
                                        currentlinecomponents[8].Trim(), currentlinecomponents[9].Trim(),
                                        currentlinecomponents[10].Trim(), int.Parse(currentlinecomponents[11].Trim()),
                                        currentlinecomponents[12].Trim(), currentlinecomponents[13].Trim()));
                                }
                            }
                        }

                        Program.logEvent("CSV file read into memory");
                        //serialize list to XML file
                        list.SerializeToXmlFile("temp.xml");
                        Program.logEvent("CSV file converted to XML");
                        if ((Program.userMgr.ReloadFromFile("temp.xml")))
                        {
                            Program.logEvent("XML file Verified");
                            File.Copy("temp.xml", DestinationFileName, true);
                            Program.logEvent("XML file Copied");
                            try
                            {
                                File.Delete("temp.xml");
                                Program.logEvent("XML temp file deleted");
                            }
                            catch (Exception ex)
                            {
                                Program.logEvent("Error trying to delete temp.xml: " + ex.Message);
                            }
                        }
                        else
                        {
                            Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_HAS_ERRORS, 4000);
                            Program.logEvent("Error with file during user import");
                        }
                    }
                }
                catch(Exception ex)
                {

                    Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_ERRORS_NOT_EXIST, 4000);
                    Program.logEvent("Error during user import: " + ex.Message);
                    this.Close();
                }
            }
            #endregion
            #region Import Access Codes
            else if (ImportType == ImportType.AccessCode)
            {
                try
                {
                    Program.logEvent("Importing KeyPassword Data");
                    if (Path.GetExtension(ImportFileName) == ".xml")
                    {
                        Program.logEvent("XML file identified");
                        if ((Program.passwordMgr.ReloadFromFile(ImportFileName)))
                        {
                            Program.logEvent("XML file verified");
                            File.Copy(ImportFileName, DestinationFileName, true);
                            Program.logEvent("XML file copied");
                        }
                        else
                        {
                            Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_HAS_ERRORS, 4000);
                            Program.logEvent("Error with file during department import");
                        }
                    }
                    else if (Path.GetExtension(ImportFileName) == ".csv")
                    {
                        Program.logEvent("CSV file identified");
                        KeyPassWordList list = new KeyPassWordList();
                        //read in list from .csv into List
                        StreamReader reader = new StreamReader(ImportFileName);
                        string currentline; string[] currentlinecomponents;

                        currentline = reader.ReadLine(); //read the first line and ignore it
                        while (!reader.EndOfStream)
                        {
                            currentline = reader.ReadLine();                            
                            if ((currentline.Trim() != "") && (currentline != null))
                            {
                                currentlinecomponents = currentline.Split(',');
                                if (currentlinecomponents.Length == 2)
                                {
                                    list.AddKeyPassword(new KeyPassword(currentlinecomponents[0].Trim(),
                                        currentlinecomponents[1].Trim()));
                                }
                                else if (currentlinecomponents.Length == 5)
                                {
                                    list.AddKeyPassword(new KeyPassword(currentlinecomponents[0].Trim(),
                                        currentlinecomponents[1].Trim(), currentlinecomponents[2].Trim(), currentlinecomponents[3].Trim(),
                                        currentlinecomponents[4].Trim()));
                                }
                                else if (currentlinecomponents.Length == 9)
                                {
                                    list.AddKeyPassword(new KeyPassword(currentlinecomponents[0].Trim(),
                                        currentlinecomponents[1].Trim(), currentlinecomponents[2].Trim(),
                                        currentlinecomponents[3].Trim(), currentlinecomponents[4].Trim(),
                                        currentlinecomponents[5].Trim(), int.Parse(currentlinecomponents[6].Trim()),
                                        currentlinecomponents[7].Trim(), currentlinecomponents[9].Trim()));
                                }
                            }
                        }
                        Program.logEvent("CSV file read into memory");
                        //serialize list to XML file
                        list.SerializeToXmlFile("temp.xml");
                        Program.logEvent("CSV file converted to XML");
                        if ((Program.passwordMgr.ReloadFromFile("temp.xml")))
                        {
                            Program.logEvent("XML file verified");
                            File.Copy("temp.xml", DestinationFileName, true);
                            Program.logEvent("XML file copied");
                            try
                            {
                                File.Delete("temp.xml");
                                Program.logEvent("XML temp file deleted");
                            }
                            catch (Exception ex)
                            {
                                Program.logEvent("Error trying to delete temp.xml: " + ex.Message);
                            }
                        }
                        else
                        {
                            Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_HAS_ERRORS, 4000);
                            Program.logEvent("Error with file during department import");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Program.ShowErrorMessage(LanguageTranslation.DATA_FILE_ERRORS_NOT_EXIST, 4000);
                    Program.logEvent("Error during department import: " + ex.Message);
                    this.Close();
                }
            }  
            #endregion
            button_cancel.Visible = false;
            SetLabelText(label, LanguageTranslation.IMPORT_COMPLETE);
            resetCurrentWorkingDirectory();
        }

        void USBDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
        {
            Program.logEvent("USB Device Removed");
            this.Close();
            resetCurrentWorkingDirectory();
        }

        private void SetLabelText(System.Windows.Forms.Label label, string text)
        {
            label.Text = text;
            HorizontalCenterLabel(label);
            VerticalCenterLabel(label);
        }

        private void HorizontalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);
        }

        private void VerticalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(label.Location.X, (label.Parent.Height - label.Height) / 2);

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Program.logEvent("Cancelling Importing Process");
            this.Close();
        }

        private void resetCurrentWorkingDirectory()
        {
            Directory.SetCurrentDirectory(System.Windows.Forms.Application.StartupPath);
        }
    }

    public enum ImportType
    {
        AccessCode, User
    }
}
