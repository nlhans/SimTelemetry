/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
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
        private List<string> _classes;
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

        public List<string> Classes
        {
            get { return _classes; }
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
                string c = _mScanner.TryGetString("Main", "Vehicle Filter");
                if (c.StartsWith("\""))
                    c = c.Substring(1, c.Length - 2);
                if(c.Contains(" "))
                {
                    _classes = new List<string>( c.Split(" ".ToCharArray()));
                }
                else
                {
                    _classes = new List<string>(c.Split(",".ToCharArray()));
                }

                // Pitspeeds
                Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring", "RacePitKPH"), out _pitSpeedRaceDefault);
                Int32.TryParse(_mScanner.TryGetString("Main.DefaultScoring", "NormalPitKPH"),
                               out _pitSpeedPracticeDefault);

                // TODO: Add more properties like ParcFerme properties, flag settings, point scoring, etc.
                foreach (KeyValuePair<string, Dictionary<string, string[]>> GroupKey in _mScanner.Data)
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

                foreach (string veh in vehicles)
                {
                    ICar car = rFactor.Garage.CarFactory(this, veh);
                    if (car.InClass(Classes))
                    {
                        _models.Add(car);
                    }
                }
                Console.WriteLine("Found " + _models.Count + " vehicles for MOD [" + _name + "]");
            }

        }
    }
}