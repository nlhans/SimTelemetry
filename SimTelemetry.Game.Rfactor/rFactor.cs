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


        /*public static string rFactor_Directory
        {
            get
            {
                string meugen = rFactor.Game.ReadString(new IntPtr(0x00AEB320), 256);
                string rfactor_map = meugen.Substring(0, meugen.Length-Path.GetFileName(meugen).Length);
                return rfactor_map;
            }
        }

        public static string GameData_TrackFile
        {
            get
            {
                return Game.ReadString(new IntPtr(0x00709D28), 256);
            }
        }*/

        public static void Kill()
        {
            Game.Active = false;
        }
    }
}