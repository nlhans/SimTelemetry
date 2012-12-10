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
using SimTelemetry.Game.rFactor2.Garage;
using SimTelemetry.Game.rFactor2.v1005b;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.rFactor2
{
    public class rFactor2
    {
        public const double Revision = 0.1;
        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryPolledReader Game;
        private static Simulator Sim;
        public static rFactor2Garage Garage;


        // TODO: This class should do version detect first before initializing session, drivers and driverplayer classes.
        
        public rFactor2(Simulator simulator)
        {
            Sim = simulator;
            Game = new MemoryPolledReader(simulator);

            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
            Garage = new rFactor2Garage();
        }

        public static void Kill()
        {

            Game.Active = false;
        }
    }
}
