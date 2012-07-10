namespace SimTelemetry.Objects
{
    public interface IDriverPlayer
    {
        [Loggable(1)]
        double Engine_Lifetime_Live { set; get; }

        [Loggable(0.01)]
        double Engine_Lifetime_Typical { set; get; }

        [Loggable(0.01)]
        double Engine_Lifetime_Variation { set; get; }

        [Loggable(0.01)]
        double Engine_Lifetime_Oil_Base{ set; get; }

        [Loggable(0.01)]
        double Engine_Lifetime_RPM_Base { set; get; }

        [Loggable(0.01)]
        double Engine_Temperature_Oil { get; set; }

        [Loggable(0.01)]
        double Engine_Temperature_Water { get; set; }

        [Loggable(1)]
        int Engine_BoostSetting { get; set; }

        [Loggable(0.2)]
        int Gear { set; get; }

        [Loggable(0.2)]
        int PitStop_Number { set; get; }

        [Loggable(0.2)]
        int PitStop_FuelStop1 { set; get; }

        [Loggable(0.2)]
        int PitStop_FuelStop2 { set; get; }

        [Loggable(0.2)]
        int PitStop_FuelStop3 { set; get; }

        [Loggable(0.01)]
        double RAM_Fuel { set; get; }

        [Loggable(0.01)]
        double RAM_Torque { set; get; }

        [Loggable(0.01)]
        double RAM_Power { set; get; }

        [Loggable(1)]
        double Fuel { set; get; }

        [Loggable(0.01)]
        double Suspension_SpendleTorque_LF { set; get; }

        [Loggable(0.01)]
        double Suspension_SpendleTorque_RF { set; get; }

        [Loggable(0.01)]
        double Suspension_RideHeight_LF_G { set; get; }

        [Loggable(0.01)]
        double Suspension_RideHeight_LR_G { set; get; }

        [Loggable(0.01)]
        double Suspension_RideHeight_RF_G { set; get; }

        [Loggable(0.01)]
        double Suspension_RideHeight_RR_G { set; get; }

        [Loggable(25)]
        double Suspension_RideHeight_LF { set; get; }

        [Loggable(25)]
        double Suspension_RideHeight_LR { set; get; }

        [Loggable(25)]
        double Suspension_RideHeight_RF { set; get; }

        [Loggable(25)]
        double Suspension_RideHeight_RR { set; get; }

        [Loggable(0.01)]
        string Tyre_Compound_Front { set; get; }

        [Loggable(0.01)]
        string Tyre_Compound_Rear { set; get; }

        [Loggable(0.01)]
        double Tyre_Grip_Forwards_LF { set; get; }

        [Loggable(0.01)]
        double Weight_Rearwheel { get; set; }

        [Loggable(0.01)]
        double DryWeight { get; set; }

        [Loggable(0.5)]
        double Tyre_Pressure_Optimal_LF { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_Optimal_LR { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_Optimal_RF { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_Optimal_RR { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_LF { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_LR { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_RF { set; get; }

        [Loggable(0.5)]
        double Tyre_Pressure_RR { set; get; }

        [Loggable(0.01)]
        double Tyre_Pressure_LF_G { set; get; }

        [Loggable(0.01)]
        double Tyre_Pressure_LR_G { set; get; }

        [Loggable(0.01)]
        double Tyre_Pressure_RF_G { set; get; }

        [Loggable(0.01)]
        double Tyre_Pressure_RR_G { set; get; }

        [Loggable(0.01)]
        double Tyre_Grip_Sidewards_LF { set; get; }

        [Loggable(0.01)]
        double Tyre_Grip_Sidewards_LR { set; get; }

        [Loggable(0.01)]
        double Tyre_Grip_Sidewards_RF { set; get; }

        [Loggable(0.01)]
        double Tyre_Grip_Sidewards_RR { set; get; }

        [Loggable(25)]
        double Tyre_Speed_LF { set; get; }

        [Loggable(25)]
        double Tyre_Speed_LR { set; get; }

        [Loggable(25)]
        double Tyre_Speed_RF { set; get; }

        [Loggable(25)]
        double Tyre_Speed_RR { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LF_Inside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LR_Inside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RF_Inside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RR_Inside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LF_Middle { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LR_Middle { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RF_Middle { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RR_Middle { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LF_Outside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_LR_Outside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RF_Outside { set; get; }

        [Loggable(1)]
        double Tyre_Temperature_RR_Outside { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_LF_Optimal { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_LR_Optimal { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_RF_Optimal { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_RR_Optimal { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_LF_Fresh { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_LR_Fresh { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_RF_Fresh { set; get; }

        [Loggable(0.01)]
        double Tyre_Temperature_RR_Fresh { set; get; }

        [Loggable(0.01)]
        double Wheel_Radius_LF { set; get; }

        [Loggable(0.01)]
        double Wheel_Radius_LR { set; get; }

        [Loggable(0.01)]
        double Wheel_Radius_RF { set; get; }

        [Loggable(0.01)]
        double Wheel_Radius_RR { set; get; }

        [Loggable(0.2)]
        int Aerodynamics_FrontWing_Setting { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_FrontWing_Downforce { set; get; }

        [Loggable(0.2)]
        int Aerodynamics_RearWing_Setting { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_RearWing_Downforce { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_FrontWing_Drag_Total { set; get; }

        [Loggable(0.01)]
        double Aerodynamics_FrontWing_Drag_Base { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_RearWing_Drag_Total { set; get; }

        [Loggable(0.01)]
        double Aerodynamics_RearWing_Drag_Base { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_LeftFender_Drag { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_RightFender_Drag { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_Body_Drag { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_Body_DragHeightDiff { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_Body_DragHeightAvg { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_Radiator_Drag { set; get; }

        [Loggable(0.2)]
        int Aerodynamics_Radiator_Setting { set; get; }

        [Loggable(0.2)]
        double Aerodynamics_BrakeDuct_Drag { set; get; }

        [Loggable(0.2)]
        int Aerodynamics_BrakeDuct_Setting { set; get; }

        [Loggable(25)]
        double Engine_RPM { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Max_Live { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Max_Scale { set; get; }

        [Loggable(0.01)]
        int Engine_RPM_Max_Step { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Idle_Min { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Idle_Max { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Launch_Min { set; get; }

        [Loggable(0.01)]
        double Engine_RPM_Launch_Max { set; get; }

        [Loggable(0.01)]
        double Engine_Idle_ThrottleGain { set; get; }

        [Loggable(10)]
        double Engine_Torque_Negative { set; get; }

        [Loggable(10)]
        double Engine_Torque { set; get; }

        [Loggable(0.01)]
        PowerTrain_DrivenWheels Powertrain_DrivenWheels { set; get; }

        [Loggable(0.01)]
        double Powertrain_DriverDistribution { set; get; }

        [Loggable(5)]
        double Brake_TypicalFailure_LF { set; get; }

        [Loggable(5)]
        double Brake_TypicalFailure_LR { set; get; }

        [Loggable(5)]
        double Brake_TypicalFailure_RF { set; get; }

        [Loggable(5)]
        double Brake_TypicalFailure_RR { set; get; }

        [Loggable(5)]
        double Brake_Temperature_LF { set; get; }

        [Loggable(5)]
        double Brake_Temperature_LR { set; get; }

        [Loggable(5)]
        double Brake_Temperature_RF { set; get; }

        [Loggable(5)]
        double Brake_Temperature_RR { set; get; }

        [Loggable(0.2)]
        double Brake_Thickness_LF { set; get; }

        [Loggable(0.2)]
        double Brake_Thickness_LR { set; get; }

        [Loggable(0.2)]
        double Brake_Thickness_RF { set; get; }

        [Loggable(0.2)]
        double Brake_Thickness_RR { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_LF_Low { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_LR_Low { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_RF_Low { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_RR_Low { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_LF_High { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_LR_High { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_RF_High { set; get; }

        [Loggable(5)]
        double Brake_OptimalTemperature_RR_High { set; get; }

        [Loggable(0.01)]
        double Brake_Torque_LF { set; get; }

        [Loggable(0.01)]
        double Brake_Torque_LR { set; get; }

        [Loggable(0.01)]
        double Brake_Torque_RF { set; get; }

        [Loggable(0.01)]
        double Brake_Torque_RR { set; get; }

        [Loggable(0.01)]
        double Clutch_Torque { set; get; }

        [Loggable(0.01)]
        double Clutch_Friction { set; get; }

        [Loggable(25)]
        double Pedals_Clutch { set; get; }

        [Loggable(25)]
        double Pedals_Throttle { set; get; }

        [Loggable(25)]
        double Pedals_Brake { set; get; }

        [Loggable(25)]
        double SteeringAngle { set; get; }

        [Loggable(0.01)]
        LevelIndicator DrivingHelp_BrakingHelp { set; get; }

        [Loggable(0.01)]
        LevelIndicator DrivingHelp_SteeringHelp { set; get; }

        [Loggable(0.01)]
        LevelIndicator DrivingHelp_TractionControl { set; get; }

        [Loggable(0.01)]
        bool DrivingHelp_OppositeLock { set; get; }

        [Loggable(0.01)]
        bool DrivingHelp_SpinRecovery { set; get; }

        [Loggable(0.01)]
        bool DrivingHelp_StabilityControl { set; get; }

        [Loggable(0.01)]
        bool DrivingHelp_AutoClutch { set; get; }

        [Loggable(25)]
        double Speed { set; get; }

        [Loggable(0.01)]
        double Brake_InitialThickness_LF { get; set; }

        [Loggable(0.01)]
        double Brake_InitialThickness_RF { get; set; }

        [Loggable(0.01)]
        double Brake_InitialThickness_LR { get; set; }

        [Loggable(0.01)]
        double Brake_InitialThickness_RR { get; set; }

        [Loggable(25)]
        double SpeedSlipping { get; set; }
    }

}