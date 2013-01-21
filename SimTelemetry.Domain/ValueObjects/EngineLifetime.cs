using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class EngineLifetime : IValueObject<EngineLifetime>
    {
        public NormalDistrbution Time { get; private set; }

        public NormalDistrbution EngineRpm { get; private set; }
        public NormalDistrbution OilTemperature { get; private set; }

        public EngineLifetime(NormalDistrbution time, NormalDistrbution engineRpm, NormalDistrbution oilTemperature)
        {
            Time = time;
            EngineRpm = engineRpm;
            OilTemperature = oilTemperature;
        }

        public bool Equals(EngineLifetime other)
        {
            return other.Time.Equals(Time) && EngineRpm.Equals(other.EngineRpm) &&
                   OilTemperature.Equals(other.OilTemperature);
        }
    }
}