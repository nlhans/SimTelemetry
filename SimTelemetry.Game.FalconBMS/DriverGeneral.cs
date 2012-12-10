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

namespace SimTelemetry.Game.FalconBMS
{
    public class DriverGeneral : IDriverGeneral
    {
        public double GetSplitTime(IDriverGeneral player)
        {
            return 0;
        }

        public List<ILap> GetLapTimes()
        {
            return new List<ILap>();
            throw new NotImplementedException();
        }

        public ILap GetBestLap()
        {
            return null;
        }

        public ILap GetLapTime(int lap)
        {
            return null;
        }

        public bool Driving
        {
            get
            {
                    return true;
            }
            set { }
        }

        public bool Ignition
        {
            get { return false; }
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
            get { return true; }

            set { }
        }

        public string Name
        {
            get { return "Hans"; }
            set { }
        }

        public int BaseAddress { get; set; }
        public double CoordinateX
        {
            get { return 0; }
            set { }
        }

        public double CoordinateY
        {
            get { return 0; }
            set { }
        }

        public double CoordinateZ
        {
            get { return 0; }
            set { }
        }

        public double Throttle
        {
            get { return 0; }
            set { }
        }

        public double Brake
        {
            get { return 0; }
            set { }
        }

        public double Fuel
        {
            get { return FalconBms.Game.ReadFloat(new IntPtr(0x4d0c210)); }
            set { }
        }

        public double Fuel_Max
        {
            get { return 12500; }
            set { }
        }

        public string CarModel
        {
            get { return "F16"; }
            set { }
        }

        public string CarClass
        {
            get { return "F16"; }
            set { }
        }

        public bool Control_AI_Aid
        {
            get { return false; }
            set { }
        }

        public bool PitLimiter
        {
            get { return false; }
            set { }
        }

        public bool Pits
        {
            get { return false; }
            set { }
        }

        public bool HeadLights
        {
            get { return false; }
            set { }
        }

        public int Laps
        {
            get { return 0; }
            set { }
        }

        public float LapTime_Best
        {
            get { return 0; }
            set { }
        }

        public float LapTime_Last
        {
            get
            {
                return 0;
                    // This is last split/lap: return LFS.Game.ReadInt32(new IntPtr(BaseAddress + 0x284)) / 100.0f; }
            }
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
            get { return 0; }
            set { }
        }

        public int PitStopRuns
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
            get { return 0; }
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
            get { return FalconBms.Game.ReadFloat(new IntPtr(0x04D0C1FC))/3.6*1.8421; }
            set { }
        }

        public double RPM
        {
            get
            {
                if (FalconBms.Game.ReadFloat(new IntPtr(0x04D0C21C)) > 90)
                return 1000*
                       ((FalconBms.Game.ReadFloat(new IntPtr(0x04D0C21C)) - 70)/30 +
                        FalconBms.Game.ReadFloat(new IntPtr(0x04D0BDAC)));
                else
                    return 1000*(FalconBms.Game.ReadFloat(new IntPtr(0x04D0C21C)) - 70)/30;
            }
            set { }
        }

        public int Position
        {
            get { return 0; }
            set { }
        }

        public int Gear
        {
            get { return 1; }
            set { }
        }

        public int Gears
        {
            get { return 2; }
            set { }
        }

        public float GearRatio1
        {
            get { return 2; }
            set { }
        }

        public float GearRatio2
        {
            get { return 1; }
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

        public float GearRatio9
        {
            get { return 0; }
            set { }
        }

        public float GearRatio10
        {
            get { return 0; }
            set { }
        }

        public float GearRatio11
        {
            get { return 0; }
            set { }
        }

        public float GearRatio12
        {
            get { return 0; }
            set { }
        }

        public float GearRatio13
        {
            get { return 0; }
            set { }
        }

        public float GearRatio14
        {
            get { return 0; }
            set { }
        }

        public float GearRatio15
        {
            get { return 0; }
            set { }
        }

        public float GearRatio16
        {
            get { return 0; }
            set { }
        }

        public float GearRatio17
        {
            get { return 0; }
            set { }
        }

        public float GearRatio18
        {
            get { return 0; }
            set { }
        }

        public float GearRatioR
        {
            get { return 0; }
            set { }
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