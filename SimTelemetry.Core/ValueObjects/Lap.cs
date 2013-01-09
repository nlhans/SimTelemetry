using System;
using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.ValueObjects
{
    public class Lap : IEquatable<Lap>
    {
        public int LapNumber { get; private set; }
        public Track Track { get; private set; }
        public float Sector1 { get; private set; }
        public float Sector2 { get; private set; }
        public float Sector3 { get; private set; }
        public float Total { get; private set; }

        public bool Equals(Lap other)
        {
            return (other.LapNumber == LapNumber && other.Total == Total);
        }
    }
}