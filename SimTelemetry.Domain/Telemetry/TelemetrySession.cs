using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySession : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public float Time { get; protected set; }
        public int Cars { get; protected set; }

        public void Update()
        {
            var sessionPool = Telemetry.Memory.Get("Session");

            Time = sessionPool.ReadAs<float>("Time");
        }
        public TelemetrySession(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}