using SimTelemetry.Domain.Memory;


namespace SimTelemetry.Domain.Logger
{
    public interface ITelemetryLogger
    {
        void Update();
        void Initialize(Aggregates.Telemetry telemetry, MemoryProvider memory);
        void Deinitialize();
    }
}