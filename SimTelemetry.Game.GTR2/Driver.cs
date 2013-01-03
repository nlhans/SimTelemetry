/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System;
using System.Collections.Generic;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;

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
        public ILap GetLapTime(int lap)
        {
            return new GTR2_Lap();
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
            set { }
        }

        public int SectorsDriven
        {
            get { return 0; }
            set { }
        }

        public bool Active
        {
            get { return false; }
            set { }
        }

        public bool Driving
        {
            get { return true; }
            set { }
        }

        public bool IsPlayer
        {
            get { return false; }
            set { }
        }

        public string Name
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5440), 128); }
            set { }
        }

        public int BaseAddress
        {
            get { return this.Base-0x10; }
            set { }
        }

        public double CoordinateX
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAE8));}
            set { }
        }

        public double CoordinateY
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAEC)); }
            set { }
        }

        public double CoordinateZ
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xAF0)); }
            set { }
        }

        public double Throttle
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0xB8)); } // 0x163C)); }
            set { }
        }

        public double Brake
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x1640)); }
            set { }
        }

        public double Fuel
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x432C)); }
            set { }
        }

        public double Fuel_Max
        {
            get { return 100; } // TODO: Search memory address.
            set { }
        }

        public string CarModel
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5530), 32); }
            set { }
        }

        public string CarClass
        {
            get { return GTR2.Game.ReadString(new IntPtr(BaseAddress + 0x5630), 32); }
            set { }
        }

        public bool Control_AI_Aid
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x39E4)) == 1)?true:false); }
            set { }
        }

        public bool PitLimiter
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x1690)) == 1) ? true : false); }
            set { }
        }

        public bool Pits
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x4C34)) == 1) ? true : false); }
            set { }
        }

        public bool HeadLights
        {
            get { return ((GTR2.Game.ReadByte(new IntPtr(BaseAddress + 0x1685)) == 1) ? true : false); }
            set { }
        }

        public int Laps
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x4B54)); }
            set { }
        }

        public float LapTime_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BEC)); }
            set { }
        }

        public float LapTime_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { }
        }

        public float LapTime_Best_Sector1
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BFC)); }
            set { }
        }

        public float LapTime_Best_Sector2
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4C00)) - LapTime_Best_Sector1; }
            set { }
        }

        public float LapTime_Best_Sector3
        {
            get { return LapTime_Best - LapTime_Best_Sector2; }
            set { }
        }

        public float Sector_1_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BF0)); }
            set { }
        }

        public float Sector_2_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BF4)); }
            set { }
        }

        public float Sector_3_Best
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { }
        }

        public float Sector_1_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4BE8)); }
           // get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B6C)); }
            set { }
        }

        public float Sector_2_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B70)); }
            set { }
        }

        public float Sector_3_Last
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4B68)); }
            set { }
        }

        public double MetersDriven
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x500C)); }
            set { }
        }

        public int Pitstops
        {
            get { return 0; }
            set { }
        }

        public bool Retired
        {
            get { return false; }
            set { }
        }

        public TrackPosition TrackPosition
        {
            get { return Objects.TrackPosition.SECTOR2; }
            set { }
        }

        public LevelIndicator SteeringHelp
        {
            get { return 0; }
            set { }
        }

        public int PitStop_FrontWingSetting
        {
            get { return 0; }
            set { }
        }

        public int PitStop_RearWingSetting
        {
            get { return 0; }
            set { }
        }

        public int PitStop_FuelSetting
        {
            get { return 0; }
            set { }
        }

        public double FuelSetting_Offset
        {
            get { return 0; }
            set { }
        }

        public double FuelSetting_Scale
        {
            get { return 0; }
            set { }
        }

        public double MassEmpty
        {
            get { return 0; }
            set { }
        }

        public double Mass
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3EB4)); }
            set { }
        }

        public double RPM_Stationary
        {
            get { return 0; }
            set { }
        }

        public double RPM_Max_Offset
        {
            get { return 0; }
            set { }
        }

        public double RPM_Max_Scale
        {
            get { return 0; }
            set { }
        }

        public double Speed
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4FFC)); }
            set { }
        }

        public double RPM
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(BaseAddress + 0x4318)); }
            set { }
        }

        public double RPM_Max
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Position
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x4B7C)); }
            set { }
        }

        public int Gear
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x1644)); }
            set { }
        }

        public int Gears
        {
            get { return GTR2.Game.ReadInt32(new IntPtr(BaseAddress + 0x43B0)); }
            set { }
        }

        public float GearRatio1
        {
            get { return 0; }
            set { }
        }

        public float GearRatio2
        {
            get { return 0; }
            set { }
        }

        public float GearRatio3
        {
            get { return 0; }
            set { }
        }

        public float GearRatio4
        {
            get { return 0; }
            set { }
        }

        public float GearRatio5
        {
            get { return 0; }
            set { }
        }

        public float GearRatio6
        {
            get { return 0; }
            set { }
        }

        public float GearRatio7
        {
            get { return 0; }
            set { }
        }

        public float GearRatio8
        {
            get { return 0; }
            set { }
        }

        public float GearRatio9 { get { return 0; } set { } }
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
            get { return 0; }
            set { }
        }

        public IWheel Wheel_LeftFront
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_RightFront
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_LeftRear
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IWheel Wheel_RightRear
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TyreWear_LF
        {
            get { return 0; }
            set { }
        }

        public float TyreWear_RF
        {
            get { return 0; }
            set { }
        }

        public float TyreWear_LR
        {
            get { return 0; }
            set { }
        }

        public float TyreWear_RR
        {
            get { return 0; }
            set { }
        }

        public bool Flag_Blue
        {
            get { return false; }
            set { }
        }

        public bool Flag_Yellow
        {
            get { return false; }
            set { }
        }

        public bool Flag_Black
        {
            get { return false; }
            set { }
        }
    }

    public class GTR2_Lap : ILap
    {
        private int _lap;

        private float _sector1;

        private float _sector2;

        private float _sector3;

        private float _lapTime;

        public int LapNumber
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