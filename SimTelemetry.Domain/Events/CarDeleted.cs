using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class CarDeleted
    {
        public Car Car { get; private set; }

        public CarDeleted(Car car)
        {
            Car = car;
        }
    }
}