using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Entities
{
    public class Wheel
    {
        public WheelLocation Location { get; private set; }

        public float Perimeter { get; private set; }
        public float RollResistance { get; private set; }
        public float PitsTemperature { get; private set; }

        public float PeakTemperature { get; private set; }
        public float PeakPressure { get; private set; }
        public float PeakPressureWeightSlope { get; private set; }

        public Wheel(WheelLocation location, float perimeter, float rollResistance, float pitsTemperature, float peakTemperature, float peakPressure, float peakPressureWeightSlope)
        {
            Location = location;
            Perimeter = perimeter;
            RollResistance = rollResistance;
            PitsTemperature = pitsTemperature;
            PeakTemperature = peakTemperature;
            PeakPressure = peakPressure;
            PeakPressureWeightSlope = peakPressureWeightSlope;
        }

        public double GetOptimalPressure(double forceOnWheel)
        {
            return PeakPressure + PeakPressureWeightSlope * forceOnWheel;
        }
    }
}