using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Core.Aggregates;
using SimTelemetry.Core.Enumerations;
using SimTelemetry.Core.Events;
using SimTelemetry.Core.Exceptions;
using SimTelemetry.Core.ValueObjects;

namespace SimTelemetry.Core.Entities
{
    public class LiveScoringDriver : IEquatable<LiveScoringDriver>
    {
        private readonly IList<Lap> _lapTimes = new List<Lap>();
        private IList<double> _driverSplits = new List<double>();
        private IList<double> _trackSpeeds = new List<double>();
        private IList<double> _trackTimes = new List<double>();

        public TelemetryDriver Driver { get; private set; }

        public RecordedLap ReferenceLapSplit { get; private set; }

        public double BestSector1 { get; private set; }
        public double BestSector2 { get; private set; }
        public double BestSector3 { get; private set; }
        public double BestLapTime { get; private set; }

        public double LastSector1 { get; private set; }
        public double LastSector2 { get; private set; }
        public double LastSector3 { get; private set; }
        public double LastLapTime { get; private set; }

        public IEnumerable<Lap> Laptimes { get { return _lapTimes; } }
        public IEnumerable<double> DriverSplits { get { return _driverSplits; } }

        public IEnumerable<double> TrackSpeeds { get { return _trackSpeeds; } }
        public IEnumerable<double> TrackTimes { get { return _trackTimes; } }

        public LiveScoringDriver(TelemetryDriver driver)
        {
            Driver = driver;
        }

        // Updating last times
        public void SetLastSector(TrackPointType type, double time)
        {
            if (type == TrackPointType.SECTOR1)
                LastSector1 = time;
            if (type == TrackPointType.SECTOR2)
                LastSector2 = time;
            if (type == TrackPointType.SECTOR3)
                LastSector3 = time;
        }

        //
        public void AssignReferenceLap(RecordedLap lap)
        {
            ReferenceLapSplit = lap;

            GlobalEvents.Fire(new ReferenceLapChanged(this), true);
        }

        public void AddLap(Lap lap)
        {
            if (Laptimes.Any(lap.Equals))
                throw new LapWasAlreadyAddedException();

            _lapTimes.Add(lap);
            _lapTimes.OrderBy(x => lap.LapNumber);

            // TODO: How fast does this run?
            LastLapTime = _lapTimes.Where(x => x.LapNumber == _lapTimes.Max(y => y.LapNumber)).FirstOrDefault().Total;

            // Update best times
            BestLapTime = _lapTimes.Min(x => x.Total);
            BestSector1 = _lapTimes.Min(x => x.Sector1);
            BestSector2 = _lapTimes.Min(x => x.Sector2);
            BestSector3 = _lapTimes.Min(x => x.Sector3);

            GlobalEvents.Fire(new LapAdded(this, lap), true);
        }

        public void SetDriverSplits(IEnumerable<double> splits)
        {
            _driverSplits = new List<double>(splits);
        }
        public void SetTrackSpeeds(IEnumerable<double> speeds)
        {
            _trackSpeeds = new List<double>(speeds);
        }
        public void SetTrackTimes(IEnumerable<double> speeds)
        {
            _trackTimes = new List<double>(speeds);
        }


        public bool Equals(LiveScoringDriver other)
        {
            return Driver.Equals(other.Driver);
        }
    }
}