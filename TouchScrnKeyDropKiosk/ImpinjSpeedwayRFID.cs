using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Impinj.OctaneSdk;
using System.Threading;

namespace ImpinjRFID
{
    class ImpinjSpeedwayRFID
    {
        public ImpinjReader Reader { get; private set; }
        public Status status { get; private set; }
        bool AntennaHubPresent;
        public bool IsReading { get; private set; }
        TagReport CurrentTagReport;

        public ImpinjSpeedwayRFID(string readerName)
        {
            Reader = new ImpinjReader();

            // Connect to the reader. The name is the host name
            // or IP address.
            Console.WriteLine("Connecting...");
            Reader.ConnectTimeout = 60000;

            int connectioncount = 0;
            while (true)
            {
                try
                {
                    Reader.Connect(readerName);
                }
                catch (Exception ex)
                {
                    connectioncount++;
                }
                if (Reader.IsConnected)
                    break;
                if (connectioncount == 5)
                    throw new OctaneSdkException("Impinj Reader Connection Timed Out " + connectioncount + " Times");                        
            }
            status = Reader.QueryStatus();
            AntennaHubPresent = false;
            IsReading = false;
            foreach (AntennaHubStatus hubStatus in status.AntennaHubs)
            {
                if (hubStatus.Connected == HubConnectedStatus.Connected)
                    AntennaHubPresent = true;
            }

            Reader.TagsReported += new ImpinjReader.TagsReportedHandler(Reader_TagsReported);
            CurrentTagReport = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsRead">How many milliseconds do you want to read for?</param>
        /// <param name="Antenna1Enable"></param>
        /// <param name="Antenna1Power">0 -> 30.00 dBM</param>
        /// <param name="Antenna2Enable"></param>
        /// <param name="Antenna2Power">0 -> 30.00 dBM</param>
        /// <param name="Antenna3Enable"></param>
        /// <param name="Antenna3Power">0 -> 30.00 dBM</param>
        /// <param name="Antenna4Enable"></param>
        /// <param name="Antenna4Power">0 -> 30.00 dBM</param>
        /// <returns>Return value can be null if the Tagreport is not returned from the report thread</returns>
        public TagReport ReadTags(int millisecondsRead, bool Antenna1Enable, double Antenna1Power, bool Antenna2Enable, double Antenna2Power,
            bool Antenna3Enable, double Antenna3Power, bool Antenna4Enable, double Antenna4Power)
        {
            IsReading = true;
            // Query the default settings for the reader. Settings
            // include which antennas are enabled, when to report and
            // optional report fields. Typically you will prepare the
            // reader by getting the default settings and adjusting them.
            //
            // In this example we make sure the reader holds
            // tag reports until we query for them, and also
            // include which antenna the tag was seen on.
            Settings settings = Reader.QueryDefaultSettings();

            //settings.Gpis.GetGpi(1).IsEnabled = true;

            settings.Report.IncludeAntennaPortNumber = true;
            settings.Report.Mode = ReportMode.BatchAfterStop;
            //GEN2 Settings
            settings.ReaderMode = ReaderMode.Hybrid;   //This should send out the call for response 8 times with each inventory session, thus making more certain the tags hear it.
            settings.SearchMode = SearchMode.SingleTarget;        //Dual Target will continuously respond to tags as they are available. Single Target will respond
            settings.Session = 1;                             //to tags based upon the session number (anywhere from immediate, to a 5 sec delay). Tag Focus
            //will only respond to a tag once and then not again until it has been powered down
            //Antennae Power                                  //for at least 20 seconds.
            if (!AntennaHubPresent)
            {
                settings.Antennas.GetAntenna(1).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(1).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(2).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(2).TxPowerInDbm = Antenna2Power;

                if (settings.Antennas.Length > 2)
                {
                    settings.Antennas.GetAntenna(3).IsEnabled = Antenna3Enable;
                    settings.Antennas.GetAntenna(3).TxPowerInDbm = Antenna3Power;
                }

                if (settings.Antennas.Length > 3)
                {
                    settings.Antennas.GetAntenna(4).IsEnabled = Antenna4Enable;
                    settings.Antennas.GetAntenna(4).TxPowerInDbm = Antenna4Power;
                }
            }
            else
            {
                #region Antenna ports 1-8
                settings.Antennas.GetAntenna(1).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(1).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(2).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(2).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(3).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(3).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(4).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(4).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(5).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(5).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(6).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(6).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(7).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(7).TxPowerInDbm = Antenna1Power;

                settings.Antennas.GetAntenna(8).IsEnabled = Antenna1Enable;
                settings.Antennas.GetAntenna(8).TxPowerInDbm = Antenna1Power;
                #endregion

                #region Antenna ports 9-16
                settings.Antennas.GetAntenna(9).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(9).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(10).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(10).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(11).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(11).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(12).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(12).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(13).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(13).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(14).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(14).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(15).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(15).TxPowerInDbm = Antenna2Power;

                settings.Antennas.GetAntenna(16).IsEnabled = Antenna2Enable;
                settings.Antennas.GetAntenna(16).TxPowerInDbm = Antenna2Power;
                #endregion

                #region Antenna ports 17-24
                settings.Antennas.GetAntenna(17).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(17).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(18).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(18).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(19).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(19).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(20).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(20).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(21).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(21).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(22).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(22).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(23).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(23).TxPowerInDbm = Antenna3Power;

                settings.Antennas.GetAntenna(24).IsEnabled = Antenna3Enable;
                settings.Antennas.GetAntenna(24).TxPowerInDbm = Antenna3Power;
                #endregion

                #region Antenna ports 25-32
                settings.Antennas.GetAntenna(25).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(25).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(26).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(26).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(27).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(27).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(28).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(28).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(29).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(29).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(30).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(30).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(31).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(31).TxPowerInDbm = Antenna4Power;

                settings.Antennas.GetAntenna(32).IsEnabled = Antenna4Enable;
                settings.Antennas.GetAntenna(32).TxPowerInDbm = Antenna4Power;
                #endregion
            }

            // Order the reader to change its settings to these.
            Reader.ApplySettings(settings);

            // Start reading.
            Reader.Start();

            Thread.Sleep(millisecondsRead);

            Reader.Stop();

            for (int i = 0; i < 10; i++)
            {
                if (CurrentTagReport != null)
                    break;
                else
                    Thread.Sleep(200);
            }
            return CurrentTagReport;
        }

        public void DoneReading()
        {
            IsReading = false;
        }

        public void Disconnect()
        {
            IsReading = false;
            Reader.Disconnect();
        }

        void Reader_TagsReported(ImpinjReader reader, TagReport report)
        {
            // This event handler is called asynchronously 
            // when tag reports are available.
            CurrentTagReport = report;
        }
    }
}
