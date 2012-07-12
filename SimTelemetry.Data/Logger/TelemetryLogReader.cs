using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data.Logger
{
    public class TelemetryLogReader
    {
        private List<TelemetryPacket> Packets = new List<TelemetryPacket>();
        private Dictionary<double, TelemetrySample> Data = new Dictionary<double, TelemetrySample>();
        private byte[] data = new byte[0];
        private string _File;
        private int _DataPointer = 0;
        private int _PacketPointer = 0;
        private int _ReadStage = 0;
        public Signal Done;

        public Dictionary<double, TelemetrySample> Samples { get { return Data; } }
        public Dictionary<int, string> Instances { get; set; }

        public Dictionary<string, Dictionary<int, string>> Properties { get; set; }
        public Dictionary<string, Dictionary<int, int>> Types { get; set; }

        public int PacketCount
        {
            get { return Packets.Count; }
        }

        public int Stage
        {
            get { return _ReadStage; }
        }

        public double Progress
        {
            get
            {
                switch (Stage)
                {
                    case 0:
                        return 0;
                        break;
                    case 1:
                        if (data.Length > 0)
                            return _DataPointer * 1000 / data.Length;
                        else
                            return 0;
                        break;
                    case 2:
                        if (Packets.Count > 0)
                            return _PacketPointer * 1000 / Packets.Count;
                        else
                            return 1000;
                        break;
                    case 3:
                        return 1000;
                        break;
                    default:
                        return 0;
                        break;
                }
            }
        }

        public TelemetryLogReader(string path_file)
        {
            if (File.Exists(path_file) == false)
                throw new FileLoadException("File does not exist");

            _File = path_file;
            if (new FileInfo(path_file).Extension == ".gz")
            {
                // Uncompress it.
                using (MemoryStream DatFile = new MemoryStream())
                using (FileStream GzFile = File.OpenRead(path_file))
                using (GZipStream Decompress = new GZipStream(GzFile, CompressionMode.Decompress))
                {
                    Decompress.CopyTo(DatFile);

                    data = new byte[DatFile.Length];
                    DatFile.Seek(0, SeekOrigin.Begin);
                    DatFile.Read(data, 0, (int)DatFile.Length);
                    // Done.
                }

            }
            else
            {
                data = File.ReadAllBytes(_File);
            }

        }

        public void Read()
        {
            if (data.Length != 0)
            {
                _ReadStage = 0;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReadWorker), null);
            }
        }

        private void ReadWorker(object n)
        {
            lock (this.Data)
            {
                _DataPointer = 0;
                _ReadStage = 1;

                for (int i = 0; i < data.Length - 8; i++)
                {
                    if (data[i] == '$' && data[i + 1] == '#')
                    {
                        ushort PacketID = BitConverter.ToUInt16(data, i + 2);
                        ushort Type = BitConverter.ToUInt16(data, i + 4);
                        ushort InstanceID = BitConverter.ToUInt16(data, i + 6);
                        ushort PacketSize = BitConverter.ToUInt16(data, i + 8);
                        byte[] PacketData = new byte[PacketSize];
                        ByteMethods.memcpy(PacketData, data, PacketSize, 0, i + 10);

                        TelemetryPacket packet = new TelemetryPacket
                                                     {
                                                         Data = PacketData,
                                                         ID = PacketID,
                                                         Type = (TelemetryLogPacket) Type,
                                                         InstanceID = InstanceID,
                                                         Size = PacketSize
                                                     };
                        Packets.Add(packet);
                        _DataPointer = i + PacketSize;
                        i += PacketSize;
                    }
                }
                _DataPointer = data.Length;
                _ReadStage = 2;

                // parse packets
                TelemetrySample Sample = new TelemetrySample();
                Instances = new Dictionary<int, string>();
                Properties = new Dictionary<string, Dictionary<int, string>>();
                Types = new Dictionary<string, Dictionary<int, int>>();

                string last_type = "";
                int last_type_id = 0;
                bool first_packet = true;
                double time = -1.0;
                foreach (TelemetryPacket packet in Packets)
                {
                    _PacketPointer++;
                    switch (packet.Type)
                    {
                        case TelemetryLogPacket.InstanceName:
                            last_type = ASCIIEncoding.ASCII.GetString(packet.Data).Replace('\0', ' ').Trim();
                            last_type_id = packet.InstanceID;
                            Instances.Add(last_type_id, last_type);
                            Types.Add(last_type, new Dictionary<int, int>());
                            ;
                            Properties.Add(last_type, new Dictionary<int, string>());
                            ;
                            break;

                        case TelemetryLogPacket.InstanceMember:
                            int id = BitConverter.ToInt16(packet.Data, 2);

                            if (!Sample.Data.ContainsKey(last_type_id))
                                Sample.Data.Add(last_type_id, new Dictionary<int, object>());
                            if (Sample.Data[last_type_id].ContainsKey(id) == false)
                                Sample.Data[last_type_id].Add(id, 0);


                            int type_id = BitConverter.ToInt16(packet.Data, 0);

                            if (Types[last_type].ContainsKey(id) == false)
                            {
                                Types[last_type].Add(id, type_id);

                                byte[] member_name = new byte[packet.Data.Length - 4];
                                ByteMethods.memcpy(member_name, packet.Data, packet.Data.Length - 4, 0, 4);

                                string key = ASCIIEncoding.ASCII.GetString(member_name).Replace('\0', ' ').Trim();
                                Properties[last_type].Add(id, key);
                            }

                            break;

                        case TelemetryLogPacket.Data:

                            int instance_id = BitConverter.ToInt16(packet.Data, 0);
                            int last_typeid = 0;
                            for (int i = 2; i < packet.Data.Length;)
                            {
                                int id_type = BitConverter.ToInt32(packet.Data, i);
                                object v = 0;
                                Dictionary<int, int> TypeArray = Types[Instances[instance_id]];
                                i += 4;
                                if (TypeArray.ContainsKey(id_type))
                                {
                                    switch (TypeArray[id_type])
                                    {
                                        case 0: //double
                                            v = BitConverter.ToDouble(packet.Data, i);
                                            i += 8;
                                            break;

                                        case 1: //float
                                            v = BitConverter.ToSingle(packet.Data, i);
                                            i += 4;
                                            break;

                                        case 2: // int32
                                            v = BitConverter.ToInt32(packet.Data, i);
                                            i += 4;
                                            break;

                                        case 3: // int16
                                            v = BitConverter.ToInt16(packet.Data, i);
                                            i += 2;
                                            break;

                                        case 4: // int8
                                            v = packet.Data[i];
                                            i += 1;
                                            break;

                                        case 5: // uint32
                                            v = BitConverter.ToUInt32(packet.Data, i);
                                            i += 4;
                                            break;

                                        case 6: // uint16
                                            v = BitConverter.ToUInt16(packet.Data, i);
                                            i += 2;
                                            break;

                                        case 7: // uint8
                                            v = packet.Data[i];
                                            i += 1;
                                            break;

                                        case 8: // string
                                            int k = i;
                                            for (; k < packet.Data.Length; k++)
                                            {
                                                if (packet.Data[k] == 0) break;
                                            }
                                            v = ASCIIEncoding.ASCII.GetString(packet.Data, i, k - i);
                                            i = k + 1;
                                            break;

                                        case 9: // bool
                                            v = ((packet.Data[i] == 1) ? true : false);
                                            i += 1;
                                            break;
                                        default:
                                            v = 0;
                                            break;
                                    }

                                    Sample.Data[instance_id][id_type] = (object) v;
                                    last_typeid = id_type;
                                }
                            }
                            break;

                        case TelemetryLogPacket.Time:
                            double t = time;
                            // Store previous samples.
                            if (t != -1.0 && this.Data.ContainsKey(t) == false)
                            {
                                this.Data.Add(t, Sample.Clone());
                                this.Data[t].Time = t;
                            }
                            time = BitConverter.ToDouble(packet.Data, 0);
                            break;
                    }

                }

                _ReadStage = 3;

                string db = "";
                foreach (KeyValuePair<double, TelemetrySample> sv in this.Data)
                {
                    TelemetrySample s = sv.Value;
                    db += s.Time + "," + s.Data[3][48];
                    db += "," + s.Data[3][47] + "\r\n";
                }
                File.WriteAllText("test.csv", db);
            }
        }
    }
}