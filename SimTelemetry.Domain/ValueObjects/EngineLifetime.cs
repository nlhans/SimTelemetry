using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class EngineLifetime : IValueObject<EngineLifetime>
    {
        public int Time { get; private set; }

        public NormalDistrbution EngineRpm { get; private set; }
        public NormalDistrbution OilTemperature { get; private set; }
        public NormalDistrbution WaterTemperature { get; private set; }

        public EngineLifetime(int time, NormalDistrbution engineRpm, NormalDistrbution oilTemperature, NormalDistrbution waterTemperature)
        {
            Time = time;
            EngineRpm = engineRpm;
            OilTemperature = oilTemperature;
            WaterTemperature = waterTemperature;
        }

        public bool Equals(EngineLifetime other)
        {
            return other.Time == Time && EngineRpm.Equals(other.EngineRpm) &&
                   OilTemperature.Equals(other.OilTemperature) && WaterTemperature.Equals(WaterTemperature);
        }
    }
}