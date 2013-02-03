using System.Diagnostics;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginSimulator : IPluginBase
    {
        Simulator GetSimulator();

        IPluginTelemetryProvider TelemetryProvider { get; }

        ILazyRepositoryDataSource<Car, string> CarProvider { get; }
        ILazyRepositoryDataSource<Track, string> TrackProvider { get; }

        void Initialize(); // Initialize Car&Track providers
        void SimulatorStart(Process p); // Initialize TelemetryProvider, based on exe-version.
        void SimulatorStop(); // Deinitialize TelemetryProvider
        void Deinitialize(); // Deinitialize Car&Track providers
    }
}
