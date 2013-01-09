using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginSimulator : IPluginBase
    {
        Simulator GetSimulator();
        Telemetry GetTelemetry();
        ICarDataProvider GetCarDataProvider();
        ITrackDataProvider GetTrackDataProvider();

        void Initialize();
        void Deinitialize();
    }
}
