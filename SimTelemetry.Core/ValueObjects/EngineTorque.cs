using System;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.ValueObjects
{
    public class EngineTorque : IValueObject<EngineTorque>
    {
        public float RPM { get; private set; }
        public Range Torque { get; private set; }

        public EngineTorque(float rpm, Range torque)
        {
            RPM = rpm;
            Torque = torque;
        }
        public EngineTorque(float rpm, float torque_coast, float torque_power)
        {
            RPM = rpm;
            Torque = new Range(torque_coast, torque_power);
        }

        public bool Equals(EngineTorque other)
        {
            return other.RPM == RPM && Torque.Maximum == other.Torque.Maximum && Torque.Minimum == other.Torque.Minimum;
        }
    }
}