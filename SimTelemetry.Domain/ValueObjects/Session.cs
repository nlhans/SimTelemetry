using System;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Enumerations;

namespace SimTelemetry.Domain.ValueObjects
{
    public class Session : IValueObject<Session>
    {
        public string Name { get; private set; }
        public SessionType Type { get; private set; }

        public string Day { get; private set; }
        public Time Start { get; private set; }

        public TimeSpan Duration { get; private set; }
        public int Laps { get; private set; }

        public int PitlaneSpeed { get; private set; }

        public Session(string name, SessionType type, string day, Time start, TimeSpan duration, int laps, int pitlaneSpeed)
        {
            Name = name;
            Type = type;
            Day = day;
            Start = start;
            Duration = duration;
            Laps = laps;
            PitlaneSpeed = pitlaneSpeed;
        }

        public bool Equals(Session other)
        {
            return other.Type == Type && Name == other.Name;
        }
    }
}