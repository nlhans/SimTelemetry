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
        public double Engine_Lifetime { get; set; }
        public double Engine_Lifetime_Typical { get; set; }
        public double Engine_Lifetime_Variation { get; set; }
        public double Engine_Lifetime_Oil_Base { get; set; }
        public double Engine_Lifetime_RPM_Base { get; set; }
        public double Engine_Temperature_Oil { get; set; }
        public double Engine_Temperature_Water { get; set; }
        public int EngineMode { get; set; }
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


            nwPlayer.Engine_Lifetime = player.Engine_Lifetime;
            nwPlayer.Engine_Temperature_Oil = player.Engine_Temperature_Oil;
            nwPlayer.Engine_Temperature_Water = player.Engine_Temperature_Water;
            nwPlayer.EngineMode = player.EngineMode;
            nwPlayer.Gear = player.Gear;
            nwPlayer.PitStop_Number = player.PitStop_Number;
            nwPlayer.Fuel = player.Fuel;
            nwPlayer.Suspension_SpendleTorque_LF = player.Suspension_SpendleTorque_LF;
            nwPlayer.Suspension_SpendleTorque_RF = player.Suspension_SpendleTorque_RF;
            nwPlayer.Suspension_RideHeight_LF = player.Suspension_RideHeight_LF;
            nwPlayer.Suspension_RideHeight_LR = player.Suspension_RideHeight_LR;
            nwPlayer.Suspension_RideHeight_RF = player.Suspension_RideHeight_RF;
            nwPlayer.Suspension_RideHeight_RR = player.Suspension_RideHeight_RR;
            nwPlayer.Tyre_Compound_Front = player.Tyre_Compound_Front;
            nwPlayer.Tyre_Compound_Rear = player.Tyre_Compound_Rear;
            nwPlayer.DryWeight = player.DryWeight;
            nwPlayer.Tyre_Pressure_LF = player.Tyre_Pressure_LF;
            nwPlayer.Tyre_Pressure_LR = player.Tyre_Pressure_LR;
            nwPlayer.Tyre_Pressure_RF = player.Tyre_Pressure_RF;
            nwPlayer.Tyre_Pressure_RR = player.Tyre_Pressure_RR;
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
            nwPlayer.Engine_RPM = player.Engine_RPM;
            nwPlayer.Engine_RPM_Max_Live = player.Engine_RPM_Max_Live;
            nwPlayer.Engine_RPM_Max_Scale = player.Engine_RPM_Max_Scale;
            nwPlayer.Engine_RPM_Max_Step = player.Engine_RPM_Max_Step;
            nwPlayer.Engine_Torque = player.Engine_Torque;
            nwPlayer.Powertrain_DriverDistribution = player.Powertrain_DriverDistribution;
            nwPlayer.Brake_Temperature_LF = player.Brake_Temperature_LF;
            nwPlayer.Brake_Temperature_LR = player.Brake_Temperature_LR;
            nwPlayer.Brake_Temperature_RF = player.Brake_Temperature_RF;
            nwPlayer.Brake_Temperature_RR = player.Brake_Temperature_RR;
            nwPlayer.Brake_Thickness_LF = player.Brake_Thickness_LF;
            nwPlayer.Brake_Thickness_LR = player.Brake_Thickness_LR;
            nwPlayer.Brake_Thickness_RF = player.Brake_Thickness_RF;
            nwPlayer.Brake_Thickness_RR = player.Brake_Thickness_RR;
            nwPlayer.Brake_Torque_LF = player.Brake_Torque_LF;
            nwPlayer.Brake_Torque_LR = player.Brake_Torque_LR;
            nwPlayer.Brake_Torque_RF = player.Brake_Torque_RF;
            nwPlayer.Brake_Torque_RR = player.Brake_Torque_RR;
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

            return nwPlayer;

        }

    }
}
