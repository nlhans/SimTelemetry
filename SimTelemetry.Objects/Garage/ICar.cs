using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface ICar
    {
        string File { get; }
        string Team { get; }
        string Driver { get; }
        int Number { get; }

        Dictionary<string, string> Files { get; }

        string Info_Engine_Manufacturer { get; }

        int Info_YearFounded { get; }
        string Info_HQ { get; }
        int Info_Starts { get; }
        int Info_Poles { get; }
        int Info_Wins { get; }
        int Info_Championships { get; }

        ICarEngine Engine { get; }
        ICarGearbox Gearbox { get; }
        ICarAerodynamics Aerodynamics { get; }
        ICarWheels Wheels { get;}
        ICarBrakes Brakes { get; }
        ICarGeneral General { get; }

        void Scan();
    }
}