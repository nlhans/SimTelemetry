using System;

namespace SimTelemetry.Objects
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Loggable : Attribute
    {
        public bool LogOnChange { get; internal set; }

        private double _Frequency = 0;
        public double Freqency { get { return _Frequency; } }

        public Loggable(bool OnChange)
        {
            _Frequency = 100; // check 100x per second
            LogOnChange = OnChange;
        }

        public Loggable(double frequency)
        {
            _Frequency = frequency;
            if (_Frequency <= 1) _Frequency = 1;
            else
            _Frequency = 100;

            LogOnChange = false;
        }
    }
}