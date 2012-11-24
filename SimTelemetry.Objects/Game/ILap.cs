namespace SimTelemetry.Objects
{
    public delegate void LapEvent(ISimulator sim, ILap lap);
    public interface ILap
    {
        int LapNumber { get; }
        float Sector1 { get; }
        float Sector2 { get; }
        float Sector3 { get; }

        float LapTime { get; }
    }
}
