using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using DPUruNet;

namespace KeyCabinetKiosk
{
    public class BiometricData
    {
        [XmlIgnore()]
        internal List<Fmd> FMDs;

        private List<string> FIDs;

        [XmlElementAttribute()]
        public string ID { get; set; }            // User ID associated with fingerprint - unique ID for each person

        [XmlArrayItem("Fid", typeof(string))]
        public List<string> FIDList        //xml interface for serialization
        {
            get { return FIDs; }       // password number as string"
            set { FIDs = value; }
        }

        /// <summary>
        ///  KeyPassword - no parameter constructor needed for streaming object
        /// </summary>
        public BiometricData()
        {
            FMDs = new List<Fmd>(); FIDList = new List<string>();
        }

        public BiometricData(string id)
        {
            ID = id; FMDs = new List<Fmd>(); FIDList = new List<string>();
        }

        internal BiometricData(string id, Fmd initialdata)
        {
            ID = id; FMDs = new List<Fmd>(); FIDList = new List<string>(); FMDs.Add(initialdata); FIDList.Add(Fmd.SerializeXml(initialdata));
        }

        internal void FillFMDs()
        {
            foreach (string s in FIDs)
            {
                FMDs.Add(Fmd.DeserializeXml(s));
            }
        }
    }  // end class
    
    public class BiometricDataList
    {
        private List<BiometricData> biometricdata;

        [XmlArrayItem("BiometricData", typeof(BiometricData))]
        public List<BiometricData> biometricDataList        //xml interface for serialization
        {
            get { return biometricdata; }
            set { biometricdata = value; }
        }

        public BiometricDataList()
        {
            biometricDataList = new List<BiometricData>();
        }

        /// <summary>
        /// AddKeyPassword - add a new key password location object
        ///                - not used by application - only test harness
        /// </summary>
        /// <param name="keypassword"></param>
        public void AddBiometricData(BiometricData data)
        {
            biometricdata.Add(data);
        }

        /// <summary>
        /// SerializeToXmlFile - stream the biometric data list to the XML file
        /// </summary>
        /// <param name="xmlfile"></param>
        public void SerializeToXmlFile(string xmlfile)
        {
            TextWriter tr = new StreamWriter(xmlfile);
            XmlSerializer sr = new XmlSerializer(typeof(BiometricDataList));

            sr.Serialize(tr, this);

            tr.Close();
        }

        /// <summary>
        /// FindFingerprintsbyID - finds and returns the list of FMD data for a given ID
        ///           - returns null if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public List<Fmd> FindFingerprintsbyID(string id)
        {
            foreach (BiometricData bd in biometricDataList)
            {
                if (bd.ID == id)
                {
                    return bd.FMDs;
                }
            }
            return null;
        }

        /// <summary>
        /// FindIDbyFingerprints - finds and returns the ids which contain the FMD data
        ///           - returns null if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public List<string> FindIDbyFingerprints(Fmd finger)
        {
            List<string> list = new List<string>();
            foreach (BiometricData bd in biometricDataList)
            {
                foreach (Fmd f in bd.FMDs)
                {
                    if (f == finger)
                    {
                        list.Add(bd.ID);
                        break;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// ClearFingerprintsbyID - finds and clears the list of FMD data for a given ID
        ///           - returns false if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public bool ClearFingerprintsbyID(string id)
        {
            foreach (BiometricData bd in biometricDataList)
            {
                if (bd.ID == id)
                {
                    bd.FIDList.Clear();
                    bd.FMDs.Clear();
                    biometricDataList.Remove(bd);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ClearFingerprintsbyID - finds and clears the list of FMD data for a given ID
        ///           - returns false if not found
        /// </summary>
        /// <param name="lc"></param>
        /// <returns></returns>
        public bool AddFingerprintbyID(string id, Fmd data)
        {
            foreach (BiometricData bd in biometricDataList)
            {
                if (bd.ID == id)
                {
                    bd.FIDList.Add(Fmd.SerializeXml(data));
                    bd.FMDs.Add(data);
                    return true;
                }
            }
            AddBiometricData(new BiometricData(id, data));
            return false;
        }  
    }
}
