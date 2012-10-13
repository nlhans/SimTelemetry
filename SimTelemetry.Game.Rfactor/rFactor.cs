using SimTelemetry.Objects.Utilities;
using SimTelemetry.Objects;
using SimTelemetry.Game.Rfactor.Garage;

namespace SimTelemetry.Game.Rfactor
{
    public class rFactor
    {
        public static ISimulator Simulator { get; set; }
        public const double Revision = 0.1;

        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryPolledReader Game;
        public static rFactorGarage Garage;
        
        public rFactor(ISimulator sim)
        {
            Simulator = sim;
            Game = new MemoryPolledReader(sim);

            Garage = new rFactorGarage();
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }

        public static void Kill()
        {
            Game.Active = false;
        }
    }
}