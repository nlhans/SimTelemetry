using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.FalconBMS
{
    public class FalconBms
    {
        
        public static MemoryPolledReader Game
        {
            get
            {
                return Simulator.Game;
            }
        }

        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;

        public FalconBms()
        {
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }
    }
}