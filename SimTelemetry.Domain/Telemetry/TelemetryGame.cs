using System;
using SimTelemetry.Domain.LoggerO;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryGame : ITelemetryObject
    {
        public string Version;

        public void Update(Aggregates.Telemetry telemetry)
        {

        }

        public void Update(LogFile logFile)
        {
            throw new NotImplementedException();
        }


        public TelemetryGame Clone()
        {
            return (TelemetryGame)MemberwiseClone();
        }
    }
}