using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.LoggerO
{
    public class LogGroup : ILogNode, IEquatable<LogGroup>
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }
        public ILogNode Master { get; protected set; }

        public LogFile File { get; protected set; }

        public IEnumerable<ILogField> Fields { get { return _fields; } }
        protected IList<ILogField> _fields = new List<ILogField>();

        public IEnumerable<LogGroup> Groups { get { return _groups; } }
        protected IList<LogGroup> _groups = new List<LogGroup>();

        public bool Equals(LogGroup other)
        {
            return other.ID == ID;
        }

        public ILogNode FindGroup(int id)
        {
            return Groups.Where(x => x.ID == id).FirstOrDefault();
        }

        public ILogNode FindGroup(string name)
        {
            return Groups.Where(x => x.Name == name).FirstOrDefault();
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

        public T ReadAs<T>(string field)
        {
            return File.ReadAs<T>(Name, field, File.TimeReplay);
        }

        public LogGroup CreateGroup(string name)
        {
            if (File.ReadOnly) return null;
            var oGroup = new LogGroup(File.RequestNewGroupId(), name, this, File);
            _groups.Add(oGroup);
            return oGroup;
        }

        public LogGroup CreateGroup(string name, int id)
        {
            if (!File.ReadOnly) return null;
            var oGroup = new LogGroup(id, name, this, File);
            _groups.Add(oGroup);
            return oGroup;
        }

        public LogField<T> CreateField<T>(string name, bool isConstant)
        {
            var field = new LogField<T>(File.RequestNewFieldId(), name, this, File, isConstant);
            _fields.Add(field);
            return field;
        }

        public ILogField CreateField(string name, Type valueType, bool isConstant)
        {
            if (valueType == null) return null;
            var logFieldType = typeof(LogField<>).MakeGenericType(new[] { valueType });
            var logFieldInstance = (ILogField)Activator.CreateInstance(logFieldType,
                                                            new object[5] { File.RequestNewFieldId(), name, this, File , isConstant });
            _fields.Add(logFieldInstance);
            return logFieldInstance;
        }


        public ILogField CreateField(string name, Type valueType, int id, bool isConstant)
        {
            if (valueType == null) return null;
            var logFieldType = typeof(LogField<>).MakeGenericType(new[] { valueType });
            var logFieldInstance = (ILogField)Activator.CreateInstance(logFieldType,
                                                            new object[5] { id, name, this, File, isConstant });
            _fields.Add(logFieldInstance);
            return logFieldInstance;
        }

        public LogGroup(int id, string name, LogFile file)
        {
            ID = id;
            Name = name;
            File = file;
        }

        public LogGroup(int id, string name, ILogNode master, LogFile file)
        {
            ID = id;
            Name = name;
            Master = master;
            File = file;
        }

    }
}