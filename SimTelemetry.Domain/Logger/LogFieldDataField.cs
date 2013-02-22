using System;
using SimTelemetry.Domain.Telemetry;

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
}