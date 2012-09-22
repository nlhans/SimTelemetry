using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor
{
    public class Lap : ILap
    {
        private int _lap;
        private float _sector1;
        private float _sector2;
        private float _sector3;

        public Lap(int lap, float s1, float s2, float s3)
        {
            _lap = lap;
            _sector1 = s1;
            _sector2 = s2;
            _sector3 = s3;
        }

        public int LapNumber { get { return _lap; } }

        public float Sector1 { get { return _sector1; } }
        public float Sector2 { get { return _sector2; } }
        public float Sector3 { get { return _sector3; } }

        public float LapTime
        {
            get { return _sector1 + _sector2 + _sector3; }
        }
    }
}