using System;

namespace SimTelemetry.Peripherals
{
    struct GameData_LS
    {
        /** TYRE TEMPERATURES **/
        public byte TyreTemperature_LF;
        public byte TyreTemperature_RF;
        public byte TyreTemperature_LR;
        public byte TyreTemperature_RR;

        /** TYRE & BRAKE WEAR **/
        public byte TyreWear_F;
        public byte TyreWear_R;
        public byte BrakeWear_F;
        public byte BrakeWear_R;

        /** CURRENT FUEL INFO **/
        public UInt16 Fuel_Litre;
        public UInt16 Fuel_Laps;

        /** DRIVING TIMES **/
        public float Laptime_Last;
        public float Laptime_Best;
        public float Split_Sector;
        public float Split_Lap;

        public float RaceTime;
        public float RaceLength;
    }
}