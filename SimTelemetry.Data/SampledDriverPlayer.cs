using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class SampledDriverPlayer : IDriverPlayer
    {
        public SampledDriverPlayer Duplicate()
        {
            return (SampledDriverPlayer) this.MemberwiseClone();
        }

        private int _pitStopNumber;

        private int _pitStopFuelStop1;

        private int _pitStopFuelStop2;

        private int _pitStopFuelStop3;

        private double _ramFuel;

        private double _ramTorque;

        private double _ramPower;

        private double _fuel;

        private double _suspensionSpendleTorqueLf;

        private double _suspensionSpendleTorqueRf;

        private double _suspensionRideHeightLfG;

        private double _suspensionRideHeightLrG;

        private double _suspensionRideHeightRfG;

        private double _suspensionRideHeightRrG;

        private double _suspensionRideHeightLf;

        private double _suspensionRideHeightLr;

        private double _suspensionRideHeightRf;

        private double _suspensionRideHeightRr;

        private string _tyreCompoundFront;

        private string _tyreCompoundRear;

        private double _tyreGripForwardsLf;

        private double _tyrePressureLf;

        private double _tyrePressureLr;

        private double _tyrePressureRf;

        private double _tyrePressureRr;

        private double _tyrePressureLfG;

        private double _tyrePressureLrG;

        private double _tyrePressureRfG;

        private double _tyrePressureRrG;

        private double _tyreGripSidewardsLf;

        private double _tyreGripSidewardsLr;

        private double _tyreGripSidewardsRf;

        private double _tyreGripSidewardsRr;

        private double _tyreSpeedLf;

        private double _tyreSpeedLr;

        private double _tyreSpeedRf;

        private double _tyreSpeedRr;

        private double _tyreTemperatureLfInside;

        private double _tyreTemperatureLrInside;

        private double _tyreTemperatureRfInside;

        private double _tyreTemperatureRrInside;

        private double _tyreTemperatureLfMiddle;

        private double _tyreTemperatureLrMiddle;

        private double _tyreTemperatureRfMiddle;

        private double _tyreTemperatureRrMiddle;

        private double _tyreTemperatureLfOutside;

        private double _tyreTemperatureLrOutside;

        private double _tyreTemperatureRfOutside;

        private double _tyreTemperatureRrOutside;

        private double _tyreTemperatureLfFresh;

        private double _tyreTemperatureLrFresh;

        private double _tyreTemperatureRfFresh;

        private double _tyreTemperatureRrFresh;

        private double _wheelRadiusLf;

        private double _wheelRadiusLr;

        private double _wheelRadiusRf;

        private double _wheelRadiusRr;

        private int _aerodynamicsFrontWingSetting;

        private double _aerodynamicsFrontWingDownforce;

        private int _aerodynamicsRearWingSetting;

        private double _aerodynamicsRearWingDownforce;

        private double _aerodynamicsFrontWingDragTotal;

        private double _aerodynamicsFrontWingDragBase;

        private double _aerodynamicsRearWingDragTotal;

        private double _aerodynamicsRearWingDragBase;

        private double _engineRpm;

        private double _engineRpmMaxLive;

        private double _engineRpmMaxScale;

        private int _engineRpmMaxStep;

        private double _engineRpmIdleMin;

        private double _engineRpmIdleMax;

        private double _engineRpmLaunchMin;

        private double _engineRpmLaunchMax;

        private double _engineIdleThrottleGain;

        private double _engineTorqueNegative;

        private double _engineTorque;

        private PowerTrain_DrivenWheels _powertrainDrivenWheels;

        private double _powertrainDriverDistribution;

        private double _brakeTemperatureLf;

        private double _brakeTemperatureLr;

        private double _brakeTemperatureRf;

        private double _brakeTemperatureRr;

        private double _brakeThicknessLf;

        private double _brakeThicknessLr;

        private double _brakeThicknessRf;

        private double _brakeThicknessRr;

        private double _brakeTorqueLf;

        private double _brakeTorqueLr;

        private double _brakeTorqueRf;

        private double _brakeTorqueRr;

        private double _clutchTorque;

        private double _clutchFriction;

        private double _pedalsClutch;

        private double _pedalsThrottle;

        private double _pedalsBrake;

        private LevelIndicator _drivingHelpBrakingHelp;

        private LevelIndicator _drivingHelpSteeringHelp;

        private LevelIndicator _drivingHelpTractionControl;

        private bool _drivingHelpOppositeLock;

        private bool _drivingHelpSpinRecovery;

        private bool _drivingHelpStabilityControl;

        public int PitStop_Number
        {
            get { return _pitStopNumber; }
            set { _pitStopNumber = value; }
        }

        public int PitStop_FuelStop1
        {
            get { return _pitStopFuelStop1; }
            set { _pitStopFuelStop1 = value; }
        }

        public int PitStop_FuelStop2
        {
            get { return _pitStopFuelStop2; }
            set { _pitStopFuelStop2 = value; }
        }

        public int PitStop_FuelStop3
        {
            get { return _pitStopFuelStop3; }
            set { _pitStopFuelStop3 = value; }
        }

        public double RAM_Fuel
        {
            get { return _ramFuel; }
            set { _ramFuel = value; }
        }

        public double RAM_Torque
        {
            get { return _ramTorque; }
            set { _ramTorque = value; }
        }

        public double RAM_Power
        {
            get { return _ramPower; }
            set { _ramPower = value; }
        }

        public double Fuel
        {
            get { return _fuel; }
            set { _fuel = value; }
        }

        public double Suspension_SpendleTorque_LF
        {
            get { return _suspensionSpendleTorqueLf; }
            set { _suspensionSpendleTorqueLf = value; }
        }

        public double Suspension_SpendleTorque_RF
        {
            get { return _suspensionSpendleTorqueRf; }
            set { _suspensionSpendleTorqueRf = value; }
        }

        public double Suspension_RideHeight_LF_G
        {
            get { return _suspensionRideHeightLfG; }
            set { _suspensionRideHeightLfG = value; }
        }

        public double Suspension_RideHeight_LR_G
        {
            get { return _suspensionRideHeightLrG; }
            set { _suspensionRideHeightLrG = value; }
        }

        public double Suspension_RideHeight_RF_G
        {
            get { return _suspensionRideHeightRfG; }
            set { _suspensionRideHeightRfG = value; }
        }

        public double Suspension_RideHeight_RR_G
        {
            get { return _suspensionRideHeightRrG; }
            set { _suspensionRideHeightRrG = value; }
        }

        public double Suspension_RideHeight_LF
        {
            get { return _suspensionRideHeightLf; }
            set { _suspensionRideHeightLf = value; }
        }

        public double Suspension_RideHeight_LR
        {
            get { return _suspensionRideHeightLr; }
            set { _suspensionRideHeightLr = value; }
        }

        public double Suspension_RideHeight_RF
        {
            get { return _suspensionRideHeightRf; }
            set { _suspensionRideHeightRf = value; }
        }

        public double Suspension_RideHeight_RR
        {
            get { return _suspensionRideHeightRr; }
            set { _suspensionRideHeightRr = value; }
        }

        public string Tyre_Compound_Front
        {
            get { return _tyreCompoundFront; }
            set { _tyreCompoundFront = value; }
        }

        public string Tyre_Compound_Rear
        {
            get { return _tyreCompoundRear; }
            set { _tyreCompoundRear = value; }
        }

        public double Tyre_Grip_Forwards_LF
        {
            get { return _tyreGripForwardsLf; }
            set { _tyreGripForwardsLf = value; }
        }

        public double Tyre_Pressure_LF
        {
            get { return _tyrePressureLf; }
            set { _tyrePressureLf = value; }
        }

        public double Tyre_Pressure_LR
        {
            get { return _tyrePressureLr; }
            set { _tyrePressureLr = value; }
        }

        public double Tyre_Pressure_RF
        {
            get { return _tyrePressureRf; }
            set { _tyrePressureRf = value; }
        }

        public double Tyre_Pressure_RR
        {
            get { return _tyrePressureRr; }
            set { _tyrePressureRr = value; }
        }

        public double Tyre_Pressure_LF_G
        {
            get { return _tyrePressureLfG; }
            set { _tyrePressureLfG = value; }
        }

        public double Tyre_Pressure_LR_G
        {
            get { return _tyrePressureLrG; }
            set { _tyrePressureLrG = value; }
        }

        public double Tyre_Pressure_RF_G
        {
            get { return _tyrePressureRfG; }
            set { _tyrePressureRfG = value; }
        }

        public double Tyre_Pressure_RR_G
        {
            get { return _tyrePressureRrG; }
            set { _tyrePressureRrG = value; }
        }

        public double Tyre_Grip_Sidewards_LF
        {
            get { return _tyreGripSidewardsLf; }
            set { _tyreGripSidewardsLf = value; }
        }

        public double Tyre_Grip_Sidewards_LR
        {
            get { return _tyreGripSidewardsLr; }
            set { _tyreGripSidewardsLr = value; }
        }

        public double Tyre_Grip_Sidewards_RF
        {
            get { return _tyreGripSidewardsRf; }
            set { _tyreGripSidewardsRf = value; }
        }

        public double Tyre_Grip_Sidewards_RR
        {
            get { return _tyreGripSidewardsRr; }
            set { _tyreGripSidewardsRr = value; }
        }

        public double Tyre_Speed_LF
        {
            get { return _tyreSpeedLf; }
            set { _tyreSpeedLf = value; }
        }

        public double Tyre_Speed_LR
        {
            get { return _tyreSpeedLr; }
            set { _tyreSpeedLr = value; }
        }

        public double Tyre_Speed_RF
        {
            get { return _tyreSpeedRf; }
            set { _tyreSpeedRf = value; }
        }

        public double Tyre_Speed_RR
        {
            get { return _tyreSpeedRr; }
            set { _tyreSpeedRr = value; }
        }

        public double Tyre_Temperature_LF_Inside
        {
            get { return _tyreTemperatureLfInside; }
            set { _tyreTemperatureLfInside = value; }
        }

        public double Tyre_Temperature_LR_Inside
        {
            get { return _tyreTemperatureLrInside; }
            set { _tyreTemperatureLrInside = value; }
        }

        public double Tyre_Temperature_RF_Inside
        {
            get { return _tyreTemperatureRfInside; }
            set { _tyreTemperatureRfInside = value; }
        }

        public double Tyre_Temperature_RR_Inside
        {
            get { return _tyreTemperatureRrInside; }
            set { _tyreTemperatureRrInside = value; }
        }

        public double Tyre_Temperature_LF_Middle
        {
            get { return _tyreTemperatureLfMiddle; }
            set { _tyreTemperatureLfMiddle = value; }
        }

        public double Tyre_Temperature_LR_Middle
        {
            get { return _tyreTemperatureLrMiddle; }
            set { _tyreTemperatureLrMiddle = value; }
        }

        public double Tyre_Temperature_RF_Middle
        {
            get { return _tyreTemperatureRfMiddle; }
            set { _tyreTemperatureRfMiddle = value; }
        }

        public double Tyre_Temperature_RR_Middle
        {
            get { return _tyreTemperatureRrMiddle; }
            set { _tyreTemperatureRrMiddle = value; }
        }

        public double Tyre_Temperature_LF_Outside
        {
            get { return _tyreTemperatureLfOutside; }
            set { _tyreTemperatureLfOutside = value; }
        }

        public double Tyre_Temperature_LR_Outside
        {
            get { return _tyreTemperatureLrOutside; }
            set { _tyreTemperatureLrOutside = value; }
        }

        public double Tyre_Temperature_RF_Outside
        {
            get { return _tyreTemperatureRfOutside; }
            set { _tyreTemperatureRfOutside = value; }
        }

        public double Tyre_Temperature_RR_Outside
        {
            get { return _tyreTemperatureRrOutside; }
            set { _tyreTemperatureRrOutside = value; }
        }

        public double Tyre_Temperature_LF_Fresh
        {
            get { return _tyreTemperatureLfFresh; }
            set { _tyreTemperatureLfFresh = value; }
        }

        public double Tyre_Temperature_LR_Fresh
        {
            get { return _tyreTemperatureLrFresh; }
            set { _tyreTemperatureLrFresh = value; }
        }

        public double Tyre_Temperature_RF_Fresh
        {
            get { return _tyreTemperatureRfFresh; }
            set { _tyreTemperatureRfFresh = value; }
        }

        public double Tyre_Temperature_RR_Fresh
        {
            get { return _tyreTemperatureRrFresh; }
            set { _tyreTemperatureRrFresh = value; }
        }

        public double Wheel_Radius_LF
        {
            get { return _wheelRadiusLf; }
            set { _wheelRadiusLf = value; }
        }

        public double Wheel_Radius_LR
        {
            get { return _wheelRadiusLr; }
            set { _wheelRadiusLr = value; }
        }

        public double Wheel_Radius_RF
        {
            get { return _wheelRadiusRf; }
            set { _wheelRadiusRf = value; }
        }

        public double Wheel_Radius_RR
        {
            get { return _wheelRadiusRr; }
            set { _wheelRadiusRr = value; }
        }

        public int Aerodynamics_FrontWing_Setting
        {
            get { return _aerodynamicsFrontWingSetting; }
            set { _aerodynamicsFrontWingSetting = value; }
        }

        public double Aerodynamics_FrontWing_Downforce
        {
            get { return _aerodynamicsFrontWingDownforce; }
            set { _aerodynamicsFrontWingDownforce = value; }
        }

        public int Aerodynamics_RearWing_Setting
        {
            get { return _aerodynamicsRearWingSetting; }
            set { _aerodynamicsRearWingSetting = value; }
        }

        public double Aerodynamics_RearWing_Downforce
        {
            get { return _aerodynamicsRearWingDownforce; }
            set { _aerodynamicsRearWingDownforce = value; }
        }

        public double Aerodynamics_FrontWing_Drag_Total
        {
            get { return _aerodynamicsFrontWingDragTotal; }
            set { _aerodynamicsFrontWingDragTotal = value; }
        }

        public double Aerodynamics_FrontWing_Drag_Base
        {
            get { return _aerodynamicsFrontWingDragBase; }
            set { _aerodynamicsFrontWingDragBase = value; }
        }

        public double Aerodynamics_RearWing_Drag_Total
        {
            get { return _aerodynamicsRearWingDragTotal; }
            set { _aerodynamicsRearWingDragTotal = value; }
        }

        public double Aerodynamics_RearWing_Drag_Base
        {
            get { return _aerodynamicsRearWingDragBase; }
            set { _aerodynamicsRearWingDragBase = value; }
        }

        public double Engine_RPM
        {
            get { return _engineRpm; }
            set { _engineRpm = value; }
        }

        public double Engine_RPM_Max_Live
        {
            get { return _engineRpmMaxLive; }
            set { _engineRpmMaxLive = value; }
        }

        public double Engine_RPM_Max_Scale
        {
            get { return _engineRpmMaxScale; }
            set { _engineRpmMaxScale = value; }
        }

        public int Engine_RPM_Max_Step
        {
            get { return _engineRpmMaxStep; }
            set { _engineRpmMaxStep = value; }
        }

        public double Engine_RPM_Idle_Min
        {
            get { return _engineRpmIdleMin; }
            set { _engineRpmIdleMin = value; }
        }

        public double Engine_RPM_Idle_Max
        {
            get { return _engineRpmIdleMax; }
            set { _engineRpmIdleMax = value; }
        }

        public double Engine_RPM_Launch_Min
        {
            get { return _engineRpmLaunchMin; }
            set { _engineRpmLaunchMin = value; }
        }

        public double Engine_RPM_Launch_Max
        {
            get { return _engineRpmLaunchMax; }
            set { _engineRpmLaunchMax = value; }
        }

        public double Engine_Idle_ThrottleGain
        {
            get { return _engineIdleThrottleGain; }
            set { _engineIdleThrottleGain = value; }
        }

        public double Engine_Torque_Negative
        {
            get { return _engineTorqueNegative; }
            set { _engineTorqueNegative = value; }
        }

        public double Engine_Torque
        {
            get { return _engineTorque; }
            set { _engineTorque = value; }
        }

        public PowerTrain_DrivenWheels Powertrain_DrivenWheels
        {
            get { return _powertrainDrivenWheels; }
            set { _powertrainDrivenWheels = value; }
        }

        public double Powertrain_DriverDistribution
        {
            get { return _powertrainDriverDistribution; }
            set { _powertrainDriverDistribution = value; }
        }

        public double Brake_Temperature_LF
        {
            get { return _brakeTemperatureLf; }
            set { _brakeTemperatureLf = value; }
        }

        public double Brake_Temperature_LR
        {
            get { return _brakeTemperatureLr; }
            set { _brakeTemperatureLr = value; }
        }

        public double Brake_Temperature_RF
        {
            get { return _brakeTemperatureRf; }
            set { _brakeTemperatureRf = value; }
        }

        public double Brake_Temperature_RR
        {
            get { return _brakeTemperatureRr; }
            set { _brakeTemperatureRr = value; }
        }

        public double Brake_Thickness_LF
        {
            get { return _brakeThicknessLf; }
            set { _brakeThicknessLf = value; }
        }

        public double Brake_Thickness_LR
        {
            get { return _brakeThicknessLr; }
            set { _brakeThicknessLr = value; }
        }

        public double Brake_Thickness_RF
        {
            get { return _brakeThicknessRf; }
            set { _brakeThicknessRf = value; }
        }

        public double Brake_Thickness_RR
        {
            get { return _brakeThicknessRr; }
            set { _brakeThicknessRr = value; }
        }

        public double Brake_Torque_LF
        {
            get { return _brakeTorqueLf; }
            set { _brakeTorqueLf = value; }
        }

        public double Brake_Torque_LR
        {
            get { return _brakeTorqueLr; }
            set { _brakeTorqueLr = value; }
        }

        public double Brake_Torque_RF
        {
            get { return _brakeTorqueRf; }
            set { _brakeTorqueRf = value; }
        }

        public double Brake_Torque_RR
        {
            get { return _brakeTorqueRr; }
            set { _brakeTorqueRr = value; }
        }

        public double Clutch_Torque
        {
            get { return _clutchTorque; }
            set { _clutchTorque = value; }
        }

        public double Clutch_Friction
        {
            get { return _clutchFriction; }
            set { _clutchFriction = value; }
        }

        public double Pedals_Clutch
        {
            get { return _pedalsClutch; }
            set { _pedalsClutch = value; }
        }

        public double Pedals_Throttle
        {
            get { return _pedalsThrottle; }
            set { _pedalsThrottle = value; }
        }

        public double Pedals_Brake
        {
            get { return _pedalsBrake; }
            set { _pedalsBrake = value; }
        }

        public double SteeringAngle
        {
            get { return _SteeringAngle; }
            set { _SteeringAngle = value; }
        }

        public LevelIndicator DrivingHelp_BrakingHelp
        {
            get { return _drivingHelpBrakingHelp; }
            set { _drivingHelpBrakingHelp = value; }
        }

        public LevelIndicator DrivingHelp_SteeringHelp
        {
            get { return _drivingHelpSteeringHelp; }
            set { _drivingHelpSteeringHelp = value; }
        }

        public LevelIndicator DrivingHelp_TractionControl
        {
            get { return _drivingHelpTractionControl; }
            set { _drivingHelpTractionControl = value; }
        }

        public bool DrivingHelp_OppositeLock
        {
            get { return _drivingHelpOppositeLock; }
            set { _drivingHelpOppositeLock = value; }
        }

        public bool DrivingHelp_SpinRecovery
        {
            get { return _drivingHelpSpinRecovery; }
            set { _drivingHelpSpinRecovery = value; }
        }

        public bool DrivingHelp_StabilityControl
        {
            get { return _drivingHelpStabilityControl; }
            set { _drivingHelpStabilityControl = value; }
        }

        private double _speed = 0;
        private double _SteeringAngle = 0;

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
    }
}