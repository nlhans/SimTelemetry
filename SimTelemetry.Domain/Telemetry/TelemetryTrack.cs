using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryTrack : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public void Update()
        {

        }
        public TelemetryTrack(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}