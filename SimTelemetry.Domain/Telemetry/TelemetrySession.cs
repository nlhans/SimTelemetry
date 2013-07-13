using System;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

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

        public int RaceLaps { get; protected set; }

        public double TrackTemperature
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public double AmbientTemperature
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Session Info { get; private set; }

        public string Track { get; private set; }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
            IDataNode sessionGroup = Memory.Get("Session");

            Cars = sessionGroup.ReadAs<int>("Cars");
            Time = sessionGroup.ReadAs<float>("Time");

            IsActive = sessionGroup.ReadAs<bool>("IsActive");
            IsOffline = sessionGroup.ReadAs<bool>("IsOffline");
            IsReplay = sessionGroup.ReadAs<bool>("IsReplay");
            IsLoading = sessionGroup.ReadAs<bool>("IsLoading");

            Track = sessionGroup.ReadAs<string>("LocationTrack");

           Info = new Session("race 1", SessionType.PRACTICE, 1, "Sunday", new Time(16,30,0,0), new TimeSpan(0,3,0,0), 150, 80);
        }

        public TelemetrySession Clone()
        {
            return (TelemetrySession)MemberwiseClone();
        }
    }
}