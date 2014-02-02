using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class TrackUnloaded
    {
        public Track Track { get; private set; }

        public TrackUnloaded(Track t)
        {
            Track = t;
        }
    }
}