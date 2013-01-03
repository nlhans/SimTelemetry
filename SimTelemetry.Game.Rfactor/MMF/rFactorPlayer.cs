using System.Runtime.InteropServices;

namespace SimTelemetry.Game.Rfactor.MMF
{
    [StructLayout(LayoutKind.Explicit)]
    public struct rFactorPlayer
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string VehicleName;

        [FieldOffset(64)]
        public float LapStart;

        [FieldOffset(68)]
        public int Lap;
        [FieldOffset(72)]
        public int Gear;

        [FieldOffset(76)]
        public float EngineRPM;
        [FieldOffset(80)]
        public float EngineRPM_Max;

        [FieldOffset(84)]
        public float EngineTemp_Water;
        [FieldOffset(88)]
        public float EngineTemp_Oil;

        [FieldOffset(92)]
        public float Pedals_Throttle;
        [FieldOffset(96)]
        public float Pedals_Brake;
        [FieldOffset(100)]
        public float Pedals_Clutch;
        [FieldOffset(104)]
        public float Pedals_Steering;

        [FieldOffset(108)]
        public float Fuel;

        [FieldOffset(112)]
        public float Speed;

        [FieldOffset(116)]
        public rFactorWheel Wheel_LF; // 44 bytes

        [FieldOffset(160)]
        public rFactorWheel Wheel_RF;
        [FieldOffset(204)]
        public rFactorWheel Wheel_LR;
        [FieldOffset(248)]
        public rFactorWheel Wheel_RR;

        [FieldOffset(292)]
        public bool EngineHot;


        // TODO: Add acceleration.
    }
}