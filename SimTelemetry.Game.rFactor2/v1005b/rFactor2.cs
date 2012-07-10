using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    public class rFactor2
    {
        public const double Revision = 0.1;
        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryPolledReader Game;
        private static Simulator Sim;

        public rFactor2(Simulator simulator)
        {
            Sim = simulator;
            Game = new MemoryPolledReader(simulator);

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
