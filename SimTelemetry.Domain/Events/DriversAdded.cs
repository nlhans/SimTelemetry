using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Events
{
    public class DriversAdded
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }
        public IEnumerable<TelemetryDriver> Drivers { get; protected set; }

        public DriversAdded(Aggregates.Telemetry telemetry, IEnumerable<TelemetryDriver> drivers)
        {
            Telemetry = telemetry;
            Drivers = drivers;
        }
    }
}