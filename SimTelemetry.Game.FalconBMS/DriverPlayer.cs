using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.FalconBMS
{
    public class DriverPlayer : IDriverPlayer
    {
        public double Engine_Lifetime_Live
        {
            get { return 0; }
            set { }
        }

        public double Engine_Lifetime_Typical
        {
            get { return 0; }
            set { }
        }

        public double Engine_Lifetime_Variation
        {
            get { return 0; }
            set { }
        }

        public double Engine_Lifetime_Oil_Base
        {
            get { return 0; }
            set { }
        }

        public double Engine_Lifetime_RPM_Base
        {
            get { return Engine_RPM_Max_Live; }
            set { }
        }

        public double Engine_Temperature_Oil
        {
            get { return 0; }
            set { }
        }

        public double Engine_Temperature_Water
        {
            get { return 0; }
            set { }
        }

        public int Engine_BoostSetting
        {
            get { return 0; }
            set { }
        }

        public int Gear
        {
            get { return 0; }
            set { }
        }

        public int PitStop_Number
        {
            get { return 0; }
            set { }
        }

        public int PitStop_FuelStop1
        {
            get { return 0; }
            set { }
        }

        public int PitStop_FuelStop2
        {
            get { return 0; }
            set { }
        }

        public int PitStop_FuelStop3
        {
            get { return 0; }
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
            get { return 0; }
            set { }
        }

        public double Suspension_SpendleTorque_LF
        {
            get { return 0; }
            set { }
        }

        public double Suspension_SpendleTorque_RF
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_LF_G
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_LR_G
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_RF_G
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_RR_G
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_LF
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_LR
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_RF
        {
            get { return 0; }
            set { }
        }

        public double Suspension_RideHeight_RR
        {
            get { return 0; }
            set { }
        }

        public string Tyre_Compound_Front
        {
            get { return ""; }
            set { }
        }

        public string Tyre_Compound_Rear
        {
            get { return ""; }
            set { }
        }

        public double Tyre_Grip_Forwards_LF
        {
            get { return 0; }
            set { }
        }

        public double Weight_Rearwheel
        {
            get { return 0; }
            set { }
        }

        public double DryWeight
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_Optimal_LF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_Optimal_LR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_Optimal_RF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_Optimal_RR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_LF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_LR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_RF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_RR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_LF_G
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_LR_G
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_RF_G
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Pressure_RR_G
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Grip_Sidewards_LF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Grip_Sidewards_LR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Grip_Sidewards_RF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Grip_Sidewards_RR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Speed_LF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Speed_LR
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Speed_RF
        {
            get { return 0; }
            set { }
        }

        public double Tyre_Speed_RR
        {
            get { return 0; }
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
            get { return 0; }
            set { }
        }

        public double Wheel_Radius_LR
        {
            get { return 0; }
            set { }
        }

        public double Wheel_Radius_RF
        {
            get { return 0; }
            set { }
        }

        public double Wheel_Radius_RR
        {
            get { return 0; }
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
            get { return Rotations.RPM_Rads(FalconBms.Drivers.Player.RPM); }
            set { }
        }

        public double Engine_RPM_Max_Live
        {
            get { return Rotations.RPM_Rads(1900*0.6); }
            set { }
        }

        public double Engine_RPM_Max_Scale
        {
            get { return 0; }
            set { }
        }

        public int Engine_RPM_Max_Step
        {
            get { return 0; }
            set { }
        }

        public double Engine_RPM_Idle_Min
        {
            get { return 0; }
            set { }
        }

        public double Engine_RPM_Idle_Max
        {
            get { return 0; }
            set { }
        }

        public double Engine_RPM_Launch_Min
        {
            get { return 0; }
            set { }
        }

        public double Engine_RPM_Launch_Max
        {
            get { return 0; }
            set { }
        }

        public double Engine_Idle_ThrottleGain
        {
            get { return 0; }
            set { }
        }

        public double Engine_Torque_Negative
        {
            get { return 0; }
            set { }
        }

        public double Engine_Torque
        {
            get { return 0; }
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
            get { return FalconBms.Drivers.Player.Speed; }
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