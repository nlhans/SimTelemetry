using System;
using SimTelemetry.Core.Enumerations;

namespace SimTelemetry.Core.ValueObjects
{
    public class Session
    {
        public string Name { get; private set; }
        public SessionType Type { get; private set; }

        public string Day { get; private set; }
        public Time Start { get; private set; }

        public TimeSpan Duration { get; private set; }
        public int Laps { get; private set; }

        public int PitlaneSpeed { get; private set; }
    }
}