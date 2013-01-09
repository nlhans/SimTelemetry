using System;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.ValueObjects
{
    public class EngineMode : IValueObject<EngineMode>
    {
        public float Power { get; private set; }
        public float Torque { get; private set; }
        public float RPM { get; private set; }
        public float Fuel { get; private set; }

        public EngineMode(float power, float torque, float rpm, float fuel)
        {
            Power = power;
            Torque = torque;
            RPM = rpm;
            Fuel = fuel;
        }

        public bool Equals(EngineMode other)
        {
            return Power == other.Power && Torque == other.Torque && RPM == other.RPM && Fuel == other.Fuel;
        }
    }
}