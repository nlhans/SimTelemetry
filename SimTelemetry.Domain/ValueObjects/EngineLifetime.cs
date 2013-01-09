using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class EngineLifetime : IValueObject<EngineLifetime>
    {
        public int Time { get; private set; }

        public Range EngineRpm { get; private set; }
        public Range OilTemperature { get; private set; }
        public Range WaterTemperature { get; private set; }

        public EngineLifetime(int time, Range engineRpm, Range oilTemperature, Range waterTemperature)
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