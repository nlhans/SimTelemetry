using System;
using System.Runtime.Serialization;
using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Exceptions
{
    public class CarAlreadyHasWheelException : Exception
    {
        public Car Car { get; private set; }

        public CarAlreadyHasWheelException(string message, Car car)
            : base(message)
        {
            Car = car;
        }

        public CarAlreadyHasWheelException(string message, Exception innerException, Car car)
            : base(message, innerException)
        {
            Car = car;
        }

        protected CarAlreadyHasWheelException(SerializationInfo info, StreamingContext context, Car car)
            : base(info, context)
        {
            Car = car;
        }

        public CarAlreadyHasWheelException(Car car)
        {
            Car = car;
        }
    }
}