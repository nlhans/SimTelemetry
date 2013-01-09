using SimTelemetry.Core.Entities;

namespace SimTelemetry.Core.Events
{
    public class ReferenceLapChanged
    {
        public LiveScoringDriver Driver { get; private set; }

        public ReferenceLapChanged(LiveScoringDriver driver)
        {
            Driver = driver;
        }
    }
}