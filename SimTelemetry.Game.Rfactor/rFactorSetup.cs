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
    }
}