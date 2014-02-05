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
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Telemetry;

namespace LiveTelemetry
{
    public partial class LapChart : UserControl
    {
        private Dictionary<string, int> DisplayTimers = new Dictionary<string, int>();
        private Dictionary<string, int> LastPitLap = new Dictionary<string, int>();

        public LapChart()
        {
            InitializeComponent();

            this.Size = new Size(460, 500);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

        }

        private string PrintLapTime(double time, bool sector)
        {
            if(time < 60)
            {
                return !sector ? "0:" + time.ToString("00.000") : time.ToString("00.000");
            }
            
            int minutes = Convert.ToInt32(Math.Floor(Convert.ToDouble(time/60f)));

            return minutes + ":" + (time%60f).ToString("00.000");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.Size = new Size(460, this.Size.Height);
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (TelemetryApplication.TelemetryAvailable ==false) return;

            // Draw header
            var f = new Font("Arial", 8, FontStyle.Regular);

            var columns = new Dictionary<string, float>();

            try
            {
                var sessionIsRace = TelemetryApplication.Telemetry.Session.Info.Type == SessionType.RACE;
                if (!sessionIsRace)
                {
                    columns.Add("P", 10);
                    columns.Add("Driver", 38);
                    columns.Add("Laptime", 140);
                    columns.Add("Gap", 195);
                    columns.Add("S1", 245);
                    columns.Add("S2", 295);
                    columns.Add("S3", 345);
                    columns.Add("Lap", 395);
                }else
                {
                    columns.Add("P", 10);
                    columns.Add("Driver", 38);
                    columns.Add("Gap", 140);
                    columns.Add("Interval", 165);
                    columns.Add("Lap", 222);
                    columns.Add("S1", 245);
                    columns.Add("S2", 295);
                    columns.Add("S3", 345);
                    columns.Add("Pit", 395);
                }

                foreach (var c in columns)
                    g.DrawString(c.Key, f, Brushes.DarkGray, c.Value, 10f);

                int ind = 1;

                var player = TelemetryApplication.Telemetry.Player;
                if(player == null)
                {
                    g.DrawString("Could not find player memory object", f, Brushes.White, 10f, 30f);
                    return;
                }

                var lapTimeBest = TelemetryApplication.Telemetry.Laps.BestLap.Total;

                var overallBestS1 = TelemetryApplication.Telemetry.Laps.BestS1;
                var overallBestS2 = TelemetryApplication.Telemetry.Laps.BestS2;
                var overallBestS3 = TelemetryApplication.Telemetry.Laps.BestS3;

                var BestSector1Position = TelemetryApplication.Telemetry.Laps.BestS1Lap.Driver;
                var BestSector2Position = TelemetryApplication.Telemetry.Laps.BestS2Lap.Driver;
                var BestSector3Position = TelemetryApplication.Telemetry.Laps.BestS3Lap.Driver;

                var myBestSector1 = TelemetryApplication.Telemetry.Player.BestS1;
                var myBestSector2 = TelemetryApplication.Telemetry.Player.BestS2;
                var myBestSector3 = TelemetryApplication.Telemetry.Player.BestS3;

                var drivers = TelemetryApplication.Telemetry.Drivers.OrderBy(x => x.Position);
                int driverCount = drivers.Count();

                // Calculate the line height
                var lineHeight = (float) (driverCount < 16 ? 20 : Math.Max(10, (Size.Height - 20)/(4.0 + driverCount)));



                // Go through all drivers
                if (driverCount>0)
                {
                    var currentSessionTime = TelemetryApplication.Telemetry.Session.Time;

                    double Previous_SplitLeader = 0;
                    foreach (TelemetryDriver driver in drivers)
                    {
                        var y = 10f + ind * lineHeight;

                        if (y > Size.Height - 4 * 20) break;

                        var bestSector1 = driver.BestS1;
                        var bestSector2 = driver.BestS2;
                        var bestSector3 = driver.BestS3;

                        var bestLapObj = driver.BestLap;
                        var bestLap = bestLapObj.Total;
                        var bestSector1HotLap = bestLapObj.Sector1;
                        var bestSector2HotLap = bestLapObj.Sector2;
                        var bestSector3HotLap = bestLapObj.Sector3;

                        var lastLap = driver.LastLap;
                        var lastLapValid = lastLap.Total > 0;

                        var lastS1 = driver.LastS1;
                        var lastS2 = driver.LastS2;
                        var lastS3 = driver.LastS3;

                        // **** Position **** //
                        g.DrawString(driver.Position.ToString("00"), f, Brushes.White, columns["P"], y);

                        // **** NAME **** //
                        var nameBrush = Brushes.White;
                        if(driver.Speed < 10) nameBrush = Brushes.OrangeRed;
                        if (driver.IsPits) nameBrush = Brushes.DarkGray;
                        g.DrawString(driver.Name, f, nameBrush, columns["Driver"], y);

                        bool showLastLap = false;

                        if(columns.ContainsKey("Lap"))
                        {
                            g.DrawString(driver.Laps.ToString("000"), f, Brushes.White, columns["Lap"], y);
                        }

                        if (columns.ContainsKey("Laptime"))
                        {
                            // TODO: Add UI setting "Show laptimes for.."
                             showLastLap = lastLapValid && (currentSessionTime - lastLap.TimeEnd) < 10;
                             var lastLapIsBestLap = showLastLap && Math.Abs(lastLap.Total - lapTimeBest) < 0.0001;
                             var lastLapIsPersonalBestLap = showLastLap && Math.Abs(lastLap.Total - driver.BestLap.Total) < 0.0001;
                            var laptimeBrush = Brushes.DarkGray;

                            if(showLastLap) laptimeBrush = Brushes.Yellow;
                            if (showLastLap && lastLapIsPersonalBestLap) laptimeBrush = Brushes.YellowGreen;
                            if (lastLapIsBestLap) laptimeBrush = Brushes.Magenta;

                            var laptimeToPrint = showLastLap ? lastLap.Total : bestLap;
                            var gaptimeToPrint = laptimeToPrint - bestLap;

                            if(laptimeToPrint>0)
                            g.DrawString(PrintLapTime(laptimeToPrint, false), f, laptimeBrush, columns["Laptime"], y);

                            // Also display gap
                            if (gaptimeToPrint > 0)
                                g.DrawString(PrintLapTime(gaptimeToPrint, false), f, laptimeBrush, columns["Gap"], y);
                        }
                        if (columns.ContainsKey("S1") && columns.ContainsKey("S2") && columns.ContainsKey("S3"))
                        {
                            // Draw sector lap times
                            var showBestLap = driver.IsPits;
                            var showCurrentLap = !showLastLap && !showBestLap;

                            var sector1ToShow = (showBestLap)
                                                    ? driver.BestLap.Sector1
                                                    : (showLastLap)
                                                          ? lastLap.Sector1
                                                          : driver.CurrentLap.Sector1;
                            var sector2ToShow = (showBestLap)
                                                    ? driver.BestLap.Sector2
                                                    : (showLastLap)
                                                          ? lastLap.Sector2
                                                          : driver.CurrentLap.Sector2;
                            var sector3ToShow = (showBestLap)
                                                    ? driver.BestLap.Sector3
                                                    : (showLastLap)
                                                          ? lastLap.Sector3
                                                          : driver.CurrentLap.Sector3;
                            
                            var sector1Brush = (showLastLap) ? Brushes.Yellow : Brushes.White;
                            var sector2Brush = (showLastLap) ? Brushes.Yellow : Brushes.White;
                            var sector3Brush = (showLastLap) ? Brushes.Yellow : Brushes.White;

                            if (sector1ToShow == driver.BestS1) sector1Brush = Brushes.YellowGreen;
                            if (sector2ToShow == driver.BestS2) sector2Brush = Brushes.YellowGreen;
                            if (sector3ToShow == driver.BestS3) sector3Brush = Brushes.YellowGreen;

                            if (sector1ToShow == TelemetryApplication.Telemetry.Laps.BestS1) sector1Brush = Brushes.Magenta;
                            if (sector2ToShow == TelemetryApplication.Telemetry.Laps.BestS2) sector2Brush = Brushes.Magenta;
                            if (sector3ToShow == TelemetryApplication.Telemetry.Laps.BestS3) sector3Brush = Brushes.Magenta;

                            var showSector1AtAll = showLastLap || showBestLap ||
                                                   showCurrentLap &&
                                                   (driver.TrackPosition == TrackPointType.SECTOR2 ||
                                                    driver.TrackPosition == TrackPointType.SECTOR3);
                            var showSector2AtAll = showLastLap || showBestLap ||
                                                   showCurrentLap && driver.TrackPosition == TrackPointType.SECTOR3;
                            var showSector3AtAll = showLastLap || showBestLap;

                            showSector1AtAll &= sector1ToShow > 0;
                            showSector2AtAll &= sector2ToShow > 0;
                            showSector3AtAll &= sector3ToShow > 0;

                            // Is the driver on outlap?
                            if (driver.CurrentLap.OutLap && !driver.IsPits)
                            {
                                g.DrawString("OUT LAP", f, Brushes.Yellow, columns["S1"], y);
                            }
                            else if (showSector1AtAll)
                            {
                                g.DrawString(PrintLapTime(sector1ToShow, true), f, sector1Brush, columns["S1"], y);
                            }

                            if (showSector2AtAll)
                            {
                                g.DrawString(PrintLapTime(sector2ToShow, true), f, sector2Brush, columns["S2"], y);
                            }

                            if (showSector3AtAll)
                            {
                                g.DrawString(PrintLapTime(sector3ToShow, true), f, sector3Brush, columns["S3"], y);
                            }

                        }

                        ind++;

                    }

                    // Place fastest lap & combined lap
                    if (BestSector3Position > 0)
                    {
                        g.DrawString("Fastest lap: ", f, Brushes.DarkGray, 10f, 10f + ind*lineHeight);
                        g.DrawString(TelemetryApplication.Telemetry.Laps.BestLap.Driver.ToString(), f, Brushes.Yellow, 195f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(TelemetryApplication.Telemetry.Laps.BestLap.Total, false), f, Brushes.Magenta, 140f, 10f + ind * lineHeight);



                        g.DrawString("P" + BestSector1Position, f, Brushes.Yellow, 275f, 10f + ind*lineHeight);
                        g.DrawString("P" + BestSector2Position, f, Brushes.Yellow, 325f, 10f + ind*lineHeight);
                        g.DrawString("P" + BestSector3Position, f, Brushes.Yellow, 375f, 10f + ind*lineHeight);

                    }
                    if (overallBestS3 > 0)
                    {
                        ind++;
                        g.DrawString("Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(overallBestS1 + overallBestS2 + overallBestS3, false), f, Brushes.Magenta,
                                     140f,
                                     10f + ind*lineHeight);

                        g.DrawString(PrintLapTime(overallBestS1, true), f, Brushes.Magenta, 245f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(overallBestS2, true), f, Brushes.Magenta, 295f, 10f + ind * lineHeight);
                        g.DrawString(PrintLapTime(overallBestS3, true), f, Brushes.Magenta, 345f, 10f + ind * lineHeight);
                        ind++;
                        g.DrawString("Personal Combined lap: ", f, Brushes.DarkGray, 10f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(myBestSector1 + myBestSector2 + myBestSector3, false), f,
                                     Brushes.Magenta, 140f,
                                     10f + ind*lineHeight);

                        g.DrawString(PrintLapTime(myBestSector1, true), f, Brushes.Magenta, 245f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(myBestSector2, true), f, Brushes.Magenta, 295f, 10f + ind*lineHeight);
                        g.DrawString(PrintLapTime(myBestSector3, true), f, Brushes.Magenta, 345f, 10f + ind*lineHeight);
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