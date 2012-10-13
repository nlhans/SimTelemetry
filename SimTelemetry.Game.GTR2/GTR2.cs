using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.GTR2
{
    public class GTR2
    {
        public static MemoryPolledReader Game {
    get
            {
                return Simulator.Game;
            }
        }

        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;

        public GTR2()
        {
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }
    }
    
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        public ITelemetry Host { get; set; }
        private SimulatorModules _Modules;
        private static MemoryPolledReader _Memory;
        public static MemoryPolledReader Game
        {
            get { return _Memory; }
        }
        public void Initialize()
        {
            _Memory = new MemoryPolledReader(this);
            new GTR2();

            _Modules = new SimulatorModules();
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = true;
            _Modules.Times_LapsBasic = true;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = true;
            _Modules.Engine_Power = true;
            _Modules.Engine_PowerCurve = true;
            _Modules.Aero_Drag = false;
        }

        public void Deinitialize()
        {
            //rFactor.Kill();
        }

        public string ProcessName
        {
            get { return "GTR2"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "GTR2 v1.1.0.0"; }
        }

        public IDriverCollection Drivers
        {
            get { return GTR2.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return GTR2.Player; }
        }

        public ISession Session
        {
            get { return GTR2.Session; }
        }

        public IGarage Garage
        {
            get { return null; }
        }

        public MemoryPolledReader Memory
        {
            get { return _Memory; }
        }
    }




}
