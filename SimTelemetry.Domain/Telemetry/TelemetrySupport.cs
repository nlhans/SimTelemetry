using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySupport : ITelemetryObject
    {
        public Dictionary<string, bool> Supported { get; protected set; }

        public void Update(Aggregates.Telemetry telemetry)
        {

            Supported = new Dictionary<string, bool>();
            Supported.Add("Driver.Meters", telemetry.Memory.Get("DriverTemplate").Fields.ContainsKey("MetersDriven"));

        }

        public void Update(LogFile logFile)
        {
            throw new NotImplementedException();
        }
    }
}