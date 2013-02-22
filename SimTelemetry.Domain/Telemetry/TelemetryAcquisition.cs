using System;
using SimTelemetry.Domain.LoggerO;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryAcquisition : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }

        public void Update(Aggregates.Telemetry telemetry)
        {

        }

        public void Update(LogFile logFile)
        {
            throw new NotImplementedException();
        }
    }
}