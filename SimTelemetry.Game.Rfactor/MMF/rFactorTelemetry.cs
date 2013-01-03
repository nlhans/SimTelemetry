using System.Runtime.InteropServices;

namespace SimTelemetry.Game.Rfactor.MMF
{
    [StructLayout(LayoutKind.Explicit)]
    public struct rFactorTelemetry
    {
        [FieldOffset(0)]
        public bool SessionRunning;
        [FieldOffset(1)]
        public bool PlayerDriving;

        [FieldOffset(4)]
        public rFactorSession Session;
        [FieldOffset(4 + 124)]
        public rFactorPlayer Player;

    }
}