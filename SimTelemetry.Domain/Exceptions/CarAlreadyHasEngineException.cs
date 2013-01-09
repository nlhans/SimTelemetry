using System;
using System.Runtime.Serialization;
using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Exceptions
{
    public class CarAlreadyHasEngineException : Exception
    {
        public Car Car { get; private set; }

        public CarAlreadyHasEngineException(string message, Car car) : base(message)
        {
            Car = car;
        }

        public CarAlreadyHasEngineException(string message, Exception innerException, Car car) : base(message, innerException)
        {
            Car = car;
        }

        protected CarAlreadyHasEngineException(SerializationInfo info, StreamingContext context, Car car) : base(info, context)
        {
            Car = car;
        }

        public CarAlreadyHasEngineException(Car car)
        {
            Car = car;
        }
    }
}