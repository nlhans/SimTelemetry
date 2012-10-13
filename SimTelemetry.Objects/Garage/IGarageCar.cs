using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface IGarageCar
    {
        string File { get; }
        string Team { get; }
        string Name { get; }
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

        IGarageCarEngine Engine { get; }
        IGarageCarGearbox Gearbox { get; }
        IGarageCarAerodynamics Aerodynamics { get; }
        IGarageCarWheels Wheels { get;}
        IGarageCarBrakes Brakes { get; }
        IGarageCarGeneral General { get; }
    }
}