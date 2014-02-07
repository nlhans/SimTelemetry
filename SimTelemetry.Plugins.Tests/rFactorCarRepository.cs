using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Utils;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Plugins.Tests
{
    public class rFactorCarRepository : ILazyRepositoryDataSource<Car, string>
    {
        // Search for all vehicles
        string path = @"C:\Program Files (x86)\rFactor\GameData\Vehicles\";

        private IEnumerable<string> hdvFiles;
        private IEnumerable<string> iniFiles;
        private IEnumerable<string> tbcFiles;

        public string SearchHDV(string id)
        {
            id = id.ToLower();
            return hdvFiles.Any(x => x.Contains(id)) ? hdvFiles.FirstOrDefault(x => x.Contains(id)) : string.Empty;
        }

        public string SearchTBC(string id)
        {
            id = id.ToLower();
            return tbcFiles.Any(x => x.Contains(id)) ? tbcFiles.FirstOrDefault(x => x.Contains(id)) : string.Empty;
        }


        public string SearchIni(string id)
        {
            id = id.ToLower();
            return iniFiles.Any(x => x.Contains(id))
                       ? iniFiles.FirstOrDefault(x => x.Contains(id))
                       : string.Empty;
        }

        public IList<string> GetIds()
        {
            // Search also for other files.
            hdvFiles = Directory.GetFiles(path, "*.hdv", SearchOption.AllDirectories).Select(x => x.ToLower());
            iniFiles = Directory.GetFiles(path, "*.ini", SearchOption.AllDirectories).Select(x => x.ToLower());
            tbcFiles = Directory.GetFiles(path, "*.tbc", SearchOption.AllDirectories).Select(x => x.ToLower());

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
            Debug.WriteLine("Car::Get("+id+")");
            string vehFile = path + id;

            // Helpers for other car parts.
            var tbcFile = "";
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
                                           switch(x.Key)
                                           {
                                               case "Team":
                                                   team =  x.ReadAsString();
                                                   break;

                                               case "Driver":
                                                   driver = x.ReadAsString();
                                                   break;

                                               case "Description":
                                                   description = x.ReadAsString();
                                                   break;

                                               case "Number":
                                                   number = x.ReadAsInteger();
                                                   break;

                                               case "Classes":
                                                   classes.AddRange(x.ReadAsString().Split(" ".ToCharArray()));
                                                   break;

                                               case "HDVehicle":
                                                   hdvFile = x.ReadAsString();
                                                   break;

                                               case "Engine":
                                                   engineName = x.ReadAsString();
                                                   break;

                                               case "Manufacturer":
                                                   engineManufacturer = x.ReadAsString();
                                                   break;
                                           }
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
                                          if (x.Key == "TireBrand") tbcFile = x.ReadAsString();
                                          if (x.Key == "Normal" && x.Group == "ENGINE") engFile = x.ReadAsString();
                                      });
                hdvIni.Parse();
            }

            engFile = SearchIni(engFile);
            if (engFile == string.Empty) throw new Exception("could not build this car, engine file not found");

            carObj.Assign(BuildEngine(engFile, engineName, engineManufacturer));
            carObj.Assign(BuildChassis(hdvFile));
            carObj.Assign(BuildTyres(tbcFile));
            carObj.Assign(BuildBrakes(hdvFile));

            return carObj;
        }

        private IEnumerable<Wheel> BuildTyres(string tbcFile)
        {
            List<Wheel> wheels = new List<Wheel>();

            var filePath = SearchTBC(tbcFile);

            if (string.IsNullOrEmpty(filePath)) return wheels;

            using(IniReader tbcReader = new IniReader(filePath, true))
            {
                bool frontIsDone = false;
                bool rearIsDone = false;
                bool tyreWaiting = false;
                var currentCompound = "";
                var radius = 0.0f;
                var rollResistance = 0.0f;

                var peakTemp = 0.0f;
                var peakPressure = 0.0f;
                var peakPressureWeight = 0.0f;
                var pitsTemp = 0.0f;

                Action addWheel = () =>
                                   {
                                       if (tyreWaiting && (!frontIsDone || !rearIsDone))
                                       {
                                           WheelLocation locationA;
                                           WheelLocation locationB;
                                           if (frontIsDone)
                                           {
                                               locationA = WheelLocation.REARLEFT;
                                               locationB = WheelLocation.REARRIGHT;
                                               rearIsDone = true;
                                           }else
                                           {
                                               locationA = WheelLocation.FRONTLEFT;
                                               locationB = WheelLocation.FRONTRIGHT;
                                               frontIsDone = true;
                                           }
                                           Wheel wA = new Wheel(locationA, radius * 2, rollResistance, pitsTemp, peakTemp,
                                                               peakPressure, peakPressureWeight);

                                           Wheel wB = new Wheel(locationB, radius * 2, rollResistance, pitsTemp, peakTemp,
                                                               peakPressure, peakPressureWeight);

                                           wheels.Add(wA);
                                           wheels.Add(wB);


                                       }
                                       tyreWaiting = false;
                                   };

                tbcReader.AddHandler(x =>
                                         {
                                             if (x.Group.ToLower() != "compound") return;
                                             switch(x.Key)
                                             {
                                                 case "Name":
                                                     currentCompound = x.ReadAsString();
                                                     break;
                                                 case "DryLatLong":
                                                     if (tyreWaiting) addWheel();
                                                     tyreWaiting = true;
                                                     break;

                                                 case "Radius":
                                                     radius = x.ReadAsFloat();
                                                     break;

                                                 case "RollingResistance":
                                                     rollResistance = x.ReadAsFloat();
                                                     break;

                                                 case "Temperatures":
                                                     peakTemp = x.ReadAsFloat(0);
                                                     if (x.ValueCount == 2)
                                                     {
                                                         pitsTemp = x.ReadAsFloat(1);
                                                     }
                                                     break;

                                                 case "OptimumPressure":
                                                     peakPressure = x.ReadAsFloat(0);
                                                     peakPressureWeight = x.ReadAsFloat(1);
                                                     break;
                                             }
                                         });

                tbcReader.Parse();
                addWheel();
            }

            return wheels;
        }

        private IEnumerable<Brake> BuildBrakes(string hdvFile)
        {
            List<Brake> brakes = new List<Brake>();

            using (IniReader hdvReader = new IniReader(hdvFile, true))
            {
                bool wheelDone = false;

                WheelLocation location = WheelLocation.UNKNOWN;

                var optimumTemperature = new Range(0,1000);
                var thicknessStart = new Range(1, 2);
                var thicknessFailure = 0.5f;
                var torque = 10000.0f;

                Action addBrake = () =>
                                      {
                                          if(wheelDone)
                                          {
                                              var br = new Brake(location, optimumTemperature, thicknessStart,
                                                                 thicknessFailure, torque);
                                              brakes.Add(br);

                                              wheelDone = false;
                                          }
                                      };

                hdvReader.AddHandler((x) =>
                                         {
                                             switch(x.Key)
                                             {
                                                 case "SpringRange":
                                                     if (wheelDone) addBrake();
                                                     wheelDone = true;

                                                     switch(x.Group.ToLower())
                                                     {
                                                         case "frontleft":
                                                             location = WheelLocation.FRONTLEFT;
                                                             break;
                                                         case "frontright":
                                                             location = WheelLocation.FRONTRIGHT;
                                                             break;
                                                         case "rearleft":
                                                             location = WheelLocation.REARLEFT;
                                                             break;
                                                         case "rearright":
                                                             location = WheelLocation.REARRIGHT;
                                                             break;
                                                     }
                                                     break;

                                                 case "BrakeResponseCurve":
                                                     var minT = x.ReadAsFloat(1);
                                                     var maxT = x.ReadAsFloat(2);
                                                     var opt = (maxT + minT)/2;
                                                     optimumTemperature = new Range(minT, maxT, opt);
                                                     break;

                                                 case "BrakeFailure":
                                                     var avg = x.ReadAsFloat(0);
                                                     var stdDev = x.ReadAsFloat(1);

                                                     thicknessFailure = avg;
                                                     break;

                                                 case "BrakeTorque":
                                                     torque = x.ReadAsFloat(0);
                                                     break;

                                                 case "BrakeDiscRange":

                                                     var min = x.ReadAsFloat(0);
                                                     var steps = x.ReadAsInteger(2);
                                                     var stepSize = x.ReadAsFloat(1);
                                                     var max = min + steps * stepSize;

                                                     thicknessStart = new Range(min, max);
                                                     break;
                                             }
                                         });

                hdvReader.Parse();

                addBrake();
            }

            return brakes;
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
            var s = new Chassis(weight, fuelTankSize, dragBody, new Polynomial(0), new Polynomial(0), new Polynomial(0), new Polynomial(0), 0 );
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

            return new Engine(name, manufacturer, 0, 0, idleRpm, maximumRpm, engineMode, engineTorque, lifetime, lifetimeOil.Optimum, lifetimeOil.Optimum);
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