using System;

namespace SimTelemetry.Domain.Common
{
    public interface IEntity<T> : IEquatable<T>
    { 
        T ID { get; }
    }
}