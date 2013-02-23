using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Logger
{
    public class LogSampleGroup : IDataNode
    {
        internal byte[] Buffer;

        private Dictionary<int, int> TimeLine_Current = new Dictionary<int, int>();
        private Dictionary<int, int> TimeLine_Next = new Dictionary<int, int>();

        public string Name { get; protected set; }

        public Dictionary<string, IDataField> Fields { get { return _fields; } }
        private Dictionary<string, IDataField> _fields = new Dictionary<string, IDataField>();
        private Dictionary<int, IDataField> _fieldById = new Dictionary<int, IDataField>();

        public LogSampleGroup (LogGroup group)
        {
            Buffer = group.ExtractDataBuffer();
            Name = group.Name;

            /**** Create custom timeline containing offset for current and next time samples *****/
            TimeLine_Current = group.Timeline;
            var lastTime = -123;
            foreach(var kvp in TimeLine_Current)
            {
                if (lastTime != -123)
                    TimeLine_Next.Add(lastTime, kvp.Value);
                lastTime = kvp.Key;
            }
            TimeLine_Next.Add(lastTime, Buffer.Length);

            foreach (var field in group.Fields)
            {
                var logFieldType = typeof (LogSampleField<>).MakeGenericType(new[] {field.ValueType});
                var logFieldInstance =
                    (IDataField)
                    Activator.CreateInstance(logFieldType, new object[3] {field.Name, field.ValueType, this});
                _fields.Add(field.Name, logFieldInstance);
                _fieldById.Add(field.ID, logFieldInstance);
            }
        }

        internal void Close()
        {
            Buffer = null;
        }

        public T ReadAs<T>(string field)
        {
            if (Fields.ContainsKey(field) == false) 
                return (T)Convert.ChangeType(0, typeof(T));

            return Fields[field].ReadAs<T>();
        }

        public IEnumerable<IDataField> GetDataFields()
        {
            return Fields.Values;
        }

        public byte[] ReadBytes(string field)
        {
            return new byte[0];
        }

        public void GetDebugInfo(XmlWriter file)
        {
            // nothing
        }

        public IDataNode Clone(string newName, int newAddress)
        {
            return (IDataNode) MemberwiseClone();
        }

        public void Update(int timestamp, bool onlyThisSample)
        {
            if ( !TimeLine_Current.ContainsKey(timestamp)) return;

            var startOffset = TimeLine_Current[timestamp];
            var endOffset = TimeLine_Next[timestamp];
            var index = endOffset - 1;
            if (index % 4 != 0)
                index += 4 - (index % 4);
            if (index >= Buffer.Length)
                index = Buffer.Length - (Buffer.Length % 4) - 4;

            if (onlyThisSample)
            {
                // Fast scanning.

                // Scan backwards
                // Untill either; we reached end of our sample containing data.
                // Or: we have run out of fields to initialize.
                while (index >= startOffset)
                {
                    bool isValue = (Buffer[index] == 0x1F)
                                   && (Buffer[index + 1] == 0x1F);

                    if (!isValue)
                    {
                        index -= 4;
                        continue;
                    }

                    int fieldId = BitConverter.ToInt32(Buffer, index + 2);

                    if (!_fieldById.ContainsKey(fieldId))
                    {
                        index -= 4;
                        continue;
                    }
                    ((ILogSampleField) _fieldById[fieldId]).Offset = index + 6;
                    index -= 8;
                }
            }
            else
            {
                startOffset = 0; // scan all the way to beginning if required.

                // Extended scanning:
                var fieldsToDo = _fieldById.ToDictionary(x => x.Key, x => 1);
                var fieldsToDoCount = fieldsToDo.Count;

                if (fieldsToDoCount == 0)
                    return;

                // Scan backwards
                // Untill either; we reached end of our sample containing data.
                // Or: we have run out of fields to initialize.
                while (index >= startOffset)
                {
                    bool isValue = (Buffer[index] == 0x1F)
                                   && (Buffer[index + 1] == 0x1F);

                    if (!isValue)
                    {
                        index -= 4;
                        continue;
                    }

                    int fieldId = BitConverter.ToInt32(Buffer, index + 2);

                    if (!fieldsToDo.ContainsKey(fieldId)
                      || !_fieldById.ContainsKey(fieldId))
                    {
                        index -= 4;
                        continue;
                    }

                    ((ILogSampleField) _fieldById[fieldId]).Offset = index + 6;
                    fieldsToDo.Remove(fieldId);
                    fieldsToDoCount--;
                    if (fieldsToDoCount == 0) break;

                    index -= 8;
                }
            }
        }
    }
}