using System;
using System.Collections.Generic;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    public class Driver : IDriverGeneral
    {
        private int addr = 0;
        public Driver(int baseaddr)
        {
            this.addr = baseaddr;

        }

        public double GetSplitTime(IDriverGeneral player)
        {
            return 0;
        }

        public List<ILap> GetLapTimes()
        {
            return new List<ILap>();
        }

        public ILap GetBestLap()
        {
            return new rf2Lap();
        }


        public bool Ignition
        {
            get { return true; }
            set { }
        }
        public int MemoryBlock
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public int SectorsDriven
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public bool Active
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool IsPlayer
        {
            get
            {
                return Name == "Hans de Jong";
                return ((rFactor2.Game.ReadInt32(new IntPtr(0x00D49F60)) == BaseAddress) ? true : false);
            }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return rFactor2.Game.ReadString(new IntPtr(0x0077CC+BaseAddress),128 ); }
            set { throw new NotImplementedException(); }
        }

        public int BaseAddress
        {
            get { return addr; }
            set { throw new NotImplementedException(); }
        }

        public double CoordinateX
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x10+BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double CoordinateY
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x14 + BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double CoordinateZ
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x18 + BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double Throttle
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0xAC + BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double Brake
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x24C8 + BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double Fuel
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x3ED0+BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public double Fuel_Max
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x3ED4 + BaseAddress)); } // TODO
            set { throw new NotImplementedException(); }
        }

        public string CarModel
        {
            get { return rFactor2.Game.ReadString(new IntPtr(0x007946 + BaseAddress), 128); }
            set { throw new NotImplementedException(); }
        }

        public string CarClass
        {
            get { return ""; }
            set { throw new NotImplementedException(); }
        }

        public bool Control_AI_Aid
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool PitLimiter
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B79)) == 1?true:false; }
            set { throw new NotImplementedException(); }
        }

        public bool Pits
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x3504)) == 1 ? true : false; }
            set { throw new NotImplementedException(); }
        }

        public bool HeadLights
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B75)) == 1 ? true : false; }
            set { throw new NotImplementedException(); }
        }

        public int Laps
        {
            get { return rFactor2.Game.ReadInt32(new IntPtr(BaseAddress + 0x59A4)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x5A44+BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Last
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x59BC + BaseAddress)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector1
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector2
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector3
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_1_Best
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_2_Best
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_3_Best
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_1_Last
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_2_Last
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_3_Last
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double MetersDriven
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x59B8)); }
            set { throw new NotImplementedException(); }
        }

        public int PitStopRuns
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public bool Retired
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public TrackPosition TrackPosition
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public LevelIndicator SteeringHelp
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_FrontWingSetting
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_RearWingSetting
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_FuelSetting
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double FuelSetting_Offset
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double FuelSetting_Scale
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double MassEmpty
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double Mass
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x363C)); }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Stationary
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Max_Offset
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3EF4))-30; }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Max_Scale
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public double Speed
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x7454)); }
            set { throw new NotImplementedException(); }
        }

        public double RPM
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0xA8)); }
            set { throw new NotImplementedException(); }
        }

        public int Position
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x59F8)); }
            set { throw new NotImplementedException(); }
        }

        public int Gear
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B80)); }
            set { throw new NotImplementedException(); }
        }

        public int Gears
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x3FAC)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio1
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4*0)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio2
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 1)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio3
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 2)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio4
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 3)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio5
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 4)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio6
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 *5)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio7
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 6)); }
            set { throw new NotImplementedException(); }
        }

        // TODO: Implement gears 8-10? (Not sure how many rFactor 2 supports)
        public float GearRatio8 { get; set; }
        public float GearRatio9 { get; set; }

        public float GearRatio10 { get { return 0; } set { } }
        public float GearRatio11 { get { return 0; } set { } }
        public float GearRatio12 { get { return 0; } set { } }
        public float GearRatio13 { get { return 0; } set { } }
        public float GearRatio14 { get { return 0; } set { } }
        public float GearRatio15 { get { return 0; } set { } }
        public float GearRatio16 { get { return 0; } set { } }
        public float GearRatio17 { get { return 0; } set { } }
        public float GearRatio18 { get { return 0; } set { } }

        public float GearRatioR
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C - 0x4 * 1)); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_LF
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_RF
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_LR
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_RR
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Blue
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Yellow
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public bool Flag_Black
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }
    }
}