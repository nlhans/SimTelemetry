using SimTelemetry.Objects;

namespace SimTelemetry.Game.rFactor2.v1005b
{
    public class rf2Lap : ILap
    {
        private int _lap;

        private float _sector1;

        private float _sector2;

        private float _sector3;

        private float _lapTime;

        public int LapNumber
        {
            get { return _lap; }
        }

        public float Sector1
        {
            get { return _sector1; }
        }

        public float Sector2
        {
            get { return _sector2; }
        }

        public float Sector3
        {
            get { return _sector3; }
        }

        public float LapTime
        {
            get { return _lapTime; }
        }
    }
}