using System;
using SimTelemetry.Objects.Game;

namespace SimTelemetry.Game.Rfactor
{
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
            get { throw new NotImplementedException(); }
        }

        public ISetupWheel Wheel_RightFront
        {
            get { throw new NotImplementedException(); }
        }

        public ISetupWheel Wheel_LeftRear
        {
            get { throw new NotImplementedException(); }
        }

        public ISetupWheel Wheel_RightRear
        {
            get { throw new NotImplementedException(); }
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