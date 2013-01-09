using SimTelemetry.Core.Aggregates;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.ValueObjects
{
    public class Lap : IValueObject<Lap>
    {
        public int LapNumber { get; private set; }
        public Track Track { get; private set; }
        public float Sector1 { get; private set; }
        public float Sector2 { get; private set; }
        public float Sector3 { get; private set; }
        public float Total { get; private set; }

        public Lap(int lapNumber, Track track, float sector1, float sector2, float sector3)
        {
            LapNumber = lapNumber;
            Track = track;
            Sector1 = sector1;
            Sector2 = sector2;
            Sector3 = sector3;
            Total = sector1 + sector2 + sector3;
        }

        public bool Equals(Lap other)
        {
            return (other.LapNumber == LapNumber && other.Total == Total);
        }
    }
}