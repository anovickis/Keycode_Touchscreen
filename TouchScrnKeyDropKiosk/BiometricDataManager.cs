using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using DPUruNet;
using System.Threading;

namespace KeyCabinetKiosk
{
    class BiometricDataManager
    {
        /// <summary>
        ///  The BiometricDataManager is the interface to the User ID and fingerprint data.
        ///         this class will throw exceptions 
        /// </summary>    
        public BiometricDataList Biometrics{get; set;}
        private string xmlFileName;

        /// <summary>
        /// KeyPasswordManager - constructor - reads the key password file into a key password list 
        ///                       that is use for all key code and location look ups
        /// </summary>
        /// <param name="fileName"></param>
        public BiometricDataManager(string fileName)
        {
            if(String.IsNullOrEmpty(fileName))
            {
                throw new Exception("bad file name for BiometricData xml file");
            }
            xmlFileName = fileName;
            LoadFromFile();
        }

        /// <summary>
        /// LoadFromFile- reads the "filename" xml file into a list structure - the keyPassWordList
        /// </summary>
        private void LoadFromFile()
        {
            try
            {
                LoadFromFile(xmlFileName);               
            }
            catch(Exception  ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// LoadFromFile- reads the "filename" xml file into a list structure - the keyPassWordList
        /// </summary>
        internal void LoadFromFile(string filename)
        {   
            //read in xml data from file
            TextReader tr = new StreamReader(filename);
            try
            {
                Biometrics = new BiometricDataList();

                XmlSerializer sr = new XmlSerializer(typeof(BiometricDataList));

                Biometrics = (BiometricDataList)sr.Deserialize(tr);

                foreach (BiometricData bd in Biometrics.biometricDataList)
                {
                    bd.FillFMDs();
                }

                CheckDataValid();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                tr.Close();
            }
        }

        /// <summary>
        /// ReloadFromFile - reads in xml data file and re-creates the key password list.
        /// </summary>
        public void ReloadFromFile()
        {
            LoadFromFile();
        }

        /// <summary>
        ///  SaveFile - writes the Key Password list to the xml file
        /// </summary>
        public void SaveFile()
        {
            Thread.Sleep(500); //This is to give other processes time to let go of their handle on the file
            Biometrics.SerializeToXmlFile(xmlFileName);
        }
        
        /// <summary>
        /// CheckDataValid - do basic checks on the data to ensure that it is valid for operations
        ///                 required. Throw exception if not valid
        /// </summary>
        private void CheckDataValid()
        {         
            //Check if the user ID of each fingerprint set is Unique
            for(int i = 0; i < Biometrics.biometricDataList.Count; i++)
            {
                for (int j = i + 1; j < Biometrics.biometricDataList.Count - 1; j++)
                {
                    if (Biometrics.biometricDataList[i].ID == Biometrics.biometricDataList[j].ID)
                        throw new Exception("User ID " + Biometrics.biometricDataList[i].ID + " is not unique");
                }
            }              
        }
                
        /// <summary>
        ///  FindFingerprint  will search list for a match with an id and returns the list of FMDs
        ///     if id is not found a null object is returned
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public List<Fmd> FindFingerprint(string id)
        {
            return Biometrics.FindFingerprintsbyID(id);
        }

        public bool ClearFingerprint(string id)
        {
            return Biometrics.ClearFingerprintsbyID(id);
        }

        public bool AddFingerprint(string id, Fmd data)
        {
            return Biometrics.AddFingerprintbyID(id, data);
        }

        public List<string> FindIDByFingerprint(Fmd data)
        {
            return Biometrics.FindIDbyFingerprints(data);
        }
    }
}
