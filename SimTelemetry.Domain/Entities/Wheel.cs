using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Domain.Entities
{
    public class Wheel
    {
        public WheelLocation Location { get; private set; }

        public float Perimeter { get; private set; }
        public float RollResistance { get; private set; }

        public Range PeakTemperature { get; private set; }
        public Range PitsTemperature { get; private set; }
        public Range PeakPressure { get; private set; }

        public Wheel(WheelLocation location, float perimeter, float rollResistance, Range peakTemperature, Range pitsTemperature, Range peakPressure)
        {
            Location = location;
            Perimeter = perimeter;
            RollResistance = rollResistance;
            PeakTemperature = peakTemperature;
            PitsTemperature = pitsTemperature;
            PeakPressure = peakPressure;
        }
    }
}