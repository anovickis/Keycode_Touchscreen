using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace KeyCabinetKiosk
{
    public partial class AddUsersForm : baseForm
    {
        private DataTable table;

        public AddUsersForm()
            :base()  //no timeout
        {
            InitializeComponent();
            button_Page_Down.Text = LanguageTranslation.PAGE_DOWN;
            button_Page_up.Text = LanguageTranslation.PAGE_UP;
            buttonAdd.Text = LanguageTranslation.ADD_USER;
            buttonDelete.Text = LanguageTranslation.DELETE_USER;
            buttonModify.Text = LanguageTranslation.MODIFY_USER;
            buttonExit.Text = LanguageTranslation.EXIT;

            threeDArrayCreateAndSort();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Program.logEvent("User selected to add a new user");
            try
            {
                YesNoForm doAnotherDlg = new YesNoForm(LanguageTranslation.ADD_ANOTHER_USER);
                do
                {
                    string IDNumber = "";

                    //need to add swipe feature  TBD

                    LongNameEntryForm IDForm = new LongNameEntryForm(Program.USER_ID_LENGTH, false, false);
                    IDForm.UseSpaceBar = true;
                    IDForm.DialogTitle = LanguageTranslation.ENTER_ID;
                    IDForm.ShowDialog();

                    if (IDForm.Ok)
                    {                        
                        IDNumber = IDForm.Description;
                        Program.logEvent("User entered new user ID: " + IDNumber);

                        if (!Program.userMgr.IsUnique(IDNumber))
                        {
                            Program.logEvent("The user ID Number entered is not unique. User not added");
                            Program.ShowErrorMessage(LanguageTranslation.ID_NUM_NOT_UNIQUE, 3000);
                            return;
                        }
                    }
                    else
                    {
                        Program.logEvent("User cancelled addition");
                        return;
                    }

                    User user = new User(IDNumber);
                    AddUser(user);
                    Program.ShowErrorMessage(LanguageTranslation.USER_ADDED, 2500);

                    ModifyUsersForm ModifyNewUser = new ModifyUsersForm(user);
                    ModifyNewUser.ShowDialog();
                    
                    doAnotherDlg.ShowDialog();

                } while (doAnotherDlg.YesResult);

                Program.userMgr.SaveFile();

                threeDArrayCreateAndSort();
            }
            catch (Exception ex)
            {
                throw new Exception("AddUsersForm:AddUser exception: " + ex.Message);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Program.logEvent("Exiting User Modification Screen");
            this.Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Program.logEvent("User selected to delete a user");
                LongNameEntryForm IDEntry = new LongNameEntryForm(Program.USER_ID_LENGTH, false, false);
                IDEntry.InitialString = "";
                IDEntry.DialogTitle = LanguageTranslation.ENTER_USER_ID_DELETE;
                IDEntry.ShowDialog();
                Program.logEvent("User selected for deletion ID Number: " + IDEntry.Description);
                if (Program.userMgr.RemoveUser(IDEntry.Description))
                {
                    Program.logEvent("User deleted with ID Number: " + IDEntry.Description);
                    Program.userMgr.SaveFile();
                    threeDArrayCreateAndSort();
                }
                else
                {
                    if (!IDEntry.Ok)
                        Program.ShowErrorMessage(LanguageTranslation.USER_NOT_EXIST, 4000);
                    Program.logEvent("User with ID Number " + IDEntry.Description + " does not exist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AddUsersForm:AddUser exception: " + ex.Message);
            }
        }

        private void  AddUser(User user)
        {
            try
            {
                if (!Program.userMgr.AddUser(user))
                {
                    Program.logEvent("User ID was not unique");
                    throw new Exception("AddUser - user not added");
                }
                Program.userMgr.SaveFile();
            }
            catch (Exception ex)
            {
                throw new Exception("AddUsersForm:AddUser exception: " + ex.Message);
            }

        }

        private void threeDArrayCreateAndSort()
        {
            table = new DataTable();
                        
            table.Columns.Add(LanguageTranslation.ID_NUMBER, typeof(string));
            table.Columns.Add(LanguageTranslation.NAME, typeof(string));
            table.Columns.Add(LanguageTranslation.PHONE, typeof(string));
            table.Columns.Add(LanguageTranslation.EMAIL, typeof(string));

            for (int i = 0; i < Program.userMgr.UsersList.Users.Count; i++)
            {
                table.Rows.Add(Program.userMgr.UsersList.Users[i].ID,
                               Program.userMgr.UsersList.Users[i].FirstName + " " + Program.userMgr.UsersList.Users[i].LastName,
                               Program.userMgr.UsersList.Users[i].phoneNumber,
                               Program.userMgr.UsersList.Users[i].emailAddress);
            }

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DataSource = table;
            dataGridView1.AutoResizeColumnHeadersHeight();
            dataGridView1.Columns[3].Width = dataGridView1.Width - (dataGridView1.Columns[0].Width + dataGridView1.Columns[1].Width + dataGridView1.Columns[2].Width) - 20;
            dataGridView1.Width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 20;
            dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(int.Parse(Program.WINDOW_BACKGROUND_COLOR.Split(',')[0]), int.Parse(Program.WINDOW_BACKGROUND_COLOR.Split(',')[1]), int.Parse(Program.WINDOW_BACKGROUND_COLOR.Split(',')[2])); ;
            dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            try
            {
                Program.logEvent("User selected to modify a user");
                LongNameEntryForm IDEntry = new LongNameEntryForm(Program.USER_ID_LENGTH, false, false);
                IDEntry.InitialString = "";
                IDEntry.DialogTitle = LanguageTranslation.ENTER_USER_ID_MODIFY;
                IDEntry.ShowDialog();
                Program.logEvent("User selected for modification ID Number: " + IDEntry.Description);

                User moduser = Program.userMgr.GetUser(IDEntry.Description);
                if (moduser != null)
                {
                    ModifyUsersForm ModForm = new ModifyUsersForm(moduser);
                    ModForm.ShowDialog();
                    if (!ModForm.Cancelled)
                    {
                        Program.logEvent("User modified with ID Number: " + IDEntry.Description);
                        threeDArrayCreateAndSort();
                    }
                    else
                    {
                        Program.logEvent("User modification cancelled");
                    }
                }
                else
                {
                    if (!IDEntry.Ok)
                        Program.ShowErrorMessage(LanguageTranslation.USER_NOT_EXIST, 4000);
                    Program.logEvent("User with ID Number " + IDEntry.Description + " does not exist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AddUsersForm:AddUser exception: " + ex.Message);
            }
        }
        
        private void button_Page_up_Click(object sender, EventArgs e)
        {
            try
            {
                int ViewableRows = 12;

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

        private void button_Page_Down_Click(object sender, EventArgs e)
        {
            try
            {
                int ViewableRows = 12;
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
    }
}
