using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Repositories;

namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginSimulator : IPluginBase
    {
        Simulator GetSimulator();
        Telemetry GetTelemetry();

        ILazyRepositoryDataSource<Car, string> CarProvider { get; }
        ILazyRepositoryDataSource<Track, string> TrackProvider { get; }

        void Initialize();
        void Deinitialize();
    }
}
