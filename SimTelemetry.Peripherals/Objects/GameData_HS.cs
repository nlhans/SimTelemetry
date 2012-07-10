using System;
using System.Runtime.InteropServices;

namespace SimTelemetry.Peripherals
{
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct GameData_HS
    {
        /** CAR INPUTS **/
        public byte Throttle;
        public byte Brake;
        public byte Clutch;
        public byte Steer;

        /** PITS SITUATION ETC **/
        public byte PitLimiter;
        public byte InPits;
        public byte PitRequired;
        public byte EngineStall;

        /** DRIVING INFO **/
        public byte Gear;
        public byte Position;
        public byte Wheelslip;
        public byte Cars;

        /** TRACK INFO **/
        public byte Flag;
        public byte Temp_Water;
        public byte Temp_Oil;
        public byte Temp_Track;

        /** MORE DRIVING **/
        public UInt16 Speed;
        public UInt16 RPM;
        public UInt16 Engine_HP;
        public UInt16 MetersDriven;


        /* MORE SHIT */
        public byte Ignition;
        public byte Lights;
        public byte Pause;
        public byte Wipers;

        /** LIVE DRIVING TIMES **/
        public float Laptime_Current;
        public float Gap_Front;
        public float Gap_Back;
        public byte FlagIntensity;

    }
}