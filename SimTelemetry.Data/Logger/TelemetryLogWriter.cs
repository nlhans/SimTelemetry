using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
        private string AnnotationFile = ""; // File to dump in.
        private StreamWriter _mWrite;
        private byte[] Header = new byte[0]; // Header per annotation file.
        private DateTime AnnotationStart = DateTime.Now;
        private ManualResetEvent AnnotationWaiter = new ManualResetEvent(false);

        private Timer _Worker;
        public bool Active { get; protected set; }

        public void Subscribe<T>(string name, object instance)
        {
            if (!Active && !Instances.ContainsKey(name))
            {
                Instances.Add(name, new TelemetryLoggerSubscribedInstance(typeof(T), instance));
                Instances[name].ID = InstanceID;
                InstanceID++;

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
                AnnotationWaiter.WaitOne();
                lock (_mWrite)
                {
                    _mWrite.Close();
                }
            }

            _mWrite = new StreamWriter(name);
            // Write the header
            _mWrite.BaseStream.Write(Header, 0, Header.Length);
            _mWrite.Flush();

            AnnotationStart = DateTime.Now;
            // Yay


        }

        public void Start()
        {
            if (Active == false)
            {
                Active = true;

                // TODO: Build & Dump header

                _Worker = new Timer();
                _Worker.Elapsed += new ElapsedEventHandler(_Worker_Elapsed);
                _Worker.Interval = 2; // 100Hz
                _Worker.AutoReset = true;
                _Worker.Start();
            }

        }

        public void Start(string file)
        {
            if (Active == false)
            {
                Active = true;

                // TODO: Build & Dump header
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
                    if (t == typeof (double)) type = 0;
                    if (t == typeof (float)) type = 1;
                    if (t == typeof (Int32)) type = 2;
                    if (t == typeof (Int16)) type = 3;
                    if (t == typeof (byte)) type = 4;
                    if (t == typeof (UInt32)) type = 5;
                    if (t == typeof (UInt16)) type = 6;
                    if (t == typeof (char)) type = 7;
                    if (t == typeof (string)) type = 8;
                    if (t == typeof (bool)) type = 9;
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
                AnnotationWaiter.Reset();
                lock (_mWrite)
                {

                    /************** TIME SYNC *************/
                    byte[] data = new byte[8];
                    byte[] header = new byte[10];
                    header[0] = (byte)'$';
                    header[1] = (byte)'#';                                                                      // Sync
                    ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.Time), 2, 4, 0); // Packet ID
                    ByteMethods.memcpy(header, BitConverter.GetBytes(0), 2, 6, 0);          // Instance ID
                    ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)8), 2, 8, 0);            // Data length

                    // Write time sync packet.
                    // TODO: NEed better syncing with game.
                    TimeSpan dt = DateTime.Now.Subtract(AnnotationStart);
                    double dt_ms = dt.TotalMilliseconds;
                    data = BitConverter.GetBytes(dt_ms);

                    _mWrite.BaseStream.Write(header, 0, header.Length);
                    _mWrite.BaseStream.Write(data, 0, data.Length);


                    /************** DATA *************/
                    foreach (KeyValuePair<string, TelemetryLoggerSubscribedInstance> tel_instance in Instances)
                    {
                        data = tel_instance.Value.Dump(new List<string>());
                        if (data.Length > 0)
                        {

                            // Write a packet to the stream.
                            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort) TelemetryLogPacket.Data), 2, 4, 0);
                                // Packet ID
                            ByteMethods.memcpy(header, BitConverter.GetBytes(tel_instance.Value.ID), 2, 6, 0);
                                // Instance ID
                            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort) data.Length), 2, 8, 0);
                                // Data length


                            _mWrite.BaseStream.Write(header, 0, header.Length);
                            _mWrite.BaseStream.Write(data, 0, data.Length);
                        }
                    }
                }
                AnnotationWaiter.Set();
            }
        }
    }
}