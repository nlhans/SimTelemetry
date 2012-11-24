using System.ComponentModel.Composition;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.FalconBMS
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
            new FalconBms();

            _Modules = new SimulatorModules();
            _Modules.Track_Coordinates = false;
            _Modules.Track_MapFile = false;
            _Modules.Times_LapsBasic = false;
            _Modules.Times_LastSectors = false;
            _Modules.Times_History_LapTimes = false;
            _Modules.Engine_Power = false;
            _Modules.Engine_PowerCurve = false;
            _Modules.Aero_Drag = false;
        }

        public void Deinitialize()
        {
            //rFactor.Kill();
        }

        public string ProcessName
        {
            get { return "Falcon BMS"; }
        }

        public SimulatorModules Modules
        {
            get { return _Modules; }
        }

        public string Name
        {
            get { return "FalconBMS"; }
        }

        public IDriverCollection Drivers
        {
            get { return FalconBms.Drivers; }
        }

        public IDriverPlayer Player
        {
            get { return FalconBms.Player; }
        }

        public ISession Session
        {
            get { return FalconBms.Session; }
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
