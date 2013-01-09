using SimTelemetry.Domain.Entities;

namespace SimTelemetry.Domain.Events
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