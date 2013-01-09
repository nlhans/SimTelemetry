using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class CarAdded
    {
        public Car Car { get; private set; }

        public CarAdded(Car car)
        {
            Car = car;
        }

    }
}