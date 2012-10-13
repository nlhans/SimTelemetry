using System;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorTrack : IGarageTrack
    {
        private string _file;

        private string _name;

        private string _location;

        private string _type;

        private bool _imageCache;

        private double _length;

        private string _qualifyDay;

        private double _qualifyStart;

        private int _qualifyLaps;

        private int _qualifyMinutes;

        private string _fullRaceDay;

        private double _fullRaceStart;

        private int _fullRaceMinutes;

        private int _fullRaceLaps;

        private bool _pitlane;

        private int _startingGridSize;

        private int _pitSpots;

        private double _pitSpeedPractice;

        private double _pitSpeedRace;

        private double _laprecordRaceTime;

        private string _laprecordRaceDriver;

        private double _laprecordQualifyTime;

        private string _laprecordQualifyDriver;

        public string File
        {
            get { return _file; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Location
        {
            get { return _location; }
        }

        public string Type
        {
            get { return _type; }
        }

        public bool ImageCache
        {
            get { return _imageCache; }
        }

        public double Length
        {
            get { return _length; }
        }

        public string Qualify_Day
        {
            get { return _qualifyDay; }
        }

        public double Qualify_Start
        {
            get { return _qualifyStart; }
        }

        public int Qualify_Laps
        {
            get { return _qualifyLaps; }
        }

        public int Qualify_Minutes
        {
            get { return _qualifyMinutes; }
        }

        public string FullRace_Day
        {
            get { return _fullRaceDay; }
        }

        public double FullRace_Start
        {
            get { return _fullRaceStart; }
        }

        public int FullRace_Minutes
        {
            get { return _fullRaceMinutes; }
        }

        public int FullRace_Laps
        {
            get { return _fullRaceLaps; }
        }

        public bool Pitlane
        {
            get { return _pitlane; }
        }

        public int StartingGridSize
        {
            get { return _startingGridSize; }
        }

        public int PitSpots
        {
            get { return _pitSpots; }
        }

        public double PitSpeed_Practice
        {
            get { return _pitSpeedPractice; }
        }

        public double PitSpeed_Race
        {
            get { return _pitSpeedRace; }
        }

        public double Laprecord_Race_Time
        {
            get { return _laprecordRaceTime; }
        }

        public string Laprecord_Race_Driver
        {
            get { return _laprecordRaceDriver; }
        }

        public double Laprecord_Qualify_Time
        {
            get { return _laprecordQualifyTime; }
        }

        public string Laprecord_Qualify_Driver
        {
            get { return _laprecordQualifyDriver; }
        }

        public rFactorTrack(string file)
        {
            _file = file;
        }

        public void Scan()
        {
            IniScanner scanner = new IniScanner{IniFile=_file};
            scanner.Read();

            _name = scanner.TryGetString("TrackName");
            _location = scanner.TryGetString("Location");

        }
    }
}