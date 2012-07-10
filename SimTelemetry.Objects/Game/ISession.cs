namespace SimTelemetry.Objects
{
    public interface ISession
    {
        [Unloggable]
        [LogProperty("Track Location", "Used for track loader. Location of track AIW.")]
        string GameData_TrackFile { get; set; }
        [Unloggable]
        [LogProperty("Game Location", "Used for track loader. Location of game directory.")]
        string GameDirectory { get; set; }

        [Loggable(true)]
        [LogProperty("No. of race laps", "Number of race laps in this session.")]
        int RaceLaps { get; set; }

        [Loggable(true)]
        [LogProperty("Is Race Session", "Indicates TRUE or FALSE whether this is a race session")]
        bool IsRace { get; set; }

        [Loggable(true)]
        [LogProperty("Track", "Name of the track")]
        string CircuitName { get; set; }

        [Loggable(true)]
        [LogProperty("Track Temperature", "The track asphalt temperature in degrees Celsius.")]
        float TrackTemperature { get; set; }

        [Loggable(true)]
        [LogProperty("Ambient Temperature", "The air temperature in degrees Celsius.")]
        float AmbientTemperature { get; set; }

        [Loggable(true)]
        [LogProperty("Session Info", "Information about the session. Not yet logged.")]
        SessionInfo Type { get; set; }

        [Loggable(10)]
        [LogProperty("Time", "The session time, with begin of session taken as zero. Seconds.")]
        float Time { get; set; }

        [Loggable(1)]
        [LogProperty("Clock", "The actual time of day.")]
        float TimeClock { get; set; }

        [Loggable(1)]
        [LogProperty("No. of cars in pits", "The amount of cars in the pit street.")]
        int Cars_InPits { get; set; }

        [Loggable(1)]
        [LogProperty("No. of cars on track", "The amount of cars on track.")]
        int Cars_OnTrack { get; set; }

        [Loggable(true)]
        [LogProperty("No. of cars", "Total amount of cars in s ession")]
        int Cars { get; set; }

        [Unloggable]
        [LogProperty("??", "Active?")]
        bool Active { get; set; }

        [Loggable(1)]
        [LogProperty("Flags: Full Course Yellow", "Indicates a full course yellow situation.")]
        bool Flag_YellowFull { get; set; }

        [Loggable(1)]
        [LogProperty("Flags: Red", "Indicates a red flag situation.")]
        bool Flag_Red { get; set; }

        [Loggable(1)]
        [LogProperty("Flags: Green", "Indicates a green flag situation, start/restart.")]
        bool Flag_Green { get; set; }

        [Loggable(1)]
        [LogProperty("Flags: Chequered Flag", "Indicates a race finish situation.")]
        bool Flag_Finish { get; set; }
    }
}