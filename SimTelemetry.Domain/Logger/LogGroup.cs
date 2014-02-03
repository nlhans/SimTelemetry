using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Logger
{
    public class LogGroup
    {
        public string Name { get; protected set; }


        public IEnumerable<LogField> Fields { get { return _fields; } }
        protected List<LogField> _fields = new List<LogField>();

        public Dictionary<int, int> Timeline { get { return _timeline; } }
        private new Dictionary<int, int> _timeline = new Dictionary<int, int>();

        #region Reader fields
        internal LogFileReader FileReader { get; set; }
        #endregion
        #region Writer fields
        public IDataNode DataSource { get; protected set; }
        public bool Subscribed { get; protected set; }
        internal LogFileWriter FileWriter { get; set; }

        private LogGroupStream DataStream;
        private LogGroupStream TimeStream;

        #endregion
        protected int fieldCounter = 0;

        public LogGroup(LogFileReader reader, string name)
        {
            Name = name;
            FileReader = reader;
            
            var xmlData = reader.ReadArchiveFile(name + "/Structure.xml");
            var xmlString = Encoding.ASCII.GetString(xmlData);

            using (var xmlReader = XmlTextReader.Create(new StringReader(xmlString)))
            {
                while(xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.Name)
                        {
                            case "entity":
                                if (xmlReader.GetAttribute("name") != this.Name)
                                    Debug.WriteLine("Something is not right");
                                break;

                            case "field":
                                var fieldObj = new LogField(this, xmlReader.GetAttribute("name"),
                                                            xmlReader.GetAttribute("id"),
                                                            xmlReader.GetAttribute("type"));
                                _fields.Add(fieldObj);
                                break;
                        }
                    }
                }
            }
            
            var timeline = FileReader.ReadArchiveFile(Name + "/Time.bin");
            int index = 0;

            while(index<timeline.Length)
            {
                int time = BitConverter.ToInt32(timeline, index);
                int offset = BitConverter.ToInt32(timeline, index + 4);
                _timeline.Add(time, offset);
                index += 8;
            }

        }

        public LogGroup(LogFileWriter writer, string name, IDataNode dataSource)
        {
            FileWriter = writer;
            Name = name;
            DataSource = dataSource;
            Subscribed = true;

            TimeStream = new LogGroupStream(this, LogFileType.Time, 1024 * 1024);
            DataStream = new LogGroupStream(this, LogFileType.Data, 1024 * 1024);

            _fields = dataSource.GetDataFields().Select(x => new LogField(this, x, GetNewFieldId())).ToList();
        }

        public LogGroup(LogFileWriter writer, string name, IDataNode dataSource, IEnumerable<string> fieldLimit)
        {
            FileWriter = writer;
            Name = name;
            DataSource = dataSource;
            Subscribed = true;

            TimeStream = new LogGroupStream(this, LogFileType.Time, 1024 * 1024);
            DataStream = new LogGroupStream(this, LogFileType.Data, 1024 * 1024);

            _fields = dataSource.GetDataFields().Where(x => fieldLimit.Any(y => x.Name==y)).Select(x => new LogField(this, x, GetNewFieldId())).ToList();
        }


        public bool Resubscribe(IDataNode dataSource)
        {
            DataSource = dataSource;
            Subscribed = true;

            var newFields = dataSource
                .GetDataFields()
                .Where(x => !_fields.Any(y => y.Name == x.Name))
                .Select(x => new LogField(this, x, GetNewFieldId()))
                .ToList();

            _fields.AddRange(newFields);

            // Reinit streams

            TimeStream = new LogGroupStream(this, LogFileType.Time, 1024 * 1024);
            DataStream = new LogGroupStream(this, LogFileType.Data, 1024 * 1024);

            return false;
        }

        protected int GetNewFieldId()
        {
            return fieldCounter++;
        }

        public void Close()
        {
            TimeStream.Close();
            DataStream.Close();

            // Save XML structure
            var xmlText = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            var structureFile = XmlWriter.Create(xmlText, settings);

            structureFile.WriteStartDocument();
            structureFile.WriteStartElement("group");
            structureFile.WriteStartElement("entity");
            structureFile.WriteAttributeString("name", this.Name);
            structureFile.WriteAttributeString("fields", this.Fields.Count().ToString());
            DataSource.GetDebugInfo(structureFile);
            structureFile.WriteEndElement();

            structureFile.WriteStartElement("fields");

            foreach (var field in _fields)
            {
                // Close it.
                structureFile.WriteStartElement("field");
                structureFile.WriteAttributeString("id", field.ID.ToString());
                structureFile.WriteAttributeString("name", field.Name);
                structureFile.WriteAttributeString("type", field.DataSource.ValueType.ToString());
                structureFile.WriteEndElement();

            }
            structureFile.WriteEndElement();
            structureFile.WriteEndElement();
            structureFile.WriteEndDocument();
            structureFile.Flush();

            var xmlString = xmlText.ToString();
            var xmlData = Encoding.ASCII.GetBytes(xmlString);
            GlobalEvents.Fire(new LogFileWriteAction(FileWriter, Name, LogFileType.Structure, xmlData, 0), false);

            Subscribed = false;
        }

        public void Update(int time)
        {
            if (!Subscribed)
                return;
            if (DataStream.Closed || TimeStream.Closed)
                return;

            var frameStart = DataStream.Index;

            // Update the data of all fields.
            foreach(var field in _fields)
            {
                var data = DataSource.ReadBytes(field.Name);

                if (data.Length > 0 && DataStream != null)
                {
                    var dataLength = 6 + data.Length;
                    if (dataLength % 4 != 0)
                        dataLength += 4 - (dataLength%4);

                    var outData = new byte[dataLength];
                    outData[0] = 0x1F;
                    outData[1] = 0x1F;
                    Array.Copy(BitConverter.GetBytes(field.ID), 0, outData, 2, 4);
                    Array.Copy(data, 0, outData, 6, data.Length);

                    DataStream.Write(outData);
                }
            }

            if (frameStart != DataStream.Index)
            {
                // There is new data, so write it to the TimeStream file.
                var timePtr = new byte[8];
                Array.Copy(BitConverter.GetBytes(time), 0, timePtr, 0, 4);
                Array.Copy(BitConverter.GetBytes(DataStream.Offset + frameStart), 0, timePtr, 4, 4);

                _timeline.Add(time, DataStream.Offset + frameStart);

                TimeStream.Write(timePtr);
            }
        }

        public byte[] ExtractDataBuffer()
        {
            return FileReader.ReadArchiveFile(Name + "/Data.bin");
        }

    }
}