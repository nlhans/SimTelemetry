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
using SimTelemetry.Objects;

namespace SimTelemetry.Game.GTR2
{
    public class DriverPlayer : IDriverPlayer
    {
        public double Engine_Lifetime_Live
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68370)); }
            set { }
        }

        public double Engine_Lifetime_Typical
        {
            get { return 0; } // return GTR2.Game.ReadDouble(new IntPtr(0x00B68388)); } // TODO
            set { }
        }

        public double Engine_Lifetime_Variation
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public double Engine_Lifetime_Oil_Base
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68388)); }
            set { }
        }

        public double Engine_Lifetime_RPM_Base
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68378)); }
            set { }
        }

        public double Engine_Temperature_Oil
        {
            get { return 1/GTR2.Game.ReadDouble(new IntPtr(0x00B68390)); }
            set { }
        }

        public double Engine_Temperature_Water
        {
            get { return Engine_Temperature_Oil; } // TODO: Search memory address.
            set { }
        }

        public int Engine_BoostSetting
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public int Gear
        {
            get { return GTR2.Game.ReadByte(new IntPtr(0x00B684E0)); }
            set { }
        }

        public int PitStop_Number
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public int PitStop_FuelStop1
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public int PitStop_FuelStop2
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public int PitStop_FuelStop3
        {
            get { return 0; } // TODO: Search memory address.
            set { }
        }

        public double RAM_Fuel
        {
            get { return 0; }
            set { }
        }

        public double RAM_Torque
        {
            get { return 0; }
            set { }
        }

        public double RAM_Power
        {
            get { return 0; }
            set { }
        }

        public double Fuel
        {
            get { return GTR2.Drivers.Player.Fuel; } 
            set { }
        }

        public double Suspension_SpendleTorque_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_SpendleTorque_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_LF_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_LR_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_RF_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_RR_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Suspension_RideHeight_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public string Tyre_Compound_Front
        {
            get { return ""; }// TODO: Search memory address.
            set { }
        }

        public string Tyre_Compound_Rear
        {
            get { return ""; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Grip_Forwards_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Weight_Rearwheel
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double DryWeight
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_Optimal_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_Optimal_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_Optimal_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_Optimal_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_LF_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_LR_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_RF_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Pressure_RR_G
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Grip_Sidewards_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Grip_Sidewards_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Grip_Sidewards_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Grip_Sidewards_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Speed_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Speed_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Speed_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Speed_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Tyre_Temperature_LF_Inside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LR_Inside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RF_Inside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RR_Inside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LF_Middle
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LR_Middle
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RF_Middle
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RR_Middle
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LF_Outside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LR_Outside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RF_Outside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RR_Outside
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LF_Optimal
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LR_Optimal
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RF_Optimal
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RR_Optimal
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LF_Fresh
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_LR_Fresh
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RF_Fresh
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Temperature_RR_Fresh
        {
            get { return 0; }
            set { }
        }

        public double Wheel_Radius_LF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Wheel_Radius_LR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Wheel_Radius_RF
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public double Wheel_Radius_RR
        {
            get { return 0; }// TODO: Search memory address.
            set { }
        }

        public int Aerodynamics_FrontWing_Setting
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_FrontWing_Downforce
        {
            get { return 0; }
            set { }
        }

        public int Aerodynamics_RearWing_Setting
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_RearWing_Downforce
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_FrontWing_Drag_Total
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_FrontWing_Drag_Base
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_RearWing_Drag_Total
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_RearWing_Drag_Base
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_LeftFender_Drag
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_RightFender_Drag
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_Body_Drag
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_Body_DragHeightDiff
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_Body_DragHeightAvg
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_Radiator_Drag
        {
            get { return 0; }
            set { }
        }

        public int Aerodynamics_Radiator_Setting
        {
            get { return 0; }
            set { }
        }

        public double Aerodynamics_BrakeDuct_Drag
        {
            get { return 0; }
            set { }
        }

        public int Aerodynamics_BrakeDuct_Setting
        {
            get { return 0; }
            set { }
        }

        public double Engine_RPM
        {
            get { return GTR2.Game.ReadFloat(new IntPtr(0x920554)); }
            set { }
        }

        public double Engine_RPM_Max_Live
        {
            get { return GTR2.Game.ReadFloat(new IntPtr( GTR2.Drivers.Player.BaseAddress + 0x4350)); }
            set { }
        }

        public double Engine_RPM_Max_Scale
        {
            get { return 0; }
            set { }
        }

        public int Engine_RPM_Max_Step
        {
            get { return 0;  }
            set { }
        }

        public double Engine_RPM_Idle_Min
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68238)); }
            set { }
        }

        public double Engine_RPM_Idle_Max
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68240)); }
            set { }
        }

        public double Engine_RPM_Launch_Min
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B682F8)); }
            set { }
        }

        public double Engine_RPM_Launch_Max
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68300)); }
            set { }
        }

        public double Engine_Idle_ThrottleGain
        {
            get { return GTR2.Game.ReadDouble(new IntPtr(0x00B68230)); }
            set { }
        }

        public double Engine_Torque_Negative
        {
            get { return 0; }
            set { }
        }

        public double Engine_Torque
        {
            get
            {
                return GTR2.Game.ReadFloat(new IntPtr(0x920588)) / Rotations.Rads_RPM(Engine_RPM) * 5252;
            }
            set { }
        }

        public PowerTrainDrivenWheels Powertrain_DrivenWheels
        {
            get { return 0; }
            set { }
        }

        public double Powertrain_DriverDistribution
        {
            get { return 0; }
            set { }
        }

        public double Brake_TypicalFailure_LF
        {
            get { return 0; }
            set { }
        }

        public double Brake_TypicalFailure_LR
        {
            get { return 0; }
            set { }
        }

        public double Brake_TypicalFailure_RF
        {
            get { return 0; }
            set { }
        }

        public double Brake_TypicalFailure_RR
        {
            get { return 0; }
            set { }
        }

        public double Brake_Temperature_LF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Temperature_LR
        {
            get { return 0; }
            set { }
        }

        public double Brake_Temperature_RF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Temperature_RR
        {
            get { return 0; }
            set { }
        }

        public double Brake_Thickness_LF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Thickness_LR
        {
            get { return 0; }
            set { }
        }

        public double Brake_Thickness_RF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Thickness_RR
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_LF_Low
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_LR_Low
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_RF_Low
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_RR_Low
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_LF_High
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_LR_High
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_RF_High
        {
            get { return 0; }
            set { }
        }

        public double Brake_OptimalTemperature_RR_High
        {
            get { return 0; }
            set { }
        }

        public double Brake_Torque_LF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Torque_LR
        {
            get { return 0; }
            set { }
        }

        public double Brake_Torque_RF
        {
            get { return 0; }
            set { }
        }

        public double Brake_Torque_RR
        {
            get { return 0; }
            set { }
        }

        public double Clutch_Torque
        {
            get { return 0; }
            set { }
        }

        public double Clutch_Friction
        {
            get { return 0; }
            set { }
        }

        public double Pedals_Clutch
        {
            get { return 0; }
            set { }
        }

        public double Pedals_Throttle
        {
            get { return 0; }
            set { }
        }

        public double Pedals_Brake
        {
            get { return 0; }
            set { }
        }

        public double SteeringAngle
        {
            get { return 0; }
            set { }
        }

        public LevelIndicator DrivingHelp_BrakingHelp
        {
            get { return 0; }
            set { }
        }

        public LevelIndicator DrivingHelp_SteeringHelp
        {
            get { return 0; }
            set { }
        }

        public LevelIndicator DrivingHelp_TractionControl
        {
            get { return 0; }
            set { }
        }

        public bool DrivingHelp_OppositeLock
        {
            get { return false; }
            set { }
        }

        public bool DrivingHelp_SpinRecovery
        {
            get { return false; }
            set { }
        }

        public bool DrivingHelp_StabilityControl
        {
            get { return false; }
            set { }
        }

        public bool DrivingHelp_AutoClutch
        {
            get { return false; }
            set { }
        }

        public double Speed
        {
            get { return GTR2.Drivers.Player.Speed; }
            set { }
        }

        public double Brake_InitialThickness_LF
        {
            get { return 0; }
            set { }
        }

        public double Brake_InitialThickness_RF
        {
            get { return 0; }
            set { }
        }

        public double Brake_InitialThickness_LR
        {
            get { return 0; }
            set { }
        }

        public double Brake_InitialThickness_RR
        {
            get { return 0; }
            set { }
        }

        public double SpeedSlipping
        {
            get { return 0; }
            set { }
        }
    }
}