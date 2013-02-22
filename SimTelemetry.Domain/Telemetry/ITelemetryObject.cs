namespace SimTelemetry.Domain.Telemetry
{
    public interface ITelemetryObject
    {
        void Update(Aggregates.Telemetry telemInstance);
        void Update(LoggerO.LogFile logFile);
    }
}