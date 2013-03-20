using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Events
{
    public class TelemetryLapComplete
    {
        public TelemetryLapComplete(Aggregates.Telemetry tel, TelemetryDriver driver, Lap lap)
        {
            Telemetry = tel;
            Driver = driver;
            Lap = lap;
        }

        public Aggregates.Telemetry Telemetry { get; private set; }
        public TelemetryDriver Driver { get; private set; }
        public Lap Lap { get; private set; }
    }
}