using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class CarLoaded
    {
        public Car Car { get; private set; }

        public CarLoaded(Car c)
        {
            Car = c;
        }
    }
}