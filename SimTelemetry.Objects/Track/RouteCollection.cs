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
    public class RouteCollection : ICloneable
    {
        public List<TrackWaypoint> Racetrack { get; private set; }
        public List<TrackWaypoint> Pitlane { get; private set; }

        public RouteCollection()
        {
            
        }

        public double Length = 0;

        public double x_min = Double.MaxValue, x_max = double.MinValue, y_min = double.MaxValue, y_max = double.MinValue;

        public void Add(TrackWaypoint wp)
        {
            x_min = Math.Min(wp.X, x_min);
            x_max = Math.Max(wp.X, x_max);

            y_min = Math.Min(wp.Z, y_min);
            y_max = Math.Max(wp.Z, y_max);

            if (wp.Route == TrackRoute.MAIN)
            {
                if (Racetrack == null) Racetrack = new List<TrackWaypoint>();
                Racetrack.Add(wp);

            }
            if (wp.Route == TrackRoute.PITLANE)
            {
                if (Pitlane == null) Pitlane = new List<TrackWaypoint>();
                Pitlane.Add(wp);

            }

        }

        public void Finalize()
        {
            if (Racetrack != null)
            {
                Racetrack.Sort(delegate(TrackWaypoint wp1, TrackWaypoint wp2)
                                   {
                                       if (wp1.Meters > wp2.Meters) return 1;
                                       if (wp2.Meters > wp1.Meters) return -1;

                                       return 0; // equal?

                                   });
                Length = Racetrack[Racetrack.Count - 1].Meters;
            }
            if (Pitlane != null)
            {
                Pitlane.Sort(delegate(TrackWaypoint wp1, TrackWaypoint wp2)
                                 {
                                     if (wp1.Meters > wp2.Meters) return 1;
                                     if (wp2.Meters > wp1.Meters) return -1;
                                     return 0; // equal?

                                 });
            }

            // TODO: Check ascending order
        }

        public object Clone()
        {

            RouteCollection c = new RouteCollection();
            c.y_min = y_min;
            c.y_max = y_max;
            c.x_min = x_min;
            c.x_max = x_max;

            c.Racetrack = new List<TrackWaypoint>(Racetrack);
            c.Pitlane = new List<TrackWaypoint>(Pitlane);
            c.Length = Length;

            return c;


        }
    }
}