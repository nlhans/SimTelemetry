using System;

namespace SimTelemetry.Peripherals
{
    public struct CarData
    {

        /** SETUP INFO **/

        public ushort RPM_Shift0;
        public ushort RPM_Shift1;
        public ushort RPM_Shift2;
        public ushort RPM_Shift3;
        public ushort RPM_Shift4;
        public ushort RPM_Shift5;
        public ushort RPM_Shift6;
        public ushort RPM_Shift7;

        public float GearRatio0;
        public float GearRatio1;
        public float GearRatio2;
        public float GearRatio3;
        public float GearRatio4;
        public float GearRatio5;
        public float GearRatio6;
        public float GearRatio7;
        /** CAR INFO **/
        public UInt16 RPM_Max;
        public UInt16 RPM_Idle;

        public UInt16 Fuel_Max;
        public UInt16 HP_Max;

        public byte Gears;
    }
}