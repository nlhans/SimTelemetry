using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Events
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