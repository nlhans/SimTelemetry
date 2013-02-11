using System.Collections.Generic;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Events
{
    public class DriversRemoved
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }
        public IEnumerable<TelemetryDriver> Drivers { get; protected set; }

        public DriversRemoved(Aggregates.Telemetry telemetry, IEnumerable<TelemetryDriver> drivers)
        {
            Telemetry = telemetry;
            Drivers = drivers;
        }
    }
}