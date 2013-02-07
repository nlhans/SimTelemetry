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
using System.Linq;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;

namespace SimTelemetry.Data.Net.Objects
{
    [Serializable]
    public class NetworkDriverGeneral : IDriverGeneral
    {
        public double GetSplitTime(IDriverGeneral player)
        {
            return 0;
            throw new NotImplementedException();
        }

        public List<ILap> GetLapTimes()
        {
            return new List<ILap>();
            throw new NotImplementedException();
        }

        public ILap GetBestLap()
        {
            return null;
            throw new NotImplementedException();
        }

        public ILap GetLapTime(int lap)
        {
            return null;
            throw new NotImplementedException();
        }

        public double Heading { get; set; }

        public bool Driving { get; set; }
        public bool Ignition { get; set; }
        public int MemoryBlock { get; set; }
        public int SectorsDriven { get; set; }
        public bool Active { get; set; }
        public bool IsPlayer { get; set; }
        public string Name { get; set; }
        public int BaseAddress { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }
        public double Throttle { get; set; }
        public double Brake { get; set; }
        public double Fuel { get; set; }
        public double Fuel_Max { get; set; }
        public string CarModel { get; set; }
        public string CarClass { get; set; }
        public bool Control_AI_Aid { get; set; }
        public bool PitLimiter { get; set; }
        public bool Pits { get; set; }
        public bool HeadLights { get; set; }
        public int Laps { get; set; }
        public float LapTime_Best { get; set; }
        public float LapTime_Last { get; set; }
        public float LapTime_Best_Sector1 { get; set; }
        public float LapTime_Best_Sector2 { get; set; }
        public float LapTime_Best_Sector3 { get; set; }
        public float Sector_1_Best { get; set; }
        public float Sector_2_Best { get; set; }
        public float Sector_3_Best { get; set; }
        public float Sector_1_Last { get; set; }
        public float Sector_2_Last { get; set; }
        public float Sector_3_Last { get; set; }
        public double MetersDriven { get; set; }
        public int Pitstops { get; set; }
        public bool Retired { get; set; }
        public TrackPosition TrackPosition { get; set; }
        public LevelIndicator SteeringHelp { get; set; }
        public int PitStop_FrontWingSetting { get; set; }
        public int PitStop_RearWingSetting { get; set; }
        public int PitStop_FuelSetting { get; set; }
        public double FuelSetting_Offset { get; set; }
        public double FuelSetting_Scale { get; set; }
        public double MassEmpty { get; set; }
        public double Mass { get; set; }
        public double RPM_Stationary { get; set; }
        public double RPM_Max_Offset { get; set; }
        public double RPM_Max_Scale { get; set; }
        public double Speed { get; set; }
        public double RPM { get; set; }

        public double RPM_Max { get; set; }

        public int Position { get; set; }
        public int Gear { get; set; }
        public int Gears { get; set; }
        public float GearRatio1 { get; set; }
        public float GearRatio2 { get; set; }
        public float GearRatio3 { get; set; }
        public float GearRatio4 { get; set; }
        public float GearRatio5 { get; set; }
        public float GearRatio6 { get; set; }
        public float GearRatio7 { get; set; }
        public float GearRatio8 { get; set; }
        public float GearRatio9 { get; set; }
        public float GearRatio10 { get; set; }
        public float GearRatio11 { get; set; }
        public float GearRatio12 { get; set; }
        public float GearRatio13 { get; set; }
        public float GearRatio14 { get; set; }
        public float GearRatio15 { get; set; }
        public float GearRatio16 { get; set; }
        public float GearRatio17 { get; set; }
        public float GearRatio18 { get; set; }
        public float GearRatioR { get; set; }

        public IWheel Wheel_LeftFront { get; set; }
        public IWheel Wheel_RightFront { get; set; }
        public IWheel Wheel_LeftRear { get; set; }
        public IWheel Wheel_RightRear { get; set; }

        public bool Flag_Blue { get; set; }
        public bool Flag_Yellow { get; set; }
        public bool Flag_Black { get; set; }

        public static IDriverGeneral Create(IDriverGeneral player)
        {
            NetworkDriverGeneral nwGeneral = new NetworkDriverGeneral();

            // TODO: Make a object copy machine.
            nwGeneral.Driving = player.Driving;
            nwGeneral.Ignition = player.Ignition;
            nwGeneral.MemoryBlock = player.MemoryBlock;
            nwGeneral.SectorsDriven = player.SectorsDriven;
            nwGeneral.Active = player.Active;
            nwGeneral.IsPlayer = player.IsPlayer;
            nwGeneral.Name = player.Name;
            nwGeneral.BaseAddress = player.BaseAddress;
            nwGeneral.CoordinateX = player.CoordinateX;
            nwGeneral.CoordinateY = player.CoordinateY;
            nwGeneral.CoordinateZ = player.CoordinateZ;
            nwGeneral.Throttle = player.Throttle;
            nwGeneral.Brake = player.Brake;
            nwGeneral.Fuel = player.Fuel;
            nwGeneral.CarModel = player.CarModel;
            nwGeneral.CarClass = player.CarClass;
            nwGeneral.Control_AI_Aid = player.Control_AI_Aid;
            nwGeneral.PitLimiter = player.PitLimiter;
            nwGeneral.Pits = player.Pits;
            nwGeneral.HeadLights = player.HeadLights;
            nwGeneral.Laps = player.Laps;
            nwGeneral.MetersDriven = player.MetersDriven;
            nwGeneral.Pitstops = player.Pitstops;
            nwGeneral.Retired = player.Retired;
            nwGeneral.TrackPosition = player.TrackPosition;
            nwGeneral.SteeringHelp = player.SteeringHelp;
            nwGeneral.Mass = player.Mass;
            nwGeneral.Speed = player.Speed;
            nwGeneral.RPM = player.RPM;
            nwGeneral.RPM_Max = player.RPM_Max;
            nwGeneral.Position = player.Position;
            nwGeneral.Gear = player.Gear;
            nwGeneral.GearRatio1 = player.GearRatio1;
            nwGeneral.GearRatio2 = player.GearRatio2;
            nwGeneral.GearRatio3 = player.GearRatio3;
            nwGeneral.GearRatio4 = player.GearRatio4;
            nwGeneral.GearRatio5 = player.GearRatio5;
            nwGeneral.GearRatio6 = player.GearRatio6;
            nwGeneral.GearRatio7 = player.GearRatio7;
            nwGeneral.GearRatio8 = player.GearRatio8;
            nwGeneral.GearRatio9 = player.GearRatio9;
            nwGeneral.GearRatio10 = player.GearRatio10;
            nwGeneral.GearRatio11 = player.GearRatio11;
            nwGeneral.GearRatio12 = player.GearRatio12;
            nwGeneral.GearRatio13 = player.GearRatio13;
            nwGeneral.GearRatio14 = player.GearRatio14;
            nwGeneral.GearRatio15 = player.GearRatio15;
            nwGeneral.GearRatio16 = player.GearRatio16;
            nwGeneral.GearRatio17 = player.GearRatio17;
            nwGeneral.GearRatio18 = player.GearRatio18;
            nwGeneral.GearRatioR = player.GearRatioR;
            nwGeneral.Flag_Blue = player.Flag_Blue;
            nwGeneral.Flag_Yellow = player.Flag_Yellow;
            nwGeneral.Flag_Black = player.Flag_Black;
            // TODO: Copy wheel info.
            return nwGeneral;
        }
    }
}
