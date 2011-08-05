using System;
using System.Diagnostics;
using System.IO;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    public class rFactor
    {
        public const double Revision = 0.1;

        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryReader Game;


        public static string rFactor_Directory
        {
            get
            {
                string meugen = rFactor.Game.ReadString(new IntPtr(0x00AEB320), 256);
                string rfactor_map = meugen.Substring(0, meugen.Length-Path.GetFileName(meugen).Length);
                return rfactor_map;
            }
        }

        public rFactor()
        {
            Process[] ps = Process.GetProcessesByName("rfactor");
            if(ps.Length==1)
            {
                Game = new MemoryReader();
                Game.ReadProcess = ps[0];
                Game.OpenProcess();

                
            }
            else
            {
                throw new Exception("Could not find rfactor");
            }

            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }

        public static string Track_AIW
        {
            get
            {
                return Game.ReadString(new IntPtr(0x00709D28), 256);
            }
        }
    }
}