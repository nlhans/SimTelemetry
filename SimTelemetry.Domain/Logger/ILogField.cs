using System;

namespace SimTelemetry.Domain.Logger
{
    public interface ILogField
    {
        int ID { get; }
        string Name { get; }
        LogGroup Group { get; }
        Type ValueType { get; }
        bool IsConstant { get;  }
        //TOut ReadAs<TOut>(byte[] data, int index);
        T ReadAs<T>(byte[] databuffer, int index);
    }
}