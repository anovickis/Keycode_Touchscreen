using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace KeyCabinetKiosk
{
    public enum userType
    {
        A_USER = 1,
        B_USER = 2,
        C_USER = 3,
        UNKNOWN = 4
    }

    public class User : IComparable<User>
    {
        

        [XmlElementAttribute()]
        public string ID;           //note ID is a string not a number - comparison is by character - this field is required to be unique

        [XmlElementAttribute()]
        public string LastName;     //may not be required

        [XmlElementAttribute()]
        public string FirstName;    //may not be required

        [XmlElementAttribute()]
        public string AccessCode;   // may not be required

        [XmlElementAttribute()]
        public string BoxNumber;   // may not be required

        [XmlElementAttribute()]
        public string Department;   // may not be required

        [XmlElementAttribute()]
        public string Group;        // may not be required

        [XmlElementAttribute()]
        public string emailAddress;        // may not be required

        [XmlElementAttribute()]
        public string phoneNumber;        // may not be required

        [XmlElementAttribute()]
        public int intType;            //may be unknown type  cast as useType enum

        [XmlElementAttribute()]
        public int accessTimeIntType { get; set; }    //time of access which is allowed (i.e. one-time use, always allowed, time frame allowed

        [XmlElementAttribute()]
        public int LimitedUseCount { get; set; }      // If Multi_TIME_USE selected, then

        [XmlElementAttribute()]
        public DateTime startAccessTimeframe { get; set; }  

        [XmlElementAttribute()]
        public DateTime endAccessTimeframe { get; set; }  

        public User() 
        { 
            ID = "666666";          // default ID should not be a legal user

            intType = 4;            // default unknown
        }

        public User(string id)
        {
            ID = id;
            intType = 4;            // no check here for uniqueness
            LastName = "";
            FirstName = "";
            AccessCode = "";
            BoxNumber = "";
            Department = "";
            Group = "";
            emailAddress = "";
            phoneNumber = ""; 
            accessTimeIntType = 1;
            LimitedUseCount = 0;
            startAccessTimeframe = DateTime.Today;
            endAccessTimeframe = DateTime.Today;
        }

        public User(string id, string lastname, string firstname, string email, string phone)
        {
            ID = id;
            LastName = lastname;
            FirstName = firstname;
            Department = "";
            Group = "";
            AccessCode = "";
            BoxNumber = "";
            emailAddress = email;
            phoneNumber = phone;
            intType = 4;
            accessTimeIntType = 1;
            LimitedUseCount = 0;
            startAccessTimeframe = DateTime.Today;
            endAccessTimeframe = DateTime.Today;
        }

        public User(string id, string lastname, string firstname, string department, string group, string email, string phone, int type, string access, string box,
                    string accesstype, int usecnt, string starttime, string endtime)
        {
            ID = id;
            LastName = lastname;
            FirstName = firstname;
            AccessCode = access;
            BoxNumber = box;
            Department = department;
            Group = group;
            emailAddress = email;
            phoneNumber = phone;
            intType = type;
            accessTimeIntType = int.Parse(accesstype);
            LimitedUseCount = usecnt;
            startAccessTimeframe = DateTime.Parse(starttime);
            endAccessTimeframe = DateTime.Parse(endtime);
        }

        public User(string id, string email, string phone, string accesstype, int usecnt, string starttime, string endtime)
        {
            ID = id;
            LastName = "";
            FirstName = "";
            AccessCode = "";
            BoxNumber = "";
            Department = "";
            Group = "";
            emailAddress = email;
            phoneNumber = phone;
            intType = 4;
            accessTimeIntType = int.Parse(accesstype);
            LimitedUseCount = usecnt;
            startAccessTimeframe = DateTime.Parse(starttime);
            endAccessTimeframe = DateTime.Parse(endtime);
        }
                
        public User(string id, string lastname, string firstname, string department, string group, string email, string phone)
        {
            ID = id;
            LastName = lastname;
            FirstName = firstname;
            AccessCode = "";
            BoxNumber = "";
            Department = department;
            Group = group;
            emailAddress = email;
            phoneNumber = phone;
            intType = 4;
            accessTimeIntType = 1;
            LimitedUseCount = 0;
            startAccessTimeframe = DateTime.Today;
            endAccessTimeframe = DateTime.Today;
        }

        public userType Type
        {
            get
            {
                return (userType)intType;
            }
            set
            {
                intType = (int)value;
            }
        }

        public accessTimeType AccessType
        {
            get
            {
                return (accessTimeType)accessTimeIntType;
            }
            set
            {
                accessTimeIntType = (int)value;
            }
        }

        public int CompareTo(User other)
        {
            return this.ID.CompareTo(other.ID);   //this is a string to string compare
        }
    }

    public class usersList
    {

        private List<User> users;

        [XmlArrayItem("User", typeof(User))]  
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        public usersList()
        {
            users = new List<User>();
        }

        public bool AddUser(User u)
        {
            if (UserIsInList(u.ID))
            {
                return false;
            }
            else
            {
                users.Add(u);
                return true;
            }
        }

        public bool RemoveUser(string id)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users.RemoveAt(index);
            return true;
        }

        public string FindIDByAccessCode(string Access)
        {
            foreach (User u in Users)
            {
                if (u.AccessCode == Access)
                {
                    return u.ID;
                }
            }
            return "";
        }

        public string FindFirstNameByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.FirstName;
                }
            }
            return "";
        }

        public string FindLastNameByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.LastName;
                }
            }
            return "";
        }

        public string FindAccessCodeByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.AccessCode;
                }
            }
            return "";
        }

        public string FindBoxNumberByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.BoxNumber;
                }
            }
            return "";
        }

        public string FindEmailByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.emailAddress;
                }
            }
            return "";
        }

        public string FindPhoneNumberByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.phoneNumber;
                }
            }
            return "";
        }

        public string FindGroupByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.Group;
                }
            }
            return "";
        }

        public string FindDepartmentByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.Department;
                }
            }
            return "";
        }

        public userType FindUserTypeByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.Type;
                }
            }
            return userType.UNKNOWN;
        }

        public accessTimeType FindUserAccessTimeTypeByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.AccessType;
                }
            }
            return accessTimeType.ALWAYS_ON;
        }

        public int FindUserLimitedUsesCountByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.LimitedUseCount;
                }
            }
            return -1;
        }

        public DateTime FindUserAccessTimeFrameStartByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.startAccessTimeframe;
                }
            }
            return new DateTime(1970, 1, 1);
        }

        public DateTime FindUserAccessTimeFrameEndByID(string id)
        {
            foreach (User u in Users)
            {
                if (u.ID == id)
                {
                    return u.endAccessTimeframe;
                }
            }
            return new DateTime(1970, 1, 1);
        }


        public bool ChangeID(string oldid, string newid)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == oldid);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].ID = newid;
            return true;
        }

        public bool ChangeFirstName(string id, string newfirstname)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].FirstName = newfirstname;
            return true;
        }

        public bool ChangeLastName(string id, string newlastname)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].LastName = newlastname;
            return true;
        }

        public bool ChangeAccessCode(string id, string newaccess)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].AccessCode = newaccess;
            return true;
        }

        public bool ChangeBoxNumber(string id, string newbox)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].BoxNumber = newbox;
            return true;
        }

        public bool ChangeEmail(string id, string newemail)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].emailAddress = newemail;
            return true;
        }

        public bool ChangePhoneNum(string id, string newphone)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].phoneNumber = newphone;
            return true;
        }

        public bool ChangeGroup(string id, string newgroup)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].Group = newgroup;
            return true;
        }

        public bool ChangeDepartment(string id, string newdepartment)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].Department = newdepartment;
            return true;
        }

        public bool ChangeType(string id, userType type)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].Type = type;
            return true;
        }

        public bool ChangeAccessType(string id, accessTimeType type)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].AccessType = type;
            return true;
        }

        public bool ChangeLimitedUsesCount(string id, int newcount)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].LimitedUseCount = newcount;
            return true;
        }

        public bool ChangeAccessTimeFrameStart(string id, DateTime starttime)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].startAccessTimeframe = starttime;
            return true;
        }

        public bool ChangeAccessTimeFrameEnd(string id, DateTime endtime)
        {
            int index = users.FindIndex(delegate(User u)
            {
                return (u.ID == id);
            });

            if (index == -1)
            {
                return false;
            }
            users[index].endAccessTimeframe = endtime;
            return true;
        }

        private bool UserIsInList(string id)
        {
            bool b = users.Exists(delegate(User u)
            {
                return (u.ID==id);
            });
            return b;
        }

        public bool IsUnique(string id)
        {
            return (!UserIsInList(id)); //if found not unique
        }

        public User GetUser(string id)
        {
            User user = Users.Find(delegate(User u)
            {
                return (u.ID == id);
            });

            return user;  //may be null
        }

        public void SerializeToXmlFile(string xmlfile)
        {
            TextWriter tr = new StreamWriter(xmlfile);
            XmlSerializer sr = new XmlSerializer(typeof(usersList));

            sr.Serialize(tr, this);

            tr.Close();

        }
        public string DisplayUsers()
        {

            StringBuilder sb = new StringBuilder();
            users.Sort(CompareUserLastNames);
            sb.AppendLine("USER ID, USER NAME, USER TYPE, USER EMAIL");
            foreach (User u in users)
            {
                string typeName;
                if (u.Type == userType.A_USER)
                {
                    typeName = "Admin";
                }
                else
                {
                    typeName = "Customer";
                }
                sb.AppendFormat("{0} {1} {2}, {3}, Email:{4}\r\n", u.ID, u.LastName, u.FirstName, typeName,u.emailAddress);
            }

            return sb.ToString();

        }

        private static int CompareUserLastNames(User x, User y)
        {
            return x.LastName.CompareTo(y.LastName);
        }

    }
}

