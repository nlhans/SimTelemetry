using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Objects
{
    public struct SimulatorModules
    {
        public bool Times_LapsBasic;
        public bool Times_BestSectors;
        public bool Times_LastSectors;
        public bool Times_AllSectors;
        public bool Times_History_LapTimes;
        public bool Times_History_SectorTimes;
        public bool Times_RaceSplits;

        public bool Engine_Power;
        public bool Engine_PowerCurve;
        public bool Engine_Health;

        public bool Aero_Drag_Cw;
        public bool Aero_Lift;

        public bool Track_MapFile;
        public bool Track_Coordinates;
    }

    public interface ISimulator
    {
        ITelemetry Host { get; set; }

        void Initialize();
        void Deinitialize();

        string Name { get; }
        string ProcessName { get; }

        SimulatorModules Modules { get; }

        IDriverCollection Drivers { get; }
        IDriverPlayer Player { get; }
        ISession Session { get; }

        MemoryPolledReader Memory { get; }
        //bool Attached { get; }
    }
}