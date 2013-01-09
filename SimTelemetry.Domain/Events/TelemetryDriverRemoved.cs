using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class TelemetryDriverRemoved
    {
        public ITelemetryDriver Driver { get; private set; }

        public TelemetryDriverRemoved(ITelemetryDriver driver)
        {
            Driver = driver;
        }
    }
}