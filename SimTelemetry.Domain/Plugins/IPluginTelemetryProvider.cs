using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Plugins
{
    public interface IPluginTelemetryProvider
    {
        /// <summary>
        /// Initialize the simulator's memory structure description,
        /// - Simulator (Game data, version, etc.)
        /// - Session
        /// - Track
        /// - Driver list pool
        /// - Generic driver object template
        /// Do (additional) signature scanning here, and try to standardize in template objects.
        /// And, last but not least: add custom object converters for SessionType, SectorType etc.
        /// To the MemoryDataConverter
        /// </summary>
        /// <param name="provider"></param>
        void Initialize(MemoryProvider provider);

        // Create pool based on given template, and extra fields if necessary.
        MemoryPool CreateDriver(MemoryPool template, bool isPlayer);

        // Clear all your helper objects beside memorypool
        void Deinitialize();
    }
}