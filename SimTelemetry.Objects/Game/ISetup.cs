using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Game
{
    public interface ISetup
    {
        int Aero_FrontWing { get; }
        int Aero_RearWing { get; }
        int Brakes_DuctSize { get; }
        int Engine_RadiatorSize { get; }

        int Aero_FenderLeft { get; }
        int Aero_FenderRight { get; }
    }
}
