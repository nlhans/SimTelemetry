using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryDriver : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public void Update()
        {

        }

        public TelemetryDriver(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}