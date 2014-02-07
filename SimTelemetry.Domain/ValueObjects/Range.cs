using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class NormalDistrbution : IValueObject<NormalDistrbution>
    {
        public float Optimum { get; private set; }
        public float Deviation { get; private set; }

        public NormalDistrbution(float optimum, float deviation)
        {
            Optimum = optimum;
            Deviation = deviation;
        }

        public bool Equals(NormalDistrbution other)
        {
            return Optimum == other.Optimum && Deviation == other.Deviation;
        }
    }

    public class Range : IValueObject<Range>
    {
        public float Minimum { get; private set; }
        public float Optimum { get; private set; }
        public float Maximum { get; private set; }
        public float Step { get; private set; }
        public float Span { get { return Maximum - Minimum; } }

        public float Average { get { return (Maximum + Minimum)/2; } }

        public Range()
        {
            Minimum = -1;
            Maximum = -1;
            Optimum = -1;
            Step = -1;
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

        public Range(float minimum, float maximum, float optimum, float step)
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            Optimum = optimum;
            Step = step;
        }

        public bool Equals(Range other)
        {
            return other.Minimum == Minimum && other.Maximum == Maximum && other.Optimum == Optimum && Step == other.Step;
        }
    }
}
