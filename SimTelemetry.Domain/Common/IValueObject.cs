using System;

namespace SimTelemetry.Domain.Common
{
    public interface IValueObject<T> : IEquatable<T>
    {
    }
}
