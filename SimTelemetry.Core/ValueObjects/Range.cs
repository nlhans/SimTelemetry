using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Core.ValueObjects
{
    public class Range
    {
        public float Minimum { get; private set; }
        public float Optimum { get; private set; }
        public float Maximum { get; private set; }
        public float Step { get; private set; }
        public float Deviation { get; private set; }

        public Range(float minimum, float optimum, float maximum, float step, float dev)
        {
            Minimum = minimum;
            Optimum = optimum;
            Maximum = maximum;
            Step = step;
            Deviation = dev;
        }
    }
}
