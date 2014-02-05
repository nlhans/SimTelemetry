using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryLapsPool : ITelemetryObject
    {
        private Lap dummyLap;

        public IEnumerable<Lap> Laps { get { return _laps; } }
        private readonly List<Lap> _laps = new List<Lap>();
        private Dictionary<int, int> lapsKeeper = new Dictionary<int, int>();

        public Lap BestLap { get; private set; }

        public double BestS1 { get; private set; }
        public Lap BestS1Lap { get; private set; }
        public double BestS2 { get; private set; }
        public Lap BestS2Lap { get; private set; }
        public double BestS3 { get; private set; }
        public Lap BestS3Lap { get; private set; }

        public TelemetryLapsPool()
        {
            GlobalEvents.Hook<SessionStarted>(ResetLapPool, true);

            ResetLapPool(null);
        }

        private void ResetLapPool(SessionStarted e)
        {
            _laps.Clear();
            lapsKeeper.Clear();
            dummyLap = new Lap(-1, false, -1, -1, -1, -1, -1, false, false);

            BestLap = dummyLap;

            BestS1Lap = dummyLap;
            BestS2Lap = dummyLap;
            BestS3Lap = dummyLap;

            BestS1 = -1;
            BestS2 = -1;
            BestS3 = -1;
        }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {
            // Update best sectors, for this we need to "dig deeper" into all laps driven, even that are not done yet.
            var allLaps = telemetry.Drivers.SelectMany(x => x.GetLaps());
            BestS1 = allLaps.Any(x => x.Sector1 > 0) ? allLaps.Where(x => x.Sector1 > 0).Min(x => x.Sector1) : -1;
            BestS2 = allLaps.Any(x => x.Sector2 > 0) ? allLaps.Where(x => x.Sector2 > 0).Min(x => x.Sector2) : -1;
            BestS3 = allLaps.Any(x => x.Sector3 > 0) ? allLaps.Where(x => x.Sector3 > 0).Min(x => x.Sector3) : -1;
            if (BestS1 > 0)
                BestS1Lap = allLaps.FirstOrDefault(x => x.Sector1 == BestS1);
            if (BestS2 > 0)
                BestS2Lap = allLaps.FirstOrDefault(x => x.Sector2 == BestS2);
            if (BestS3 > 0)
                BestS3Lap = allLaps.FirstOrDefault(x => x.Sector3 == BestS3);

            BestLap = allLaps.Any(x => x.Completed && x.Total > 0) ? allLaps.Where(x => x.Completed && x.Total > 0).OrderBy(x => x.Total).FirstOrDefault() : dummyLap;
        }
    }

}