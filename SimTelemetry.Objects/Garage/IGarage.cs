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
    public interface IGarage
    {
        string InstallationDirectory { get; }
        string GamedataDirectory { get; }

        string Simulator { get; }
        bool Available { get; }

        bool Available_Tracks { get; }
        bool Available_Mods { get; }

        List<IMod> Mods { get; }
        List<ITrack> Tracks { get; }

        /// <summary>
        /// Fill-up data for Mods and tracks. Do-not yet fill them out completely as this will take too much time.
        /// On request data should be read and stored(cached).
        /// </summary>
        void Scan();

        /// <summary>
        /// Factory method to cache/create Car objects.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="veh"></param>
        /// <returns></returns>
        ICar CarFactory(IMod mod, string veh);

        /// <summary>
        /// Searches for a car based on class and model
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        ICar SearchCar(string CarClass, string CarModel);

        /// <summary>
        /// Searches for a track based on path
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ITrack SearchTrack(string  path);
    }
}