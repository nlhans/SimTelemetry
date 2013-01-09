using System;
using System.Runtime.Serialization;
using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Exceptions
{
    public class CarAlreadyHasDrivetrainException : Exception
    {
        public Car Car { get; private set; }

        public CarAlreadyHasDrivetrainException(string message, Car car)
            : base(message)
        {
            Car = car;
        }

        public CarAlreadyHasDrivetrainException(string message, Exception innerException, Car car)
            : base(message, innerException)
        {
            Car = car;
        }

        protected CarAlreadyHasDrivetrainException(SerializationInfo info, StreamingContext context, Car car)
            : base(info, context)
        {
            Car = car;
        }

        public CarAlreadyHasDrivetrainException(Car car)
        {
            Car = car;
        }
    }
}