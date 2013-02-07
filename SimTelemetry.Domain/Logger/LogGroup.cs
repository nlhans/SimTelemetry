using System;
using System.Collections.Generic;

namespace SimTelemetry.Domain.Logger
{
    public class LogGroup : ILogNode, IEquatable<LogGroup>
    {
        public uint ID { get; protected set; }
        public string Name { get; protected set; }
        public LogGroup Master { get; protected set; }

        public LogFile File { get; protected set; }

        public IEnumerable<ILogField> Fields { get { return _fields; } }
        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected IList<ILogField> _fields = new List<ILogField>();
        protected IList<LogGroup> _groups = new List<LogGroup>();

        public bool Equals(LogGroup other)
        {
            return other.ID == ID;
        }

        public LogGroup CreateGroup(string name)
        {
            if (File.ReadOnly) return null;
            var oGroup = new LogGroup(File.RequestNewGroupId(), name, this, File);
            _groups.Add(oGroup);
            return oGroup;
        }

        public LogGroup CreateGroup(string name, uint id)
        {
            if (!File.ReadOnly) return null;
            var oGroup = new LogGroup(id, name, this, File);
            _groups.Add(oGroup);
            return oGroup;
        }


        public LogField<T> CreateField<T>(string name)
        {
            var field = new LogField<T>(File.RequestNewFieldId(), name, this, File);
            _fields.Add(field);
            return field;
        }

        public ILogField CreateField(string name, Type valueType)
        {
            if (valueType == null) return null;
            var logFieldType = typeof(LogField<>).MakeGenericType(new[] { valueType });
            var logFieldInstance = (ILogField)Activator.CreateInstance(logFieldType,
                                                            new object[4] { File.RequestNewFieldId(), name, this, File });
            _fields.Add(logFieldInstance);
            return logFieldInstance;
        }


        public ILogField CreateField(string name, Type valueType, uint id)
        {
            if (valueType == null) return null;
            var logFieldType = typeof(LogField<>).MakeGenericType(new[] { valueType });
            var logFieldInstance = (ILogField)Activator.CreateInstance(logFieldType,
                                                            new object[4] { id, name, this, File });
            _fields.Add(logFieldInstance);
            return logFieldInstance;
        }

        public LogGroup(uint id, string name, LogFile file)
        {
            ID = id;
            Name = name;
            File = file;
        }

        public LogGroup(uint id, string name, LogGroup master, LogFile file)
        {
            ID = id;
            Name = name;
            Master = master;
            File = file;
        }
    }
}