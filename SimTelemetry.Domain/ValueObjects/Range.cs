using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class NormalDistrbution : IValueObject<NormalDistrbution>
    {
        public float Optimum { get; private set; }
        public float Deviation { get; private set; }

        public NormalDistrbution(float step, float deviation)
        {
            Optimum = step;
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

        public Range()
        {
            Minimum = -1;
            Maximum = -1;
            Optimum = -1;
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

        public bool Equals(Range other)
        {
            return other.Minimum == Minimum && other.Maximum == Maximum && other.Optimum == Optimum;
        }
    }
}
