using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Core.Common;
using SimTelemetry.Core.Enumerations;
using SimTelemetry.Core.ValueObjects;

namespace SimTelemetry.Core.Aggregates
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

        public IEnumerable<TrackPoint> Route { get; private set; }
        public IEnumerable<TrackPoint> Pits { get; private set; }
        public IEnumerable<TrackPoint> Grid { get; private set; }

        private IList<Session> _sessions { get; set; }
        public IEnumerable<Session> Sessions { get { return _sessions; } }

        public bool Equals(Track other) { return other.ID == ID; }

        /********* INITIAL DATA *********/
        public Track(int id, string file, string name, string location, double length, float laprecordRace, float laprecordQualify)
        {
            ID = id;
            File = file;
            Name = name;
            Location = location;
            Length = length;
            LaprecordRace = laprecordRace;
            LaprecordQualify = laprecordQualify;
        }

        /********* TRACK ROUTE *********/
        public void SetRoute(IList<TrackPoint> route)
        {
            Route = route.OrderBy(x => x.Meter).Where(x => x.Type == TrackPointType.SECTOR1
                                                           || x.Type == TrackPointType.SECTOR2
                                                           || x.Type == TrackPointType.SECTOR3);

        }

        public void SetPits(IList<TrackPoint> route)
        {
            Pits = route.OrderBy(x => x.Meter).Where(x => x.Type == TrackPointType.PITS);

        }

        public void SetGrid(IList<TrackPoint> route)
        {
            Grid = route.OrderBy(x => x.Meter).Where(x => x.Type == TrackPointType.GRID);

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
