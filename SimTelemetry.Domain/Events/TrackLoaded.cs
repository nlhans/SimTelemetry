using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Events
{
    public class TrackLoaded
    {
        public Track Track { get; private set; }

        public TrackLoaded(Track t)
        {
            Track = t;
        }
    }
}