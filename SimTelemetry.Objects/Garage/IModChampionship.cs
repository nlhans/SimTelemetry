using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface IModChampionship
    {
        string File { get; }
        
        string Name { get; }
        int Opponents { get; }

        List<string> Tracks {get;}
    }
}