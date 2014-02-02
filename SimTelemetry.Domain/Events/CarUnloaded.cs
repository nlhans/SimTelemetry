using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class CarUnloaded
    {
        public Car Car { get; private set; }

        public CarUnloaded(Car c)
        {
            Car = c;
        }
    }
}