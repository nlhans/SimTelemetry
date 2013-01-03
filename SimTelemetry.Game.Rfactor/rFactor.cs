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
using SimTelemetry.Game.Rfactor.MMF;
using SimTelemetry.Objects.Plugins;
using SimTelemetry.Objects.Utilities;
using SimTelemetry.Objects;
using SimTelemetry.Game.Rfactor.Garage;

namespace SimTelemetry.Game.Rfactor
{
    public class rFactor
    {

        public static ISimulator Simulator { get; set; }
        public const double Revision = 0.1;

        public static rFactorMMF MMF;
        public static Session Session;
        public static Drivers Drivers;
        public static DriverPlayer Player;
        public static MemoryPolledReader Game;
        public static rFactorGarage Garage;
        
        public rFactor(ISimulator sim)
        {
            Simulator = sim;
            MMF = new rFactorMMF();
            
            if (Simulator.UseMemoryReader)
            {
                Game = new MemoryPolledReader(sim);
            }
            else
            {
                Game = null;
            }

            Garage = new rFactorGarage();
            Session = new Session();
            Drivers = new Drivers();

            Player = new DriverPlayer();
        }

        public static void Kill()
        {
            if (Simulator.UseMemoryReader)
            {
                Game.Active = false;
            }

        }
    }
}