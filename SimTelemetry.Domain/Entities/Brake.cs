using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Entities
{
    public class Brake
    {
        public WheelLocation Location { get; private set; }

        public Range OptimumTemperature { get; private set; }

        public Range ThicknessStart { get; private set; }
        public float ThicknessFailure { get; private set; }

        public float Torque { get; private set; }

        public Brake(WheelLocation location, Range optimumTemperature, Range thicknessStart, float thicknessFailure, float torque)
        {
            Location = location;
            OptimumTemperature = optimumTemperature;
            ThicknessStart = thicknessStart;
            ThicknessFailure = thicknessFailure;
            Torque = torque;
        }
    }
}