using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class Lap : IValueObject<Lap>
    {
        public int Driver { get; private set; }
        public int LapNumber { get; private set; }

        public float Sector1 { get; private set; }
        public float Sector2 { get; private set; }
        public float Sector3 { get; private set; }
        public float Total { get; private set; }

        public bool OutLap { get; private set; }
        public bool InLap { get; private set; }

        public float TimeStart { get; private set; }
        public float TimeEnd { get; private set; }

        public bool Completed { get; private set; }

        public Lap(int driver, bool done, int lapNumber, float time, float sector1, float sector2, float sector3, bool inl, bool outl)
        {
            Driver = driver;
            LapNumber = lapNumber;
            Completed = done;
            Sector1 = sector1;
            Sector2 = sector2;
            Sector3 = sector3;
            Total = sector1 + sector2 + sector3;
            TimeStart = time;
            TimeEnd = time+ Total;

            InLap = inl;
            OutLap = outl;
        }

        public bool Equals(Lap other)
        {
            return (other.Driver==this.Driver && other.LapNumber == LapNumber && other.Total == Total);
        }
    }
}