namespace SimTelemetry.Objects
{
    public interface ILap
    {
        int LapNumber { get; }
        float Sector1 { get; }
        float Sector2 { get; }
        float Sector3 { get; }

        float LapTime { get; }
    }
}
