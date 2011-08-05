using System;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Loggable : Attribute
    {
        private double _Frequency = 0;
        public double Freqency { get { return _Frequency; } }

        public Loggable(double frequency)
        {
            _Frequency = frequency;
            if (_Frequency <= 1) _Frequency = 1;
            else
            _Frequency = 100;
        }
    }
}