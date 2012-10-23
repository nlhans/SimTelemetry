using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Garage;
using Triton;

namespace SimTelemetry.Game.rFactor2.Garage
{
    public class rFactor2Track : ITrack
    {
        private IniScanner track_gdb;
        private IniScanner track_aiw;

        private MAS2File masfile_gdb;
        private MAS2File masfile_aiw;

        private RouteCollection _Route;

        public bool FoundFiles { get; private set; }

        #region ITrack Properties
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
            get { return masfile_gdb.Filename; }
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
        #endregion

        public rFactor2Track(string trackfile)
        {
            FoundFiles = false;
            try
            {
                trackfile = trackfile.ToLower();
                if (trackfile.EndsWith("gdb"))
                {
                    masfile_gdb = rFactor2.Garage.Files.SearchFile(trackfile);
                    masfile_aiw = rFactor2.Garage.Files.SearchFile(trackfile.Replace("gdb", "aiw"));
                }
                else
                {
                    masfile_aiw = rFactor2.Garage.Files.SearchFile(trackfile);
                    masfile_gdb = rFactor2.Garage.Files.SearchFile(trackfile.Replace("aiw", "gdb"));
                }
                FoundFiles = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error when looking for data files for track " + trackfile);
            }

        }

        private bool ScannedGDB=false;
        private bool ScannedAIW = false;
        public void Scan()
        {
            if (FoundFiles)
            {
                if (!ScannedGDB)
                {
                    ScannedGDB = true;
                    track_gdb = new IniScanner { IniData = masfile_gdb.Master.ExtractString(masfile_gdb) };
                    track_gdb.Read();
                }
            }
        }

        public void ScanRoute()
        {
            if (!ScannedAIW)
            {
                _Route = new RouteCollection();

                track_aiw = new IniScanner {IniData = masfile_aiw.Master.ExtractString(masfile_aiw)};
                track_aiw.HandleCustomKeys += new Signal(Scan_AIWKey);
                track_aiw.FireEventsForKeys = new List<string>();
                track_aiw.FireEventsForKeys.AddRange(new string[6]
                                                         {
                                                             "Main.wp_pos", "Main.wp_score", "Main.wp_branchid",
                                                             "Main.wp_perp", "Main.wp_width", "Main.wp_ptrs"
                                                         });
                track_aiw.Read();
            }
        }

        private TrackWaypoint Waypoint_Temp;

        private void Scan_AIWKey(object d)
        {
            object[] a = (object[]) d;
            string key = (string)a[0];
            List<string> values = (List<string>)a[1];

            switch(key)
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
                    if (values.Count == 1)
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