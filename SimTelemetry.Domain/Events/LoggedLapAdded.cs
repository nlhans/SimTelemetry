using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Events
{
    // Lap added to log
    public class LoggedLapAdded
    {
        public ScoringDriver Driver { get; private set; }
        public Lap Lap { get; private set; }

        public LoggedLapAdded(ScoringDriver driver, Lap lap)
        {
            Driver = driver;
            Lap = lap;
        }
    }
}