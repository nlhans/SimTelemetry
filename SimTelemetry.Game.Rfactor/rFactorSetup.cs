using System;
using SimTelemetry.Objects.Game;

namespace SimTelemetry.Game.Rfactor
{
    public class rFactorSetupWheel : ISetupWheel
    {
        public double Rideheight { get; set; }
        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public string Compound { get; set; }
        public double BrakeThickness { get; set; }

        public rFactorSetupWheel(double rideheight, double pressure, double temperature, string compound, double brakeThickness)
        {
            Rideheight = rideheight;
            Pressure = pressure;
            Temperature = temperature;
            Compound = compound;
            BrakeThickness = brakeThickness;
        }
        public rFactorSetupWheel()
        {
            Rideheight = 0.075;
            Pressure = 165;
            Temperature = 40;
            Compound = "Rubber";
            BrakeThickness = 3.5/100;
        }
    }

    public class rFactorSetup : ISetup
    {
        public int Aero_FrontWing
        {
            get { return rFactor.Player.Aerodynamics_FrontWing_Setting; }
        }

        public int Aero_RearWing
        {
            get { return rFactor.Player.Aerodynamics_RearWing_Setting; }
        }

        public int Brakes_DuctSize
        {
            get { return rFactor.Player.Aerodynamics_BrakeDuct_Setting; }
        }

        public int Engine_RadiatorSize
        {
            get { return rFactor.Player.Aerodynamics_Radiator_Setting; }
        }

        public int Aero_FenderLeft
        {
            get { return 0; }
        }

        public int Aero_FenderRight
        {
            get { return 0; }
        }

        public ISetupWheel Wheel_LeftFront
        {
            get { return new rFactorSetupWheel(); }
        }

        public ISetupWheel Wheel_RightFront
        {
            get { return new rFactorSetupWheel(); }
        }

        public ISetupWheel Wheel_LeftRear
        {
            get { return new rFactorSetupWheel(); }
        }

        public ISetupWheel Wheel_RightRear
        {
            get { return new rFactorSetupWheel(); }
        }

        public double Suspension_RideHeight_LF
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Suspension_RideHeight_LR
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Suspension_RideHeight_RF
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Suspension_RideHeight_RR
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Tyre_Pressure_LF
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Tyre_Pressure_LR
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Tyre_Pressure_RF
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double Tyre_Pressure_RR
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}