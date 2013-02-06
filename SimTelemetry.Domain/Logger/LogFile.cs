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

        //
        public IEnumerable<ILogField> Fields { get { return new List<ILogField>(); } }

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected readonly IList<LogGroup> _groups = new List<LogGroup>();

        protected List<Dictionary<float, int>> TimeMarkers = new List<Dictionary<float, int>>();
        protected byte[] Data = new byte[BufferSize];
        protected byte[] FrameData = new byte[64 * 1024];
        protected int FrameDataIndex = 0;
        protected int DataIndex = 0;
        protected int FileIndex = 0;

        private uint _groupId;
        private uint _fieldId = 0;

        #region House keeping methods
        public LogError Add(LogGroup group)
        {
            if (Groups.Any(x => x.ID == group.ID || x.Name == group.Name))
                return LogError.GroupAlreadyExists;
            _groups.Add(group);
            return LogError.OK;
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

        protected uint GetFieldId(string group, string name)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();

            if (oGroup == null)
                return (uint)LogError.GroupNotFound;

            var oField = oGroup.Fields.Where(x => x.Name == name).FirstOrDefault();
            if (oField == null)
                return (uint)LogError.FieldNotFound;
            else
                return oField.ID;

        }

        protected uint GetGroupId(string group)
        {
            var oGroup = Groups.Where(x => x.Name == group).FirstOrDefault();
            if (oGroup == null)
                return (uint) LogError.GroupNotFound;
            else
                return oGroup.ID;
        }

        public LogGroup CreateGroup(string name)
        {
            var oGroup = new LogGroup(RequestNewGroupId(), name, this);
            Add(oGroup);
            return oGroup;
        }

        public uint RequestNewGroupId()
        {
            return _groupId++;
        }

        public uint RequestNewFieldId()
        {
            return _fieldId++;
        }
        #endregion


        public LogFile(string file)
        {
            // Open it
        }

        public LogFile()
        {
            // New start
            if (!Directory.Exists("./LogTemp/"))
                Directory.CreateDirectory("./LogTemp/");

            TimeMarkers.Add(new Dictionary<float, int>());
        }

        public void Finish()
        {
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

            ZipStorer zipFile = ZipStorer.Create("Telemetry.zip","");
            for (int DataFile = 1; DataFile <= FileIndex;DataFile++)
            {
                zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Data" + DataFile + ".bin", "Data" + DataFile + ".bin", "");
            }
            zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Time.bin", "Time.bin", "");
            zipFile.AddFile(ZipStorer.Compression.Deflate, "./LogTemp/Structure.bin", "Structure.bin", "");
            zipFile.Close();
        }

        protected void WriteLogStructure(ILogNode node, byte[] buffer, ref int bufferIndex)
        {
            byte[] tmp = new byte[128];
            foreach(var fields in node.Fields)
            {
                byte[] strName = Encoding.ASCII.GetBytes(fields.Name);
                Array.Copy(BitConverter.GetBytes(fields.ID), 0, tmp, 1, 4);
                Array.Copy(BitConverter.GetBytes(fields.Group.ID), 0, tmp, 5, 4);
                Array.Copy(BitConverter.GetBytes(strName.Length+1), 0, tmp, 9, 4);
                Array.Copy(strName, 0, tmp, 13, strName.Length);
                tmp[0] = 0x1E;
                tmp[13 + strName.Length] = 0;

                Array.Copy(tmp, 0, buffer, bufferIndex, 13+strName.Length+1);
                bufferIndex += 13 + strName.Length + 1;
            }

            foreach(var group in node.Groups)
            {
                byte[] strName = Encoding.ASCII.GetBytes(group.Name);
                Array.Copy(BitConverter.GetBytes(group.ID), 0, tmp, 1, 4);
                if (group.Master != null) 
                    Array.Copy(BitConverter.GetBytes(group.Master.ID), 0, tmp, 5, 4);
                else
                    Array.Copy(new byte[4] { 0, 0, 0, 0 }, 0, tmp, 5, 4);
                Array.Copy(BitConverter.GetBytes(strName.Length + 1), 0, tmp, 9, 4);
                Array.Copy(strName, 0, tmp, 13, strName.Length);
                tmp[0] = 0x1D;
                tmp[13 + strName.Length] = 0;

                Array.Copy(tmp, 0, buffer, bufferIndex, 13 + strName.Length + 1);
                bufferIndex += 13 + strName.Length + 1;

                WriteLogStructure(group, buffer, ref bufferIndex);
            }
        }

        protected void WriteNewDataFile(object info)
        {
            byte[] data = (byte[]) info;
            int fileID = FileIndex;
            
            if (!Directory.Exists("./LogTemp")) return; // TODO: Handle error
            if (File.Exists("./LogTemp/Data"+fileID+".bin")) File.Delete("./LogTemp/Data"+fileID+".bin");
            File.WriteAllBytes("./LogTemp/Data" + fileID + ".bin", data);

        }

        public void Flush(float timestamp)
        {
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
            var stream = File.OpenWrite(file);
            stream.Write(data, 0, length);
            stream.Close();
        }
    }

}