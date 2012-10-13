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

        private int _pitSpeedPractice;

        private int _pitSpeedRace;

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

        public int PitSpeed_Practice
        {
            get { return _pitSpeedPractice; }
        }

        public int PitSpeed_Race
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
            _type = scanner.TryGetString("TrackType");
            _imageCache = System.IO.File.Exists("tracks/rfactor_" + File.Replace(".gdb", ".png")); // TODO: Fix a more unique ID globally!

            _length = 0; // Read from AIW.

            /******************* QUALIFY DAY *******************/
            _qualifyDay = scanner.TryGetString("QualifyDay");
            Int32.TryParse(scanner.TryGetString("QualifyLaps"), out _qualifyLaps);
            Int32.TryParse(scanner.TryGetString("QualifyDuration"), out _qualifyMinutes);
            int qualify_start_hr = 0, qualify_start_min = 0;
            string qstart = scanner.TryGetString("QualifyStart").Trim();
            if (qstart.Length == 5)
            {
                Int32.TryParse(qstart.Substring(0, 2), out qualify_start_hr);
                Int32.TryParse(qstart.Substring(3, 2), out qualify_start_min);
                _qualifyStart = qualify_start_min*60 + qualify_start_hr*3600;
            }


            /******************* RACE DAY *******************/
            _fullRaceDay = scanner.TryGetString("RaceDay");
            Int32.TryParse(scanner.TryGetString("RaceLaps"), out _fullRaceLaps);
            Int32.TryParse(scanner.TryGetString("RaceDuration"), out _fullRaceMinutes);
            int race_start_hr = 0, race_start_min = 0;
            string rstart = scanner.TryGetString("RaceStart").Trim();
            if (rstart.Length == 5)
            {
                Int32.TryParse(rstart.Substring(0, 2), out race_start_hr);
                Int32.TryParse(rstart.Substring(3, 2), out race_start_min);
                _fullRaceMinutes = race_start_min*60 + race_start_hr*3600;
            }

            _pitlane = true;
            //Int32.TryParse(scanner.TryGetString("NormalPitKPH"), out _startingGridSize);
            //Int32.TryParse(scanner.TryGetString("NormalPitKPH"), out _pitSpots);
            // TODO: Headlights required.

            Int32.TryParse(scanner.TryGetString("NormalPitKPH"), out _pitSpeedPractice);
            Int32.TryParse(scanner.TryGetString("RacePitKPH"), out _pitSpeedRace);

            Double.TryParse(scanner.TryGetString("Qualify Laptime").Trim(), out _laprecordQualifyTime);
            Double.TryParse(scanner.TryGetString("Race Laptime").Trim(), out _laprecordRaceTime);
            // Unfortunately, no driver strings in rFactor for lap records.


        }
    }
}