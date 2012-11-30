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
using System.Linq;
using SimTelemetry.Objects.Garage;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;
using Timer = System.Timers.Timer;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data.Track
{
    public class Track : ITrackParser
    {
        public List<Lap> TrackLogger = new List<Lap>(); // logger data with laptimes.

        private Timer LapLogger;

        public event AnonymousSignal DriverLap;
        public event AnonymousSignal PlayerLap;

        #region Track information
        public RouteCollection Route { get; set; }
        public SectionsCollection Sections { get; set; }
        public ApexCollection Apexes { get; set; }

        public string Location { get; protected set; }
        public double Length { get { return Route.Length; } }
        public string Type { get; protected set; }
        public string Name { get; protected set; }
        #endregion

        public Track(string name)
        {
            if (name == null) return;
            Route = new RouteCollection();
            Apexes = new ApexCollection();
            Sections = new SectionsCollection();

            if (Telemetry.m.Sim.Garage != null && Telemetry.m.Sim.Garage.Available
                 && Telemetry.m.Sim.Garage.Available_Tracks)
            {
                ITrack track = Telemetry.m.Sim.Garage.SearchTrack(name);
                if (track != null)
                {
                    track.Scan();
                    track.ScanRoute();
                    Location = track.Location;
                    Type = track.Type;
                    Name = track.Name;

                    Route = (RouteCollection) track.Route.Clone();
                }
            }
            LapLogger = new Timer();
            LapLogger.Interval = 2;
            LapLogger.Elapsed += LapLogger_Elapsed;
            LapLogger.AutoReset = true;
            LapLogger.Start();
        }

        private double PreviousTime;
        public void LapLogger_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Telemetry.m.Active_Session && Telemetry.m.Sim.Modules.Time_Available)
            {
                try
                {
                    double dt = Telemetry.m.Sim.Session.Time - PreviousTime;
                    PreviousTime = Telemetry.m.Sim.Session.Time;
                    lock (Telemetry.m.Sim.Drivers.AllDrivers)
                    {
                        foreach (IDriverGeneral driver in Telemetry.m.Sim.Drivers.AllDrivers)
                        {
                            if (driver != null)
                            {
                                // Look up the lap.
                                int lap = driver.Laps;
                                int drv = driver.MemoryBlock;
                                Lap l = GetLap(driver, 0);

                                l.Distance += driver.Speed*dt;
                                // Create lap if it doesnt exist
                                if (l.LapNo == -1)
                                {
                                    l = new Lap
                                            {
                                                ApexSpeeds = new List<double>(),
                                                Checkpoints = new List<double>(),
                                                DriverNo = drv,
                                                LapNo = lap,
                                                Distance = 0,
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
                                    if (l.ApexSpeeds[i] == -1 && l.PrevMeters < apex.Key &&
                                        driver.MetersDriven >= apex.Key && apex.Key > (driver.MetersDriven - 100))
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
                                    if (l.Checkpoints[i] == -1 && l.PrevMeters <= checkpoint.Key &&
                                        driver.MetersDriven >= checkpoint.Key &&
                                        checkpoint.Key > (driver.MetersDriven - 100))
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
                }
                catch (Exception ex)
                {

                }
            }
        }

        public Lap GetLap(IDriverGeneral driver, int offset)
        {
            int lap = driver.Laps - offset;
            int drv = driver.MemoryBlock;
            List<Lap> l = TrackLogger.Where(lm => lm.LapNo == lap && lm.DriverNo == drv).ToList();

            if (l.Count == 0) return new Lap { LapNo = -1 };
            else return l[0];
        }
        private void SetLap(IDriverGeneral driver, Lap l)
        {
            int i = TrackLogger.FindIndex(lm => l.LapNo == lm.LapNo && l.DriverNo == lm.DriverNo);

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
