using SimTelemetry.Core.Entities;
using SimTelemetry.Core.ValueObjects;

namespace SimTelemetry.Core.Events
{
    public class LapAdded
    {
        public LiveScoringDriver Driver { get; private set; }
        public Lap Lap { get; private set; }

        public LapAdded(LiveScoringDriver driver, Lap lap)
        {
            Driver = driver;
            Lap = lap;
        }
    }
}