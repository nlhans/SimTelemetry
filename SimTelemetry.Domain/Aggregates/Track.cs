using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class Track : IEntity<string>, IEquatable<Track>
    {
        public string ID { get; private set; }

        public string Name { get; private set; }
        public string Image { get; private set; }
        public string Location { get; private set; }
        public double Length { get; private set; }

        public float LaprecordRace { get; private set; }
        public float LaprecordQualify { get; private set; }

        public float TrackCoordinateMinX { get; private set; }
        public float TrackCoordinateMaxX { get; private set; }
        public float TrackCoordinateMinY { get; private set; }
        public float TrackCoordinateMaxY { get; private set; }

        public IEnumerable<TrackPoint> Route { get; private set; }
        public IEnumerable<TrackPoint> Pits { get; private set; }
        public IEnumerable<TrackPoint> Grid { get; private set; }

        private IList<Session> _sessions { get; set; }
        public IEnumerable<Session> Sessions { get { return _sessions; } }

        public string Version { get; private set; }
        public bool Equals(Track other) { return other.ID == ID; }
        public bool Equals(string other) { return other == ID; }

        /********* INITIAL DATA *********/
        public Track()
        {
            TrackCoordinateMinX = float.MaxValue;
            TrackCoordinateMaxX = float.MinValue;
            TrackCoordinateMinY = float.MaxValue;
            TrackCoordinateMaxY = float.MinValue;
        }

        public Track(string file, string name, string image, string location, float laprecordRace, float laprecordQualify, string version) : this()
        {
            ID = file;
            Name = name;
            Image = image;
            Location = location;
            LaprecordRace = laprecordRace;
            LaprecordQualify = laprecordQualify;
            Version = version;
        }

        /********* TRACK ROUTE *********/
        private void UpdateTrackCoordinates(IEnumerable<TrackPoint> points)
        {
            if (points.Count() > 0)
            {
                TrackCoordinateMinX = Math.Min(points.Min(x => x.X), TrackCoordinateMinX);
                TrackCoordinateMaxX = Math.Max(points.Max(x => x.X), TrackCoordinateMaxX);

                TrackCoordinateMinY = Math.Min(points.Min(x => x.Y), TrackCoordinateMinY);
                TrackCoordinateMaxY = Math.Max(points.Max(x => x.Y), TrackCoordinateMaxY);
            }
        }

        protected void SetRoute(IList<TrackPoint> route)
        {
            Route = route.Where(x => x.Type == TrackPointType.SECTOR1
                                     || x.Type == TrackPointType.SECTOR2
                                     || x.Type == TrackPointType.SECTOR3)
                .OrderBy(x => x.Meter);

            if (Route.Count() > 0)
                Length = Route.Max(x => x.Meter) - Route.Min(x => x.Meter);

            UpdateTrackCoordinates(Route);

        }

        protected void SetPits(IList<TrackPoint> route)
        {
            Pits = route.Where(x => x.Type == TrackPointType.PITS)
                .OrderBy(x => x.Meter);

            UpdateTrackCoordinates(Pits);

        }

        protected void SetGrid(IList<TrackPoint> route)
        {
            Grid = route.Where(x => x.Type == TrackPointType.GRID)
                .OrderBy(x => x.Meter);

            // Grid is always located 'on' the route array.

        }

        public void SetTrack(IList<TrackPoint> points)
        {
            SetRoute(points);
            SetPits(points);
            SetGrid(points);
        }

        /********* SESSIONS *********/
        public void AddSession(Session s)
        {
            if (_sessions.Any(x => x.Type == s.Type && x.Name == s.Name) == false)
            {
                _sessions.Add(s);
            }
        }
        public void RemoveSession(Session s)
        {
            if (_sessions.Contains(s))
                _sessions.Remove(s);
        }
    }
}
