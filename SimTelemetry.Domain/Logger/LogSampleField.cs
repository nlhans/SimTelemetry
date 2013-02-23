using System;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Logger
{
    public class LogSampleField<T> : IDataField, ILogSampleField
    {
        public string Name { get; protected set; }
        public Type ValueType { get; protected set; }
        private LogSampleGroup Group { get; set; }
        public int Offset { get; set; }

        public LogSampleField(string name, Type valueType, LogSampleGroup @group)
        {
            Name = name;
            ValueType = valueType;
            Group = group;
            Offset = -1;
        }


        public void SetOffset(int offset)
        {
            Offset = offset;
        }

        public TOut ReadAs<TOut>()
        {
            if (Offset == -1) // no offset initialized; field not in file??
                return (TOut) Convert.ChangeType(0, typeof (TOut));
            return MemoryDataConverter.Unrawify<T, TOut>(Group.Buffer, Offset);
        }

        public bool HasChanged()
        {
            return true;
        }
    }
}