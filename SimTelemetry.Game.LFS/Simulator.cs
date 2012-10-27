using System.ComponentModel.Composition;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.LFS
{
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
            new LFS();

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
            get { return "LFS"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "LFS"; }
        }

        public IDriverCollection Drivers
        {
            get { return LFS.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return LFS.Player; }
        }

        public ISession Session
        {
            get { return LFS.Session; }
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
