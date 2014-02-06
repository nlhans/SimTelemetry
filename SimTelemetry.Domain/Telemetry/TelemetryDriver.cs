using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Memory;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryDriver : ITelemetryObject
    {
        private Lap dummyLap;

        private bool _initial = true;

        internal IDataNode Pool { get; set; }

        public bool IsPlayer { get; protected set; }
        public string Name { get; protected set; }

        public string CarFile { get; protected set; }

        public string CarTeam { get; protected set; }
        public string CarModel { get; protected set; }
        public List<string> CarClasses { get; protected set; }
        public int CarNumber { get; private set; }

        public int Pitstops { get; protected set; }
        public int Position { get; protected set; }
        public int Laps { get; protected set; }

        public bool IsRetired { get; protected set; }
        public bool IsLimiter { get; protected set; }
        public bool IsPits { get; protected set; }
        public bool IsDriving { get; protected set; }

        public bool FlagYellow { get; protected set; }
        public bool FlagBlue { get; protected set; }
        public bool FlagBlack { get; protected set; }
        public bool Ignition { get; protected set; }

        public float Mass { get; protected set; }
        public float Fuel { get; protected set; }
        public float FuelCapacity { get; protected set; }

        public float Meter { get; protected set; }
        public float EngineRpm { get; protected set; }
        public float EngineRpmMax { get; protected set; }
        public float Speed { get; protected set; }
        public int Gear { get; protected set; }

        public float CoordinateX { get; protected set; }
        public float CoordinateY { get; protected set; }
        public float CoordinateZ { get; protected set; }

        public double Heading { get; private set; }

        public float InputThrottle { get; protected set; }
        public float InputBrake { get; protected set; }

        public int BaseAddress { get; private set; }

        public double EngineLifetime { get; private set; }

        public double OilTemperature { get; private set; }
        public double WaterTemperature { get; private set; }

        public double EngineTorque { get; private set; }

        public TrackPointType TrackPosition { get; private set; }

        public TelemetryWheel WheelRR { get; private set; }
        public TelemetryWheel WheelLR { get; private set; }
        public TelemetryWheel WheelRF { get; private set; }
        public TelemetryWheel WheelLF { get; private set; }

        public double BestS1 { get; internal set; }
        public double BestS2 { get; internal set; }
        public double BestS3 { get; internal set; }

        public Lap BestLap { get; internal set; }
        public Lap LastLap { get; internal set; }
        public Lap CurrentLap { get; internal set; }

        protected DateTime LastLapsUpdate { get; set; }
        protected List<Lap> LapsList { get; set; }

        public void UpdateSlow(ITelemetry telemetry, IDataProvider Memory)
        {
            BestS1 = LapsList.Any(x => x.Sector1 > 0) ? LapsList.Where(x => x.Sector1 > 0).Min(x => x.Sector1) : -1;
            BestS2 = LapsList.Any(x => x.Sector2 > 0) ? LapsList.Where(x => x.Sector2 > 0).Min(x => x.Sector2) : -1;
            BestS3 = LapsList.Any(x => x.Sector3 > 0) ? LapsList.Where(x => x.Sector3 > 0).Min(x => x.Sector3) : -1;

            BestLap = LapsList.Any(x => x.Total > 0) ? LapsList.Where(x => x.Total > 0).OrderBy(x => x.Total).FirstOrDefault() : dummyLap;
            LastLap = LapsList.Any(x => x.Completed) ? LapsList.LastOrDefault(x => x.Completed) : dummyLap;
            CurrentLap = LapsList.Any() ? LapsList.LastOrDefault() : dummyLap;
        }

        public void Update(ITelemetry telemetry, IDataProvider Memory)
        {
            if(_initial)
            {
                _initial = false;

                IsPlayer = Pool.ReadAs<bool>("IsPlayer");
                Name = Pool.ReadAs<string>("Name");
                CarTeam = Pool.ReadAs<string>("CarTeam");

                FuelCapacity = Pool.ReadAs<float>("FuelCapacity");

                // TODO: check
                EngineLifetime = Pool.ReadAs<float>("EngineLifetime");
            }

            Meter = Pool.ReadAs<float>("Meter");
            Speed = Pool.ReadAs<float>("Speed");
            EngineRpm = Pool.ReadAs<float>("RPM");
            EngineRpmMax = Pool.ReadAs<float>("RPMMax");
            Gear = Pool.ReadAs<int>("Gear");

            Mass = Pool.ReadAs<float>("Mass");
            Fuel = Pool.ReadAs<float>("Fuel");

            CoordinateX = Pool.ReadAs<float>("CoordinateX");
            CoordinateY = Pool.ReadAs<float>("CoordinateY");
            CoordinateZ = Pool.ReadAs<float>("CoordinateZ");

            InputThrottle = Pool.ReadAs<float>("InputThrottle");
            InputBrake = Pool.ReadAs<float>("InputBrake");

            Pitstops = Pool.ReadAs<int>("Pitstops");
            Position = Pool.ReadAs<int>("Position");
            Laps = Pool.ReadAs<int>("Laps");

            IsRetired = Pool.ReadAs<bool>("IsRetired");
            IsLimiter = Pool.ReadAs<bool>("IsLimiter");
            IsPits = Pool.ReadAs<bool>("IsPits");
            IsDriving = Pool.ReadAs<bool>("IsDriving");

            FlagYellow = Pool.ReadAs<bool>("FlagYellow");
            FlagBlue = Pool.ReadAs<bool>("FlagBlue");
            FlagBlack = Pool.ReadAs<bool>("FlagBlack");
            Ignition = Pool.ReadAs<bool>("Ignition");

            CarFile = Pool.ReadAs<string>("CarFile");
            if (string.IsNullOrEmpty(CarFile))
                CarFile = Pool.ReadAs<string>("CarFileName");

            CarClasses = Pool.ReadAs<string>("CarClasses").Split(" ".ToCharArray()).ToList();
            CarModel = Pool.ReadAs<string>("CarModel");
            CarNumber = Pool.ReadAs<int>("CarNumber");

            Heading = Pool.ReadAs<float>("Yaw");

            Name = Pool.ReadAs<string>("Name");

            WaterTemperature = Pool.ReadAs<float>("EngineWater");
            OilTemperature = Pool.ReadAs<float>("EngineOil");

            if (Pool is MemoryPool && (Pool as MemoryPool).Pools.ContainsKey("Laps") && DateTime.Now.Subtract(LastLapsUpdate).TotalMilliseconds > 500)
            {
                LapsList = (Pool as MemoryPool).Pools["Laps"].ReadAs<List<Lap>>("List");
                LastLapsUpdate = DateTime.Now;

                for(int i = 0; i < LapsList.Count; i++)
                {
                    LapsList[i].SetDriver(BaseAddress);
                }
            }

            if (Pool.Contains("TrackSector"))
            {
                TrackPosition = Pool.ReadAs<TrackPointType>("TrackSector");
            }
            else
            {
                // Generate from lap list.
                // TODO: Write test code for this
                var lastlap = LapsList.LastOrDefault();
                if (lastlap != null)
                {
                    if (lastlap.Sector1 <= 0) TrackPosition = TrackPointType.SECTOR1;
                    else if (lastlap.Sector2 <= 0) TrackPosition = TrackPointType.SECTOR2;
                    else if (lastlap.Sector3 <= 0) TrackPosition = TrackPointType.SECTOR3;
                }
                else
                {
                    TrackPosition = TrackPointType.SECTOR1;
                }
            }

            CurrentLap = LapsList.LastOrDefault();

            if (WheelLF != null) WheelLF.Update(telemetry, Memory);
            if (WheelRF != null) WheelRF.Update(telemetry, Memory);
            if (WheelLR != null) WheelLR.Update(telemetry, Memory);
            if (WheelRR != null) WheelRR.Update(telemetry, Memory);

            if (IsPlayer)
            {
                
            }
        }

        public TelemetryDriver(IDataNode pool)
        {
            Pool = pool;
            dummyLap = new Lap(-1, false, -1, -1, -1, -1, -1, false, false);

            // Last&Best Lap is done in TelemetryLapsPool
            LastLap = dummyLap;
            BestLap = dummyLap;

            BestS1 = -1;
            BestS2 = -1;
            BestS3 = -1;

            if (Pool is MemoryPool)
            {
                BaseAddress = ((MemoryPool) Pool).Address;
            }

            // Add 4 wheels if they exist
            if (Pool.Contains("TyreTemperatureMiddleLF"))
            {
                // We have something atleast..
                WheelLF = new TelemetryWheel(WheelLocation.FRONTLEFT, pool);
                WheelLR = new TelemetryWheel(WheelLocation.REARLEFT, pool);
                WheelRF = new TelemetryWheel(WheelLocation.FRONTRIGHT, pool);
                WheelRR = new TelemetryWheel(WheelLocation.REARRIGHT, pool);
            }
        }


        public TelemetryDriver Clone()
        {
            return (TelemetryDriver)MemberwiseClone();
        }

        public IEnumerable<Lap> GetLaps()
        {
            return LapsList;
        }
        
        public double GetSplitTime(TelemetryDriver telemetryDriver)
        {
            return 0;
        }
    }
}