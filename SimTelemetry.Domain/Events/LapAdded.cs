using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Events
{
    public class LapAdded
    {
        public ScoringDriver Driver { get; private set; }
        public Lap Lap { get; private set; }

        public LapAdded(ScoringDriver driver, Lap lap)
        {
            Driver = driver;
            Lap = lap;
        }
    }
}