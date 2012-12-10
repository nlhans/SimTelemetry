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
using System;
using System.Collections.Generic;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Network
{
    public class NetworkGarage : IGarage
    {
        public string InstallationDirectory { get; private set; }
        public string GamedataDirectory { get; private set; }
        public string Simulator { get; private set; }
        public bool Available { get { return false; } set { } }
        public bool Available_Tracks { get { return false; } set { } }
        public bool Available_Mods { get { return false; } set { } }
        public List<IMod> Mods { get; private set; }
        public List<ITrack> Tracks { get; private set; }


        public void Scan()
        {
            return;
        }

        public ICar CarFactory(IMod mod, string veh)
        {
            return null;
        }

        public ICar SearchCar(string CarClass, string CarModel)
        {
            return null;
        }

        public ITrack SearchTrack(string path)
        {
            return null;
        }
    }
}