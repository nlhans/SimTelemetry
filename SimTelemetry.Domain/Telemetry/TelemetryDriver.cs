using System;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryDriver : ITelemetryObject
    {
        public int BaseAddress { get { return Pool.Address; }}

        internal MemoryPool Pool { get; set; }

        public bool IsPlayer { get; protected set; }
        public string Name { get; protected set; }
        public string CarTeam { get; protected set; }
        public string CarModel { get; protected set; }
        public string CarClasses { get; protected set; }

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

        public float InputThrottle { get; protected set; }
        public float InputBrake { get; protected set; }

        private bool _initial = true;

        public void Update(Aggregates.Telemetry telemetry)
        {
            if(_initial)
            {
                _initial = false;

                IsPlayer = Pool.ReadAs<bool>("IsPlayer");
                Name = Pool.ReadAs<string>("Name");
                CarTeam = Pool.ReadAs<string>("CarTeam");
                CarModel = Pool.ReadAs<string>("CarModel");
                CarClasses = Pool.ReadAs<string>("CarClasses");

                FuelCapacity = Pool.ReadAs<float>("FuelCapacity");
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
            
            if (IsPlayer)
            {

            }
        }

        public void Update(LogFile logFile)
        {
            throw new NotImplementedException();
        }

        public TelemetryDriver(MemoryPool pool)
        {
            Pool = pool;
        }

        public TelemetryDriver(ILogNode f)
        {
        }

        public TelemetryDriver Clone()
        {
            return (TelemetryDriver)MemberwiseClone();
        }
    }
}