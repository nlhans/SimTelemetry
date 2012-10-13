using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        private SimulatorModules _Modules;
        public ITelemetry Host { get; set; }

        public void Initialize()
        {
            new rFactor2(this);
            _Modules = new SimulatorModules();
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = true;
            _Modules.Times_LapsBasic = true;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = false;
            _Modules.Times_History_SectorTimes = false;
            _Modules.Engine_Power = true;
            _Modules.Engine_PowerCurve = true;
            _Modules.Aero_Drag = false;
        }

        public void Deinitialize()
        {
            rFactor2.Kill();
        }

        public string ProcessName
        {
            get { return "rfactor2"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "rFactor2 v1.005(beta)"; }
        }

        public IDriverCollection Drivers
        {
            get { return rFactor2.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return rFactor2.Player; }
        }

        public ISession Session
        {
            get { return rFactor2.Session; }
        }

        public IGarage Garage
        {
            get { return null; }
        }

        public MemoryPolledReader Memory
        {
            get { return rFactor2.Game; }
        }
    }
}
