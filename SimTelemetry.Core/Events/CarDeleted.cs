using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Events
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