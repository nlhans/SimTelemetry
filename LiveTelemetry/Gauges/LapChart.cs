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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveTelemetry.Gauges;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.ValueObjects;

namespace LiveTelemetry
{
    public partial class LapChart : UserControl
    {
        private Dictionary<string, int> DisplayTimers = new Dictionary<string, int>();
        private Dictionary<string, int> LastPitLap = new Dictionary<string, int>();

        private SessionType LastSession = SessionType.TEST_DAY;
        public LapChart()
        {
            InitializeComponent();

            this.Size = new Size(460, 500);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

        }

        private int sortDriver(TelemetryDriver d1, TelemetryDriver d2)
        {
            if (d1 == null || d2 == null) return 0;
            if (d1.Position == d2.Position) return 0;
            if (d1.Position > 120 || d1.Position == 0) return 1;
            if (d2.Position > 120 || d2.Position == 0) return -1;
            if (d1.Position > d2.Position) return 1;
            else return -1;
        }

        private string PrintLapTime(float time, bool sector)
        {
            if(time < 60)
            {
                if (sector) return time.ToString("00.000");
                else
                {
                    return "0:" + time.ToString("00.000");
                }
            }
            else
            {
                int minutes = Convert.ToInt32(Math.Floor(Convert.ToDouble(time/60f)));

                return minutes + ":" + (time%60f).ToString("00.000");
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.Size = new Size(460, this.Size.Height);
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (TelemetryApplication.TelemetryAvailable ==false) return;

            // Draw header
            var f = new Font("Arial", 8, FontStyle.Regular);

            try
            {
                switch (TelemetryApplication.Telemetry.Session.Info.Type)
                {
                    default:
                        g.DrawString("P", f, Brushes.DarkGray, 10f, 10f);
                        g.DrawString("Name", f, Brushes.DarkGray, 38f, 10f);
                        g.DrawString("Lap", f, Brushes.DarkGray, 140f, 10f);
                        g.DrawString("Interval", f, Brushes.DarkGray, 195f, 10f);
                        g.DrawString("S1", f, Brushes.DarkGray, 245f, 10f);
                        g.DrawString("S2", f, Brushes.DarkGray, 295f, 10f);
                        g.DrawString("S3", f, Brushes.DarkGray, 345f, 10f);
                        g.DrawString("Laps", f, Brushes.DarkGray, 395f, 10f);

                        break;
                    case SessionType.RACE:
                        g.DrawString("P", f, Brushes.DarkGray, 10f, 10f);
                        g.DrawString("Name", f, Brushes.DarkGray, 38f, 10f);
                        g.DrawString("Gap", f, Brushes.DarkGray, 140f, 10f);
                        g.DrawString("Interval", f, Brushes.DarkGray, 165f, 10f);
                        g.DrawString("Lap", f, Brushes.DarkGray, 222f, 10f);
                        g.DrawString("S1", f, Brushes.DarkGray, 245f, 10f);
                        g.DrawString("S2", f, Brushes.DarkGray, 295f, 10f);
                        g.DrawString("S3", f, Brushes.DarkGray, 345f, 10f);
                        g.DrawString("Pit", f, Brushes.DarkGray, 395f, 10f);

                        break;
                }
                int ind = 1;
                float LineHeight = 15f;

                var player = TelemetryApplication.Telemetry.Player;
                if(player == null)
                {
                    g.DrawString("Could not find player memory object", f, Brushes.White, 10f, 30f);
                    return;
                }
                var playerLaps = player.GetLaps();
                var allLaps = TelemetryApplication.Telemetry.Drivers.SelectMany(x => x.GetLaps());
                var drivers = TelemetryApplication.Telemetry.Drivers.OrderBy(x => x.Position);

                // TODO: Fix when first lap is driven, this skips all
                int validLaps = allLaps.Where(x => x.Total > 0).Count();
                if (validLaps == 0)
                    return;

                double lapTimeBest = allLaps.Where(x => x.Total > 0).Min(x => x.Total);

                float BestSector1 = allLaps.Where(x => x.Sector1 > 0).Min(x => x.Sector1);
                float BestSector2 = allLaps.Where(x => x.Sector2 > 0).Min(x => x.Sector2);
                float BestSector3 = allLaps.Where(x => x.Sector3 > 0).Min(x => x.Sector3);

                float MyBestSector1 = playerLaps.Where(x => x.Sector1 > 0).Min(x => x.Sector1);
                float MyBestSector2 = playerLaps.Where(x => x.Sector2 > 0).Min(x => x.Sector2);
                float MyBestSector3 = playerLaps.Where(x => x.Sector3 > 0).Min(x => x.Sector3);

                int BestSector1Position = 0;
                int BestSector2Position = 0;
                int BestSector3Position = 0;

                float FastestLap = 1000f;
                string FastestDriver = "";
                int DriverCount = 0;

                // Calculate the line height
                if (DriverCount < 16)
                    LineHeight = 20f;
                else
                    LineHeight = Math.Max(10, Convert.ToSingle((this.Size.Height - 20)/(4.0 + DriverCount)));


                // Go through all drivers
                if (drivers.Any())
                {
                    double Previous_SplitLeader = 0;
                    foreach (TelemetryDriver driver in drivers)
                    {
                        var hisLaps = driver.GetLaps();
                        if (!hisLaps.Any()) continue;
                        var bestSector1 = hisLaps.Where(x=>x.Sector1>0).Min(x => x.Sector1);
                        var bestSector2 = hisLaps.Where(x => x.Sector2 > 0).Min(x => x.Sector2);
                        var bestSector3 = hisLaps.Where(x => x.Sector3 > 0).Min(x => x.Sector3);

                        var bestLapObj = hisLaps.Where(x => x.Total > 0 && x.Sector1 > 0 && x.Sector2 > 0 && x.Sector3 > 0 ).MinBy(x => x.Total);
                        var bestLap = bestLapObj.Total;
                        var bestSector1HotLap = bestLapObj.Sector1;
                        var bestSector2HotLap = bestLapObj.Sector2;
                        var bestSector3HotLap = bestLapObj.Sector3;

                        var lastlap = hisLaps.LastOrDefault(x => x.Total>0).Total;
                        var lastLap = lastlap;

                        var lastSector1 = hisLaps.LastOrDefault(x => x.Sector1>0).Sector1; // TODO: Figure out which lap the last sector was at
                        var lastSector2 = hisLaps.LastOrDefault(x => x.Sector2 > 0).Sector2;
                        var lastSector3 = hisLaps.LastOrDefault(x => x.Sector3 > 0).Sector3;

                        if (ind*LineHeight > Size.Height - 20 - 3*20)
                            break;

                        var OntrackBrush = ((!driver.IsPits && driver.Speed > 5) ? Brushes.White : Brushes.Red);
                        g.DrawString(driver.Position.ToString(), f, Brushes.White, 10f, 10f + ind*LineHeight);

                        var name = driver.Name.ToUpper().Split(" ".ToCharArray());
                        if (name.Length == 1)
                            g.DrawString(name[0], f, OntrackBrush, 38f, 10f + ind*LineHeight);
                        else if (name.Length > 1)
                            g.DrawString(name[0].Substring(0, 1) + ". " + name[name.Length - 1], f, OntrackBrush, 38f,
                                         10f + ind*LineHeight);

                        if (!LastPitLap.ContainsKey(driver.Name))
                            LastPitLap.Add(driver.Name, -1);
                        
                        if (driver.IsPits)
                        {
                            LastPitLap[driver.Name] = driver.Laps;
                        }

                        if (TelemetryApplication.Telemetry.Session.Info.Type == SessionType.RACE)
                        {
                            if (lastlap < lapTimeBest && lastlap != -1f)
                                lapTimeBest = lastlap;
                            if (lastlap != -1f)
                                g.DrawString(PrintLapTime(lastlap, false), f, Brushes.White, 140f,
                                             10f + ind*LineHeight);

                            if (driver.Position == 1)
                                g.DrawString("LAP " + drivers.ElementAt(0).Laps, f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                            else
                            {
                                double split_leader = driver.GetSplitTime(drivers.ElementAt(0));
                                if (split_leader >= 10000)
                                {
                                    int laps = Convert.ToInt32(split_leader/10000);
                                    if (laps < 100)
                                        g.DrawString("+" + laps + "L", f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                                    else
                                        g.DrawString("???", f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                                }
                                else
                                    g.DrawString(Math.Round(split_leader, 1).ToString(), f, Brushes.White, 190f,
                                                 10f + ind*LineHeight);

                                double split_next = split_leader - Previous_SplitLeader;
                                Previous_SplitLeader = split_leader;
                                if (split_next < 1000 && split_next > 0)
                                    g.DrawString(Math.Round(split_next, 1).ToString(), f, Brushes.White, 220f,
                                                 10f + ind*LineHeight);
                                else if (split_next > 0)
                                {
                                    int laps = Convert.ToInt32(split_next/10000);
                                    g.DrawString("+" + laps + "L", f, Brushes.Yellow, 220f, 10f + ind*LineHeight);
                                }

                            }


                            if (driver.IsPits && bestSector1 > 0)
                            {
                                Brush Sector1Brush = Brushes.Yellow;
                                Brush Sector2Brush = Brushes.Yellow;
                                Brush Sector3Brush = Brushes.Yellow;
                                if (bestSector1 <= BestSector1) Sector1Brush = Brushes.Magenta;
                                if (bestSector2 <= BestSector2) Sector2Brush = Brushes.Magenta;
                                if (bestSector3 <= BestSector3) Sector3Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(bestSector1, true), f, Sector1Brush, 245f,
                                             10f + ind*LineHeight);
                                g.DrawString(PrintLapTime(bestSector2, true), f, Sector2Brush, 295f,
                                             10f + ind*LineHeight);
                                g.DrawString(PrintLapTime(bestSector3, true), f, Sector3Brush, 345f,
                                             10f + ind*LineHeight);
                            }
                            else if (lastSector1 > 0)
                            {
                                bool DisplayAllSectors = false;

                                if (!DisplayTimers.ContainsKey(driver.Name))
                                    DisplayTimers.Add(driver.Name, -1);
                                if (driver.TrackPosition == TrackPointType.SECTOR1)
                                    DisplayTimers[driver.Name]--;
                                if (driver.TrackPosition == TrackPointType.SECTOR2)
                                    DisplayTimers[driver.Name] = 10;
                                if (DisplayTimers[driver.Name] > 0 && driver.TrackPosition == TrackPointType.SECTOR1)
                                    DisplayAllSectors = true;

                                if (driver.TrackPosition == TrackPointType.SECTOR2 ||
                                    driver.TrackPosition == TrackPointType.SECTOR3 || DisplayAllSectors)
                                {
                                    Brush Sector1Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR2) Sector1Brush = Brushes.White;
                                    if (lastSector1 == bestSector1)
                                        Sector1Brush = Brushes.Green;
                                    if (lastSector1 <= BestSector1) Sector1Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector1, true), f, Sector1Brush, 245f,
                                                 10f + ind*LineHeight);
                                }
                                if (driver.TrackPosition == TrackPointType.SECTOR3 || DisplayAllSectors)
                                {

                                    Brush Sector2Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR3) Sector2Brush = Brushes.White;
                                    if (lastSector2 == bestSector2)
                                        Sector2Brush = Brushes.Green;
                                    if (lastSector2 <= BestSector2) Sector2Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector2, true), f, Sector2Brush, 295f,
                                                 10f + ind*LineHeight);
                                }
                                if (DisplayAllSectors && lastSector3 > 0)
                                    //if (data[driverIndex].CurrentSector == 0)
                                {
                                    Brush Sector3Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR1) Sector3Brush = Brushes.White;
                                    if (lastSector3 == bestSector3)
                                        Sector3Brush = Brushes.Green;
                                    if (lastSector3 <= BestSector3) Sector3Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector3, true), f, Sector3Brush, 345f,
                                                 10f + ind*LineHeight);
                                }
                                if (!driver.IsPits && driver.Speed < 5)
                                {
                                    if (driver.TrackPosition == TrackPointType.SECTOR1)
                                        g.DrawString("[STOP]", f, Brushes.Red, 245f, 10f + ind*LineHeight);
                                    if (driver.TrackPosition == TrackPointType.SECTOR2)
                                        g.DrawString("[STOP]", f, Brushes.Red, 295f, 10f + ind*LineHeight);
                                    if (driver.TrackPosition == TrackPointType.SECTOR3)
                                        g.DrawString("[STOP]", f, Brushes.Red, 345f, 10f + ind*LineHeight);

                                }
                            }
                            else if (driver.Laps > 0 || (driver.Laps == 0 && driver.IsPits == false))
                                g.DrawString("OUT", f, Brushes.Red, 265f, 10f + ind*LineHeight);

                            g.DrawString(driver.Pitstops.ToString(), f, Brushes.White, 395f, 10f + ind*LineHeight);
                        }
                        else
                        {
                            if (driver.IsPits && bestSector1HotLap > 0)
                            {

                                if (bestLap< lapTimeBest && bestLap!= -1f)
                                    lapTimeBest = bestLap;
                                if (bestLap!= -1f)
                                    g.DrawString(PrintLapTime(bestLap, false), f, Brushes.Yellow, 140f,
                                                 10f + ind*LineHeight);
                                if (bestLap!= lapTimeBest && bestLap!= -1f)
                                {
                                    double diff = bestLap- lapTimeBest;
                                    g.DrawString(diff.ToString("0.000"), f, Brushes.Yellow, 195f, 10f + ind*LineHeight);
                                }

                                Brush Sector1Brush = Brushes.Yellow;
                                Brush Sector2Brush = Brushes.Yellow;
                                Brush Sector3Brush = Brushes.Yellow;
                                if (bestSector1HotLap <= BestSector1) Sector1Brush = Brushes.Magenta;
                                if (bestSector2HotLap <= BestSector2) Sector2Brush = Brushes.Magenta;
                                if (bestSector3HotLap <= BestSector3) Sector3Brush = Brushes.Magenta;
                                g.DrawString(PrintLapTime(bestSector1HotLap, true), f, Sector1Brush, 245f,
                                             10f + ind*LineHeight);
                                g.DrawString(PrintLapTime(bestSector2HotLap, true), f, Sector2Brush, 295f,
                                             10f + ind*LineHeight);
                                g.DrawString(PrintLapTime(bestSector3HotLap, true), f, Sector3Brush, 345f,
                                             10f + ind*LineHeight);
                            }
                            else
                            {
                                bool DisplayAllSectors = false;
                                if (!DisplayTimers.ContainsKey(driver.Name))
                                    DisplayTimers.Add(driver.Name, -1);
                                if (driver.TrackPosition == TrackPointType.SECTOR1)
                                    DisplayTimers[driver.Name]--;
                                if (driver.TrackPosition == TrackPointType.SECTOR2)
                                    DisplayTimers[driver.Name] = 10;
                                if (DisplayTimers[driver.Name] > 0 && driver.TrackPosition == TrackPointType.SECTOR1)
                                    DisplayAllSectors = true;
                                //if (data[driverIndex].CurrentSector >= 2 || data[driverIndex].CurrentSector == 0)

                                if ((LastPitLap[driver.Name] == driver.Laps || lastSector1 < 0) && !driver.IsPits)
                                    g.DrawString("OUT", f, Brushes.Red, 245f, 10f + ind*LineHeight);
                                else if (driver.TrackPosition == TrackPointType.SECTOR2 ||
                                         driver.TrackPosition == TrackPointType.SECTOR3 || DisplayAllSectors)
                                {
                                    Brush Sector1Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR2) Sector1Brush = Brushes.White;
                                    if (lastSector1 == bestSector1)
                                        Sector1Brush = Brushes.Green;
                                    if (lastSector1 <= BestSector1) Sector1Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector1, true), f, Sector1Brush, 245f,
                                                 10f + ind*LineHeight);
                                }

                                if (driver.TrackPosition == TrackPointType.SECTOR3 || DisplayAllSectors)
                                    //if (data[driverIndex].CurrentSector == 3 || data[driverIndex].CurrentSector == 0)
                                {

                                    Brush Sector2Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR3) Sector2Brush = Brushes.White;
                                    if (lastSector2 == bestSector2)
                                        Sector2Brush = Brushes.Green;
                                    if (lastSector2 <= BestSector2) Sector2Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector2, true), f, Sector2Brush, 295f,
                                                 10f + ind*LineHeight);
                                }
                                if (DisplayAllSectors && lastSector3 > 0)
                                    //if (data[driverIndex].CurrentSector == 0)
                                {
                                    Brush Sector3Brush = Brushes.Yellow;
                                    if (driver.TrackPosition == TrackPointType.SECTOR1) Sector3Brush = Brushes.White;
                                    if (lastSector3 == bestSector3)
                                        Sector3Brush = Brushes.Green;
                                    if (lastSector3 <= BestSector3) Sector3Brush = Brushes.Magenta;
                                    g.DrawString(PrintLapTime(lastSector3, true), f, Sector3Brush, 345f,
                                                 10f + ind*LineHeight);



                                    if (lastLap < lapTimeBest && lastLap != -1f)
                                        lapTimeBest = bestLap;
                                    if (lastLap != -1f)
                                        g.DrawString(PrintLapTime(lastLap, false), f, Brushes.White, 140f,
                                                     10f + ind*LineHeight);
                                    if (lastLap != lapTimeBest && lastLap != -1f)
                                    {
                                        double diff = lastLap - lapTimeBest;
                                        g.DrawString(diff.ToString("0.000"), f, Brushes.White, 195f,
                                                     10f + ind*LineHeight);
                                    }
                                }
                                else
                                {


                                    if (bestLap< lapTimeBest && bestLap!= -1f)
                                        lapTimeBest = bestLap;
                                    if (bestLap!= -1f)
                                        g.DrawString(PrintLapTime(bestLap, false), f, Brushes.Yellow, 140f,
                                                     10f + ind*LineHeight);
                                    if (bestLap!= lapTimeBest && bestLap!= -1f)
                                    {
                                        double diff = bestLap- lapTimeBest;
                                        g.DrawString(diff.ToString("0.000"), f, Brushes.Yellow, 195f,
                                                     10f + ind*LineHeight);
                                    }
                                }
                                if (!driver.IsPits && driver.Speed < 5)
                                {
                                    if (driver.TrackPosition == TrackPointType.SECTOR1)
                                        g.DrawString("[STOP]", f, Brushes.Red, 245f, 10f + ind*LineHeight);
                                    if (driver.TrackPosition == TrackPointType.SECTOR2)
                                        g.DrawString("[STOP]", f, Brushes.Red, 295f, 10f + ind*LineHeight);
                                    if (driver.TrackPosition == TrackPointType.SECTOR3)
                                        g.DrawString("[STOP]", f, Brushes.Red, 345f, 10f + ind*LineHeight);

                                }
                            }

                            g.DrawString(driver.Laps.ToString(), f, Brushes.White, 395f, 10f + ind*LineHeight);
                        }

                        ind++;

                    }

                    // Place fastest lap & combined lap
                    if (BestSector3Position != 0)
                    {
                        g.DrawString("Fastest lap: ", f, Brushes.DarkGray, 10f, 10f + ind*LineHeight);
                        g.DrawString(FastestDriver, f, Brushes.Yellow, 195f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(FastestLap, false), f, Brushes.Magenta, 140f, 10f + ind*LineHeight);



                        g.DrawString("P" + BestSector1Position, f, Brushes.Yellow, 275f, 10f + ind*LineHeight);
                        g.DrawString("P" + BestSector2Position, f, Brushes.Yellow, 325f, 10f + ind*LineHeight);
                        g.DrawString("P" + BestSector3Position, f, Brushes.Yellow, 375f, 10f + ind*LineHeight);

                    }
                    if (BestSector3 != 1000)
                    {
                        ind++;
                        g.DrawString("Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(BestSector1 + BestSector2 + BestSector3, false), f, Brushes.Magenta,
                                     140f,
                                     10f + ind*LineHeight);

                        g.DrawString(PrintLapTime(BestSector1, true), f, Brushes.Magenta, 245f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(BestSector2, true), f, Brushes.Magenta, 295f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(BestSector3, true), f, Brushes.Magenta, 345f, 10f + ind*LineHeight);
                        ind++;
                        g.DrawString("Personal Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(MyBestSector1 + MyBestSector2 + MyBestSector3, false), f,
                                     Brushes.Magenta, 140f,
                                     10f + ind*LineHeight);

                        g.DrawString(PrintLapTime(MyBestSector1, true), f, Brushes.Magenta, 245f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(MyBestSector2, true), f, Brushes.Magenta, 295f, 10f + ind*LineHeight);
                        g.DrawString(PrintLapTime(MyBestSector3, true), f, Brushes.Magenta, 345f, 10f + ind*LineHeight);
                    }
                }
            }
            catch (Exception ex)
            {
                /*
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                g.DrawString("ERROR", f, Brushes.Red, 10f, 10f);
                g.DrawString(ex.Message, f, Brushes.Yellow, 10f, 50f);
                string[] lines = ex.StackTrace.Split("\n".ToCharArray());

                int i = 1;
                foreach (string line in lines)
                {
                    i++;
                    g.DrawString(line, f, Brushes.White, 10f, 10f + i * 30f);
                }*/
            }
            base.OnPaint(e);
        }

    }

}