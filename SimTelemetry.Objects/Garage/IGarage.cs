using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface IGarage
    {
        string InstallationDirectory { get; }
        string GamedataDirectory { get; }

        string Simulator { get; }
        bool Available { get; }

        bool Available_Tracks { get; }
        bool Available_Mods { get; }

        List<IMod> Mods { get; }
        List<ITrack> Tracks { get; }

        /// <summary>
        /// Fill-up data for Mods and tracks. Do-not yet fill them out completely as this will take too much time.
        /// On request data should be read and stored(cached).
        /// </summary>
        void Scan();

    }
}