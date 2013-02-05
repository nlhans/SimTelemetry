using System;

namespace SimTelemetry.Domain.Logger
{
    public class LogField<T> : ILogField
    {
        public uint ID { get; protected set; }
        public string Name { get; protected set; }

        public LogGroup Group { get; protected set; }
        public LogFile File { get; protected set; }

        public Type FieldType { get; protected set; }

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

        public LogField(uint id, string name, LogGroup @group, LogFile file)
        {
            ID = id;
            Name = name;
            Group = group;
            File = file;
            FieldType = typeof(T);
        }


    }
}