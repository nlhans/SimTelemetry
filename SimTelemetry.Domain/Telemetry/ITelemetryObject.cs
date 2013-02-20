namespace SimTelemetry.Domain.Telemetry
{
    public interface ITelemetryObject
    {
        void Update(Aggregates.Telemetry telemInstance);
        void Update(Logger.LogFile logFile);
    }
}