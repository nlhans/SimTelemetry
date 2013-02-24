using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Domain.Telemetry
{
    public interface IFileAnnotater
    {
        bool QualifiesForStorage(TelemetryLogger logger);
        string GetPath(TelemetryLogger logger);
        void Store(TelemetryLogger logger, LogFileWriter writer);
    }
}