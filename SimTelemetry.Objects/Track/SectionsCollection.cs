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
    public class SectionsCollection
    {
        public Dictionary<double, string> Lines = new Dictionary<double, string>();

        public SectionsCollection()
        {
            // Jackonsville
            /*
            Lines.Add(0, "Straight 1");
            Lines.Add(622,"Corner 1");
            Lines.Add(1556, "Straight 2");
            Lines.Add(2464, "Corner 2");
            Lines.Add(3418, "Straight 3");
            Lines.Add(3817, "Corner 3");
            */
            /*Lines.Add(0, "Start");
            Lines.Add(240, "Eau Rouge");
            Lines.Add(526, "Straight 1");
            Lines.Add(1545, "Corner 2");
            Lines.Add(1636.6, "Corner 3");
            Lines.Add(1727.83, "Corner 4");
            Lines.Add(1942, "Straight 2");
            Lines.Add(2120, "Corner 5");
            Lines.Add(2390, "Corner 6");
            Lines.Add(2556, "Straight 3");
            Lines.Add(2903, "Corner 7&8");
            Lines.Add(3374, "Straight 4");
            Lines.Add(3589, "Corner 9&10");
            Lines.Add(3941, "Straight 5");
            Lines.Add(4053, "Corner 11");
            Lines.Add(4207, "Straight 6");
            Lines.Add(4283, "Corner 12");
            Lines.Add(4435, "Straight 7");
            Lines.Add(5247, "Corner 13");
            Lines.Add(5450, "Straight 8");
            Lines.Add(5804, "Chicane");
            Lines.Add(5920, "Pitstraight");
            Lines.Add(6470, "Corner 14");
            Lines.Add(6600, "Straight 8");*/

            Dictionary<double, string> boe = new Dictionary<double, string>();
            Lines.Add(1600, "Mile 1");
            Lines.Add(2 * 1600.0, "Mile 2");
            Lines.Add(3 * 1600.0, "Mile 3");
            Lines.Add(4 * 1600.0, "Mile 4");
            Lines.Add(5 * 1600.0, "Mile 5");
            Lines.Add(6 * 1600.0, "Mile 6");
        }
    }
}
