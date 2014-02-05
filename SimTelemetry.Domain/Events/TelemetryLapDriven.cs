using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Events
{
    // Lap added to telemetry collection
    public class TelemetryLapDriven
    {
        public TelemetryDriver Driver { get; private set; }
        public Lap Lap { get; private set; }

        public TelemetryLapDriven(TelemetryDriver drv, Lap lap)
        {
            Driver = drv;
            Lap = lap;
        }
    }
}