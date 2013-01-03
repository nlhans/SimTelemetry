using System.Runtime.InteropServices;

namespace SimTelemetry.Game.Rfactor.MMF
{
    [StructLayout(LayoutKind.Explicit, Size=124, Pack=1)]
    public struct rFactorSession
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string TrackName;

        [FieldOffset(64)]
        public int RaceLaps;

        [FieldOffset(68)]
        public int MaximumLaps;
        [FieldOffset(72)]
        public int Cars;
        [FieldOffset(76)]
        public rFactorSessionType SessionType;

        [FieldOffset(80)]
        public bool IsRace;
        [FieldOffset(81)]
        public bool Flag_Green;
        [FieldOffset(82)]
        public bool Flag_FullCourseYellow;
        [FieldOffset(83)]
        public bool Flags_S1;
        [FieldOffset(84)]
        public bool Flags_S2;
        [FieldOffset(85)]
        public bool Flags_S3;

        [FieldOffset(88)]
        public int StartLight;
        [FieldOffset(92)]
        public int StartLightCount;

        [FieldOffset(96)]
        public float Time;
        [FieldOffset(100)]
        public float TimeClock;
        [FieldOffset(104)]
        public float TimeEnd;

        [FieldOffset(108)]
        public float TrackTemperature;
        [FieldOffset(112)]
        public float AmbientTemperature;
        [FieldOffset(116)]
        public float Wetness_OnPath;

        [FieldOffset(120)]
        public float Wetness_OffPath;



    }
}