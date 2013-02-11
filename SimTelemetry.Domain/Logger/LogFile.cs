using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Utils;

namespace SimTelemetry.Domain.Logger
{
    public class LogFile : ILogNode
    {
        private const int BufferSize = 16*1024*1024;

        public int ID { get { return 0; } }

        protected ZipStorer zipFile;
        public bool ReadOnly { get; protected set; }

        //
        public IEnumerable<ILogField> Fields { get { return new List<ILogField>(); } }
        protected Dictionary<int, ILogField> _fieldsCache = new Dictionary<int, ILogField>();

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected readonly IList<LogGroup> _groups = new List<LogGroup>();

        public IEnumerable<int> Timeline { get; private set; }
        public List<Dictionary<int, int>> Time { get { return _time; } }
        protected List<Dictionary<int, int>> _time = new List<Dictionary<int, int>>();

        #region Data Writer fields
        protected byte[] Data;
        protected byte[] FrameData = new byte[64 * 1024];
        protected int FrameDataIndex = 0;
        protected int DataIndex = 0;
        protected int FileIndex = 0;

        private int _groupId = 1;
        private int _fieldId = 1;
        #endregion
        #region Data Reader Fields
        protected Dictionary<int, byte[]> DataFiles = new Dictionary<int, byte[]>();
        #endregion
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
            var oGroup = new LogGroup(RequestNewGroupId(), name, this, this);
            Add(oGroup);
            return oGroup;
        }

        public LogGroup CreateGroup(string name, int id)
        {
            var oGroup = new LogGroup(id, name, this, this);
            Add(oGroup);
            return oGroup;
        }

        public ILogField CreateField(string name, Type valueType, int id)
        {
            return null;
        }

        public ILogNode FindGroup(string name)
        {
            var search = _groups.Where(x => x.Name == name);
            // TODO: Add sub-groups?
            return search.FirstOrDefault();
        }

        public ILogNode FindGroup(int id)
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

        public bool ContainsGroup(string name)
        {
            return Groups.Any(x => x.Name == name);
        }
        public bool ContainsGroup(int id)
        {
            return Groups.Any(x => x.ID == id);
        }

        public bool ContainsField(string name)
        {
            return Fields.Any(x => x.Name == name);
        }
        public bool ContainsField(int id)
        {
            return Fields.Any(x => x.ID == id);
        }

        public ILogField FindField(int fieldId)
        {
            if (fieldId == 0)
                return null;
            if (_fieldsCache.ContainsKey(fieldId))
                return _fieldsCache[fieldId];

            var lvl1 = _groups.SelectMany(x => x.Fields).Where(x => x.ID == fieldId);
            var lvl1f = lvl1.FirstOrDefault();
            if (lvl1f != null)
            {
                _fieldsCache.Add(fieldId, lvl1f);
                return lvl1f;
            }

            var fieldFromAll =_groups.SelectMany(x => x.Groups).SelectMany(x => x.Fields).Where(x => x.ID == fieldId).FirstOrDefault();
           if(fieldFromAll==null)
               return null;

            _fieldsCache.Add(fieldId, fieldFromAll);
            return fieldFromAll;
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

        public int GetFieldId(int group, string name)
        {
            var oGroup = Groups.Where(x => x.ID == group).FirstOrDefault();

            if (oGroup == null)
                return (int)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (int)LogError.FieldNotFound;
            else
                return oField.ID;

        }

        private Dictionary<string, int> fieldIdCache = new Dictionary<string, int>();
        public int GetFieldId(string group, string name)
        {
            if (fieldIdCache.ContainsKey(group + "." + name))
                return fieldIdCache[group + "." + name];

            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();
            
            if (oGroup == null)
            {
                var subGroup = Groups.SelectMany(x => x.Groups).Where(x => x.Name == group).FirstOrDefault();
                if (subGroup == null)
                    return (int)LogError.GroupNotFound;
                oGroup = subGroup;
            }

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (int)LogError.FieldNotFound;
            fieldIdCache.Add(group+"."+name, oField.ID);
            return oField.ID;
        }

        public int GetGroupId(string group)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();
            if (oGroup == null)
            {
                
                var subGroup = Groups.SelectMany(x => x.Groups).Where(x => x.Name == group).FirstOrDefault();
                if (subGroup == null)
                    return (int)LogError.GroupNotFound;
                return subGroup.ID;
            }
            else
                return oGroup.ID;
        }

        internal int RequestNewGroupId()
        {
            if (ReadOnly) return 0;
            return _groupId++;
        }

        internal int RequestNewFieldId()
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
                var i1 = BitConverter.ToInt32(times, i*8);
                var index = BitConverter.ToInt32(times, i*8 + 4);

                if(BitConverter.ToUInt32(times, i*8) == 0x80000000)
                {
                    FileIndex = index-1;
                    _time.Add(new Dictionary<int, int>());
                }
                else
                {
                    if(_time[FileIndex].ContainsKey(i1) == false)
                    _time[FileIndex].Add(i1, index);
                }
            }
            Timeline = Time.SelectMany(x => x.Keys).ToList();

            // read channel structure
            int structureIndex = 0;
            while(structureIndex < structure.Length)
            {
                var isGroup = structure[structureIndex] == 0x1D;
                var isField = structure[structureIndex] == 0x1E;

                var id = BitConverter.ToInt32(structure, structureIndex + 1);
                var parent = BitConverter.ToInt32(structure, structureIndex + 5);
                var nameLength = BitConverter.ToInt32(structure, structureIndex + 9);
                var name = Encoding.ASCII.GetString(structure, structureIndex + 13, nameLength);

                var blockSize = 13 + nameLength;
                if(isGroup)
                {
                    var master = FindGroup(parent);
                    master.CreateGroup(name, id);
                }
                else
                {
                    var typeLength = BitConverter.ToInt32(structure, structureIndex + 13 + nameLength);
                    var type = Encoding.ASCII.GetString(structure, structureIndex + 17 + nameLength, typeLength);

                    blockSize += 4 + typeLength;

                    Type valueTypeObject = Type.GetType(type);
                    var group = FindGroup(parent);

                    group.CreateField(name, valueTypeObject, id);
                }

                structureIndex += blockSize;
            }

        }

        public LogFile()
        {
            // New start
            _time.Add(new Dictionary<int, int>());
            ReadOnly = false;
            Data = new byte[BufferSize];
        }

        public void Finish(string file)
        {
            if (ReadOnly) return;
            if (_time.Select(x => x.Count).Sum() < 5)
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
            byte[] TimeTable = new byte[1024*1024*16];
            int TimeTableIndex = 0;
            int TimeFileIndex = 1;
            foreach(var dataFileTimes in _time)
            {
                Array.Copy(BitConverter.GetBytes(0x80000000), 0, TimeTable, TimeTableIndex, 4);
                TimeTableIndex += 4;
                Array.Copy(BitConverter.GetBytes(TimeFileIndex++), 0, TimeTable, TimeTableIndex, 4);
                TimeTableIndex += 4;

                // Log all times
                foreach(var timeKVP in dataFileTimes)
                {
                    int timeInt = timeKVP.Key;

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

        public void Flush(int timestamp)
        {
            if (ReadOnly) return;
            if (FrameDataIndex + DataIndex > Data.Length)
            {
                FileIndex++;
                _time.Add(new Dictionary<int, int>());

                ThreadPool.QueueUserWorkItem(WriteNewDataFile, Data);
                Data = new byte[BufferSize];
                // Queue to write this away
                DataIndex = 0;
            }


            // Copy data & mark it in time table
            if (!_time[FileIndex].ContainsKey(timestamp))
            {
                _time[FileIndex].Add(timestamp, DataIndex);
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

            var fieldID = (int) GetFieldId(group, field);
            if (fieldID == -1)
                return;

            byte[] byteFieldId = BitConverter.GetBytes(fieldID);
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

        public T ReadAs<T>(string group, string field, int time)
        {
            T errObj =  (T)Convert.ChangeType(0, typeof(T));
            byte[] databuffer = null;
            int dataFileIndex = 1;
            int dataOffset = 0;
            foreach(var dataTimeline in Time)
            {
                if (dataTimeline.ContainsKey(time))
                {
                    databuffer = GetDataBuffer(dataFileIndex);
                    dataOffset = dataTimeline[time];
                    break;
                }
                dataFileIndex++;
            }

            if (databuffer == null) return errObj;

            var fieldId = GetFieldId(group, field);
            if (fieldId == (int)LogError.FieldNotFound) return errObj;
            if (fieldId == (int)LogError.GroupNotFound) return errObj;
            var fieldObj = FindField((int)fieldId);

            int fieldOffset = 0;

            // Search inside the buffer.
            int ind = dataOffset;
            while (ind < databuffer.Length)
            {
                var isData = databuffer[ind + 1] == 0x1F && databuffer[ind] == 0x1F;
                if (isData)
                {
                    var blockFieldID = BitConverter.ToUInt32(databuffer, ind + 2);
                    if (blockFieldID == fieldId)
                    {
                        fieldOffset = ind+2+4;
                        break;
                    }
                    else
                        ind += 4;
                }
                else
                ind++;
            }

            return fieldObj.ReadAs<T>(databuffer, fieldOffset);

        }

        private byte[] GetDataBuffer(int dataFileIndex)
        {
            if (DataFiles.ContainsKey(dataFileIndex))
                return DataFiles[dataFileIndex];
            
            var dataFileSearch = zipFile.ReadCentralDir().Where(x => x.FilenameInZip == "Data" + dataFileIndex + ".bin");
            if(dataFileSearch.Count() == 0)
                return new byte[0];

            var dataFile = dataFileSearch.FirstOrDefault();

            var data = new byte[dataFile.FileSize];
            var dataStream = new MemoryStream(data);
            zipFile.ExtractFile(dataFile, dataStream);
            DataFiles.Add(dataFileIndex, data);
            return data;
        }
    }

}