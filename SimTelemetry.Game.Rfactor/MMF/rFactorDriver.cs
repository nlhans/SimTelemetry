using System.Runtime.InteropServices;

namespace SimTelemetry.Game.Rfactor.MMF
{
    [StructLayout(LayoutKind.Explicit)]
    public struct rFactorDriver
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string VehicleName;

        [FieldOffset(64)]
        public bool IsPlayer;
        [FieldOffset(65)]
        public bool AIControl;
        [FieldOffset(66)]
        public bool InPits;

        [FieldOffset(68)]
        public int Position;

        [FieldOffset(72)]
        public float SplitBehind;
        [FieldOffset(76)]
        public int LapsBehind;

        [FieldOffset(80)]
        public float SplitLeader;
        [FieldOffset(84)]
        public int LapsLeader;

        [FieldOffset(88)]
        public float LapStartTime;

        [FieldOffset(92)]
        public float X;
        [FieldOffset(96)]
        public float Y;
        [FieldOffset(100)]
        public float Z;

        [FieldOffset(104)]
        public float Speed;
    }
}