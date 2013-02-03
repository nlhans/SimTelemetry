using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Events
{
    public class TelemetryDriverRemoved
    {
        public TelemetryDriver Driver { get; private set; }

        public TelemetryDriverRemoved(TelemetryDriver driver)
        {
            Driver = driver;
        }
    }
}