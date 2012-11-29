using System.Collections.Generic;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Network
{
    public class NetworkGarage : IGarage
    {
        public string InstallationDirectory { get; private set; }
        public string GamedataDirectory { get; private set; }
        public string Simulator { get; private set; }
        public bool Available { get { return false; } set { } }
        public bool Available_Tracks { get { return false; } set { } }
        public bool Available_Mods { get { return false; } set { } }
        public List<IMod> Mods { get; private set; }
        public List<ITrack> Tracks { get; private set; }


        public void Scan()
        {
            return;
        }

        public ICar CarFactory(IMod mod, string veh)
        {
            return null;
        }
    }
}