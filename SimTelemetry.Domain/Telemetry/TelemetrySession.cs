using System;
using System.Linq;
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
        public int LeaderLaps { get; protected set; }

        public double TrackTemperature { get; private set; }
        public double AmbientTemperature { get; private set; }

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

            TrackTemperature = sessionGroup.ReadAs<float>("TemperatureTrack");
            AmbientTemperature = sessionGroup.ReadAs<float>("TemperatureAmbient");
        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {
            IDataNode sessionGroup = Memory.Get("Session");

            // Laps leader/max info
            RaceLaps = sessionGroup.ReadAs<int>("LapsLimit");
            if (telemetry.Drivers.Any(x => x.Laps > 0))
                LeaderLaps = telemetry.Drivers.Max(x => x.Laps);

            var type = sessionGroup.ReadAs<SessionType>("Type");
            var index = sessionGroup.ReadAs<int>("TypeIndex");

            var typeAsString = "";

            switch(type)
            {
                case SessionType.PRACTICE:
                    typeAsString = "Practice " + index;
                    break;
                case SessionType.QUALIFY:
                    typeAsString = "Qualifying " + index;
                    break;
                case SessionType.RACE:
                    typeAsString = "Race " + index;
                    break;
                case SessionType.TEST_DAY:
                    typeAsString = "Test Day";
                    break;
                case SessionType.WARMUP:
                    typeAsString = "Warmup";
                    break;

                default:
                    typeAsString = "???";
                    break;
            }

            var timeLimit = sessionGroup.ReadAs<TimeSpan>("TimeLimit");

            // In some cases, like test sessions, the time limit is undefined and negative.
            // Then we clip to zero seconds.
            if(timeLimit.TotalSeconds < 0)
                timeLimit = new TimeSpan(0, 0, 0, 0);

            // construct session.
            Info = new Session(typeAsString,
                               type,
                               index,
                               sessionGroup.ReadAs<string>("Day"),
                               sessionGroup.ReadAs<Time>("StartTime"),
                               timeLimit,
                               sessionGroup.ReadAs<int>("LapsLimit"),
                               sessionGroup.ReadAs<int>("PitSpeed")
                );

            //Info = new Session("race 1", SessionType.PRACTICE, 1, "Sunday", new Time(16, 30, 0, 0), new TimeSpan(0, 3, 0, 0), 150, 80);
        }

        public TelemetrySession Clone()
        {
            return (TelemetrySession)MemberwiseClone();
        }
    }
}