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
        private bool _initial = true;


        internal IDataNode Pool { get; set; }

        public bool IsPlayer { get; protected set; }
        public string Name { get; protected set; }

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
        
        public float TyreWearLF { get; protected set; }
        public float TyreWearRF { get; protected set; }
        public float TyreWearLR { get; protected set; }
        public float TyreWearRR { get; protected set; }

        public float CoordinateX { get; protected set; }
        public float CoordinateY { get; protected set; }
        public float CoordinateZ { get; protected set; }

        public double Heading { get; private set; }

        public float InputThrottle { get; protected set; }
        public float InputBrake { get; protected set; }

        public int BaseAddress { get; private set; }

        public double EngineLifetime { get; private set; }

        public TelemetryWheel WheelRF { get; private set; }

        public double OilTemperature { get; private set; }

        public double WaterTemperature { get; private set; }

        public double EngineTorque { get; private set; }

        public TrackPointType TrackPosition { get; private set; }

        public TelemetryWheel WheelRR { get; private set; }
        public TelemetryWheel WheelLR { get; private set; }
        public TelemetryWheel WheelLF { get; private set; }

        protected List<Lap> LapsList { get; set; }

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

            TyreWearLF = Pool.ReadAs<float>("TyreWearLF");
            TyreWearLR = Pool.ReadAs<float>("TyreWearLR");
            TyreWearRF = Pool.ReadAs<float>("TyreWearRF");
            TyreWearRR = Pool.ReadAs<float>("TyreWearRR");
            
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

            CarClasses = Pool.ReadAs<string>("CarClasses").Split(" ".ToCharArray()).ToList();
            CarModel = Pool.ReadAs<string>("CarModel");
            CarNumber = Pool.ReadAs<int>("CarNumber");

            Heading = Pool.ReadAs<float>("Yaw");

            Name = Pool.ReadAs<string>("Name");

            if (Pool is MemoryPool && (Pool as MemoryPool).Pools.ContainsKey("Laps"))
                LapsList = (Pool as MemoryPool).Pools["Laps"].ReadAs<List<Lap>>("List");

            if (Pool.Fields.ContainsKey("TrackSector"))
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
            if (IsPlayer)
            {
                // Minimize the amount of fields in this section
            }
        }

        public TelemetryDriver(IDataNode pool)
        {
            Pool = pool;

            if (Pool is MemoryPool)
            {
                BaseAddress = ((MemoryPool) Pool).Address;
            }
        }


        public TelemetryDriver Clone()
        {
            return (TelemetryDriver)MemberwiseClone();
        }

        public IEnumerable<Lap> GetLaps()
        {
            return LapsList;
            return new List<Lap>();
        }

        public Lap GetBestLap()
        {
            return LapsList[0] ;
        }

        public double GetSplitTime(TelemetryDriver telemetryDriver)
        {
            return 0;
        }
    }

    public class TelemetryWheel
    {
        public float BrakeThickness
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float BrakeThicknessBase
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Pressure
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Wear
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TemperatureMiddle
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TemperatureInside
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float TemperatureOutside
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float Speed
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public float BrakeTemperature
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}