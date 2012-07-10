using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class SampledSession : ISession
    {
        private string _gameDataTrackFile;

        private string _gameDirectory;

        private bool _flagYellowFull;

        private bool _flagRed;

        private bool _flagGreen;

        private bool _flagFinish;

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

        public string GameData_TrackFile
        {
            get { return _gameDataTrackFile; }
            set { _gameDataTrackFile = value; }
        }

        public string GameDirectory
        {
            get { return _gameDirectory; }
            set { _gameDirectory = value; }
        }

        private int _raceLaps;
        public int RaceLaps
        {
            get { return _raceLaps; }
            set { _raceLaps = value; }
        }

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

        public bool Active { get;set; }

        public bool Flag_YellowFull
        {
            get { return _flagYellowFull; }
            set { _flagYellowFull = value; }
        }

        public bool Flag_Red
        {
            get { return _flagRed; }
            set { _flagRed = value; }
        }

        public bool Flag_Green
        {
            get { return _flagGreen; }
            set { _flagGreen = value; }
        }

        public bool Flag_Finish
        {
            get { return _flagFinish; }
            set { _flagFinish = value; }
        }
    }
}