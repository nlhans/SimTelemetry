using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Aggregates
{
    public class Track : IEntity, IEquatable<Track>
    {
        public int ID { get; private set; }
        public string File { get; private set; }

        public string Name { get; private set; }
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

        public bool Equals(Track other) { return other.ID == ID; }

        /********* INITIAL DATA *********/
        public Track()
        {
            TrackCoordinateMinX = float.MaxValue;
            TrackCoordinateMaxX = float.MinValue;
            TrackCoordinateMinY = float.MaxValue;
            TrackCoordinateMaxY = float.MinValue;
        }

        public Track(int id, string file, string name, string location, float laprecordRace, float laprecordQualify) : this()
        {
            ID = id;
            File = file;
            Name = name;
            Location = location;
            LaprecordRace = laprecordRace;
            LaprecordQualify = laprecordQualify;
        }

        /********* TRACK ROUTE *********/
        private void UpdateTrackCoordinates(IEnumerable<TrackPoint> points)
        {
            TrackCoordinateMinX = Math.Max(points.Min(x => x.X), TrackCoordinateMinX);
            TrackCoordinateMaxX = Math.Max(points.Max(x => x.X), TrackCoordinateMaxX);

            TrackCoordinateMinY = Math.Max(points.Min(x => x.Y), TrackCoordinateMinY);
            TrackCoordinateMaxY = Math.Max(points.Max(x => x.Y), TrackCoordinateMaxY);
        }

        public void SetRoute(IList<TrackPoint> route)
        {
            Route = route.Where(x => x.Type == TrackPointType.SECTOR1
                                     || x.Type == TrackPointType.SECTOR2
                                     || x.Type == TrackPointType.SECTOR3)
                .OrderBy(x => x.Meter);

            Length = route.Max(x => x.Meter) - route.Min(x => x.Meter);

            UpdateTrackCoordinates(Route);

        }

        public void SetPits(IList<TrackPoint> route)
        {
            Pits = route.Where(x => x.Type == TrackPointType.PITS)
                .OrderBy(x => x.Meter);

            UpdateTrackCoordinates(Pits);

        }

        public void SetGrid(IList<TrackPoint> route)
        {
            Grid = route.Where(x => x.Type == TrackPointType.GRID)
                .OrderBy(x => x.Meter);

            // Grid is always located 'on' the route array.

        }

        public void SetTrackPoints(IList<TrackPoint> points)
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
