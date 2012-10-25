using System;
using System.Collections.Generic;
using System.IO;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorTrack : ITrack
    {

        private bool ScannedAIW;
        private bool ScannedGDB;
        private IniScanner track_aiw;
        private IniScanner track_gdb;


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

        private RouteCollection _Route;

        public string File
        {
            get { return _file; }
        }
        public string Version
        {
            get { return ""; }
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

        public RouteCollection Route
        {
            get { return _Route; }
        }

        public string Thumbnail
        {
            get { return "Cache/Tracks/rfactor_" + Path.GetFileNameWithoutExtension(File) + ".png"; }
        }

        public rFactorTrack(string file)
        {
            _file = file;
        }
        public void Scan()
        {
            if (!ScannedGDB)
            {
                track_gdb = new IniScanner {IniFile = _file};
                track_gdb.Read();

                _name = track_gdb.TryGetString("TrackName");
                _location = track_gdb.TryGetString("Location");
                _type = track_gdb.TryGetString("TrackType");
                _imageCache = System.IO.File.Exists("tracks/rfactor_" + File.Replace(".gdb", ".png"));
                    // TODO: Fix a more unique ID globally!

                _length = 0; // Read from AIW.

                /******************* QUALIFY DAY *******************/
                _qualifyDay = track_gdb.TryGetString("QualifyDay");
                Int32.TryParse(track_gdb.TryGetString("QualifyLaps"), out _qualifyLaps);
                Int32.TryParse(track_gdb.TryGetString("QualifyDuration"), out _qualifyMinutes);
                int qualify_start_hr = 0, qualify_start_min = 0;
                string qstart = track_gdb.TryGetString("QualifyStart").Trim();
                if (qstart.Length == 5)
                {
                    Int32.TryParse(qstart.Substring(0, 2), out qualify_start_hr);
                    Int32.TryParse(qstart.Substring(3, 2), out qualify_start_min);
                    _qualifyStart = qualify_start_min*60 + qualify_start_hr*3600;
                }


                /******************* RACE DAY *******************/
                _fullRaceDay = track_gdb.TryGetString("RaceDay");
                Int32.TryParse(track_gdb.TryGetString("RaceLaps"), out _fullRaceLaps);
                Int32.TryParse(track_gdb.TryGetString("RaceDuration"), out _fullRaceMinutes);
                int race_start_hr = 0, race_start_min = 0;
                string rstart = track_gdb.TryGetString("RaceStart").Trim();
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

                Int32.TryParse(track_gdb.TryGetString("NormalPitKPH"), out _pitSpeedPractice);
                Int32.TryParse(track_gdb.TryGetString("RacePitKPH"), out _pitSpeedRace);

                Double.TryParse(track_gdb.TryGetString("Qualify Laptime").Trim(), out _laprecordQualifyTime);
                Double.TryParse(track_gdb.TryGetString("Race Laptime").Trim(), out _laprecordRaceTime);
                // Unfortunately, no driver strings in rFactor for lap records.

            }
        }


        public void ScanRoute()
        {
            if (!ScannedAIW)
            {
                _Route = new RouteCollection();

                track_aiw = new IniScanner { IniFile = File.Replace("gdb","aiw") };
                track_aiw.HandleCustomKeys += new Signal(Scan_AIWKey);
                track_aiw.FireEventsForKeys = new List<string>();
                track_aiw.FireEventsForKeys.AddRange(new string[6]
                                                         {
                                                             "Main.wp_pos", "Main.wp_score", "Main.wp_branchid",
                                                             "Main.wp_perp", "Main.wp_width", "Main.wp_ptrs"
                                                         });
                track_aiw.Read();
                ScannedAIW = true;
            }
        }

        private TrackWaypoint Waypoint_Temp;

        private void Scan_AIWKey(object d)
        {
            object[] a = (object[])d;
            string key = (string)a[0];
            string[] values = (string[])a[1];

            switch (key)
            {
                // pos = coordinates)
                case "Main.wp_pos":

                    // 0=x, 1=0y, 2=z
                    Waypoint_Temp = new TrackWaypoint();

                    Waypoint_Temp.X = Convert.ToDouble(values[0]);
                    Waypoint_Temp.Y = Convert.ToDouble(values[1]);
                    Waypoint_Temp.Z = Convert.ToDouble(values[2]);
                    break;

                // score = sector, distance
                case "Main.wp_score":
                    Waypoint_Temp.Sector = Convert.ToInt32(values[0]);
                    Waypoint_Temp.Meters = Convert.ToDouble(values[1]);
                    break;

                // branchID = path ID, 0=main, 1=pitlane
                case "Main.wp_branchid":
                    if (values.Length == 1)
                    {
                        switch (values[0])
                        {
                            case "0":
                                Waypoint_Temp.Route = TrackRoute.MAIN;
                                break;

                            case "1":
                                Waypoint_Temp.Route = TrackRoute.PITLANE;
                                break;

                            default:
                                Waypoint_Temp.Route = TrackRoute.OTHER;
                                break;
                        }
                    }
                    else
                        Waypoint_Temp.Route = TrackRoute.OTHER;
                    break;

                case "Main.wp_perp":

                    Waypoint_Temp.PerpVector = new double[2] { Convert.ToDouble(values[0]), Convert.ToDouble(values[2]) };
                    break;

                case "Main.wp_width":
                    Waypoint_Temp.CoordinateL = new double[2]
                                                    {
                                                        Waypoint_Temp.X - Waypoint_Temp.PerpVector[0]*Convert.ToDouble(values[0]),
                                                        Waypoint_Temp.Z - Waypoint_Temp.PerpVector[1]*Convert.ToDouble(values[0])
                                                    };
                    Waypoint_Temp.CoordinateR = new double[2]
                                                    {
                                                        Waypoint_Temp.X + Waypoint_Temp.PerpVector[0]*Convert.ToDouble(values[1]),
                                                        Waypoint_Temp.Z + Waypoint_Temp.PerpVector[1]*Convert.ToDouble(values[1])
                                                    };
                    break;

                // ptrs = next path, previous path, pitbox route (-1 for no pitbox), following branchID
                case "Main.wp_ptrs":
                    switch (values[3])
                    {
                        case "0":
                            if (Waypoint_Temp.Route == TrackRoute.MAIN)
                                _Route.Add(Waypoint_Temp);
                            break;
                        case "1":
                            if (Waypoint_Temp.Route == TrackRoute.PITLANE)
                                _Route.Add(Waypoint_Temp);
                            break;
                    }
                    break;

            }
        }
    }
}