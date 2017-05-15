using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace KeyCabinetKiosk
{
    public class UserManager
    {
        public usersList UsersList {get; private set;}

        private string xmlFileName;

        public UserManager(string XmlFileName)
        {
            try
            {
                xmlFileName = XmlFileName;

                LoadFromFile(xmlFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("UserManager constructor exception: " + ex.Message);
            }
        }

        public bool ReloadFromFile(string filename)
        {
            return LoadFromFile(filename);
        }

        private bool LoadFromFile(string filename)
        {
            // read in xml data from file
            TextReader tr = new StreamReader(filename);
            try
            {
                UsersList = new usersList();
                XmlSerializer sr = new XmlSerializer(typeof(usersList));

                UsersList = (usersList)sr.Deserialize(tr);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("UserManager LoadFromFile exception: " + ex.Message);
            }
            finally
            {
                tr.Close();
            }
        }

        public bool IsUnique(string ID)
        {
            return UsersList.IsUnique(ID);
        }

        public bool AddUser(User user)
        {     
            return UsersList.AddUser(user);        
        }

        public bool RemoveUser(string ID)
        {
            return UsersList.RemoveUser(ID);
        }

        public string FindUserIDByAccessCode(string Access)
        {
            return UsersList.FindIDByAccessCode(Access);
        }

        public string FindUserFirstName(string ID)
        {
            return UsersList.FindFirstNameByID(ID);
        }

        public string FindUserLastName(string ID)
        {
            return UsersList.FindLastNameByID(ID);
        }

        public string FindUserAccessCode(string ID)
        {
            return UsersList.FindAccessCodeByID(ID);
        }

        public string FindUserBoxNumber(string ID)
        {
            return UsersList.FindBoxNumberByID(ID);
        }

        public string FindUserGroup(string ID)
        {
            return UsersList.FindGroupByID(ID);
        }

        public string FindUserDepartment(string ID)
        {
            return UsersList.FindDepartmentByID(ID);
        }

        public string FindUserEmailAddress(string ID)
        {
            return UsersList.FindEmailByID(ID);
        }

        public string FindUserPhoneNumber(string ID)
        {
            return UsersList.FindPhoneNumberByID(ID);
        }

        public userType FindUserType(string ID)
        {
            return UsersList.FindUserTypeByID(ID);
        }

        public int FindUserLimitedUsesCount(string ID)
        {
            return UsersList.FindUserLimitedUsesCountByID(ID);
        }

        public DateTime FindUserTimeframeStart(string ID)
        {
            return UsersList.FindUserAccessTimeFrameStartByID(ID);
        }

        public DateTime FindUserTimeframeEnd(string ID)
        {
            return UsersList.FindUserAccessTimeFrameEndByID(ID);
        }

        public accessTimeType FindUserTimeAccessType(string ID)
        {
            return UsersList.FindUserAccessTimeTypeByID(ID);
        }

        public bool ChangeUserID(string oldID, string newID)
        {
            return UsersList.ChangeID(oldID, newID);
        }

        public bool ChangeUserFirstName(string ID, string NewFirstName)
        {
            return UsersList.ChangeFirstName(ID, NewFirstName);
        }

        public bool ChangeUserLastName(string ID, string NewLastName)
        {
            return UsersList.ChangeLastName(ID, NewLastName);
        }

        public bool ChangeUserGroup(string ID, string NewGroup)
        {
            return UsersList.ChangeGroup(ID, NewGroup);
        }

        public bool ChangeUserDepartment(string ID, string NewDept)
        {
            return UsersList.ChangeDepartment(ID, NewDept);
        }

        public bool ChangeUserEmailAddress(string ID, string NewEmailAddress)
        {
            return UsersList.ChangeEmail(ID, NewEmailAddress);
        }

        public bool ChangeUserPhoneNumber(string ID, string NewPhoneNumber)
        {
            return UsersList.ChangePhoneNum(ID, NewPhoneNumber);
        }

        public bool ChangeUserType(string ID, userType NewType)
        {
            return UsersList.ChangeType(ID, NewType);
        }

        public bool ChangeUserLimitedUses(string ID, int uses)
        {
            return UsersList.ChangeLimitedUsesCount(ID, uses);
        }

        public bool ChangeUserTimeframestart(string ID, DateTime start)
        {
            return UsersList.ChangeAccessTimeFrameStart(ID, start);
        }

        public bool ChangeUserTimeframeend(string ID, DateTime end)
        {
            return UsersList.ChangeAccessTimeFrameEnd(ID, end);
        }

        public bool ChangeUserTimeAccessType(string ID, accessTimeType type)
        {
            return UsersList.ChangeAccessType(ID, type);
        }

        public User GetUser(string ID)
        {
            return UsersList.GetUser(ID);
        }

        public void SaveFile()
        {
            //throws if fails
            UsersList.SerializeToXmlFile(xmlFileName);
        }

        public String DisplayUsers()
        {
            return UsersList.DisplayUsers();
        }
    }
}
