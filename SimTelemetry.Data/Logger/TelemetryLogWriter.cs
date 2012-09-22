using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triton.Database;
using Timer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using ElapsedEventHandler = System.Timers.ElapsedEventHandler;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Logger
{
    public class TelemetryLogWriter
    {
        private Dictionary<string, TelemetryLoggerSubscribedInstance> Instances = new Dictionary<string, TelemetryLoggerSubscribedInstance>();
        private ushort InstanceID = 1;
        private double AnnotationStartTime = 0;
        private string AnnotationFile = ""; // File to dump in.
        private string AnnotationFileCompress = ""; // File to compress after annotation.
        private StreamWriter _mWrite;
        private byte[] Header = new byte[0]; // Header per annotation file.
        private DateTime AnnotationStart = DateTime.Now;
        private ManualResetEvent AnnotationWaiter = new ManualResetEvent(false);

        private Timer _Worker;
        public bool Active { get; protected set; }

        // Variable containing events that were fired since last log.
        // These are saved next Tick()
        private List<string> EventsFired = new List<string>();

        public void Subscribe<T>(string name, object instance)
        {
            if (!Active && !Instances.ContainsKey(name))
            {
                Instances.Add(name, new TelemetryLoggerSubscribedInstance(typeof(T), instance));
                Instances[name].ID = InstanceID;
                InstanceID++;

            }

        }

        private void Compress()
        {
            try
            {
                // Create the compressed file.
                using (FileStream DatFile = File.OpenRead(AnnotationFileCompress))
                using (FileStream GzFile = File.Create(AnnotationFileCompress.Replace(".dat", ".gz")))
                using (GZipStream Compress = new GZipStream(GzFile, CompressionMode.Compress))
                {
                    // Compress this data:
                    DatFile.CopyTo(Compress);

                    // Done.
                }

                // Delete uncompressed
                File.Delete(AnnotationFileCompress);

                // Get the PREVIOUS last lap:
                // Insert via task; sleep 1.5second for timing to appear in game

                ThreadPool.QueueUserWorkItem(new WaitCallback(InsertLapTime));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to compress file " + AnnotationFileCompress);
            }
        }

        private void InsertLapTime(object o)
        {
            System.Threading.Thread.Sleep(1500);
            List<ILap> AllLaps = Telemetry.m.Sim.Drivers.Player.GetLapTimes();
            if (AllLaps.Count > 2)
            {
                ILap LastLap = AllLaps[AllLaps.Count - 2];

                // Insert into log
                OleDbConnection con =
                    DatabaseOleDbConnectionPool.GetOleDbConnection();
                using (
                    OleDbCommand newTime =
                        new OleDbCommand(
                            "INSERT INTO laptimes (simulator,circuit,series,car,laptime,s1,s2,s3,driven,lapno,filepath) " +
                            "VALUES ('" + Telemetry.m.Sim.ProcessName + "','" +
                            Telemetry.m.Track.Name + "','" +
                            Telemetry.m.Sim.Drivers.Player.CarModel + "','" +
                            Telemetry.m.Sim.Drivers.Player.CarClass + "'," +
                            LastLap.LapTime + "," + LastLap.Sector1 + "," +
                            LastLap.Sector2 + "," + LastLap.Sector3 + ",NOW(), " +
                            (Telemetry.m.Sim.Drivers.Player.Laps).ToString() +
                            ",'" + AnnotationFileCompress.Replace(".dat", ".gz") +
                            "')", con))
                {
                    newTime.ExecuteNonQuery();
                }
                DatabaseOleDbConnectionPool.Freeup();
            }
        }

        /**
         * Start logging in a new file
         * If not yet started, start logging in this file.
         */
        public void Annotate(string name)
        {
            if (_mWrite != null)
            {
                AnnotationFileCompress = AnnotationFile;
                AnnotationWaiter.WaitOne();
                lock (_mWrite)
                {
                    _mWrite.Close();
                }

                // Start compressing.
                new Task(() => { Compress(); }).Start();
            }

            _mWrite = new StreamWriter(name);
            // Write the header
            _mWrite.BaseStream.Write(Header, 0, Header.Length);
            _mWrite.Flush();

            AnnotationStart = DateTime.Now;
            AnnotationStartTime = Telemetry.m.Sim.Session.Time;
            AnnotationFile = name;
            // Yay


        }

        /* DEPRECATED
        public void Start()
        {
            if (Active == false)
            {
                Active = true;


                _Worker = new Timer();
                _Worker.Elapsed += new ElapsedEventHandler(_Worker_Elapsed);
                _Worker.Interval = 2; // 100Hz
                _Worker.AutoReset = true;
                _Worker.Start();
            }

        }*/

        public void Start(string file)
        {
            if (Active == false)
            {
                Active = true;

                BuildHeader();
                Annotate(file);

                _Worker = new Timer();
                _Worker.Elapsed += new ElapsedEventHandler(_Worker_Elapsed);
                _Worker.Interval = 2; // 100Hz
                _Worker.AutoReset = true;
                _Worker.Start();
            }

        }

        private void BuildHeader()
        {
            List<byte> tmpheader = new List<byte>();

            foreach (KeyValuePair<string, TelemetryLoggerSubscribedInstance> tel_instance in Instances)
            {
                byte[] data = new byte[64];
                ByteMethods.memcpy(data, ASCIIEncoding.ASCII.GetBytes(tel_instance.Key), 64, 0, 0);

                byte[] header = new byte[10];
                header[0] = (byte)'$';
                header[1] = (byte)'#';                                                                      // Sync
                ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.InstanceName), 2, 4, 0); // Packet ID
                ByteMethods.memcpy(header, BitConverter.GetBytes(tel_instance.Value.ID), 2, 6, 0);          // Instance ID
                ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)data.Length), 2, 8, 0);            // Data length

                tmpheader.AddRange(header);
                tmpheader.AddRange(data);

                ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.InstanceMember), 2, 4, 0); // Packet ID
                ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)68), 2, 8, 0);            // Data length
                foreach (KeyValuePair<string, int> properyMap in tel_instance.Value.Mapping)
                {
                    data = new byte[68];
                    short type = 0xFF;
                    Type t = tel_instance.Value.PropertyDescriptors.Find(properyMap.Key, true).PropertyType;
                    if (t == typeof(double)) type = 0;
                    if (t == typeof(float)) type = 1;
                    if (t == typeof(Int32)) type = 2;
                    if (t == typeof(Int16)) type = 3;
                    if (t == typeof(byte)) type = 4;
                    if (t == typeof(UInt32)) type = 5;
                    if (t == typeof(UInt16)) type = 6;
                    if (t == typeof(char)) type = 7;
                    if (t == typeof(string)) type = 8;
                    if (t == typeof(bool)) type = 9;
                    if (type < 0xFF)
                    {
                        ByteMethods.memcpy(data, BitConverter.GetBytes(type), 2, 0, 0);
                        ByteMethods.memcpy(data, BitConverter.GetBytes(properyMap.Value), 2, 2, 0);
                        ByteMethods.memcpy(data, ASCIIEncoding.ASCII.GetBytes(properyMap.Key), 64, 4, 0);

                        tmpheader.AddRange(header);
                        tmpheader.AddRange(data);
                    }
                }
            }

            Header = tmpheader.ToArray();
        }

        public void Stop()
        {
            if (Active == true)
            {
                Active = false;
                _Worker.Stop();
                _Worker = null;
                Instances = new Dictionary<string, TelemetryLoggerSubscribedInstance>();
                InstanceID = 1;
            }

        }

        void _Worker_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_mWrite != null)
            {
                lock (_mWrite)
                {

                    try
                    {
                        /************** TIME SYNC *************/

                        double dt_ms = 0;
                        if (Telemetry.m.Sim.Modules.Time_Available)
                        {
                            dt_ms = Telemetry.m.Sim.Session.Time - AnnotationStartTime;
                            dt_ms *= 1000.0;
                        }
                        else
                        {
                            TimeSpan dt = DateTime.Now.Subtract(AnnotationStart);
                            dt_ms = dt.TotalMilliseconds;
                        }
                        if (dt_ms > 0)
                        {

                            byte[] data = new byte[8];
                            byte[] header = new byte[10];
                            header[0] = (byte)'$';
                            header[1] = (byte)'#'; // Sync
                            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.Time), 2, 4, 0);
                            // Packet ID
                            ByteMethods.memcpy(header, BitConverter.GetBytes(0), 2, 6, 0); // Instance ID
                            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)8), 2, 8, 0); // Data length

                            data = BitConverter.GetBytes(dt_ms);
                            _mWrite.BaseStream.Write(header, 0, header.Length);
                            _mWrite.BaseStream.Write(data, 0, data.Length);


                            /************** DATA *************/
                            lock (EventsFired)
                            {
                                foreach (KeyValuePair<string, TelemetryLoggerSubscribedInstance> tel_instance in Instances)
                                {
                                    data = tel_instance.Value.Dump(EventsFired);
                                    if (data.Length > 0)
                                    {

                                        // Write a packet to the stream.
                                        ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.Data), 2, 4,
                                                           0);
                                        // Packet ID
                                        ByteMethods.memcpy(header, BitConverter.GetBytes(tel_instance.Value.ID), 2, 6, 0);
                                        // Instance ID
                                        ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)data.Length), 2, 8, 0);
                                        // Data length


                                        _mWrite.BaseStream.Write(header, 0, header.Length);
                                        _mWrite.BaseStream.Write(data, 0, data.Length);
                                    }
                                }

                                // Reset the events fired.
                                EventsFired = new List<string>();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                AnnotationWaiter.Set();
            }
        }
    }

}