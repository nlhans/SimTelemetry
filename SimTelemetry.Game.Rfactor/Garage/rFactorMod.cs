using System;
using System.Collections.Generic;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorMod : IGarageMod
    {
        private string _file;

        private string _name;

        private string _author;

        private string _description;

        private string _website;

        private string _version;

        private int _pitSpeedPracticeDefault;

        private int _pitSpeedRaceDefault;

        private int _opponents;

        private List<IGarageChampionship> _championships;

        private List<IGarageCar> _models;

        public string File
        {
            get { return _file; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Author
        {
            get { return _author; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Website
        {
            get { return _website; }
        }

        public string Version
        {
            get { return _version; }
        }

        public int PitSpeed_Practice_Default
        {
            get { return _pitSpeedPracticeDefault; }
        }

        public int PitSpeed_Race_Default
        {
            get { return _pitSpeedRaceDefault; }
        }

        public int Opponents
        {
            get { return _opponents; }
        }

        public List<IGarageChampionship> Championships
        {
            get { return _championships; }
        }

        public List<IGarageCar> Models
        {
            get { return _models; }
        }

        private IniScanner _mScanner;

        public rFactorMod(string file)
        {
            _file = file;

            _models = new List<IGarageCar>();
            _championships = new List<IGarageChampionship>();
        }

        private void Handle_TrackLine(object data)
        {

        }

        public void Scan()
        {
            _mScanner = new IniScanner { IniFile = _file };
            _mScanner.HandleUnknownLine += new Signal(Handle_TrackLine);
            _mScanner.IgnoreGroups = false;
            _mScanner.Read();

            // Only name available in rFactor. Or where else?
            _name = _mScanner.TryGetString("Mod Name");
            _author = "";
            _description = "";
            _website = "";
            _version = "";

            // Pitspeeds
            Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring","RacePitKPH"), out _pitSpeedRaceDefault);
            Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring", "NormalPitKPH"), out _pitSpeedPracticeDefault);

            // TODO: Add more properties like ParcFerme properties, flag settings, point scoring, etc.
            foreach(KeyValuePair<string, Dictionary<string, List<string>>> GroupKey in _mScanner.Data)
            {
                if(GroupKey.Key.StartsWith("Season"))
                {
                    // Championship!
                    // TODO: parse track data.
                }
            }

            Int32.TryParse(_mScanner.TryGetString("Main", "Max Opponents"), out _opponents);
        }
    }
}