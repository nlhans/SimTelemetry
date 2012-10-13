using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.Rfactor
{
    [Export(typeof(ISimulator))]
    public class Simulator : ISimulator
    {
        private SimulatorModules _Modules;
        public ITelemetry Host { get; set; }

        public void Initialize()
        {
            new rFactor(this);
            _Modules = new SimulatorModules();
            _Modules.Time_Available = true;             // The plug-in knows the session time.
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = true;
            _Modules.Times_LapsBasic = true;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = true;
            _Modules.Times_History_SectorTimes = true;
            _Modules.Engine_Power = true;
            _Modules.Engine_PowerCurve = true;
            _Modules.Aero_Drag = true;
        }

        public void Deinitialize()
        {
            rFactor.Kill();
        }

        public string ProcessName
        {
            get { return "rfactor"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "rFactor v1.255"; }
        }

        public IDriverCollection Drivers
        {
            get { return rFactor.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return rFactor.Player; }
        }

        public ISession Session
        {
            get { return rFactor.Session; }
        }

        public IGarage Garage
        {
            get { return null; }
        }

        public MemoryPolledReader Memory
        {
            get { return rFactor.Game; }
        }
    }
}
