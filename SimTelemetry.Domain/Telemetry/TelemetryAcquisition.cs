using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryAcquisition : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public void Update()
        {

        }
        public TelemetryAcquisition(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}