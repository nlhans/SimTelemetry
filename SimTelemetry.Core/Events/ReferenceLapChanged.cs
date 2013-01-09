using SimTelemetry.Core.Entities;

namespace SimTelemetry.Core.Events
{
    public class ReferenceLapChanged
    {
        public ScoringDriver Driver { get; private set; }

        public ReferenceLapChanged(ScoringDriver driver)
        {
            Driver = driver;
        }
    }
}