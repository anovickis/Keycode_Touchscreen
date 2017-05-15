using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThingMagic;
using System.IO.Ports;

namespace ThingMagicRFIDM6eMicro
{
    /// <summary>
    /// AUTHOR: Adam Bertsch
    /// DATE:   1/4/2013
    /// PURPOSE:
    /// This module controls the ThinkMagic M6eMicro RFID Reader. It uses the same Mercury API that is used for the M6 and M6e models. It is designed to be used
    /// to read tags for a given amount of time and then returns a list of the tags which were read. The ReadTime should be preferrably less than a minute because 
    /// the reader can start to have heat issues after that. The heat issues can be mitigated by playing with the ASYNC_ON and ASYNC_OFF times (The longer the 
    /// ASYNC_OFF time is the safer you are). When the unit overheats, it does throttle itself though by turning off momentarily until the temperature is brought back
    /// down within it's specified range. 
    /// 
    /// The reader can handle different baudrates between 9600 and 115200, but the faster is of course better for program speed.
    /// </summary>
    class M6eMicroRFID
    {
        SerialPort SerialPort;
        Reader BaseReader;
        SerialReader RFIDReader;
        public bool IsReading { get; private set; }
        public int READER_ASYNC_ON_TIME { get; private set; }
        public int READER_ASYNC_OFF_TIME { get; private set; }
        public int READER_READ_POWER { get; private set; }
        public int READER_READ_TIME { get; private set; }

        public M6eMicroRFID(): this("COM1", 115200, 500, 100, 3000, 10000) { }

        public M6eMicroRFID(string ComPortName) : this(ComPortName, 115200, 500, 100, 3000, 10000) { }

        public M6eMicroRFID(string ComPortName, int ReadOnTime, int ReadOffTime) : this(ComPortName, 115200, ReadOnTime, ReadOffTime, 3000, 10000) { }

        public M6eMicroRFID(string ComPortName, int ReadOnTime, int ReadOffTime, int ReadTime) : this(ComPortName, 115200, ReadOnTime, ReadOffTime, 3000, ReadTime) { }

        /// <summary>
        /// Full access contructor
        /// </summary>
        /// <param name="ComPortName"></param>
        /// <param name="ComBaudrate">Should be 115200. Other rates supported and listed in Mercury Docs</param>
        /// <param name="ReadOnTime">How long in milliseconds the reader is on before being turned off for ReadOffTime</param>
        /// <param name="ReadOffTime">How long in milliseconds the reader is off before being turned back on for ReadOnTime</param>
        /// <param name="ReadPower">Power in centi-dBm sent to the antennae. The max value is 3000 and should probably always be set to max</param>
        /// <param name="ReadTime">Total time the reader reads for when ReadTags() is called in milliseconds. If this value is more than 
        ///                        60000 you begin to risk throttling due to overheating</param>
        public M6eMicroRFID(string ComPortName, int ComBaudrate, int ReadOnTime, int ReadOffTime, int ReadPower, int ReadTime)
        {
            SerialPort = new SerialPort(ComPortName, ComBaudrate);
            BaseReader = Reader.Create("eapi:///" + SerialPort.PortName);
            RFIDReader = (SerialReader)BaseReader;
            IsReading = false;

            BaseReader.Connect();

            READER_ASYNC_ON_TIME = ReadOnTime;
            READER_ASYNC_OFF_TIME = ReadOffTime;
            READER_READ_POWER = ReadPower;
            READER_READ_TIME = ReadTime;

            RFIDReader.ParamSet("/reader/region/id", Reader.Region.OPEN); //Open Region
            RFIDReader.ParamSet("/reader/gen2/q", new Gen2.DynamicQ()); //This determines how many internal tag "slots" the reader is looking to fill
            RFIDReader.ParamSet("/reader/gen2/BLF", Gen2.LinkFrequency.LINK250KHZ); //Rate at which the tags signal back to the reader
            RFIDReader.ParamSet("/reader/gen2/tari", Gen2.Tari.TARI_6_25US); //Controls how long it takes a tag to reader to send a communication
            RFIDReader.ParamSet("/reader/gen2/session", Gen2.Session.S1); //Controls how often tags respond to inventory rounds
            RFIDReader.ParamSet("/reader/gen2/tagEncoding", Gen2.TagEncoding.M8); //Controls how many times a signal is repeated so that far away tags still get the message.
            RFIDReader.ParamSet("/reader/gen2/target", Gen2.Target.A); //This controls what order tags are read in. State A are tags which have not been read. State B are tags which have been read.
            RFIDReader.ParamSet("/reader/radio/readPower", READER_READ_POWER);
            RFIDReader.ParamSet("/reader/read/asyncOnTime", READER_ASYNC_ON_TIME);
            RFIDReader.ParamSet("/reader/read/asyncOffTime", READER_ASYNC_OFF_TIME);
            RFIDReader.ParamSet("/reader/read/plan", new SimpleReadPlan(new int[] { 1, 2, 3, 4 }, TagProtocol.GEN2));
        }

        public TagReadData[] ReadTags()
        {
            IsReading = true;
            return RFIDReader.Read(READER_READ_TIME);
        }

        public TagReadData[] ReadTags(int ReadTime)
        {
            IsReading = true;
            return RFIDReader.Read(ReadTime);
        }

        /// <summary>
        /// Call this method once you are done with the reader. This is not put into ReadTags so that if you are scanning in another thread
        /// and you are processing info between reads another thread can't come in and think you are done just because you are between reads.
        /// </summary>
        public void DoneReading()
        {
            IsReading = false;
        }

        public void Destroy()
        {
            IsReading = false;
            BaseReader.Destroy();
        }
    }
}
