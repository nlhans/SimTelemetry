namespace SimTelemetry.Domain.Telemetry
{
    public interface ITelemetryObject
    {
        Aggregates.Telemetry Telemetry { get; }

        void Update();
    }
}