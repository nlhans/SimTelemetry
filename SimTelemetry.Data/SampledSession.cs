using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class SampledSession : ISession
    {
        public SampledSession Duplicate()
        {
            return (SampledSession)this.MemberwiseClone();
        }

        private bool _isRace;

        private string _circuitName;

        private float _trackTemperature;

        private float _ambientTemperature;

        private SessionInfo _type;

        private float _time;

        private float _timeClock;

        private int _carsInPits;

        private int _carsOnTrack;

        private int _cars;

        public bool IsRace
        {
            get { return _isRace; }
            set { _isRace = value; }
        }

        public string CircuitName
        {
            get { return _circuitName; }
            set { _circuitName = value; }
        }

        public float TrackTemperature
        {
            get { return _trackTemperature; }
            set { _trackTemperature = value; }
        }

        public float AmbientTemperature
        {
            get { return _ambientTemperature; }
            set { _ambientTemperature = value; }
        }

        public SessionInfo Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public float Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public float TimeClock
        {
            get { return _timeClock; }
            set { _timeClock = value; }
        }

        public int Cars_InPits
        {
            get { return _carsInPits; }
            set { _carsInPits = value; }
        }

        public int Cars_OnTrack
        {
            get { return _carsOnTrack; }
            set { _carsOnTrack = value; }
        }

        public int Cars
        {
            get { return _cars; }
            set { _cars = value; }
        }
    }
}