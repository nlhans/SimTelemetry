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

        public ILap GetLapTime(int lap)
        {
            return new rf2Lap();
        }
        public ILap GetBestLap()
        {
            return new rf2Lap();
        }


        public bool Driving
        {
            get { return true; }
            set { }
        }


        public bool Ignition
        {
            get { return true; }
            set { }
        }
        public int MemoryBlock
        {
            get { return 0; }
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

        public bool IsPlayer
        {
            get
            {
                return Name == "Hans de Jong";
                return ((rFactor2.Game.ReadInt32(new IntPtr(0x00D49F60)) == BaseAddress) ? true : false);
            }
            set { }
        }

        public string Name
        {
            get { return rFactor2.Game.ReadString(new IntPtr(0x0077CC+BaseAddress),128 ); }
            set { }
        }

        public int BaseAddress
        {
            get { return addr; }
            set { }
        }

        public double CoordinateX
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x10+BaseAddress)); }
            set { }
        }

        public double CoordinateY
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x14 + BaseAddress)); }
            set { }
        }

        public double CoordinateZ
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x18 + BaseAddress)); }
            set { }
        }

        public double Throttle
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0xAC + BaseAddress)); }
            set { }
        }

        public double Brake
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x24C8 + BaseAddress)); }
            set { }
        }

        public double Fuel
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x3ED0+BaseAddress)); }
            set { }
        }

        public double Fuel_Max
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x3ED4 + BaseAddress)); } // TODO: Search memory address.
            set { }
        }

        public string CarModel
        {
            get { return rFactor2.Game.ReadString(new IntPtr(0x007946 + BaseAddress), 128); }
            set { }
        }

        public string CarClass
        {
            get { return ""; }
            set { }
        }

        public bool Control_AI_Aid
        {
            get { return false; }
            set { }
        }

        public bool PitLimiter
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B79)) == 1?true:false; }
            set { }
        }

        public bool Pits
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x3504)) == 1 ? true : false; }
            set { }
        }

        public bool HeadLights
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B75)) == 1 ? true : false; }
            set { }
        }

        public int Laps
        {
            get { return rFactor2.Game.ReadInt32(new IntPtr(BaseAddress + 0x59A4)); }
            set { }
        }

        public float LapTime_Best
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x5A44+BaseAddress)); }
            set { }
        }

        public float LapTime_Last
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(0x59BC + BaseAddress)); }
            set { }
        }

        public float LapTime_Best_Sector1
        {
            get { return 0; }
            set { }
        }

        public float LapTime_Best_Sector2
        {
            get { return 0; }
            set { }
        }

        public float LapTime_Best_Sector3
        {
            get { return 0; }
            set { }
        }

        public float Sector_1_Best
        {
            get { return 0; }
            set { }
        }

        public float Sector_2_Best
        {
            get { return 0; }
            set { }
        }

        public float Sector_3_Best
        {
            get { return 0; }
            set { }
        }

        public float Sector_1_Last
        {
            get { return 0; }
            set { }
        }

        public float Sector_2_Last
        {
            get { return 0; }
            set { }
        }

        public float Sector_3_Last
        {
            get { return 0; }
            set { }
        }

        public double MetersDriven
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x59B8)); }
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
            get { return 0; }
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
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x363C)); }
            set { }
        }

        public double RPM_Stationary
        {
            get { return 0; }
            set { }
        }

        public double RPM_Max_Offset
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3EF4))-30; }
            set { }
        }

        public double RPM_Max_Scale
        {
            get { return 0; }
            set { }
        }

        public double Speed
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x7454)); }
            set { }
        }

        public double RPM
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0xA8)); }
            set { }
        }

        public double RPM_Max
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Position
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x59F8)); }
            set { }
        }

        public int Gear
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x1B80)); }
            set { }
        }

        public int Gears
        {
            get { return rFactor2.Game.ReadByte(new IntPtr(BaseAddress + 0x3FAC)); }
            set { }
        }

        public float GearRatio1
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4*0)); }
            set { }
        }

        public float GearRatio2
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 1)); }
            set { }
        }

        public float GearRatio3
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 2)); }
            set { }
        }

        public float GearRatio4
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 3)); }
            set { }
        }

        public float GearRatio5
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 4)); }
            set { }
        }

        public float GearRatio6
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 *5)); }
            set { }
        }

        public float GearRatio7
        {
            get { return rFactor2.Game.ReadFloat(new IntPtr(BaseAddress + 0x3F7C + 0x4 * 6)); }
            set { }
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
}