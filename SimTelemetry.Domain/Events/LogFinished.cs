using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Telemetry;

namespace SimTelemetry.Domain.Events
{
    public class LogFinished
    {
        public LogFileWriter LogWriter { get; protected set; }
        public TelemetryLoggerConfiguration Configuration { get; protected set; }

        public LogFinished(LogFileWriter logWriter, TelemetryLoggerConfiguration configuration)
        {
            LogWriter = logWriter;
            Configuration = configuration;
        }
    }
}