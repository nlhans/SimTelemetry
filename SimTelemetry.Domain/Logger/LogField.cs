using System;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Logger
{
    public class LogFieldDataField : IDataField
    {
        public string Name { get; protected set; }
        public Type ValueType { get; protected set; }

        public LogFieldDataField(string name, Type valueType)
        {
            Name = name;
            ValueType = valueType;
        }

        public LogFieldDataField(string name, string type)
        {
            Name = name;
            ValueType = Type.GetType(type);
        }

        public bool HasChanged()
        {
            return true;
        }

        public T1 ReadAs<T1>()
        {
            throw new NotImplementedException();
        }
    }

    public class LogField
    {
        public int ID;
        public string Name { get; protected set; }
        public LogGroup Group { get; protected set; }
        public IDataField DataSource { get; protected set; }

        public Type ValueType { get; protected set; }

        public LogField(LogGroup group, IDataField field, int id)
        {
            ID = id;
            Name = field.Name;
            Group = group;
            DataSource = field;
            this.ValueType = field.ValueType;
        }

        public LogField(LogGroup group, string name, string id, string type)
        {
            ID = Int32.Parse(id);
            Name = name;
            Group = group;
            DataSource = new LogFieldDataField(name, type);
            this.ValueType = Type.GetType(type);
        }

        public bool HasChanged()
        {
            return DataSource.HasChanged();
        }
    }
}