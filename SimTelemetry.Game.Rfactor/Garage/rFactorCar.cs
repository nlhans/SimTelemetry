using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorCar : ICar
    {
        private string _file;

        private string _team;

        private string _driver;

        private int _number;

        private Dictionary<string, string> _files;

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

        private ICarBrakes _brakes;

        private ICarGeneral _general;
        private IMod _mod;
        private IniScanner _mScanner;
        private IniScanner _mHDV;

        public string File
        {
            get { return _file; }
        }

        public string Team
        {
            get { return _team; }
        }

        public string Driver
        {
            get { return _driver; }
        }

        public int Number
        {
            get { return _number; }
        }

        public Dictionary<string, string> Files
        {
            get { return _files; }
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

        public ICarBrakes Brakes
        {
            get { return _brakes; }
        }

        public ICarGeneral General
        {
            get { return _general; }
        }

        public rFactorCar(IMod mod, string file)
        {
            _mod = mod;
            _file = file;
        }

        public void Scan()
        {
            _mScanner = new IniScanner { IniFile = _file };

            _mScanner.Read();

            _team = _mScanner.TryGetString("Team");
            _driver = _mScanner.TryGetString("Driver");
            _number = _mScanner.TryGetInt32("Number");

            _files = new Dictionary<string, string>();
            _files.Add("Vehicle", GarageTools.SearchFile(rFactor.Garage.GamedataDirectory, _mScanner.TryGetString("HDVehicle")));

            _infoEngineManufacturer = _mScanner.TryGetString("Engine");

            _infoYearFounded = _mScanner.TryGetInt32("TeamFounded");
            _infoHq = _mScanner.TryGetString("TeamHeadquarters");
            _infoStarts = _mScanner.TryGetInt32("TeamStarts");
            _infoPoles = _mScanner.TryGetInt32("TeamPoles");
            _infoWins = _mScanner.TryGetInt32("TeamWins");
            _infoChampionships = _mScanner.TryGetInt32("TeamWorldChampionships");

            _mHDV = new IniScanner { IniFile = _files["Vehicle"] };
            _mHDV.IgnoreGroups = false;
            _mHDV.Read();

            // Add additional files.
            _files.Add("Engine", GarageTools.SearchFile(rFactor.Garage.GamedataDirectory, _mHDV.TryGetString("ENGINE", "Normal") + ".ini"));
            _files.Add("Tyre", GarageTools.SearchFile(rFactor.Garage.GamedataDirectory, _mHDV.TryGetString("GENERAL", "TireBrand") + ".tbc"));

            // Now we have read the HDV file, we can just as well initialize all sub classes:
            _engine = new rFactorCarEngine(_files["Engine"], _mHDV);

            // TODO: Parse more data.
        }
    }

}