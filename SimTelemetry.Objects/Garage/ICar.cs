/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System.Collections.Generic;

namespace SimTelemetry.Objects.Garage
{
    public interface ICar
    {
        string File { get; }
        string Team { get; }
        string Driver { get; }
        string Description { get; }
        int Number { get; }
        List<string> Classes { get; }

        Dictionary<string, string> Files { get; }

        string Info_Engine_Manufacturer { get; }

        int Info_YearFounded { get; }
        string Info_HQ { get; }
        int Info_Starts { get; }
        int Info_Poles { get; }
        int Info_Wins { get; }
        int Info_Championships { get; }

        ICarGeneral General { get; }
        ICarEngine Engine { get; }
        ICarGearbox Gearbox { get; }
        ICarAerodynamics Aerodynamics { get; }
        ICarWheels Wheels { get;}
        string PhysicsFile { get; }

        void Scan();
        bool InClass(List<string> classes);
        void ScanEngine();
        void ScanGeneral();
        void ScanAerodynamics();
    }
}