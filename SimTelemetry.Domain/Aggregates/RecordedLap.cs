using System;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class RecordedLap : IEntity<int>, IEquatable<RecordedLap>
    {
        public int ID { get; private set; }
        public string File { get; private set; }

        public Simulator Simulator { get; private set; }

        public Car Car { get; private set; }
        public Track Track { get; private set; }

        public string Driver { get; private set; }

        public Lap Lap { get; private set; }

        public int SessionTimeStart { get; private set; }
        public int SessionTimeEnd { get; private set; }

        public bool Equals(RecordedLap other) { return other.ID == ID; }
        public bool Equals(int other) { return other == ID; }

        // etc.

        public RecordedLap(int id, string file, Lap lap, int sessionTimeStart, int sessionTimeEnd)
        {
            ID = id;
            File = file;
            Lap = lap;
            SessionTimeStart = sessionTimeStart;
            SessionTimeEnd = sessionTimeEnd;
        }
    }
}