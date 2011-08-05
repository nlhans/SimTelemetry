using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using SimTelemetry.Game.Rfactor;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data
{
    public class DataReader
    {
        public Dictionary<int, double> Laps = new Dictionary<int, double>();

        public event AnonymousSignal LapsReceived;
        public event AnonymousSignal DataReceived;
        public event AnonymousSignal SpecifiedLap_Received;

        // this lap is the target triggered by specifiedlap
        public int Lap_Awaiting = -1;

        private FileStream strw;
        private string file;

        private Dictionary<string, int> _Mapping_DriverPlayer = new Dictionary<string, int>();
        private Dictionary<string, int> _Mapping_DriverGeneral = new Dictionary<string, int>();
        private Dictionary<string, int> _Mapping_Session = new Dictionary<string, int>();

        public Dictionary<int, DataSample> Samples = new Dictionary<int, DataSample>();

        private PropertyInfo[] __DriverGeneralInfo;
        private PropertyInfo[] __DriverPlayerInfo;
        private PropertyInfo[] __SessionInfo;

        private PropertyDescriptorCollection _Properties_DriverGeneral;
        private PropertyDescriptorCollection _Properties_DriverPlayer;
        private PropertyDescriptorCollection _Properties_Session;

        private void TriggerAsync(object th)
        {
            AnonymousSignal t = (AnonymousSignal) th;
            Trigger(t);
        }

        private void Trigger(AnonymousSignal that)
        {
            if (that != null)
                that();

        }


        public DataReader(string f)
        {
            _Properties_DriverGeneral = TypeDescriptor.GetProperties(typeof (IDriverGeneral));
            _Properties_DriverPlayer = TypeDescriptor.GetProperties(typeof (IDriverPlayer));
            _Properties_Session = TypeDescriptor.GetProperties(typeof (ISession));

            __DriverGeneralInfo = typeof (IDriverGeneral).GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                                        BindingFlags.Instance);

            __DriverPlayerInfo = typeof (IDriverPlayer).GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                                      BindingFlags.Instance);
            __SessionInfo = typeof (ISession).GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                            BindingFlags.Instance);

            file = f;
            file = "testje.txt";

        }

        public void Read()
        {


        // Read all laps data
            string[] lines = File.ReadAllLines(file);

            int lapnumber = 0;
            for (int l = 0; l < lines.Length; l++)
            {
                if(lines[l] == "[Lap]")
                {
                    // get number and data
                    string[] num = lines[l + 1].Split("=".ToCharArray(), 2);
                    string[] time = lines[l + 2].Split("=".ToCharArray(), 2);

                    double laptime = 0;
                    
                    if (double.TryParse(time[1], out laptime) && Int32.TryParse(num[1], out lapnumber))
                    {
                        if (Laps.ContainsKey(lapnumber - 1) && Laps[lapnumber - 1] == laptime)
                        {
                            if (Laps.ContainsKey(lapnumber))
                                Laps[lapnumber] = -1;
                            else
                                Laps.Add(lapnumber, -1);
                        }
                        else
                        {
                            if (Laps.ContainsKey(lapnumber))
                                Laps[lapnumber] = laptime;
                            else
                                Laps.Add(lapnumber, laptime);
                        }

                    }

                }
            }
            lapnumber++;
                if (Laps.ContainsKey(lapnumber))
                    Laps[lapnumber] = -1;
                else
                    Laps.Add(lapnumber, -1);
            

            ThreadPool.QueueUserWorkItem(new WaitCallback(TriggerAsync), LapsReceived);

            strw = File.Open(file, FileMode.Open, FileAccess.Read);
            int state = 0;
            string tmp = "";

            // Read header
            // First line is [Information]
            while (state >= 0)
            {
                string line = strw.ReadLine().Trim();
                if (line == "") continue;
                switch (state)
                {
                    case 0:

                        if (line != "[Information]")
                            throw new Exception("Expected information line first");
                        state++;
                        break;

                    case 1:

                        // Revision..
                        Console.WriteLine(line);
                        state++;
                        break;

                    case 2:
                        if (line != "[DriverPlayer]")
                            throw new Exception("Expected driver mapping");
                        state++;
                        break;

                    case 3:
                        if (line == "[DriverGeneral]")
                        {
                            state++; // it's now time for Driver mapping
                        }
                        else
                        {
                            // Split on key/data
                            string[] k = line.Split("=".ToCharArray(), 2);
                            string[] d = k[1].Split(",".ToCharArray(), 2);

                            int id = Convert.ToInt32(d[0]);
                            _Mapping_DriverPlayer.Add(k[0], id);
                        }
                        break;

                    case 4:
                        if (line == "[Session]")
                        {
                            state++; // it's now time for session mapping
                        }
                        else
                        {
                            // Split on key/data
                            string[] k = line.Split("=".ToCharArray(), 2);
                            string[] d = k[1].Split(",".ToCharArray(), 2);

                            int id = Convert.ToInt32(d[0]);
                            _Mapping_DriverGeneral.Add(k[0], id);
                        }
                        break;

                    case 5:
                        if (line.Contains("START OF DATA"))
                        {
                            state = -1;
                            // DATA TIME JOEPIE!
                            break;
                        }
                        else
                        {
                            // Split on key/data
                            string[] k = line.Split("=".ToCharArray(), 2);
                            string[] d = k[1].Split(",".ToCharArray(), 2);

                            int id = Convert.ToInt32(d[0]);
                            _Mapping_Session.Add(k[0], id);
                        }
                        break;
                }
            }
            SampledDriverGeneral DriverGeneral = new SampledDriverGeneral();
            SampledDriverPlayer DriverPlayer = new SampledDriverPlayer();
            SampledSession Session = new SampledSession();
            int sample_i = 0;
            while (true)
            {
                string headerline = strw.ReadLine().Trim();
                if (headerline == "") continue;
                if (headerline.Contains("END"))
                {
                    // everything ends.. even good log files
                    break;

                }

                if (headerline == "[Lap]")
                {
                    // number
                    string[] no = strw.ReadLine().Trim().Split("=".ToCharArray(), 2);
                    string[] time = strw.ReadLine().Trim().Split("=".ToCharArray(), 2);

                    int nu = 0;
                    if (Int32.TryParse(no[1], out nu))
                    {
                        if (Lap_Awaiting == nu)
                            ThreadPool.QueueUserWorkItem(new WaitCallback(TriggerAsync), SpecifiedLap_Received);

                    }

                }

                if (headerline.StartsWith("[Data@"))
                {
                    try
                    {
                        sample_i++;
                        int sample = Convert.ToInt32(headerline.Substring(6, headerline.Length - 6 - 1));
                        DataSample ds = new DataSample();
                        ds.Time = sample * 1.0 * DataCollector.SleepTime / 1000.0;

                        // Next line indicates amount of bytes per block.
                        string block_info = strw.ReadLine().Trim();
                        string[] block_info_length = block_info.Split(",".ToCharArray());
                        int[] block_info_length_i = new int[block_info_length.Length];

                        for (int i = 0; i < block_info_length.Length; i++)
                        {
                            block_info_length_i[i] = Convert.ToInt32(block_info_length[i]);

                        }

                        // read.
                        byte[] block_Session = new byte[block_info_length_i[0]];
                        byte[] block_DriverPlayer = new byte[block_info_length_i[1]];
                        byte[] block_DriverGeneral = new byte[block_info_length_i[2]];

                        strw.Read(block_Session, 0, block_Session.Length);
                        strw.Read(block_DriverPlayer, 0, block_DriverPlayer.Length);
                        strw.Read(block_DriverGeneral, 0, block_DriverGeneral.Length);

                        // Parse
                        ds.Player = Read_DriverPlayer(block_DriverPlayer, DriverPlayer);
                        DriverPlayer = ds.Player.Duplicate();

                        ds.Session = Read_Session(block_Session, Session);
                        Session = ds.Session.Duplicate();

                        ds.Drivers = new List<SampledDriverGeneral>();
                        ds.Drivers.Add(Read_DriverGeneral(block_DriverGeneral, DriverGeneral));
                        DriverGeneral = ds.Drivers[0].Duplicate();
                        Samples.Add(sample_i, ds);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }

            // Maybe new
            int aasdft = 0;
            strw.Close();

            ThreadPool.QueueUserWorkItem(new WaitCallback(TriggerAsync), DataReceived);
        }


        public SampledDriverGeneral Read_DriverGeneral(byte[] data, SampledDriverGeneral sample)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int id = BitConverter.ToInt32(data, offset);
                offset += 4;
                string key = __GetKeyByValue(id, _Mapping_DriverGeneral);

                //foreach (PropertyInfo fi in __DriverGeneralInfo)
                foreach(PropertyDescriptor fi in _Properties_DriverGeneral)
                {
                    if (fi.Name == key)
                    {
                        object obj = null;

                        if (fi.PropertyType == typeof(string))
                        {
                            List<byte> tmp = new List<byte>();

                            while (true)
                            {
                                if (offset > data.Length) break;
                                byte b = (byte)data[offset];
                                offset++;
                                if (b == 0)
                                    break;
                                tmp.Add(b);
                                obj = ASCIIEncoding.ASCII.GetString(tmp.ToArray());
                            }

                        }
                        if (fi.PropertyType == typeof(double)) obj = BitConverter.ToDouble(data, offset);
                        if (fi.PropertyType == typeof(float)) obj = BitConverter.ToSingle(data, offset);
                        if (fi.PropertyType == typeof(Int32)) obj = BitConverter.ToInt32(data, offset);
                        if (fi.PropertyType == typeof(Int16)) obj = BitConverter.ToInt16(data, offset);
                        if (fi.PropertyType == typeof(bool)) obj = BitConverter.ToBoolean(data, offset);
                        if (fi.PropertyType == typeof(byte)) obj = BitConverter.ToChar(data, offset);

                        fi.SetValue(sample, obj);

                        offset += __SizeOfMember_DriverGeneral(id);
                        break;
                    }
                }


            }

            return sample;
        }


        public SampledDriverPlayer Read_DriverPlayer(byte[] data, SampledDriverPlayer sample)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int id = BitConverter.ToInt32(data, offset);
                offset += 4;
                string key = __GetKeyByValue(id, _Mapping_DriverPlayer);

                foreach (PropertyDescriptor fi in _Properties_DriverPlayer)
                {
                    if (fi.Name == key)
                    {
                        object obj = null;

                        if (fi.PropertyType == typeof(string))
                        {
                            List<byte> tmp = new List<byte>();

                            while (true)
                            {
                                if (offset > data.Length) break;
                                byte b = (byte)data[offset];
                                offset++;
                                if (b == 0)
                                    break;
                                tmp.Add(b);
                                obj = ASCIIEncoding.ASCII.GetString(tmp.ToArray());
                            }

                        }
                        if (fi.PropertyType == typeof(double)) obj = BitConverter.ToDouble(data, offset);
                        if (fi.PropertyType == typeof(float)) obj = BitConverter.ToSingle(data, offset);
                        if (fi.PropertyType == typeof(Int32)) obj = BitConverter.ToInt32(data, offset);
                        if (fi.PropertyType == typeof(Int16)) obj = BitConverter.ToInt16(data, offset);
                        if (fi.PropertyType == typeof(bool)) obj = BitConverter.ToBoolean(data, offset);
                        if (fi.PropertyType == typeof(byte)) obj = BitConverter.ToChar(data, offset);

                        fi.SetValue(sample, obj);

                        offset += __SizeOfMember_DriverPlayer(id);
                        break;
                    }
                }


            }

            return sample;
        }




        public SampledSession Read_Session(byte[] data, SampledSession sample)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                int id = BitConverter.ToInt32(data, offset);
                offset += 4;
                string key = __GetKeyByValue(id, _Mapping_Session);

                foreach (PropertyDescriptor fi in _Properties_Session)
                {
                    if (fi.Name == key)
                    {
                        object obj = null;


                        if (fi.PropertyType == typeof(string))
                        {
                            List<byte> tmp = new List<byte>();

                            while (true)
                            {
                                if (offset > data.Length) break;
                                byte b = (byte)data[offset];
                                offset++;
                                if (b == 0)
                                    break;
                                tmp.Add(b);
                                obj = ASCIIEncoding.ASCII.GetString(tmp.ToArray());
                            }

                        }
                        if (fi.PropertyType == typeof(double)) obj = BitConverter.ToDouble(data, offset);
                        if (fi.PropertyType == typeof(float)) obj = BitConverter.ToSingle(data, offset);
                        if (fi.PropertyType == typeof(Int32)) obj = BitConverter.ToInt32(data, offset);
                        if (fi.PropertyType == typeof(Int16)) obj = BitConverter.ToInt16(data, offset);
                        if (fi.PropertyType == typeof(bool)) obj = BitConverter.ToBoolean(data, offset);
                        if (fi.PropertyType == typeof(byte)) obj = BitConverter.ToChar(data, offset);
                        
                        fi.SetValue(sample, obj);

                        offset += __SizeOfMember_Session(id);
                        break;
                    }
                }


            }

            return sample;
        }

        private string __GetKeyByValue(int id, Dictionary<string, int> map)
        {
            if (map.ContainsValue(id))
            {
                foreach (KeyValuePair<string, int> k in map)
                {
                    if (k.Value == id) return k.Key;
                }
            }
            return string.Empty;


        }

        private int __SizeOfMember_DriverPlayer(int id)
        {

            // Found key
            string key = __GetKeyByValue(id, _Mapping_DriverPlayer);

            // Look up the size in the interface.
            foreach (PropertyInfo fi in __DriverPlayerInfo)
            {
                if (fi.Name == key)
                {
                    // GOT THE NAME FUCK YEAH
                    if (fi.PropertyType == typeof(string)) return 0;
                    if (fi.PropertyType == typeof(double)) return 8;
                    if (fi.PropertyType == typeof(float)) return 4;
                    if (fi.PropertyType == typeof(Int32)) return 4;
                    if (fi.PropertyType == typeof(Int16)) return 2;
                    if (fi.PropertyType == typeof(bool)) return 1;
                    if (fi.PropertyType == typeof(byte)) return 1;
                }
            }


            return 0;
        }

        private int __SizeOfMember_DriverGeneral(int id)
        {

            // Found key
            string key = __GetKeyByValue(id, _Mapping_DriverGeneral);

            // Look up the size in the interface.
            foreach (PropertyInfo fi in __DriverGeneralInfo)
            {
                if (fi.Name == key)
                {
                    // GOT THE NAME FUCK YEAH
                    if (fi.PropertyType == typeof(string)) return 0;
                    if (fi.PropertyType == typeof(double)) return 8;
                    if (fi.PropertyType == typeof(float)) return 4;
                    if (fi.PropertyType == typeof(Int32)) return 4;
                    if (fi.PropertyType == typeof(Int16)) return 2;
                    if (fi.PropertyType == typeof(bool)) return 1;
                    if (fi.PropertyType == typeof(byte)) return 1;
                }
            }


            return 0;
        }
        private int __SizeOfMember_Session(int id)
        {

            // Found key
            string key = __GetKeyByValue(id, _Mapping_Session);

            // Look up the size in the interface.
            foreach (PropertyInfo fi in __SessionInfo)
            {
                if (fi.Name == key)
                {
                    // GOT THE NAME FUCK YEAH
                    if (fi.PropertyType == typeof(string)) return 0;
                    if (fi.PropertyType == typeof(double)) return 8;
                    if (fi.PropertyType == typeof(float)) return 4;
                    if (fi.PropertyType == typeof(Int32)) return 4;
                    if (fi.PropertyType == typeof(Int16)) return 2;
                    if (fi.PropertyType == typeof(bool)) return 1;
                    if (fi.PropertyType == typeof(byte)) return 1;
                }
            }


            return 0;
        }

        public void Stop()
        {

        }
    }
}
