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

namespace SimTelemetry.Objects
{
    public struct Lap
    {
        // Game Info
        public double Sector1;
        public double Sector2;
        public double Sector3;
        public double Total;
        public double MinTime;
        public double MaxTime;
        public double Distance;

        public int LapNo;
        public int DriverNo;

        // Our info :D
        public List<double> ApexSpeeds; // speeds at each apex
        public List<double> Checkpoints; // times at each checkpoint

        public double PrevMeters;

    }
}