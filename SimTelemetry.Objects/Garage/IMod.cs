using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface IMod
    {
        string File { get; }
        
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Website { get; }
        string Version { get; }

        string Directory_Vehicles { get; }

        /// <summary>
        /// Pitspeed in m/s
        /// </summary>
        int PitSpeed_Practice_Default { get; }

        /// <summary>
        /// Pitspeed in m/s
        /// </summary>
        int PitSpeed_Race_Default { get; }

        /// <summary>
        /// Number of unique opponents in the mod.
        /// </summary>
        int Opponents { get; }

        /// <summary>
        /// Championship data for this mod, including no. of opponents, title and tracks.
        /// </summary>
        List<IModChampionship> Championships { get; }

        List<ICar> Models { get; }

        void Scan();
    }
}