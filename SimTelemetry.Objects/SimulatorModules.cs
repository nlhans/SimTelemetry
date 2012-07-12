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
}