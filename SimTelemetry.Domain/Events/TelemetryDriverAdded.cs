using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Events
{
    public class TelemetryDriverAdded
    {
        public TelemetryDriver Driver { get; private set; }

        public TelemetryDriverAdded(TelemetryDriver driver)
        {
            Driver = driver;
        }
    }
}