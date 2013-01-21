using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class EngineMode : IValueObject<EngineMode>
    {
        public float Power { get; private set; }
        public float Torque { get; private set; }
        public float RPM { get; private set; }
        public float Fuel { get; private set; }
        public float Wear { get; private set; }

        public EngineMode(float power, float torque, float rpm, float fuel, float wear)
        {
            Power = power;
            Torque = torque;
            RPM = rpm;
            Fuel = fuel;
            Wear = wear;
        }

        public bool Equals(EngineMode other)
        {
            return Power == other.Power && Torque == other.Torque && RPM == other.RPM && Fuel == other.Fuel && Wear==other.Wear;
        }
    }
}