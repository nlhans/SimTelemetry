using SimTelemetry.Game.rFactor2.Garage;
using SimTelemetry.Game.rFactor2.v1005b;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.rFactor2
{
    public class rFactor2
    {
        public const double Revision = 0.1;
        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryPolledReader Game;
        private static Simulator Sim;
        public static rFactor2Garage Garage;


        // TODO: This class should do version detect first before initializing session, drivers and driverplayer classes.
        
        public rFactor2(Simulator simulator)
        {
            Sim = simulator;
            Game = new MemoryPolledReader(simulator);

            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
            Garage = new rFactor2Garage();
        }

        public static void Kill()
        {

            Game.Active = false;
        }
    }
}
