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

namespace SimTelemetry.Objects
{
    [Serializable]
    public struct TrackWaypoint : ICloneable
    {
        public double X;
        public double Y;
        public double Z;

        public TrackRoute Route;
        public int Sector;
        public double Meters;
        public double[] CoordinateL;
        public double[] CoordinateR;
        public double[] PerpVector;

        public object Clone()
        {
            TrackWaypoint wp = new TrackWaypoint();

            wp.X = X;
            wp.Y = Y;
            wp.Z = Z;

            wp.Route = Route;
            wp.Sector = Sector;
            wp.Meters = Meters;
            wp.CoordinateL = CoordinateL;
            wp.CoordinateR = CoordinateR;
            wp.PerpVector = PerpVector;

            return wp;
        }
    }
}