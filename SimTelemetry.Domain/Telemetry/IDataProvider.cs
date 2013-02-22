using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Telemetry
{
    public interface IDataProvider
    {
        IDataNode Get(string name);
    }
}