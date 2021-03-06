﻿/*************************************************************************
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
    public interface IMod
    {
        string File { get; }
        
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Website { get; }
        string Version { get; }
        List<string> Classes { get; }

        string Directory_Vehicles { get; }

        /// <summary>
        /// Pitspeed in m/s
        /// </summary>
        int PitSpeed_Practice_Default { get; }

        /// <summary>
        /// Pitspeed in m/s
        /// </summary>
        int PitSpeed_Race_Default { get; }

        /// <summary>
        /// Number of unique opponents in the mod.
        /// </summary>
        int Opponents { get; }

        /// <summary>
        /// Championship data for this mod, including no. of opponents, title and tracks.
        /// </summary>
        List<IModChampionship> Championships { get; }

        List<ICar> Models { get; }
        string Image { get; }

        void Scan();
        void AddClasses(List<string> classes);
    }
}