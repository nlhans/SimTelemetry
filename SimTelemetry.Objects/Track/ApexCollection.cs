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

namespace SimTelemetry.Objects
{
    public class ApexCollection
    {
        public Dictionary<double, string> Positions = new Dictionary<double, string>();
        public ApexCollection()
        {
            // Test corners
            // Jackson ville
            /*
            Positions.Add(750, "Entry C1");
            Positions.Add(1320, "Exit C1");

            Positions.Add(2658, "Entry C2");
            Positions.Add(3270, "Exit C2");

            Positions.Add(4010, "Apex C3");
            */
            Positions.Add(240, "eau rouge start");
            Positions.Add(326, "eau rouge middle");
            Positions.Add(516, "eau rouge top of hill");
            Positions.Add(1346, "Speed Trap 1");
            Positions.Add(1636.6, "Corner 2-3");
            Positions.Add(1727.83, "Corner 3-4");
            Positions.Add(2050, "Straight 2");
            Positions.Add(2486, "Straight 3");
            Positions.Add(2847, "Straight 4");
            Positions.Add(3097, "Corner 7-8");
            Positions.Add(3508, "End straight 4");
            Positions.Add(3763, "Mid corner 9-10");
            Positions.Add(4134, "Mid corner 11");
            Positions.Add(4348, "Long Straight");
            Positions.Add(5247, "Speed trap 2");
            Positions.Add(5645, "Speed trap 3");
            Positions.Add(5864, "Low speed");
            Positions.Add(6012, "Fast Chicane");
            Positions.Add(6363, "Speed trap 4");
            Positions.Clear();
            Dictionary<double, string> boe = new Dictionary<double, string>();
            Positions.Add(1600.0 + 200, "Mile 1");
            Positions.Add(2 * 1600 + 200.0, "Mile 2");
            Positions.Add(3 * 1600 + 200.0, "Mile 3");
            Positions.Add(4 * 1600 + 200.0, "Mile 4");
            Positions.Add(5 * 1600 + 200.0, "Mile 5");
            Positions.Add(6 * 1600 + 200.0, "Mile 6");
            //Positions = new Dictionary<double, string>(boe);
        }
    }
}
