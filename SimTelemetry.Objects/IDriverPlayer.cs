namespace SimTelemetry.Objects
{
    public interface IDriverPlayer
    {

        [Loggable(0.2)]
        int PitStop_Number { get; set;}

        [Loggable(0.2)]
        int PitStop_FuelStop1 { get; set;}

        [Loggable(0.2)]
        int PitStop_FuelStop2 { get; set;}

        [Loggable(0.2)]
        int PitStop_FuelStop3 { get; set;}

        [Loggable(0.2)]
        double RAM_Fuel { get; set;}

        [Loggable(0.2)]
        double RAM_Torque { get; set;}

        [Loggable(0.2)]
        double RAM_Power { get; set;}

        [Loggable(2)]
        double Fuel { get; set;}

        [Loggable(0.1)]
        double Suspension_SpendleTorque_LF { get; set;}

        [Loggable(0.1)]
        double Suspension_SpendleTorque_RF { get; set;}

        [Loggable(0.1)]
        double Suspension_RideHeight_LF_G { get; set;}

        [Loggable(0.1)]
        double Suspension_RideHeight_LR_G { get; set;}

        [Loggable(0.1)]
        double Suspension_RideHeight_RF_G { get; set;}

        [Loggable(0.1)]
        double Suspension_RideHeight_RR_G { get; set;}

        [Loggable(25)]
        double Suspension_RideHeight_LF { get; set;}

        [Loggable(25)]
        double Suspension_RideHeight_LR { get; set;}

        [Loggable(25)]
        double Suspension_RideHeight_RF { get; set;}

        [Loggable(25)]
        double Suspension_RideHeight_RR { get; set;}

        [Loggable(0.1)]
        string Tyre_Compound_Front { get; set;}

        [Loggable(0.1)]
        string Tyre_Compound_Rear { get; set;}

        [Loggable(0.1)]
        double Tyre_Grip_Forwards_LF { get; set;}

        [Loggable(2)]
        double Tyre_Pressure_LF { get; set;}

        [Loggable(2)]
        double Tyre_Pressure_LR { get; set;}

        [Loggable(2)]
        double Tyre_Pressure_RF { get; set;}

        [Loggable(2)]
        double Tyre_Pressure_RR { get; set;}

        [Loggable(0.1)]
        double Tyre_Pressure_LF_G { get; set;}

        [Loggable(0.1)]
        double Tyre_Pressure_LR_G { get; set;}

        [Loggable(0.1)]
        double Tyre_Pressure_RF_G { get; set;}

        [Loggable(0.1)]
        double Tyre_Pressure_RR_G { get; set;}

        [Loggable(0.1)]
        double Tyre_Grip_Sidewards_LF { get; set;}

        [Loggable(0.1)]
        double Tyre_Grip_Sidewards_LR { get; set;}

        [Loggable(0.1)]
        double Tyre_Grip_Sidewards_RF { get; set;}

        [Loggable(0.1)]
        double Tyre_Grip_Sidewards_RR { get; set;}

        [Loggable(25)]
        double Tyre_Speed_LF { get; set;}

        [Loggable(25)]
        double Tyre_Speed_LR { get; set;}

        [Loggable(25)]
        double Tyre_Speed_RF { get; set;}

        [Loggable(25)]
        double Tyre_Speed_RR { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LF_Inside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LR_Inside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RF_Inside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RR_Inside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LF_Middle { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LR_Middle { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RF_Middle { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RR_Middle { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LF_Outside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_LR_Outside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RF_Outside { get; set;}

        [Loggable(2)]
        double Tyre_Temperature_RR_Outside { get; set;}

        [Loggable(0.2)]
        double Tyre_Temperature_LF_Fresh { get; set;}

        [Loggable(0.2)]
        double Tyre_Temperature_LR_Fresh { get; set;}

        [Loggable(0.2)]
        double Tyre_Temperature_RF_Fresh { get; set;}

        [Loggable(0.2)]
        double Tyre_Temperature_RR_Fresh { get; set;}

        [Loggable(0.1)]
        double Wheel_Radius_LF { get; set;}

        [Loggable(0.1)]
        double Wheel_Radius_LR { get; set;}

        [Loggable(0.1)]
        double Wheel_Radius_RF { get; set;}

        [Loggable(0.1)]
        double Wheel_Radius_RR { get; set;}

        [Loggable(0.2)]
        int Aerodynamics_FrontWing_Setting { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_FrontWing_Downforce { get; set;}

        [Loggable(0.2)]
        int Aerodynamics_RearWing_Setting { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_RearWing_Downforce { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_FrontWing_Drag_Total { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_FrontWing_Drag_Base { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_RearWing_Drag_Total { get; set;}

        [Loggable(0.2)]
        double Aerodynamics_RearWing_Drag_Base { get; set;}

        [Loggable(25)]
        double Engine_RPM { get; set;}

        [Loggable(1)]
        double Engine_RPM_Max_Live { get; set;}

        [Loggable(0.1)]
        double Engine_RPM_Max_Scale { get; set;}

        [Loggable(0.1)]
        int Engine_RPM_Max_Step { get; set;}

        [Loggable(0.1)]
        double Engine_RPM_Idle_Min { get; set;}

        [Loggable(0.1)]
        double Engine_RPM_Idle_Max { get; set;}

        [Loggable(0.1)]
        double Engine_RPM_Launch_Min { get; set;}

        [Loggable(0.1)]
        double Engine_RPM_Launch_Max { get; set;}

        [Loggable(0.1)]
        double Engine_Idle_ThrottleGain { get; set;}

        [Loggable(25)]
        double Engine_Torque_Negative { get; set;}

        [Loggable(25)]
        double Engine_Torque { get; set;}

        [Loggable(0.2)]
        PowerTrain_DrivenWheels Powertrain_DrivenWheels { get; set;}

        [Loggable(0.2)]
        double Powertrain_DriverDistribution { get; set;}

        [Loggable(10)]
        double Brake_Temperature_LF { get; set;}

        [Loggable(10)]
        double Brake_Temperature_LR { get; set;}

        [Loggable(10)]
        double Brake_Temperature_RF { get; set;}

        [Loggable(10)]
        double Brake_Temperature_RR { get; set;}

        [Loggable(0.2)]
        double Brake_Thickness_LF { get; set;}

        [Loggable(0.2)]
        double Brake_Thickness_LR { get; set;}

        [Loggable(0.2)]
        double Brake_Thickness_RF { get; set;}

        [Loggable(0.2)]
        double Brake_Thickness_RR { get; set;}

        [Loggable(0.2)]
        double Brake_Torque_LF { get; set;}

        [Loggable(0.2)]
        double Brake_Torque_LR { get; set;}

        [Loggable(0.2)]
        double Brake_Torque_RF { get; set;}

        [Loggable(0.2)]
        double Brake_Torque_RR { get; set;}

        [Loggable(0.2)]
        double Clutch_Torque { get; set;}

        [Loggable(0.2)]
        double Clutch_Friction { get; set;}

        [Loggable(25)]
        double Pedals_Clutch { get; set;}

        [Loggable(25)]
        double Pedals_Throttle { get; set;}

        [Loggable(25)]
        double Pedals_Brake { get; set;}

        [Loggable(25)]
        double SteeringAngle { set; get; }

        [Loggable(0.2)]
        LevelIndicator DrivingHelp_BrakingHelp { get; set;}

        [Loggable(0.2)]
        LevelIndicator DrivingHelp_SteeringHelp { get; set;}

        [Loggable(0.2)]
        LevelIndicator DrivingHelp_TractionControl { get; set;}

        [Loggable(0.2)]
        bool DrivingHelp_OppositeLock { get; set;}

        [Loggable(0.2)]
        bool DrivingHelp_SpinRecovery { get; set;}

        [Loggable(0.2)]
        bool DrivingHelp_StabilityControl { get; set;}

        [Loggable(25)]
        double Speed { get; set; }
    }
}