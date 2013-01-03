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

namespace SimTelemetry.Game.rFactor2.Garage
{
    public class rFactor2Car : ICar
    {
        private MAS2File masfile;
        private bool Scanned = false;
        private IniScanner _mHDV;
        private IniScanner _mScanner;

        private string _team;

        private string _driver;

        private string _description;

        private int _number;

        private List<string> _classes;

        private Dictionary<string, MAS2File> _files;

        private string _infoEngineManufacturer;

        private int _infoYearFounded;

        private string _infoHq;

        private int _infoStarts;

        private int _infoPoles;

        private int _infoWins;

        private int _infoChampionships;

        private ICarEngine _engine;

        private ICarGearbox _gearbox;

        private ICarAerodynamics _aerodynamics;

        private ICarWheels _wheels;

        private ICarGeneral _general;


        public string File
        {
            get { return masfile.Filename; }
        }

        public string Team
        {
            get { return _team; }
        }

        public string Driver
        {
            get { return _driver; }
        }

        public string Description
        {
            get { return _description; }
        }

        public int Number
        {
            get { return _number; }
        }

        public List<string> Classes
        {
            get { return _classes; }
        }

        public Dictionary<string, MAS2File> MASFiles
        {
            get { return _files; }
        }
        public Dictionary<string, string> Files
        {
            get
            {
                Dictionary<string, string> k = new Dictionary<string, string>();
                foreach (KeyValuePair<string, MAS2File> f in MASFiles)
                    k.Add(f.Key, f.Value.Filename);
                return k;
            }
        }

        public string Info_Engine_Manufacturer
        {
            get { return _infoEngineManufacturer; }
        }

        public int Info_YearFounded
        {
            get { return _infoYearFounded; }
        }

        public string Info_HQ
        {
            get { return _infoHq; }
        }

        public int Info_Starts
        {
            get { return _infoStarts; }
        }

        public int Info_Poles
        {
            get { return _infoPoles; }
        }

        public int Info_Wins
        {
            get { return _infoWins; }
        }

        public int Info_Championships
        {
            get { return _infoChampionships; }
        }

        public ICarEngine Engine
        {
            get { return _engine; }
        }

        public ICarGearbox Gearbox
        {
            get { return _gearbox; }
        }

        public ICarAerodynamics Aerodynamics
        {
            get { return _aerodynamics; }
        }

        public ICarWheels Wheels
        {
            get { return _wheels; }
        }

        public string PhysicsFile
        {
            get { return masfile.Filename; }
        }

        public ICarGeneral General
        {
            get { return _general; }
        }

        public rFactor2Car(string veh)
        {
            masfile = rFactor2.Garage.Files.SearchFile(veh);
            Scanned = false;
        }

        public void Scan()
        {
            if(!Scanned)
            {
                 _mScanner = new IniScanner{IniData = masfile.Master.ExtractString(masfile)};
                 _mScanner.Read();
                Scanned = true;

                _team = _mScanner.TryGetString("Description");
                _driver = _mScanner.TryGetString("Driver");
                _description = _mScanner.TryGetString("Team");
                _number = _mScanner.TryGetInt32("Number");

                if(_team.Length > 3)_team = _team.Substring(1, _team.Length - 2);
                if(_driver.Length>3) _driver = _driver.Substring(1, _driver.Length - 2);
                if(_description.Length>3) _description = _description.Substring(1, _description.Length - 2);

                string c = _mScanner.TryGetString("Classes");
                if (c.StartsWith("\""))
                    c = c.Substring(1, c.Length - 2);

                if (c.StartsWith("\""))
                    c = c.Substring(1, c.Length - 2);
                if (c.Contains(" "))
                {
                    _classes = new List<string>(c.Split(" ".ToCharArray()));
                }
                else
                {
                    _classes = new List<string>(c.Split(",".ToCharArray()));
                }

                _files = new Dictionary<string, MAS2File>();
                _files.Add("Vehicle", rFactor2.Garage.Files.SearchFile(_mScanner.TryGetString("HDVehicle")));

                _infoEngineManufacturer = _mScanner.TryGetString("Engine");

                _infoYearFounded = _mScanner.TryGetInt32("TeamFounded");
                _infoHq = _mScanner.TryGetString("TeamHeadquarters");
                _infoStarts = _mScanner.TryGetInt32("TeamStarts");
                _infoPoles = _mScanner.TryGetInt32("TeamPoles");
                _infoWins = _mScanner.TryGetInt32("TeamWins");
                _infoChampionships = _mScanner.TryGetInt32("TeamWorldChampionships");

                _mHDV = new IniScanner { IniData = _files["Vehicle"].Master.ExtractString(_files["Vehicle"]) };
                _mHDV.IgnoreGroups = false;
                _mHDV.Read();
            }
        }

        public bool InClass(List<string> classes)
        {
            return false;
        }

        public void ScanEngine()
        {
            if (_files.ContainsKey("Engine") == false)
            {
                string eng = _mHDV.TryGetString("ENGINE", "Normal");
                if (!eng.EndsWith(".ini"))
                    eng += ".ini";

                _files.Add("Engine", rFactor2.Garage.Files.SearchFile(eng));
                // Now we have read the HDV file, we can just as well initialize all sub classes:
                _engine = new rFactor2CarEngine(_files["Engine"], _mHDV);
            }
        }

        public void ScanGeneral()
        {
            throw new NotImplementedException();
        }

        public void ScanAerodynamics()
        {
            throw new NotImplementedException();
        }
    }
}