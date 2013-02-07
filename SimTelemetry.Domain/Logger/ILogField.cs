using System;

namespace SimTelemetry.Domain.Logger
{
    public interface ILogField
    {
        uint ID { get; }
        string Name { get; }
        LogGroup Group { get; }
        Type ValueType { get; }
        TOut ReadAs<TOut>(byte[] data, int index);
    }
}