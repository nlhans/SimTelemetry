using System;
using System.Diagnostics;
using System.IO;
using SimTelemetry.Objects.Utilities;
using Triton.Memory;
using SimTelemetry.Objects;

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
        
        public rFactor(ISimulator sim)
        {
            Simulator = sim;
            Game = new MemoryPolledReader(sim);

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