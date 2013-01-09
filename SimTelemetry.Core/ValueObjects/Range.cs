using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.ValueObjects
{
    public class Range : IValueObject<Range>
    {
        public float Minimum { get; private set; }
        public float Optimum { get; private set; }
        public float Maximum { get; private set; }
        public float Step { get; private set; }
        public float Deviation { get; private set; }

        public Range()
        {
            Minimum = -1;
            Maximum = -1;
            Step = -1;
            Optimum = -1;
            Deviation = -1;
        }

        public Range(float minimum, float maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;

        }

        public Range(float minimum, float maximum, float optimum)
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            Optimum = optimum;
        }

        public Range(float minimum, float maximum, float optimum, float deviation)
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            Optimum = optimum;
            Deviation = deviation;
        }

        public Range(float minimum, float maximum, float optimum, float dev, float step)
        {
            Minimum = minimum;
            Optimum = optimum;
            Maximum = maximum;
            Step = step;
            Deviation = dev;
        }

        public bool Equals(Range other)
        {
            return other.Minimum == Minimum && other.Maximum == Maximum && other.Optimum == Optimum;
        }
    }
}
