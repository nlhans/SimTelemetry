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
using System.Linq;
using System.Text;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net.Objects
{
    [Serializable]
    public class NetworkDrivers : IDriverCollection
    {
        public List<IDriverGeneral> AllDrivers { get; set; }
        public IDriverGeneral Player
        {
            get
            {
                if (AllDrivers == null)
                    return new NetworkDriverGeneral();
                return AllDrivers.Where(x => x.IsPlayer).FirstOrDefault();
            }

        }

        public static NetworkDrivers Create(IDriverCollection Drivers)
        {
            NetworkDrivers nwDrivers = new NetworkDrivers();
            nwDrivers.AllDrivers = new List<IDriverGeneral>();
            Drivers.AllDrivers.ForEach(x => nwDrivers.AllDrivers.Add(NetworkDriverGeneral.Create(x)));

            return nwDrivers;
        }
    }
}
