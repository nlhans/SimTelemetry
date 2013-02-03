using System;
using System.Collections.Generic;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySupport : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public Dictionary<string, bool> Supported { get; protected set; }

        public void Update()
        {

        }

        public TelemetrySupport(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;

            Supported = new Dictionary<string, bool>();
            Supported.Add("Driver.Meters", telemetry.Memory.Get("DriverTemplate").Fields.ContainsKey("MetersDriven"));
        }
    }
}