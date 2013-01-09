using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class TelemetryDriverAdded
    {
        public ITelemetryDriver Driver { get; private set; }

        public TelemetryDriverAdded(ITelemetryDriver driver)
        {
            Driver = driver;
        }
    }
}