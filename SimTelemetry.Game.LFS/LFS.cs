using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.LFS
{
    public class LFS
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

        public LFS()
        {
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }
    }
}