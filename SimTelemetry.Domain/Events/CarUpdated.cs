using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class CarUpdated
    {
        public Car Car { get; private set; }

        public CarUpdated(Car car)
        {
            Car = car;
        }
    }
}