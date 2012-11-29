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
using SimTelemetry.Objects.Utilities;
using Triton.Memory;

namespace SimTelemetry.Game.FalconBMS
{
    public class FalconBms
    {
        
        public static MemoryPolledReader Game
        {
            get
            {
                return Simulator.Game;
            }
        }

        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;

        public FalconBms()
        {
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }
    }
}