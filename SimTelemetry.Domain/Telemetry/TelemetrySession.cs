using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.LoggerO;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetrySession : ITelemetryObject
    {
        public bool IsActive { get; protected set; }
        public bool IsOffline { get; protected set; }
        public bool IsReplay { get; protected set; }
        public bool IsLoading { get; protected set; }

        public float Time { get; protected set; }
        public int Cars { get; protected set; }

        protected void Update(IDataNode sesionGroup)
        {
            Cars = sesionGroup.ReadAs<int>("Cars");
            Time = sesionGroup.ReadAs<float>("Time");

            IsActive = sesionGroup.ReadAs<bool>("IsActive");
            IsOffline = sesionGroup.ReadAs<bool>("IsOffline");
            IsReplay = sesionGroup.ReadAs<bool>("IsReplay");
            IsLoading = sesionGroup.ReadAs<bool>("IsLoading");
        }

        public void Update(Aggregates.Telemetry  telemetry)
        {
            var sessionPool = (IDataNode)telemetry.Memory.Get("Session");
            Update(sessionPool);
        }

        public void Update(LogFile logFile)
        {
            var sessionPool = (IDataNode)logFile.FindGroup("Session");
            Update(sessionPool);
        }

        public TelemetrySession Clone()
        {
            return (TelemetrySession)MemberwiseClone();
        }
    }
}