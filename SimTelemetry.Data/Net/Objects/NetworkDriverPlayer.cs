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

namespace SimTelemetry.Data.Net.Objects
{
    [Serializable]
    public class NetworkDriverPlayer : IDriverPlayer
    {
        public double Engine_Lifetime_Live { get; set; }
        public double Engine_Lifetime_Typical { get; set; }
        public double Engine_Lifetime_Variation { get; set; }
        public double Engine_Lifetime_Oil_Base { get; set; }
        public double Engine_Lifetime_RPM_Base { get; set; }
        public double Engine_Temperature_Oil { get; set; }
        public double Engine_Temperature_Water { get; set; }
        public int Engine_BoostSetting { get; set; }
        public int Gear { get; set; }
        public int PitStop_Number { get; set; }
        public int PitStop_FuelStop1 { get; set; }
        public int PitStop_FuelStop2 { get; set; }
        public int PitStop_FuelStop3 { get; set; }
        public double RAM_Fuel { get; set; }
        public double RAM_Torque { get; set; }
        public double RAM_Power { get; set; }
        public double Fuel { get; set; }
        public double Suspension_SpendleTorque_LF { get; set; }
        public double Suspension_SpendleTorque_RF { get; set; }
        public double Suspension_RideHeight_LF_G { get; set; }
        public double Suspension_RideHeight_LR_G { get; set; }
        public double Suspension_RideHeight_RF_G { get; set; }
        public double Suspension_RideHeight_RR_G { get; set; }
        public double Suspension_RideHeight_LF { get; set; }
        public double Suspension_RideHeight_LR { get; set; }
        public double Suspension_RideHeight_RF { get; set; }
        public double Suspension_RideHeight_RR { get; set; }
        public string Tyre_Compound_Front { get; set; }
        public string Tyre_Compound_Rear { get; set; }
        public double Tyre_Grip_Forwards_LF { get; set; }
        public double Weight_Rearwheel { get; set; }
        public double DryWeight { get; set; }
        public double Tyre_Pressure_Optimal_LF { get; set; }
        public double Tyre_Pressure_Optimal_LR { get; set; }
        public double Tyre_Pressure_Optimal_RF { get; set; }
        public double Tyre_Pressure_Optimal_RR { get; set; }
        public double Tyre_Pressure_LF { get; set; }
        public double Tyre_Pressure_LR { get; set; }
        public double Tyre_Pressure_RF { get; set; }
        public double Tyre_Pressure_RR { get; set; }
        public double Tyre_Pressure_LF_G { get; set; }
        public double Tyre_Pressure_LR_G { get; set; }
        public double Tyre_Pressure_RF_G { get; set; }
        public double Tyre_Pressure_RR_G { get; set; }
        public double Tyre_Grip_Sidewards_LF { get; set; }
        public double Tyre_Grip_Sidewards_LR { get; set; }
        public double Tyre_Grip_Sidewards_RF { get; set; }
        public double Tyre_Grip_Sidewards_RR { get; set; }
        public double Tyre_Speed_LF { get; set; }
        public double Tyre_Speed_LR { get; set; }
        public double Tyre_Speed_RF { get; set; }
        public double Tyre_Speed_RR { get; set; }
        public double Tyre_Temperature_LF_Inside { get; set; }
        public double Tyre_Temperature_LR_Inside { get; set; }
        public double Tyre_Temperature_RF_Inside { get; set; }
        public double Tyre_Temperature_RR_Inside { get; set; }
        public double Tyre_Temperature_LF_Middle { get; set; }
        public double Tyre_Temperature_LR_Middle { get; set; }
        public double Tyre_Temperature_RF_Middle { get; set; }
        public double Tyre_Temperature_RR_Middle { get; set; }
        public double Tyre_Temperature_LF_Outside { get; set; }
        public double Tyre_Temperature_LR_Outside { get; set; }
        public double Tyre_Temperature_RF_Outside { get; set; }
        public double Tyre_Temperature_RR_Outside { get; set; }
        public double Tyre_Temperature_LF_Optimal { get; set; }
        public double Tyre_Temperature_LR_Optimal { get; set; }
        public double Tyre_Temperature_RF_Optimal { get; set; }
        public double Tyre_Temperature_RR_Optimal { get; set; }
        public double Tyre_Temperature_LF_Fresh { get; set; }
        public double Tyre_Temperature_LR_Fresh { get; set; }
        public double Tyre_Temperature_RF_Fresh { get; set; }
        public double Tyre_Temperature_RR_Fresh { get; set; }
        public double Wheel_Radius_LF { get; set; }
        public double Wheel_Radius_LR { get; set; }
        public double Wheel_Radius_RF { get; set; }
        public double Wheel_Radius_RR { get; set; }
        public int Aerodynamics_FrontWing_Setting { get; set; }
        public double Aerodynamics_FrontWing_Downforce { get; set; }
        public int Aerodynamics_RearWing_Setting { get; set; }
        public double Aerodynamics_RearWing_Downforce { get; set; }
        public double Aerodynamics_FrontWing_Drag_Total { get; set; }
        public double Aerodynamics_FrontWing_Drag_Base { get; set; }
        public double Aerodynamics_RearWing_Drag_Total { get; set; }
        public double Aerodynamics_RearWing_Drag_Base { get; set; }
        public double Aerodynamics_LeftFender_Drag { get; set; }
        public double Aerodynamics_RightFender_Drag { get; set; }
        public double Aerodynamics_Body_Drag { get; set; }
        public double Aerodynamics_Body_DragHeightDiff { get; set; }
        public double Aerodynamics_Body_DragHeightAvg { get; set; }
        public double Aerodynamics_Radiator_Drag { get; set; }
        public int Aerodynamics_Radiator_Setting { get; set; }
        public double Aerodynamics_BrakeDuct_Drag { get; set; }
        public int Aerodynamics_BrakeDuct_Setting { get; set; }
        public double Engine_RPM { get; set; }
        public double Engine_RPM_Max_Live { get; set; }
        public double Engine_RPM_Max_Scale { get; set; }
        public int Engine_RPM_Max_Step { get; set; }
        public double Engine_RPM_Idle_Min { get; set; }
        public double Engine_RPM_Idle_Max { get; set; }
        public double Engine_RPM_Launch_Min { get; set; }
        public double Engine_RPM_Launch_Max { get; set; }
        public double Engine_Idle_ThrottleGain { get; set; }
        public double Engine_Torque_Negative { get; set; }
        public double Engine_Torque { get; set; }
        public PowerTrainDrivenWheels Powertrain_DrivenWheels { get; set; }
        public double Powertrain_DriverDistribution { get; set; }
        public double Brake_TypicalFailure_LF { get; set; }
        public double Brake_TypicalFailure_LR { get; set; }
        public double Brake_TypicalFailure_RF { get; set; }
        public double Brake_TypicalFailure_RR { get; set; }
        public double Brake_Temperature_LF { get; set; }
        public double Brake_Temperature_LR { get; set; }
        public double Brake_Temperature_RF { get; set; }
        public double Brake_Temperature_RR { get; set; }
        public double Brake_Thickness_LF { get; set; }
        public double Brake_Thickness_LR { get; set; }
        public double Brake_Thickness_RF { get; set; }
        public double Brake_Thickness_RR { get; set; }
        public double Brake_OptimalTemperature_LF_Low { get; set; }
        public double Brake_OptimalTemperature_LR_Low { get; set; }
        public double Brake_OptimalTemperature_RF_Low { get; set; }
        public double Brake_OptimalTemperature_RR_Low { get; set; }
        public double Brake_OptimalTemperature_LF_High { get; set; }
        public double Brake_OptimalTemperature_LR_High { get; set; }
        public double Brake_OptimalTemperature_RF_High { get; set; }
        public double Brake_OptimalTemperature_RR_High { get; set; }
        public double Brake_Torque_LF { get; set; }
        public double Brake_Torque_LR { get; set; }
        public double Brake_Torque_RF { get; set; }
        public double Brake_Torque_RR { get; set; }
        public double Clutch_Torque { get; set; }
        public double Clutch_Friction { get; set; }
        public double Pedals_Clutch { get; set; }
        public double Pedals_Throttle { get; set; }
        public double Pedals_Brake { get; set; }
        public double SteeringAngle { get; set; }
        public LevelIndicator DrivingHelp_BrakingHelp { get; set; }
        public LevelIndicator DrivingHelp_SteeringHelp { get; set; }
        public LevelIndicator DrivingHelp_TractionControl { get; set; }
        public bool DrivingHelp_OppositeLock { get; set; }
        public bool DrivingHelp_SpinRecovery { get; set; }
        public bool DrivingHelp_StabilityControl { get; set; }
        public bool DrivingHelp_AutoClutch { get; set; }
        public double Speed { get; set; }
        public double Brake_InitialThickness_LF { get; set; }
        public double Brake_InitialThickness_RF { get; set; }
        public double Brake_InitialThickness_LR { get; set; }
        public double Brake_InitialThickness_RR { get; set; }
        public double SpeedSlipping { get; set; }

        public static NetworkDriverPlayer Create(IDriverPlayer player)
        {
            NetworkDriverPlayer nwPlayer = new NetworkDriverPlayer();


            nwPlayer.Engine_Lifetime_Live = player.Engine_Lifetime_Live;
            nwPlayer.Engine_Lifetime_Typical = player.Engine_Lifetime_Typical;
            nwPlayer.Engine_Lifetime_Variation = player.Engine_Lifetime_Variation;
            nwPlayer.Engine_Lifetime_Oil_Base = player.Engine_Lifetime_Oil_Base;
            nwPlayer.Engine_Lifetime_RPM_Base = player.Engine_Lifetime_RPM_Base;
            nwPlayer.Engine_Temperature_Oil = player.Engine_Temperature_Oil;
            nwPlayer.Engine_Temperature_Water = player.Engine_Temperature_Water;
            nwPlayer.Engine_BoostSetting = player.Engine_BoostSetting;
            nwPlayer.Gear = player.Gear;
            nwPlayer.PitStop_Number = player.PitStop_Number;
            nwPlayer.PitStop_FuelStop1 = player.PitStop_FuelStop1;
            nwPlayer.PitStop_FuelStop2 = player.PitStop_FuelStop2;
            nwPlayer.PitStop_FuelStop3 = player.PitStop_FuelStop3;
            nwPlayer.RAM_Fuel = player.RAM_Fuel;
            nwPlayer.RAM_Torque = player.RAM_Torque;
            nwPlayer.RAM_Power = player.RAM_Power;
            nwPlayer.Fuel = player.Fuel;
            nwPlayer.Suspension_SpendleTorque_LF = player.Suspension_SpendleTorque_LF;
            nwPlayer.Suspension_SpendleTorque_RF = player.Suspension_SpendleTorque_RF;
            nwPlayer.Suspension_RideHeight_LF_G = player.Suspension_RideHeight_LF_G;
            nwPlayer.Suspension_RideHeight_LR_G = player.Suspension_RideHeight_LR_G;
            nwPlayer.Suspension_RideHeight_RF_G = player.Suspension_RideHeight_RF_G;
            nwPlayer.Suspension_RideHeight_RR_G = player.Suspension_RideHeight_RR_G;
            nwPlayer.Suspension_RideHeight_LF = player.Suspension_RideHeight_LF;
            nwPlayer.Suspension_RideHeight_LR = player.Suspension_RideHeight_LR;
            nwPlayer.Suspension_RideHeight_RF = player.Suspension_RideHeight_RF;
            nwPlayer.Suspension_RideHeight_RR = player.Suspension_RideHeight_RR;
            nwPlayer.Tyre_Compound_Front = player.Tyre_Compound_Front;
            nwPlayer.Tyre_Compound_Rear = player.Tyre_Compound_Rear;
            nwPlayer.Tyre_Grip_Forwards_LF = player.Tyre_Grip_Forwards_LF;
            nwPlayer.Weight_Rearwheel = player.Weight_Rearwheel;
            nwPlayer.DryWeight = player.DryWeight;
            nwPlayer.Tyre_Pressure_Optimal_LF = player.Tyre_Pressure_Optimal_LF;
            nwPlayer.Tyre_Pressure_Optimal_LR = player.Tyre_Pressure_Optimal_LR;
            nwPlayer.Tyre_Pressure_Optimal_RF = player.Tyre_Pressure_Optimal_RF;
            nwPlayer.Tyre_Pressure_Optimal_RR = player.Tyre_Pressure_Optimal_RR;
            nwPlayer.Tyre_Pressure_LF = player.Tyre_Pressure_LF;
            nwPlayer.Tyre_Pressure_LR = player.Tyre_Pressure_LR;
            nwPlayer.Tyre_Pressure_RF = player.Tyre_Pressure_RF;
            nwPlayer.Tyre_Pressure_RR = player.Tyre_Pressure_RR;
            nwPlayer.Tyre_Pressure_LF_G = player.Tyre_Pressure_LF_G;
            nwPlayer.Tyre_Pressure_LR_G = player.Tyre_Pressure_LR_G;
            nwPlayer.Tyre_Pressure_RF_G = player.Tyre_Pressure_RF_G;
            nwPlayer.Tyre_Pressure_RR_G = player.Tyre_Pressure_RR_G;
            nwPlayer.Tyre_Grip_Sidewards_LF = player.Tyre_Grip_Sidewards_LF;
            nwPlayer.Tyre_Grip_Sidewards_LR = player.Tyre_Grip_Sidewards_LR;
            nwPlayer.Tyre_Grip_Sidewards_RF = player.Tyre_Grip_Sidewards_RF;
            nwPlayer.Tyre_Grip_Sidewards_RR = player.Tyre_Grip_Sidewards_RR;
            nwPlayer.Tyre_Speed_LF = player.Tyre_Speed_LF;
            nwPlayer.Tyre_Speed_LR = player.Tyre_Speed_LR;
            nwPlayer.Tyre_Speed_RF = player.Tyre_Speed_RF;
            nwPlayer.Tyre_Speed_RR = player.Tyre_Speed_RR;
            nwPlayer.Tyre_Temperature_LF_Inside = player.Tyre_Temperature_LF_Inside;
            nwPlayer.Tyre_Temperature_LR_Inside = player.Tyre_Temperature_LR_Inside;
            nwPlayer.Tyre_Temperature_RF_Inside = player.Tyre_Temperature_RF_Inside;
            nwPlayer.Tyre_Temperature_RR_Inside = player.Tyre_Temperature_RR_Inside;
            nwPlayer.Tyre_Temperature_LF_Middle = player.Tyre_Temperature_LF_Middle;
            nwPlayer.Tyre_Temperature_LR_Middle = player.Tyre_Temperature_LR_Middle;
            nwPlayer.Tyre_Temperature_RF_Middle = player.Tyre_Temperature_RF_Middle;
            nwPlayer.Tyre_Temperature_RR_Middle = player.Tyre_Temperature_RR_Middle;
            nwPlayer.Tyre_Temperature_LF_Outside = player.Tyre_Temperature_LF_Outside;
            nwPlayer.Tyre_Temperature_LR_Outside = player.Tyre_Temperature_LR_Outside;
            nwPlayer.Tyre_Temperature_RF_Outside = player.Tyre_Temperature_RF_Outside;
            nwPlayer.Tyre_Temperature_RR_Outside = player.Tyre_Temperature_RR_Outside;
            nwPlayer.Tyre_Temperature_LF_Optimal = player.Tyre_Temperature_LF_Optimal;
            nwPlayer.Tyre_Temperature_LR_Optimal = player.Tyre_Temperature_LR_Optimal;
            nwPlayer.Tyre_Temperature_RF_Optimal = player.Tyre_Temperature_RF_Optimal;
            nwPlayer.Tyre_Temperature_RR_Optimal = player.Tyre_Temperature_RR_Optimal;
            nwPlayer.Tyre_Temperature_LF_Fresh = player.Tyre_Temperature_LF_Fresh;
            nwPlayer.Tyre_Temperature_LR_Fresh = player.Tyre_Temperature_LR_Fresh;
            nwPlayer.Tyre_Temperature_RF_Fresh = player.Tyre_Temperature_RF_Fresh;
            nwPlayer.Tyre_Temperature_RR_Fresh = player.Tyre_Temperature_RR_Fresh;
            nwPlayer.Wheel_Radius_LF = player.Wheel_Radius_LF;
            nwPlayer.Wheel_Radius_LR = player.Wheel_Radius_LR;
            nwPlayer.Wheel_Radius_RF = player.Wheel_Radius_RF;
            nwPlayer.Wheel_Radius_RR = player.Wheel_Radius_RR;
            nwPlayer.Aerodynamics_FrontWing_Setting = player.Aerodynamics_FrontWing_Setting;
            nwPlayer.Aerodynamics_FrontWing_Downforce = player.Aerodynamics_FrontWing_Downforce;
            nwPlayer.Aerodynamics_RearWing_Setting = player.Aerodynamics_RearWing_Setting;
            nwPlayer.Aerodynamics_RearWing_Downforce = player.Aerodynamics_RearWing_Downforce;
            nwPlayer.Aerodynamics_FrontWing_Drag_Total = player.Aerodynamics_FrontWing_Drag_Total;
            nwPlayer.Aerodynamics_FrontWing_Drag_Base = player.Aerodynamics_FrontWing_Drag_Base;
            nwPlayer.Aerodynamics_RearWing_Drag_Total = player.Aerodynamics_RearWing_Drag_Total;
            nwPlayer.Aerodynamics_RearWing_Drag_Base = player.Aerodynamics_RearWing_Drag_Base;
            nwPlayer.Aerodynamics_LeftFender_Drag = player.Aerodynamics_LeftFender_Drag;
            nwPlayer.Aerodynamics_RightFender_Drag = player.Aerodynamics_RightFender_Drag;
            nwPlayer.Aerodynamics_Body_Drag = player.Aerodynamics_Body_Drag;
            nwPlayer.Aerodynamics_Body_DragHeightDiff = player.Aerodynamics_Body_DragHeightDiff;
            nwPlayer.Aerodynamics_Body_DragHeightAvg = player.Aerodynamics_Body_DragHeightAvg;
            nwPlayer.Aerodynamics_Radiator_Drag = player.Aerodynamics_Radiator_Drag;
            nwPlayer.Aerodynamics_Radiator_Setting = player.Aerodynamics_Radiator_Setting;
            nwPlayer.Aerodynamics_BrakeDuct_Drag = player.Aerodynamics_BrakeDuct_Drag;
            nwPlayer.Aerodynamics_BrakeDuct_Setting = player.Aerodynamics_BrakeDuct_Setting;
            nwPlayer.Engine_RPM = player.Engine_RPM;
            nwPlayer.Engine_RPM_Max_Live = player.Engine_RPM_Max_Live;
            nwPlayer.Engine_RPM_Max_Scale = player.Engine_RPM_Max_Scale;
            nwPlayer.Engine_RPM_Max_Step = player.Engine_RPM_Max_Step;
            nwPlayer.Engine_RPM_Idle_Min = player.Engine_RPM_Idle_Min;
            nwPlayer.Engine_RPM_Idle_Max = player.Engine_RPM_Idle_Max;
            nwPlayer.Engine_RPM_Launch_Min = player.Engine_RPM_Launch_Min;
            nwPlayer.Engine_RPM_Launch_Max = player.Engine_RPM_Launch_Max;
            nwPlayer.Engine_Idle_ThrottleGain = player.Engine_Idle_ThrottleGain;
            nwPlayer.Engine_Torque_Negative = player.Engine_Torque_Negative;
            nwPlayer.Engine_Torque = player.Engine_Torque;
            nwPlayer.Powertrain_DrivenWheels = player.Powertrain_DrivenWheels;
            nwPlayer.Powertrain_DriverDistribution = player.Powertrain_DriverDistribution;
            nwPlayer.Brake_TypicalFailure_LF = player.Brake_TypicalFailure_LF;
            nwPlayer.Brake_TypicalFailure_LR = player.Brake_TypicalFailure_LR;
            nwPlayer.Brake_TypicalFailure_RF = player.Brake_TypicalFailure_RF;
            nwPlayer.Brake_TypicalFailure_RR = player.Brake_TypicalFailure_RR;
            nwPlayer.Brake_Temperature_LF = player.Brake_Temperature_LF;
            nwPlayer.Brake_Temperature_LR = player.Brake_Temperature_LR;
            nwPlayer.Brake_Temperature_RF = player.Brake_Temperature_RF;
            nwPlayer.Brake_Temperature_RR = player.Brake_Temperature_RR;
            nwPlayer.Brake_Thickness_LF = player.Brake_Thickness_LF;
            nwPlayer.Brake_Thickness_LR = player.Brake_Thickness_LR;
            nwPlayer.Brake_Thickness_RF = player.Brake_Thickness_RF;
            nwPlayer.Brake_Thickness_RR = player.Brake_Thickness_RR;
            nwPlayer.Brake_OptimalTemperature_LF_Low = player.Brake_OptimalTemperature_LF_Low;
            nwPlayer.Brake_OptimalTemperature_LR_Low = player.Brake_OptimalTemperature_LR_Low;
            nwPlayer.Brake_OptimalTemperature_RF_Low = player.Brake_OptimalTemperature_RF_Low;
            nwPlayer.Brake_OptimalTemperature_RR_Low = player.Brake_OptimalTemperature_RR_Low;
            nwPlayer.Brake_OptimalTemperature_LF_High = player.Brake_OptimalTemperature_LF_High;
            nwPlayer.Brake_OptimalTemperature_LR_High = player.Brake_OptimalTemperature_LR_High;
            nwPlayer.Brake_OptimalTemperature_RF_High = player.Brake_OptimalTemperature_RF_High;
            nwPlayer.Brake_OptimalTemperature_RR_High = player.Brake_OptimalTemperature_RR_High;
            nwPlayer.Brake_Torque_LF = player.Brake_Torque_LF;
            nwPlayer.Brake_Torque_LR = player.Brake_Torque_LR;
            nwPlayer.Brake_Torque_RF = player.Brake_Torque_RF;
            nwPlayer.Brake_Torque_RR = player.Brake_Torque_RR;
            nwPlayer.Clutch_Torque = player.Clutch_Torque;
            nwPlayer.Clutch_Friction = player.Clutch_Friction;
            nwPlayer.Pedals_Clutch = player.Pedals_Clutch;
            nwPlayer.Pedals_Throttle = player.Pedals_Throttle;
            nwPlayer.Pedals_Brake = player.Pedals_Brake;
            nwPlayer.SteeringAngle = player.SteeringAngle;
            nwPlayer.DrivingHelp_BrakingHelp = player.DrivingHelp_BrakingHelp;
            nwPlayer.DrivingHelp_SteeringHelp = player.DrivingHelp_SteeringHelp;
            nwPlayer.DrivingHelp_TractionControl = player.DrivingHelp_TractionControl;
            nwPlayer.DrivingHelp_OppositeLock = player.DrivingHelp_OppositeLock;
            nwPlayer.DrivingHelp_SpinRecovery = player.DrivingHelp_SpinRecovery;
            nwPlayer.DrivingHelp_StabilityControl = player.DrivingHelp_StabilityControl;
            nwPlayer.DrivingHelp_AutoClutch = player.DrivingHelp_AutoClutch;
            nwPlayer.Speed = player.Speed;
            nwPlayer.Brake_InitialThickness_LF = player.Brake_InitialThickness_LF;
            nwPlayer.Brake_InitialThickness_RF = player.Brake_InitialThickness_RF;
            nwPlayer.Brake_InitialThickness_LR = player.Brake_InitialThickness_LR;
            nwPlayer.Brake_InitialThickness_RR = player.Brake_InitialThickness_RR;
            nwPlayer.SpeedSlipping = player.SpeedSlipping;

            return nwPlayer;

        }

    }
}
