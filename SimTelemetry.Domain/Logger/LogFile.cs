using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Domain.Logger
{
    public class LogFile : ILogNode
    {
        private const int BufferSize = 16*1024*1024;


        protected ZipStorer zipFile;
        public bool ReadOnly { get; protected set; }

        //
        public IEnumerable<ILogField> Fields { get { return new List<ILogField>(); } }

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected readonly IList<LogGroup> _groups = new List<LogGroup>();

        protected List<Dictionary<float, int>> TimeMarkers = new List<Dictionary<float, int>>();
        protected byte[] Data;
        protected byte[] FrameData = new byte[64 * 1024];
        protected int FrameDataIndex = 0;
        protected int DataIndex = 0;
        protected int FileIndex = 0;

        private uint _groupId = 1;
        private uint _fieldId = 1;

        #region House keeping methods
        public LogError Add(LogGroup group)
        {
            if (Groups.Any(x => x.ID == group.ID || x.Name == group.Name))
                return LogError.GroupAlreadyExists;
            _groups.Add(group);
            return LogError.OK;
        }

        public LogGroup CreateGroup(string name)
        {
            var oGroup = new LogGroup(RequestNewGroupId(), name, this);
            Add(oGroup);
            return oGroup;
        }

        public LogGroup CreateGroup(string name, uint id)
        {
            var oGroup = new LogGroup(id, name, this);
            Add(oGroup);
            return oGroup;
        }

        public ILogField CreateField(string name, Type valueType, uint id)
        {
            return null;
        }

        public ILogNode SearchGroup(uint id)
        {
            if (id == 0)
                return this;

            var search = _groups.Where(x => x.ID == id);
            if(search.Count() == 0)
            {
                search = _groups.SelectMany(x => x.Groups).Where(x => x.ID == id);
                if (search.Count() == 0)
                    return null;
            }
            return search.FirstOrDefault();
        }

        protected ILogField SearchField(int fieldId)
        {
            if (fieldId == 0)
                return null;

            var allFields =
                _groups.SelectMany(x => x.Fields).Concat(_groups.SelectMany(x => x.Groups).SelectMany(x => x.Fields));
            var cntFields = allFields.Count();
            return allFields.Where(x => x.ID == fieldId).FirstOrDefault();
        }

        public LogError Remove(LogGroup group)
        {
            if (!Groups.Any(x => x.ID == group.ID))
                return LogError.GroupNotFound;
            _groups.Remove(group);
            return LogError.OK;
        }

        public LogError Remove(string name)
        {
            var result = Groups.Where(x => x.Name == name).FirstOrDefault();
            if (result == null)
                return LogError.GroupNotFound;
            _groups.Remove(result);
            return LogError.OK;
        }

        protected uint GetFieldId(int group, string name)
        {
            var oGroup = Groups.Where(x => x.ID == group).FirstOrDefault();

            if (oGroup == null)
                return (uint)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (uint)LogError.FieldNotFound;
            else
                return oField.ID;

        }

        private Dictionary<string, uint> fieldIdCache = new Dictionary<string, uint>();
        protected uint GetFieldId(string group, string name)
        {
            if (fieldIdCache.ContainsKey(group + "." + name))
                return fieldIdCache[group + "." + name];
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();

            if (oGroup == null)
                return (uint)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (uint)LogError.FieldNotFound;
            else
            {
                
                fieldIdCache.Add(group+"."+name, oField.ID);
                return oField.ID;
            }
        }

        protected uint GetGroupId(string group)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();
            if (oGroup == null)
                return (uint) LogError.GroupNotFound;
            else
                return oGroup.ID;
        }

        public uint RequestNewGroupId()
        {
            if (ReadOnly) return 0;
            return _groupId++;
        }

        public uint RequestNewFieldId()
        {
            if (ReadOnly) return 0;
            return _fieldId++;
        }
        #endregion

        public LogFile(string file)
        {
            // Open it
            ReadOnly = true;

            // Unrar
            zipFile = ZipStorer.Open(file, FileAccess.Read);
            
            byte[] structure = zipFile.ExtractFile("Structure.bin");
            byte[] times = zipFile.ExtractFile("Time.bin");

            // read time structure
            FileIndex = 0;
            for (int i = 0 ;i < times.Length/8; i++)
            {
                var i1 = BitConverter.ToUInt32(times, i*8);
                var index = BitConverter.ToInt32(times, i*8 + 4);

                if(i1 == 0x80000000)
                {
                    FileIndex = index-1;
                    TimeMarkers.Add(new Dictionary<float, int>());
                }
                else
                {
                    float time = i1/1000.0f;
                    if(TimeMarkers[FileIndex].ContainsKey(time) == false)
                    TimeMarkers[FileIndex].Add(time, index);
                }
            }

            uint tempSpeedfieldId = 0;
            uint tempRpmfieldId = 0;
            uint tempTimeFieldId = 0;

            // read channel structure
            int structureIndex = 0;
            while(structureIndex < structure.Length)
            {
                var isGroup = structure[structureIndex] == 0x1D;
                var isField = structure[structureIndex] == 0x1E;

                var id = BitConverter.ToUInt32(structure, structureIndex + 1);
                var parent = BitConverter.ToUInt32(structure, structureIndex + 5);
                var nameLength = BitConverter.ToInt32(structure, structureIndex + 9);
                var name = Encoding.ASCII.GetString(structure, structureIndex + 13, nameLength);

                var blockSize = 13 + nameLength;
                if(isGroup)
                {
                    var master = SearchGroup(parent);
                    master.CreateGroup(name, id);
                }
                else
                {
                    var typeLength = BitConverter.ToInt32(structure, structureIndex + 13 + nameLength);
                    var type = Encoding.ASCII.GetString(structure, structureIndex + 17 + nameLength, typeLength);

                    blockSize += 4 + typeLength;

                    Type valueTypeObject = Type.GetType(type);
                    var group = SearchGroup(parent);

                    group.CreateField(name, valueTypeObject, id);
                }
                if(name=="Speed")
                {
                    tempSpeedfieldId = id;
                }
                if (name == "RPM")
                    tempRpmfieldId = id;
                if (name == "Time")
                    tempTimeFieldId = id;
                structureIndex += blockSize;
            }

            float testtime = 0, rpm = 0, speed = 0;
            StringBuilder csvout = new StringBuilder();
            // read all data..
            byte[] data = zipFile.ExtractFile("Data1.bin");
            int dataIndex = 0;
            while (data.Length >  dataIndex)
            {
                var isData = data[dataIndex+1] == 0x1F && data[dataIndex] == 0x1F;
                if(isData)
                {
                    var fieldID = BitConverter.ToInt32(data, dataIndex + 2);
                    var field = this.SearchField(fieldID);
                    if(field != null)
                    {
                        var fieldData =field.ReadAs<float>(data, dataIndex + 6);
                        if (fieldID == tempSpeedfieldId)
                        {
                            speed = fieldData;
                            csvout.AppendLine(testtime + "," + speed + "," + rpm);
                        }
                        if(fieldID == tempRpmfieldId)
                            rpm = fieldData;
                        if (fieldID == tempTimeFieldId)
                            testtime = fieldData;
                    }

                    int j = dataIndex+6;
                    while( j < data.Length)
                    {
                        if (data[j] == 0x1F && data[j + 1] == 0x1F) 
                            break;
                        j++;
                    }

                    dataIndex = j;
                }else
                {
                    dataIndex++;
                }
            }

            File.WriteAllText("test.csv", csvout.ToString());
        }

        public LogFile()
        {
            // New start
            TimeMarkers.Add(new Dictionary<float, int>());
            ReadOnly = false;
            Data = new byte[BufferSize];
        }

        public void Finish(string file)
        {
            if (ReadOnly) return;
            if (TimeMarkers.Select(x => x.Count).Sum() < 5)
            {
                ClearTemporaryFiles();
                return;
            }
            // Finish action does the following:
            // - Flush current data file
            // - Document about the log structure
            // - Write time table to binary
            // - Compress data to ZIP archive

            // Flush current data to disk
            FileIndex++;
            WriteNewDataFile(Data);

            // Reset everything
            Data = null;
            FrameData = null;
            DataIndex = 0;
            FrameDataIndex = 0;

            // Document about the log structure..
            byte[] LogStructure = new byte[512*1024];
            int LogStructureSize = 0;
            WriteLogStructure(this, LogStructure, ref LogStructureSize);
            File_WriteAllBytes("./LogTemp/Structure.bin", LogStructure, LogStructureSize);

            // Write time table
            byte[] TimeTable = new byte[1024*1024*4];
            int TimeTableIndex = 0;
            int TimeFileIndex = 1;
            foreach(var dataFileTimes in TimeMarkers)
            {
                Array.Copy(BitConverter.GetBytes(0x80000000), 0, TimeTable, TimeTableIndex, 4);
                TimeTableIndex += 4;
                Array.Copy(BitConverter.GetBytes(TimeFileIndex++), 0, TimeTable, TimeTableIndex, 4);
                TimeTableIndex += 4;

                // Log all times
                foreach(var timeKVP in dataFileTimes)
                {
                    int timeInt = (int)Math.Floor(timeKVP.Key*1000);

                    Array.Copy(BitConverter.GetBytes(timeInt), 0, TimeTable, TimeTableIndex, 4);
                    TimeTableIndex += 4;
                    Array.Copy(BitConverter.GetBytes(timeKVP.Value), 0, TimeTable, TimeTableIndex, 4);
                    TimeTableIndex += 4;
                }
            }
            File_WriteAllBytes("./LogTemp/Time.bin", TimeTable, TimeTableIndex);

            zipFile = ZipStorer.Create(file, "SimTelemetry log");
            for (int DataFile = 1; DataFile <= FileIndex;DataFile++)
            {
                zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Data" + DataFile + ".bin", "Data" + DataFile + ".bin", "");
            }
            zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Time.bin", "Time.bin", "");
            zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Structure.bin", "Structure.bin", "");
            zipFile.Close();

            ClearTemporaryFiles();
        }

        private void ClearTemporaryFiles()
        {
            if (ReadOnly) return;
            Data = null;
            FrameData = null;
            DataIndex = 0;
            FrameDataIndex = 0;


            for (int DataFile = 0; DataFile <= 5+FileIndex; DataFile++)
            {
                if (File.Exists("./LogTemp/Data" + DataFile + ".bin"))
                File.Delete("./LogTemp/Data" + DataFile + ".bin");
            }
            if (File.Exists("./LogTemp/Time.bin"))
                File.Delete("./LogTemp/Time.bin");
            if (File.Exists("./LogTemp/Structure.bin"))
                File.Delete("./LogTemp/Structure.bin");

            Directory.Delete("./LogTemp/");
        }

        protected void WriteLogStructure(ILogNode node, byte[] buffer, ref int bufferIndex)
        {
            if (ReadOnly) return;
            byte[] tmp = new byte[128];
            foreach(var fields in node.Fields)
            {
                byte[] strName = Encoding.ASCII.GetBytes(fields.Name);
                byte[] strType = Encoding.ASCII.GetBytes(fields.ValueType.FullName);

                tmp[0] = 0x1E;
                Array.Copy(BitConverter.GetBytes(fields.ID), 0, tmp, 1, 4);
                Array.Copy(BitConverter.GetBytes(fields.Group.ID), 0, tmp, 5, 4);
                Array.Copy(BitConverter.GetBytes(strName.Length), 0, tmp, 9, 4);
                Array.Copy(strName, 0, tmp, 13, strName.Length);
                Array.Copy(BitConverter.GetBytes(strType.Length), 0, tmp, 13 + strName.Length, 4);
                Array.Copy(strType, 0, tmp, 13+4+strName.Length, strType.Length);

                int length = 17 + strName.Length + strType.Length;

                Array.Copy(tmp, 0, buffer, bufferIndex, length);
                bufferIndex += length;
            }

            foreach(var group in node.Groups)
            {
                byte[] strName = Encoding.ASCII.GetBytes(group.Name);
                Array.Copy(BitConverter.GetBytes(group.ID), 0, tmp, 1, 4);
                if (group.Master != null) 
                    Array.Copy(BitConverter.GetBytes(group.Master.ID), 0, tmp, 5, 4);
                else
                    Array.Copy(new byte[4] { 0, 0, 0, 0 }, 0, tmp, 5, 4);
                Array.Copy(BitConverter.GetBytes(strName.Length ), 0, tmp, 9, 4);
                Array.Copy(strName, 0, tmp, 13, strName.Length);
                tmp[0] = 0x1D;

                Array.Copy(tmp, 0, buffer, bufferIndex, 13 + strName.Length);
                bufferIndex += 13 + strName.Length;

                WriteLogStructure(group, buffer, ref bufferIndex);
            }
        }

        protected void WriteNewDataFile(object info)
        {
            if (ReadOnly) return;
            byte[] data = (byte[]) info;
            int fileID = FileIndex;

            if (!Directory.Exists("./LogTemp/"))
                Directory.CreateDirectory("./LogTemp/");

            if (File.Exists("./LogTemp/Data"+fileID+".bin"))
                File.Delete("./LogTemp/Data"+fileID+".bin");
        
            File.WriteAllBytes("./LogTemp/Data" + fileID + ".bin", data);

        }

        public void Flush(float timestamp)
        {
            if (ReadOnly) return;
            if (FrameDataIndex + DataIndex > Data.Length)
            {
                FileIndex++;
                TimeMarkers.Add(new Dictionary<float, int>());

                ThreadPool.QueueUserWorkItem(WriteNewDataFile, Data);
                Data = new byte[BufferSize];
                // Queue to write this away
                DataIndex = 0;
            }


            // Copy data & mark it in time table
            if (!TimeMarkers[FileIndex].ContainsKey(timestamp))
            {
                TimeMarkers[FileIndex].Add(timestamp, DataIndex);
                Array.Copy(FrameData, 0, Data, DataIndex, FrameDataIndex);

                // Increase the indexes.
                DataIndex += FrameDataIndex;
                dataSize += FrameDataIndex;
            }
            FrameDataIndex = 0;
        }

        public int dataSize = 0;
        public void Write(string group, string field, byte[] channelData)
        {
            if (ReadOnly) return;
            int PacketLength = channelData.Length + 6;

            if (FrameDataIndex + PacketLength > FrameData.Length)
                return; // TODO: Handle error

            byte[] byteFieldId = BitConverter.GetBytes((int)GetFieldId(group, field));
            FrameData[FrameDataIndex] = 0x1F;
            FrameData[FrameDataIndex + 1] = 0x1F;

            Array.Copy(byteFieldId, 0, FrameData, FrameDataIndex + 2, 4);
            Array.Copy(channelData, 0, FrameData, FrameDataIndex + 6, channelData.Length);

            FrameDataIndex += PacketLength;
        }

        protected void File_WriteAllBytes(string file, byte[] data, int length)
        {
            if (ReadOnly) return;
            var stream = File.OpenWrite(file);
            stream.Write(data, 0, length);
            stream.Close();
        }
    }

}