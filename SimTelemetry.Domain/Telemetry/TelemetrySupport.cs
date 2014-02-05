using System;
using System.Collections.Generic;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySupport : ITelemetryObject
    {
        public Dictionary<string, bool> Supported { get; protected set; }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
            Supported = new Dictionary<string, bool>();
            Supported.Add("Driver.Meters", Memory.Get("DriverTemplate").Fields.ContainsKey("MetersDriven"));

        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {
            //
        }
    }
}