using System;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.LoggerO
{
    public class LogField<T> : ILogField
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }

        public bool IsConstant { get; protected set; }
        public TOutput ReadAs<TOutput>(byte[] databuffer, int index)
        {
            return MemoryDataConverter.Unrawify<T, TOutput>(databuffer, index);
        }

        public LogGroup Group { get; protected set; }
        public LogFile File { get; protected set; }

        public Type ValueType { get; protected set; }

        public int SampleOffset;
        public T[] Data;

        public T ReadAs<T>(double time)
        {
            //
            return (T)new object();
        }
        public T ReadAs<T>(int sample)
        {
            //
            return (T)new object();
        }

        public LogField(int id, string name, LogGroup @group, LogFile file, bool isConstant)
        {
            ID = id;
            Name = name;
            Group = group;
            File = file;
            ValueType = typeof(T);
            IsConstant = isConstant;
        }


    }
}