using System.Collections.Generic;

namespace SimTelemetry.Domain.Telemetry
{
    public interface ITelemetry
    {
        IEnumerable<TelemetryDriver> Drivers { get; }
    }
}
