using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryGame : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public string Version;

        public void Update()
        {

        }

        public TelemetryGame(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}