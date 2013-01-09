using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Events
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