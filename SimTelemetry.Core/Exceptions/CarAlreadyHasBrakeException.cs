using System;
using System.Runtime.Serialization;
using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Exceptions
{
    public class CarAlreadyHasBrakeException : Exception
    {
        public Car Car { get; private set; }

        public CarAlreadyHasBrakeException(string message, Car car)
            : base(message)
        {
            Car = car;
        }

        public CarAlreadyHasBrakeException(string message, Exception innerException, Car car)
            : base(message, innerException)
        {
            Car = car;
        }

        protected CarAlreadyHasBrakeException(SerializationInfo info, StreamingContext context, Car car)
            : base(info, context)
        {
            Car = car;
        }

        public CarAlreadyHasBrakeException(Car car)
        {
            Car = car;
        }
    }
}