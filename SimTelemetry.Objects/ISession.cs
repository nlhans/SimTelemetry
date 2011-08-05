namespace SimTelemetry.Objects
{
    public interface ISession
    {
        [Loggable(0.1)]
        bool IsRace { get; set; }

        [Loggable(0.1)]
        string CircuitName { get; set; }

        [Loggable(0.1)]
        float TrackTemperature { get; set; }

        [Loggable(0.1)]
        float AmbientTemperature { get; set; }

        [Loggable(0.1)]
        SessionInfo Type { get; set; }

        [Loggable(10)]
        float Time { get; set; }

        [Loggable(1)]
        float TimeClock { get; set; }

        [Loggable(1)]
        int Cars_InPits { get; set; }

        [Loggable(1)]
        int Cars_OnTrack { get; set; }

        [Loggable(0.1)]
        int Cars { get; set; }
    }
}