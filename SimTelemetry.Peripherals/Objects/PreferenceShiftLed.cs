using System;

namespace SimTelemetry.Peripherals
{
    public struct PreferenceShiftLed
    {
        public byte Percentage;
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte Fading;
        public UInt16 BlinkPeriod;
        public UInt16 BlinkPhase;
    }
}