using System;
using System.Collections.Generic;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.GTR2
{
    public class Driver : IDriverGeneral
    {
        private int Base = 0;


        public Driver(int i)
        {
            this.Base = i;
        }

        public ILap GetBestLap()
        {
            return new GTR2_Lap();
        }

        public double GetSplitTime(IDriverGeneral player)
        {
            return 10;
        }

        public List<ILap> GetLapTimes()
        {
            return new List<ILap>();
        }
        public bool Ignition
        {
            get { return false; }
            set { }
        }

        public int MemoryBlock
        {
            get { return (BaseAddress - 0x924FE8) / 0x5858; }
            set { throw new NotImplementedException(); }
        }

        public int SectorsDriven
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Active
        {
            get { return false; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsPlayer
        {
            get { return false; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5440), 128); }
            set { throw new NotImplementedException(); }
        }

        public int BaseAddress
        {
            get { return this.Base-0x10; }
            set { throw new NotImplementedException(); }
        }

        public double CoordinateX
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAE8));}
            set { throw new NotImplementedException(); }
        }

        public double CoordinateY
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAEC)); }
            set { throw new NotImplementedException(); }
        }

        public double CoordinateZ
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAF0)); }
            set { throw new NotImplementedException(); }
        }

        public double Throttle
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xB8)); } // 0x163C)); }
            set { throw new NotImplementedException(); }
        }

        public double Brake
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x1640)); }
            set { throw new NotImplementedException(); }
        }

        public double Fuel
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x432C)); }
            set { throw new NotImplementedException(); }
        }

        public double Fuel_Max
        {
            get { return 100; throw new NotImplementedException(); } // TODO
            set { throw new NotImplementedException(); }
        }

        public string CarModel
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5530), 32); }
            set { throw new NotImplementedException(); }
        }

        public string CarClass
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5630), 32); }
            set { throw new NotImplementedException(); }
        }

        public bool Control_AI_Aid
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x39E4)) == 1)?true:false); }
            set { throw new NotImplementedException(); }
        }

        public bool PitLimiter
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x1690)) == 1) ? true : false); }
            set { throw new NotImplementedException(); }
        }

        public bool Pits
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x4C34)) == 1) ? true : false); }
            set { throw new NotImplementedException(); }
        }

        public bool HeadLights
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x1685)) == 1) ? true : false); }
            set { throw new NotImplementedException(); }
        }

        public int Laps
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x4B54)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BEC)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector1
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BFC)); }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector2
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4C00)) - LapTime_Best_Sector1; }
            set { throw new NotImplementedException(); }
        }

        public float LapTime_Best_Sector3
        {
            get { return LapTime_Best - LapTime_Best_Sector2; }
            set { throw new NotImplementedException(); }
        }

        public float Sector_1_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BF0)); }
            set { throw new NotImplementedException(); }
        }

        public float Sector_2_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BF4)); }
            set { throw new NotImplementedException(); }
        }

        public float Sector_3_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { throw new NotImplementedException(); }
        }

        public float Sector_1_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BE8)); }
           // get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B6C)); }
            set { throw new NotImplementedException(); }
        }

        public float Sector_2_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B70)); }
            set { throw new NotImplementedException(); }
        }

        public float Sector_3_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { throw new NotImplementedException(); }
        }

        public double MetersDriven
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x500C)); }
            set { throw new NotImplementedException(); }
        }

        public int PitStopRuns
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Retired
        {
            get { return false; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public TrackPosition TrackPosition
        {
            get { return Objects.TrackPosition.SECTOR2; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public LevelIndicator SteeringHelp
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_FrontWingSetting
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_RearWingSetting
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int PitStop_FuelSetting
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double FuelSetting_Offset
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double FuelSetting_Scale
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MassEmpty
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Mass
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3EB4)); }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Stationary
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Max_Offset
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double RPM_Max_Scale
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Speed
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4FFC)); }
            set { throw new NotImplementedException(); }
        }

        public double RPM
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4318)); throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Position
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x4B7C)); }
            set { throw new NotImplementedException(); }
        }

        public int Gear
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x1644)); }
            set { throw new NotImplementedException(); }
        }

        public int Gears
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x43B0)); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio1
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio2
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio3
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio4
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio5
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio6
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatio7
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float GearRatioR
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_LF
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_RF
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_LR
        {
            get { return 0; throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_RR
        {
            get { return 0; throw new NotImplementedException(); }
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

    public class GTR2_Lap : ILap
    {
        private int _lap;

        private float _sector1;

        private float _sector2;

        private float _sector3;

        private float _lapTime;

        public int Lap
        {
            get { return _lap; }
        }

        public float Sector1
        {
            get { return _sector1; }
        }

        public float Sector2
        {
            get { return _sector2; }
        }

        public float Sector3
        {
            get { return _sector3; }
        }

        public float LapTime
        {
            get { return _lapTime; }
        }
    }
}