using System;
using System.Collections.Generic;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorMod : IMod
    {
        private string _file;
        private string _name;
        private string _author;
        private string _description;
        private string _website;
        private string _version;
        private string _directoryVehicles;
        private int _pitSpeedPracticeDefault;
        private int _pitSpeedRaceDefault;
        private int _opponents;

        private List<IModChampionship> _championships;
        private List<ICar> _models;

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

        public string Directory_Vehicles
        {
            get { return _directoryVehicles; }
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

        public List<IModChampionship> Championships
        {
            get { return _championships; }
        }

        public List<ICar> Models
        {
            get { return _models; }
        }

        public string Image
        {
            get { return _file.Replace(".rfm", ".tga"); }
        }

        private IniScanner _mScanner;

        public rFactorMod(string file)
        {
            _file = file;

            _models = new List<ICar>();
            _championships = new List<IModChampionship>();
        }

        private void Handle_TrackLine(object data)
        {

        }

        private bool Scanned = false;
        public void Scan()
        {
            if (!Scanned)
            {
                Scanned = true;
                _mScanner = new IniScanner {IniFile = _file};
                _mScanner.HandleUnknownLine += new Signal(Handle_TrackLine);
                _mScanner.IgnoreGroups = false;
                _mScanner.Read();

                // Only name available in rFactor. Or where else?
                _name = _mScanner.TryGetString("Mod Name");
                _author = "";
                _description = "";
                _website = "";
                _version = "";
                _directoryVehicles = _mScanner.TryGetString("Main.ConfigOverrides", "VehiclesDir");

                // Pitspeeds
                Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring", "RacePitKPH"), out _pitSpeedRaceDefault);
                Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring", "NormalPitKPH"),
                               out _pitSpeedPracticeDefault);

                // TODO: Add more properties like ParcFerme properties, flag settings, point scoring, etc.
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> GroupKey in _mScanner.Data)
                {
                    if (GroupKey.Key.StartsWith("Season"))
                    {
                        // Championship!
                        // TODO: parse track data.
                    }
                }

                Int32.TryParse(_mScanner.TryGetString("Main", "Max Opponents"), out _opponents);

                // Search for all cars in folder.
                List<string> vehicles =
                    rFactor.Garage.Files.SearchFiles(rFactor.Garage.InstallationDirectory + _directoryVehicles,
                                            "*.veh");
                Console.WriteLine("Found " + vehicles.Count + " vehicles for MOD [" + _name + "]");

                foreach (string veh in vehicles)
                {
                    _models.Add(new rFactorCar(this, veh));
                }
            }

        }
    }
}