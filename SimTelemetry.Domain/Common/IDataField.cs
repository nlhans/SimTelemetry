using System;

namespace SimTelemetry.Domain.Common
{
    public interface IDataField
    {
        string Name { get; }
        Type ValueType { get; }

        bool HasChanged();

        T ReadAs<T>();
    }
}