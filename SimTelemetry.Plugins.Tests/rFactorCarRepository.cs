using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Utils;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorCarRepository : ILazyRepositoryDataSource<Car, string>
    {
        // Search for all vehicles
        string path = @"C:\Program Files (x86)\rFactor\GameData\Vehicles\";

        private IEnumerable<string> hdvFiles;
        private IEnumerable<string>  iniFiles;

        public string SearchHDV(string id)
        {
            id = id.ToLower();
            return hdvFiles.Any(x => x.Contains(id)) ? hdvFiles.Where(x => x.Contains(id)).FirstOrDefault() : string.Empty;
        }


        public string SearchIni(string id)
        {
            id = id.ToLower();
            return iniFiles.Any(x => x.Contains(id))
                       ? iniFiles.Where(x => x.Contains(id)).FirstOrDefault()
                       : string.Empty;
        }

        public IList<string> GetIds()
        {
            // Search also for other files.
            hdvFiles = Directory.GetFiles(path, "*.hdv", SearchOption.AllDirectories).Select(x => x.ToLower());
            iniFiles = Directory.GetFiles(path, "*.ini", SearchOption.AllDirectories).Select(x => x.ToLower());

            string[] files = Directory.GetFiles(path, "*.veh", SearchOption.AllDirectories);
            var fileList = new List<string>(files);
            return fileList.Select(x => x.Substring(path.Length).ToLower()).ToList();
        }

        public bool Add(Car obj)
        {
            return false;
        }

        public bool Store(Car obj)
        {
            return false;
        }

        public Car Get(string id)
        {
            string vehFile = path + id;

            // Helpers for other car parts.
            var hdvFile = "";
            var engFile = "";
            var engineName = "";
            var engineManufacturer = "";

            string team = "", driver = "", description = "";
            int number = 0;
            var classes = new List<string>();

            //Debug.WriteLine("Car::Get(\"" + id + "\")");

            // Parse Vehicle File
            using (var vehIni = new IniReader(vehFile, true))
            {
                vehIni.AddHandler(x =>
                                       {
                                           if (x.Key == "Team") team = x.ReadAsString();
                                           if (x.Key == "Driver") driver = x.ReadAsString();
                                           if (x.Key == "Description") description = x.ReadAsString();
                                           if (x.Key == "Number") number = x.ReadAsInteger();
                                           if (x.Key == "Classes") classes.AddRange(x.ReadAsStringArray());
                                           if (x.Key == "HDVehicle") hdvFile = x.ReadAsString();
                                           if (x.Key == "Engine") engineName = x.ReadAsString();
                                           if (x.Key == "Manufacturer") engineManufacturer = x.ReadAsString();
                                       });

                vehIni.Parse();
            }

            // Create car object
            var carObj = new Car(id, team, driver, description, number);
            carObj.Assign(classes);

            hdvFile = SearchHDV(hdvFile);
            if(hdvFile == string.Empty) throw new Exception("could not build this car, hdv file not found");


            // Create other objects.
            using (var hdvIni = new IniReader(hdvFile, true))
            {
                hdvIni.AddHandler(x =>
                                      {
                                          if (x.Key == "Normal" && x.Group == "ENGINE") engFile = x.ReadAsString();
                                      });
                hdvIni.Parse();
            }

            engFile = SearchIni(engFile);
            if (engFile == string.Empty) throw new Exception("could not build this car, engine file not found");

            carObj.Assign(BuildEngine(engFile, engineName, engineManufacturer));
            carObj.Assign(BuildChassis(hdvFile));

            return carObj;
        }

        private Chassis BuildChassis(string hdvFile)
        {
            float weight = 0;
            float fuelTankSize = 0;
            float dragBody = 0;

            // Create other objects.
            using (var hdvIni = new IniReader(hdvFile, true))
            {
                hdvIni.AddHandler(x =>
                {
                    if (x.Key == "Mass" && x.Group == "GENERAL") weight = x.ReadAsFloat();
                    if (x.Key == "BodyDragBase" && x.Group == "GENERAL") dragBody = x.ReadAsFloat();
                    if (x.Key == "FuelRange" && x.Group == "GENERAL") fuelTankSize = x.ReadAsFloat(2);
                });
                hdvIni.Parse();
            }
            //TODO: Do aero work
            Chassis s = new Chassis(weight, fuelTankSize, dragBody, new Polynomial(0), new Polynomial(0), new Polynomial(0), new Polynomial(0), 0 );
            return s;
        }

        private Engine BuildEngine(string file, string name, string manufacturer)
        {
            var idleRpm = new Range(0,0);
            var maximumRpm= new Range(0,0);
            var engineMode = new List<EngineMode>();
            var engineTorque = new List<EngineTorque>();

            // temporary helpers
            var lifetimeTime = new NormalDistrbution(0, 0);
            var lifetimeRpm = new NormalDistrbution(0, 0);
            var lifetimeOil = new NormalDistrbution(0, 0);
            var lifetimeLength = 0.0f;

            var engineBoostModes = 0.0f;
            var engineBoostScale = new Range(0, 0);
            var engineBoostRpm = 0.0f;
            var engineBoostFuel = 0.0f;
            var engineBoostWear = 0.0f;
            var engineBoostTorque = 0.0f;
            var engineBoostPower = 0.0f;

            using(var engFile = new IniReader(file,true))
            {
                engFile.AddHandler(x =>
                                       {
                                           if (x.Key == "RPMTorque") 
                                               engineTorque.Add(new EngineTorque(x.ReadAsFloat(0), x.ReadAsFloat(1), x.ReadAsFloat(2)));

                                           if (x.Key == "IdleRPMLogic")
                                               idleRpm = new Range(x.ReadAsFloat(0), x.ReadAsFloat(1));

                                           if(x.Key == "RevLimitRange")
                                           {
                                               var minRpm = x.ReadAsFloat(0);
                                               var slopeRpm = x.ReadAsFloat(1);
                                               var rpmSlopeSettings = x.ReadAsInteger(2);
                                               var maxRpm = minRpm + slopeRpm*rpmSlopeSettings;

                                               maximumRpm = new Range(minRpm, maxRpm,0,slopeRpm);
                                           }

                                           if (x.Key == "EngineBoostRange")
                                           {
                                               engineBoostModes = x.ReadAsInteger(2);
                                               engineBoostScale = new Range(x.ReadAsFloat(0), x.ReadAsFloat(1));
                                           }

                                           if (x.Key == "BoostEffects")
                                           {
                                               engineBoostRpm = x.ReadAsFloat(0);
                                               engineBoostFuel = x.ReadAsFloat(1);
                                               engineBoostWear = x.ReadAsFloat(2);
                                           }

                                           if (x.Key == "BoostTorque")
                                               engineBoostTorque = x.ReadAsFloat();
                                           if (x.Key == "BoostPower")
                                               engineBoostPower = x.ReadAsFloat();

                                           if (x.Key == "LifetimeEngineRPM")
                                               lifetimeRpm = new NormalDistrbution(x.ReadAsFloat(0), x.ReadAsFloat(1));
                                           if (x.Key == "LifetimeOilTemp")
                                               lifetimeOil = new NormalDistrbution(x.ReadAsFloat(0), x.ReadAsFloat(1));
                                           if (x.Key == "LifetimeAvg")
                                               lifetimeLength = x.ReadAsFloat();
                                           if (x.Key == "LifetimeVar")
                                               lifetimeTime = new NormalDistrbution(lifetimeLength, x.ReadAsFloat());

                                       });
                engFile.Parse();
            }

            // Combine read values:
            if(engineBoostModes > 0)
            {
                for (var mode = 0; mode < engineBoostModes; mode++)
                {
                    var factor = engineBoostScale.Minimum + mode*engineBoostScale.Span/engineBoostModes;

                    var rpmPlus = factor * engineBoostRpm;
                    var fuelPlus = factor*engineBoostScale.Maximum/engineBoostModes * engineBoostFuel;
                    var wearPlus = factor * engineBoostWear;
                    var powerPlus = factor * engineBoostPower;
                    var torquePlus = factor * engineBoostTorque;

                    engineMode.Add(new EngineMode(mode, powerPlus, torquePlus, rpmPlus, fuelPlus, wearPlus));
                }
            }
            var lifetime = new EngineLifetime(lifetimeTime, lifetimeRpm, lifetimeOil);

            return new Engine(name, manufacturer, 0, 0, idleRpm, maximumRpm, engineMode, engineTorque, lifetime);
        }

        public bool Remove(string Id)
        {
            return false;
        }

        public bool Clear()
        {
            return false;
        }
    }
}