using System;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySession : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public bool IsActive { get; protected set; }
        public bool IsOffline { get; protected set; }
        public bool IsReplay { get; protected set; }
        public bool IsLoading { get; protected set; }

        public float Time { get; protected set; }
        public int Cars { get; protected set; }


        public void Update()
        {
            var sessionPool = Telemetry.Memory.Get("Session");

            Cars = sessionPool.ReadAs<int>("Cars");
            Time = sessionPool.ReadAs<float>("Time");

            IsActive = sessionPool.ReadAs<bool>("IsActive");
            IsOffline = sessionPool.ReadAs<bool>("IsOffline");
            IsReplay = sessionPool.ReadAs<bool>("IsReplay");
            IsLoading = sessionPool.ReadAs<bool>("IsLoading");
        }
        public TelemetrySession(Aggregates.Telemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}