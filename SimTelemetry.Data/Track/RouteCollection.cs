using System;
using System.Collections.Generic;

namespace SimTelemetry.Data.Track
{
    public class RouteCollection
    {
        public List<TrackWaypoint> Racetrack { get; private set; }
        public List<TrackWaypoint> Pitlane { get; private set; }


        public double Length = 0;

        public double x_min = Double.MaxValue, x_max = double.MinValue, y_min = double.MaxValue, y_max = double.MinValue;

        internal void Add(TrackWaypoint wp)
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

        internal void Finalize()
        {
            Racetrack.Sort(delegate(TrackWaypoint wp1, TrackWaypoint wp2)
            {
                if (wp1.Meters > wp2.Meters) return 1;
                if (wp2.Meters > wp1.Meters) return -1;
                
                return 0; // equal?

            });
            Length = Racetrack[Racetrack.Count - 1].Meters;
            Pitlane.Sort(delegate(TrackWaypoint wp1, TrackWaypoint wp2)
            {
                if (wp1.Meters > wp2.Meters) return 1;
                if (wp2.Meters > wp1.Meters) return -1;
                return 0; // equal?

            });

            // TODO: Check ascending order
        }
    }
}
