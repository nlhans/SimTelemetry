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
            GlobalEvents.Hook<SessionStopped>(ResetLapPool, true);
            GlobalEvents.Hook<TelemetryLapDriven>(UpdateLapStats, true);
            GlobalEvents.Hook<DriversRemoved>(CheckLapStats, true);

            ResetLapPool(null);
        }

        private void CheckLapStats(DriversRemoved obj)
        {
            // TODO: Check if stats needs to be downgraded.
        }

        private void ResetLapPool(SessionStopped e)
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

        private void UpdateLapStats(TelemetryLapDriven e)
        {
            if (e.Lap.Total > 0 && e.Lap.Sector1 > 0 && e.Lap.Sector2 >0&& e.Lap.Sector3 > 0)
            {
                if (BestLap.LapNumber < 0 || BestLap.Driver < 0)
                    BestLap = e.Lap;
                else if (BestLap.Total > e.Lap.Total)
                    BestLap = e.Lap;

                if (e.Driver.BestLap.Total < 0)
                    e.Driver.BestLap = e.Lap;
                else
                    if (e.Driver.BestLap.Total > e.Lap.Total)
                        e.Driver.BestLap = e.Lap;
            }

            if (BestLap.Sector1 > 0)
            {
                BestS1 = BestS1 < 0 ? BestLap.Sector1 : Math.Min(BestS1, BestLap.Sector1);
                if (Math.Abs(BestS1 - BestLap.Sector1) < 0.0001) BestS1Lap = e.Lap;
            }
            if (BestLap.Sector2 > 0){
                BestS2 = BestS2 < 0 ? BestLap.Sector2 : Math.Min(BestS2, BestLap.Sector2);
                if (Math.Abs(BestS2 - BestLap.Sector2) < 0.0001) BestS2Lap = e.Lap;
            }
            if (BestLap.Sector3 > 0)
            {
                BestS3 = BestS3 < 0 ? BestLap.Sector3 : Math.Min(BestS3, BestLap.Sector3);
                if (Math.Abs(BestS3 - BestLap.Sector3) < 0.0001) BestS3Lap = e.Lap;
            }

            e.Driver.LastLap = e.Lap;


        }
        
        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
            foreach(var drv in telemetry.Drivers)
            {
                int drvIndex = drv.BaseAddress;

                if (lapsKeeper.ContainsKey(drvIndex) ==false)
                    lapsKeeper.Add(drvIndex, 0);

                if (lapsKeeper[drvIndex] != drv.Laps)
                {
                    var missingLaps = drv.GetLaps().Where(x=>x.Completed).Where(x => !_laps.Contains(x));
                    if (missingLaps.Any())
                    {
                        foreach(var l in missingLaps)
                        {
                            _laps.Add(l);
                            GlobalEvents.Fire(new TelemetryLapDriven(drv, l), true);
                        }
                    }
                }
            }
        }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {
            // Update best sectors, for this we need to "dig deeper" into all laps driven, even that are not done yet.
            var allLaps = telemetry.Drivers.SelectMany(x => x.GetLaps());
            BestS1 = allLaps.Any(x => x.Sector1 > 0) ? allLaps.Where(x => x.Sector1 > 0).Min(x => x.Sector1) : -1;
            BestS2 = allLaps.Any(x => x.Sector2 > 0) ? allLaps.Where(x => x.Sector2 > 0).Min(x => x.Sector2) : -1;
            BestS3 = allLaps.Any(x => x.Sector3 > 0) ? allLaps.Where(x => x.Sector3 > 0).Min(x => x.Sector3) : -1;

            BestLap = allLaps.Any(x => x.Completed && x.Total > 0) ? allLaps.Where(x => x.Completed && x.Total > 0).OrderBy(x => x.Total).FirstOrDefault() : dummyLap;
        }
    }

}