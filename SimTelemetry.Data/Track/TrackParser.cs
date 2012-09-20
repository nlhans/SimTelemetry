using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Triton.Maths;
using ElapsedEventHandler = System.Timers.ElapsedEventHandler;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using Timer = System.Timers.Timer;
using SimTelemetry.Objects;
using System.Globalization;
using Triton;

namespace SimTelemetry.Data.Track
{
    public class TrackParser : ITrackParser
    {
        public List<Lap> TrackLogger = new List<Lap>(); // logger data with laptimes.
        
        private double _Length;
        public double Length { get { return _Length; } }
        public SectionsCollection Sections { get; set; }
        public RouteCollection Route { get; set; }
        public ApexCollection Apexes { get; set; }

        private Timer LapLogger;

        public event AnonymousSignal DriverLap;
        public event AnonymousSignal PlayerLap;


        public string Location { get; protected set; }
        public string LengthStr { get; protected set; }
        public string Type { get; protected set; }
        public string Name { get; protected set; }


        public TrackParser(string path, string name)
        {
            Route = new RouteCollection();
            Apexes = new ApexCollection();
            Sections = new SectionsCollection();

            // Look for track files.
            if(name.ToLower().EndsWith(".aiw"))
            {
                // rFactor, GTR2, etc.
                Parse_SimBin_AIW(Path.Combine(path, name));
                Parse_SimBin_GDB(Path.Combine(path, name.ToLower().Replace(".aiw", ".gdb")));
            }
            LapLogger = new Timer();
            LapLogger.Interval = 2;
            LapLogger.Elapsed += new ElapsedEventHandler(LapLogger_Elapsed);
            LapLogger.AutoReset = true;
            LapLogger.Start();
        }

        private double PreviousTime;
        public void LapLogger_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Telemetry.m.Active_Session)
            {
                try
                {
                    double dt = Telemetry.m.Sim.Session.Time - PreviousTime;
                    PreviousTime = Telemetry.m.Sim.Session.Time;
                    lock (Telemetry.m.Sim.Drivers.AllDrivers)
                    {
                        foreach (IDriverGeneral driver in Telemetry.m.Sim.Drivers.AllDrivers)
                        {
                            // Look up the lap.
                            int lap = driver.Laps;
                            int drv = driver.MemoryBlock;
                            Lap l = GetLap(driver, 0);

                            l.Distance += driver.Speed * dt;
                            // Create lap if it doesnt exist
                            if (l.LapNo == -1)
                            {
                                l = new Lap
                                        {
                                            ApexSpeeds = new List<double>(),
                                            Checkpoints = new List<double>(),
                                            DriverNo = drv,
                                            LapNo = lap,
                                            Distance=0,
                                            MinTime = Telemetry.m.Sim.Session.Time
                                        };
                                int k = 0;
                                for (k = 0; k < Sections.Lines.Count; k++)
                                    l.Checkpoints.Add(-1);
                                for (k = 0; k < Apexes.Positions.Count; k++)
                                    l.ApexSpeeds.Add(-1);

                                //TrackLogger.Add(l);

                                if (drv == Telemetry.m.Sim.Drivers.Player.MemoryBlock && PlayerLap != null)
                                    PlayerLap();
                                else if (DriverLap != null)
                                    DriverLap();

                                Lap lastLap = GetLap(driver, 1);
                                if (lastLap.LapNo != -1)
                                {
                                    l.Sector1 = driver.Sector_1_Last;
                                    l.Sector2 = driver.Sector_2_Last;
                                    l.Sector3 = driver.Sector_3_Last;
                                    l.MaxTime = Telemetry.m.Sim.Session.Time;
                                    l.Total = l.Sector3 + l.Sector2 + l.Sector1;
                                }
                                SetLap(driver, l);
                                continue;
                            }

                            // Apex speeds
                            int i = 0;
                            foreach (KeyValuePair<double, string> apex in Apexes.Positions)
                            {
                                if (l.ApexSpeeds[i] == -1 && l.PrevMeters < apex.Key && driver.MetersDriven >= apex.Key && apex.Key > (driver.MetersDriven - 100))
                                {
                                    l.ApexSpeeds[i] = driver.Speed;
                                    break;
                                }

                                i++;
                            }

                            // Sections
                            i = 0;
                            foreach (KeyValuePair<double, string> checkpoint in Sections.Lines)
                            {
                                if (l.Checkpoints[i] == -1 && l.PrevMeters <= checkpoint.Key && driver.MetersDriven >= checkpoint.Key && checkpoint.Key > (driver.MetersDriven - 100))
                                {
                                    l.Checkpoints[i] = Telemetry.m.Sim.Session.Time;
                                    break;
                                }

                                i++;
                            }
                            l.PrevMeters = driver.MetersDriven;
                            l.MaxTime = Telemetry.m.Sim.Session.Time;
                            /*int ind = TrackLogger.FindIndex(delegate(Lap lm) { return lm.LapNo == lap && lm.DriverNo == drv; });
                            TrackLogger[ind] = l;*/
                            SetLap(driver, l);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void Parse_SimBin_AIW(string path)
        {
            string[] data = new string[0];
            try
            {
                data = File.ReadAllLines(path);

            }
            catch (Exception)
            {
                // File not found?
                Debug.WriteLine("[Track] Could not find AIW file");
                return;
            }

            TrackWaypoint temp = new TrackWaypoint();
            _Length = 0;
            // Old test filter
            //IIR WidthFilter = new IIR(new double[3] { 0.3, 0.25, 0.15}, new double[2] {0.2, 0.1});
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); 
            foreach(string line in data)
            {
                if(line.Contains("=") && line.Contains("(") && line.Contains(")"))
                {
                    // TODO: remove pit garages and start spots.
                    string key = line.Substring(0, line.IndexOf("=")).ToLower();
                    string[] values =
                        line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - line.IndexOf("(") - 1).Split(
                            ",".ToCharArray());

                    if (key.StartsWith("wp_"))
                        key = key.Substring(3);

                    // Parse:
                    // pos = coordinates)
                    if (key == "pos" && values.Length == 3)
                    {
                        temp = new TrackWaypoint();
                        temp.X = Convert.ToDouble(values[0]);
                        temp.Y = Convert.ToDouble(values[1]);
                        temp.Z = Convert.ToDouble(values[2]);
                    }

                    // score = sector, distance
                    if (key == "score" && values.Length == 2)
                    {
                        temp.Sector = Convert.ToInt32(values[0]);
                        temp.Meters = Convert.ToDouble(values[1]);
                    }

                    // branchID = path ID, 0=main, 1=pitlane
                    if (key == "branchid")
                    {
                        if (values.Length == 1)
                        {
                            switch (values[0])
                            {
                                case "0":
                                    temp.Route = TrackRoute.MAIN;
                                    break;

                                case "1":
                                    temp.Route = TrackRoute.PITLANE;
                                    break;

                                default:
                                    temp.Route = TrackRoute.OTHER;
                                    break;
                            }
                        }
                        else
                            temp.Route = TrackRoute.OTHER;
                }

                if(key == "perp" && values.Length == 3)
                    {
                        temp.PerpVector = new double[2] { Convert.ToDouble(values[0]), Convert.ToDouble(values[2]) };
                    }

                    if(key == "width" && values.Length == 4)
                    {
                        temp.CoordinateL = new double[2]
                                               {
                                                   temp.X - temp.PerpVector[0]*Convert.ToDouble(values[0]),
                                                   temp.Z - temp.PerpVector[1]*Convert.ToDouble(values[0])
                                               };
                        temp.CoordinateR = new double[2]
                                               {
                                                   temp.X + temp.PerpVector[0]*Convert.ToDouble(values[1]),
                                                   temp.Z + temp.PerpVector[1]*Convert.ToDouble(values[1])
                                               };
                    }

                    // ptrs = next path, previous path, pitbox route (-1 for no pitbox), following branchID
                    if(key == "ptrs" && values.Length == 4 )
                    {
                        bool add = false;
                        switch(values[3])
                        {
                            case "0":
                                if (temp.Route == TrackRoute.MAIN)
                                    add = true;
                                break;
                            case "1":
                                if (temp.Route == TrackRoute.PITLANE)
                                    add = true;
                                break;
                        }
                        if(add)
                        {
                            _Length = Math.Max(_Length, temp.Meters);
                            this.Route.Add(temp);
                        }

                    }

                }
            }

            Route.Finalize();
        }

        private void Parse_SimBin_GDB(string replace)
        {
            try
            {
                string[] data = File.ReadLines(replace).ToArray();

                foreach (string data_line in data)
                {
                    string[] spl = data_line.Split("=".ToCharArray());

                    if (data_line.Contains("TrackName"))
                        Name = spl[1].Trim();


                    if (data_line.Contains("Location"))
                        Location = spl[1].Trim();


                    if (data_line.Contains("Length"))
                        LengthStr = spl[1].Trim();


                    if (data_line.Contains("TrackType"))
                        Type = spl[1].Trim();

                }
            }
            catch (Exception)
            { }
        }

        public Lap GetLap(IDriverGeneral driver, int offset)
        {
            int lap = driver.Laps - offset;
            int drv = driver.MemoryBlock;
            List<Lap> l = TrackLogger.FindAll(delegate(Lap lm) { return lm.LapNo == lap && lm.DriverNo == drv; });

            if (l.Count == 0) return new Lap { LapNo = -1 };
            else return l[0];
        }
        private void SetLap(IDriverGeneral driver, Lap l)
        {
            int i = TrackLogger.FindIndex(delegate(Lap lm) { return l.LapNo == lm.LapNo && l.DriverNo == lm.DriverNo; });

            if (i < 0) TrackLogger.Add(l);
            else TrackLogger[i] = l;
        }


        public Lap GetCurrentLap()
        {
            return GetLap(Telemetry.m.Sim.Drivers.Player, 0);
        }

        public Lap GetLastLap()
        {
            return GetLap(Telemetry.m.Sim.Drivers.Player, 1);
        }
    }
}
