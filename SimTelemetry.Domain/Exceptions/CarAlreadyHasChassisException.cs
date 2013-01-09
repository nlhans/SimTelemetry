using System;
using System.Runtime.Serialization;
using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Exceptions
{
    public class CarAlreadyHasChassisException : Exception
    {
        public Car Car { get; private set; }

        public CarAlreadyHasChassisException(string message, Car car)
            : base(message)
        {
            Car = car;
        }

        public CarAlreadyHasChassisException(string message, Exception innerException, Car car)
            : base(message, innerException)
        {
            Car = car;
        }

        protected CarAlreadyHasChassisException(SerializationInfo info, StreamingContext context, Car car)
            : base(info, context)
        {
            Car = car;
        }

        public CarAlreadyHasChassisException(Car car)
        {
            Car = car;
        }
    }
}