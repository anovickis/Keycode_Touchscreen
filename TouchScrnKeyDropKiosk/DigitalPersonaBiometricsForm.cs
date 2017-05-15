using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DPUruNet;
using System.Threading;

namespace KeyCabinetKiosk
{
    public enum DPBiometricsState
    {
        Error, Enrolling, Verification, Identification
    }

    partial class DigitalPersonaBiometricsForm : baseForm
    {
        ReaderCollection AvailableReaders;
        DPBiometricsState ScanProcess;
        Reader Reader;
        BiometricDataManager BioDataManager;
        string SaveFileName;
        string UserID;
        int Timeout;
        private const int PROBABILITY_ONE = 0x7fffffff;
        public bool Cancelled { get; private set; }
        public bool Success { get; private set; }
        public List<string> IdentificationIDs { get; private set; }

        public DigitalPersonaBiometricsForm(string Savefile, DPBiometricsState scanprocess, string userid, int timeout, BiometricDataManager biodatamanager)
        {
            InitializeComponent();
            buttonScan.Text = LanguageTranslation.SCAN;
            buttonCancel.Text = LanguageTranslation.CANCEL;

            SaveFileName = Savefile;
            Cancelled = false;
            Success = false;
            UserID = userid;
            Timeout = timeout;
            BioDataManager = biodatamanager;
            IdentificationIDs = new List<string>();
            AvailableReaders = ReaderCollection.GetReaders();
            ScanProcess = scanprocess;
            if (AvailableReaders.Count > 0)
            {
                Reader = AvailableReaders[0];
            }
            else
            {
                throw new Exception("No Biometric Readers\r\nAvailable");
            }
            SetLabelText(labelMessage, LanguageTranslation.SCAN_FINGERPRINT + ScanProcess.ToString());
        }

        public bool EnrollFingerprints(string Userid, int Timeout, BiometricDataManager BioDataManager)
        {
            List<Fmd> PreenrollmentFmds = new List<Fmd>();
            if (Reader.Open(Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE) != Constants.ResultCode.DP_SUCCESS)
            {
                Reader.Dispose();
                throw new Exception("Biometric Scanner Failed\r\nto Open for\r\nEnrollment");
            }

            GetStatus();

            DataResult<Fmd> resultEnrollment; int enrollmentcount = 0;
            SetLabelText(labelMessage, LanguageTranslation.NEW_SCAN_FINGERPRINT);
            do
            {
                CaptureResult Result = Reader.Capture(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, Timeout, Reader.Capabilities.Resolutions.First<int>());
                if (!CheckCaptureResult(Result))
                {
                    Reader.Dispose();
                    return false;
                }
                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(Result.Data, Constants.Formats.Fmd.ANSI);
                PreenrollmentFmds.Add(resultConversion.Data);

                resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, PreenrollmentFmds);
                enrollmentcount++;

                SetLabelText(labelMessage, LanguageTranslation.AGAIN_SCAN_FINGERPRINT + enrollmentcount);
            } while ((resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_NOT_READY) && (enrollmentcount <= 6));

            if (resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_INVALID_SET)
            {
                Reader.Dispose();
                throw new Exception("Enrollment was unsuccessful.\r\nPlease try again.");
            }
            else if (resultEnrollment.ResultCode == Constants.ResultCode.DP_ENROLLMENT_NOT_READY)
            {
                Reader.Dispose();
                throw new Exception("Enrollment did not have\r\nsufficiently good scans.");
            }

            List<string> Prints = BioDataManager.FindIDByFingerprint(resultEnrollment.Data);
            if (Prints.Count > 0)
            {
                YesNoForm clearprints = new YesNoForm(LanguageTranslation.CLEAR_FINGERPRINT_QUESTION);
                clearprints.ShowDialog();

                if (clearprints.YesResult)
                {
                    foreach (string s in Prints)
                    {
                        BioDataManager.ClearFingerprint(s);
                    }
                    Program.ShowErrorMessage(LanguageTranslation.OLD_PRINTS_CLEARED, 3000);
                }
            }

            SetLabelText(labelMessage, LanguageTranslation.SAVING_FINGERPRINTS);
            BioDataManager.AddFingerprint(Userid, resultEnrollment.Data);
            BioDataManager.SaveFile();
            
            PreenrollmentFmds.Clear();
            Reader.Dispose();
            Thread.Sleep(2000);    
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Userid"></param>
        /// <param name="Timeout"></param>
        /// <returns>Returns dissimilarity score. Higher numbers are less of a match. 0 is a perfect match. -1 is an error</returns>
        public int VerifyFingerprint(string Userid, int Timeout, BiometricDataManager BioDataManager)
        {
            if (Reader.Open(Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE) != Constants.ResultCode.DP_SUCCESS)
            {
                Reader.Dispose();
                throw new Exception("Biometric Scanner Failed\r\nto Open for Verification");
            }

            try
            {
                GetStatus();

                SetLabelText(labelMessage, LanguageTranslation.SCAN_ENROLLED_FINGERPRINT);
                CaptureResult Result = Reader.Capture(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, Timeout, Reader.Capabilities.Resolutions.First<int>());
                if (!CheckCaptureResult(Result))
                {
                    Reader.Dispose();
                    return -1;
                }
                else if (Result.Quality == Constants.CaptureQuality.DP_QUALITY_TIMED_OUT)
                {
                    Reader.Dispose();
                    return -2;
                }
                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(Result.Data, Constants.Formats.Fmd.ANSI);
                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    Reader.Dispose();
                    throw new Exception("Biometric Scanner Verify\r\nResultConversion Error: " + resultConversion.ResultCode.ToString());
                }

                CompareResult compareResult;
                foreach (Fmd data in BioDataManager.FindFingerprint(Userid))
                {
                    compareResult = Comparison.Compare(resultConversion.Data, 0, data, 0);
                    if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        Reader.Dispose();
                        throw new Exception("Biometric Scanner Verify\r\nResultCode Error: " + compareResult.ResultCode.ToString());
                    }

                    //Analyze the score of the fingerprint to see if it is good enough to call a match
                    if (AnalyzeFingerprintVerificationScore(compareResult.Score, Program.BIOMETRIC_FALSE_POSITIVE_RATIO))
                    {
                        Reader.Dispose();
                        return 0;
                    }
                }

                Reader.Dispose();
                return -3;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Reader != null)
                    Reader.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Score"></param>
        /// <param name="FalsePosRatio"></param>
        /// <returns>Returns true if Score is considered a match</returns>
        public bool AnalyzeFingerprintVerificationScore(int Score, int FalsePosRatio)
        {
            if (Score < (PROBABILITY_ONE / FalsePosRatio))
            {
                Program.logEvent("Fingerprints Match with dissimilarity score of " + Score.ToString());
                return true;
            }
            else
            {
                Program.logEvent("Fingerprints Do Not Match with dissimilarity score of " + Score.ToString());
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="biometricDataList">BiometricDataManager.Biometrics.biometricDataList</param>
        /// <returns>Number of matches. Returns -1 or throws an exception if there's an error</returns>
        public int IdentifyFingerprint(int Timeout, BiometricDataManager BioDataManager)
        {
            if (Reader.Open(Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE) != Constants.ResultCode.DP_SUCCESS)
            {
                Reader.Dispose();
                throw new Exception("Biometric Scanner Failed\r\nto Open for Identification");
            }

            try
            {
                GetStatus();

                SetLabelText(labelMessage, LanguageTranslation.PLACE_ENROLLED_FINGER);
                CaptureResult Result = Reader.Capture(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, Timeout, Reader.Capabilities.Resolutions.First<int>());
                if (!CheckCaptureResult(Result))
                {
                    Reader.Dispose();
                    return -1;
                }
                else if (Result.Quality == Constants.CaptureQuality.DP_QUALITY_TIMED_OUT)
                {
                    Reader.Dispose();
                    return -2;
                }
                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(Result.Data, Constants.Formats.Fmd.ANSI);
                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    Reader.Dispose();
                    throw new Exception("Biometric Scanner \r\nIdentify ResultConversion Error:\r\n" + resultConversion.ResultCode.ToString());
                }

                int NumberofMatches = 0;
                IdentifyResult identifyResult;
                foreach (BiometricData bd in BioDataManager.Biometrics.biometricDataList)
                {
                    if (bd.FMDs.Count <= 0) //if there are no fingerprints for the current user. 
                        continue;
                    identifyResult = Comparison.Identify(resultConversion.Data, 0, bd.FMDs, PROBABILITY_ONE / Program.BIOMETRIC_FALSE_POSITIVE_RATIO, 1000);
                    if (identifyResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        Reader.Dispose();
                        throw new Exception("Biometric Scanner\r\nIdentify ResultCode Error:\r\n" + identifyResult.ResultCode.ToString());
                    }
                    NumberofMatches += identifyResult.Indexes.Length;
                    if (identifyResult.Indexes.Length > 0)
                        IdentificationIDs.Add(bd.ID);   
                }

                Program.logEvent("Identification resulted in " + NumberofMatches + " matches");

                Reader.Dispose();
                return NumberofMatches;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Reader != null)
                    Reader.Dispose();
            }
        }

        /// <summary>
        /// Check quality of the resulting capture.
        /// </summary>
        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            if (captureResult.Data == null)
            {
                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    throw new Exception("Biometric Scanner ResultCode Error:\r\n" + captureResult.ResultCode.ToString());
                }

                // Send message if quality shows fake finger
                if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                {
                    throw new Exception("Biometric Scanner Quality -\r\n" + captureResult.Quality);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check the device status before starting capture.
        /// </summary>
        /// <returns></returns>
        public void GetStatus()
        {
            Constants.ResultCode result = Reader.GetStatus();

            if ((result != Constants.ResultCode.DP_SUCCESS))
            {
                throw new Exception("Biometric Reader Result Error:\r\n" + result);
            }

            if ((Reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
            {
                Thread.Sleep(100);
            }
            else if ((Reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                Reader.Calibrate();
            }
            else if ((Reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Biometric Reader Status -\r\n" + Reader.Status.Status);
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            buttonCancel.Visible = false;
            buttonScan.Visible = false;
            try
            {
                switch (ScanProcess)
                {
                    case DPBiometricsState.Enrolling:
                    {
                        if (!EnrollFingerprints(UserID, Timeout, BioDataManager))
                            Cancelled = true;
                        break;
                    }
                    case DPBiometricsState.Identification:
                    {
                        int score = IdentifyFingerprint(Timeout, BioDataManager);
                        //Timed Out
                        if (score == -2)
                        {
                            Program.ShowErrorMessage(LanguageTranslation.SCAN_TIMED_OUT, 3000);
                            Program.logEvent("Scan Timed Out");
                            TimedOut = true;
                            return;
                        }

                        if (score > 0)
                        {
                            SetLabelText(labelMessage, LanguageTranslation.FINGERPRINT_MATCHED);
                            Thread.Sleep(3000);
                            Program.logEvent("Fingerprint Matched");
                            Success = true;
                            return;
                        }
                        else
                        {
                            SetLabelText(labelMessage, LanguageTranslation.FINGERPRINT_FAILED);
                            Thread.Sleep(3000);
                            Program.logEvent("Fingerprint Match Failed");
                            return;
                        }
                    }
                    case DPBiometricsState.Verification:
                    {
                        int score = VerifyFingerprint(UserID, Timeout, BioDataManager);

                        //Timed Out
                        if (score == -2)
                        {
                            Program.ShowErrorMessage(LanguageTranslation.SCAN_TIMED_OUT, 3000);
                            Program.logEvent("Scan Timed Out");
                            TimedOut = true;
                            return;
                        }
                                                
                        if (score == 0)
                        {
                            SetLabelText(labelMessage, LanguageTranslation.FINGERPRINT_MATCHED);
                            Thread.Sleep(3000);
                            Program.logEvent("Fingerprint Matched");
                            Success = true;
                            return;
                        }
                        else
                        {
                            SetLabelText(labelMessage, LanguageTranslation.FINGERPRINT_FAILED);
                            Thread.Sleep(3000);
                            Program.logEvent("Fingerprint Match Failed");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.ShowErrorMessage(ex.Message, 5000);
                Program.logEvent("Biometric Error: " + ex.Message);
            }
            finally
            {
                buttonCancel.Visible = true;
                buttonScan.Visible = true;
                SetLabelText(labelMessage, LanguageTranslation.SCAN_FINGERPRINT + ScanProcess.ToString());
                this.Update();
                this.Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void SetLabelText(Label label, string text)
        {
            label.Text = text;
            HorizontalCenterLabel(label);
            VerticalCenterLabel(label);
            label.Update();
        }

        private void HorizontalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(((label.Parent.Width - label.Width) / 2), label.Location.Y);
        }

        private void VerticalCenterLabel(System.Windows.Forms.Label label)
        {
            label.Location = new System.Drawing.Point(label.Location.X, ((panel1.Height - label.Height) / 2));
        }
    }
}
