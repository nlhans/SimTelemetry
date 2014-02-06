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
using SimTelemetry.Domain.ValueObjects;

namespace LiveTelemetry
{
    public partial class Gauge_Laps : UserControl
    {
        public Gauge_Laps()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private string PrintLapTime(float time, bool sector)
        {
            if (time < 60)
            {
                if (sector) return time.ToString("00.000");
                else
                {
                    return "0:" + time.ToString("00.000");
                }
            }
            else
            {
                int minutes = Convert.ToInt32(Math.Floor(Convert.ToDouble(time / 60f)));

                return minutes + ":" + (time % 60f).ToString("00.000");
            }

        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var f = new Font("Arial", 8, FontStyle.Regular);

            base.OnPaint(e);

            var g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (!TelemetryApplication.TelemetryAvailable) return;

            try
            {
                var columns = new Dictionary<string, float>();

                columns.Add("Lap", 70);
                columns.Add("S1", 100);
                columns.Add("S2", 150);
                columns.Add("S3", 200);
                columns.Add("Time", 250);
                columns.Add("Gap", 320);


                foreach(var c in columns)
                    g.DrawString(c.Key, f, Brushes.DarkGray, c.Value, 10f);

                var laps = TelemetryApplication.Telemetry.Player.GetLaps().ToList();
                    var bestLap = TelemetryApplication.Telemetry.Player.BestLap;

                if (!laps.Any(x => x.Completed && x.Total > 0)) return;

                float lineheight = 16f;
                int ind = 16;
                int sind = ind;

                var lapsBeforeThese16 = laps.Count() > 16
                                            ? laps.OrderBy(x => x.TimeStart).Take(laps.Count() - 16).ToList()
                                            : new List<Lap>();

                var personalBestS1SoFar = lapsBeforeThese16.Count > 0 ? lapsBeforeThese16.Where(x=>x.Sector1>0).Min(x => x.Sector1) : -1;
                var personalBestS2SoFar = lapsBeforeThese16.Count > 0 ? lapsBeforeThese16.Where(x => x.Sector2 > 0).Min(x => x.Sector2) : -1;
                var personalBestS3SoFar = lapsBeforeThese16.Count > 0 ? lapsBeforeThese16.Where(x => x.Sector3 > 0).Min(x => x.Sector3) : -1;
                var personalBestSoFar = lapsBeforeThese16.Count > 0 ? lapsBeforeThese16.Where(x => x.Total > 0).Min(x => x.Total) : -1;

                ind = 16;
                foreach(var lap in laps.OrderBy(x=>-x.TimeStart).Take(16).Reverse())
                {
                    var y = 10f + lineheight*ind;

                    g.DrawString(lap.LapNumber.ToString("000"), f, Brushes.White, columns["Lap"], y);

                    if(lap.OutLap)
                    {
                        g.DrawString("OUT LAP", f, Brushes.White, columns["S1"], y);
                    }
                    else if (lap.Sector1 > 0)
                    {
                        personalBestS1SoFar = personalBestS1SoFar < 0
                                                  ? lap.Sector1
                                                  : Math.Min(personalBestS1SoFar, lap.Sector1);

                        var sector1brush = Brushes.Yellow;
                        if (personalBestS1SoFar == lap.Sector1) sector1brush = Brushes.YellowGreen;
                        if (TelemetryApplication.Telemetry.Laps.BestS1 == lap.Sector1) sector1brush = Brushes.Magenta;

                        g.DrawString(PrintLapTime(lap.Sector1, true), f, sector1brush, columns["S1"], y);
                    }

                    if (lap.Sector2 > 0)
                    {
                        personalBestS2SoFar = personalBestS2SoFar < 0
                                                  ? lap.Sector2
                                                  : Math.Min(personalBestS2SoFar, lap.Sector2);

                        var sector2brush = Brushes.Yellow;
                        if (personalBestS2SoFar == lap.Sector2) sector2brush = Brushes.YellowGreen;
                        if (TelemetryApplication.Telemetry.Laps.BestS1 == lap.Sector1) sector2brush = Brushes.Magenta;

                        g.DrawString(PrintLapTime(lap.Sector2, true), f, sector2brush, columns["S2"], y);
                    }

                    if (lap.InLap)
                    {
                        g.DrawString("IN LAP", f, Brushes.White, columns["S3"], y);
                    }
                    else if (lap.Sector3 > 0)
                    {
                        personalBestS3SoFar = personalBestS3SoFar < 0
                                                  ? lap.Sector3
                                                  : Math.Min(personalBestS3SoFar, lap.Sector3);

                        var sector3brush = Brushes.Yellow;
                        if (personalBestS3SoFar == lap.Sector3) sector3brush = Brushes.YellowGreen;
                        if (TelemetryApplication.Telemetry.Laps.BestS2 == lap.Sector3) sector3brush = Brushes.Magenta;

                        g.DrawString(PrintLapTime(lap.Sector3, true), f, sector3brush, columns["S3"], y);
                    }

                    if (lap.Total > 0)
                    {
                        personalBestSoFar = personalBestSoFar < 0
                                                  ? lap.Total
                                                  : Math.Min(personalBestSoFar, lap.Total);

                        var lapBrush = Brushes.Yellow;
                        if (personalBestSoFar == lap.Total) lapBrush = Brushes.YellowGreen;
                        if (TelemetryApplication.Telemetry.Laps.BestLap.Total == lap.Total) lapBrush = Brushes.Magenta;

                        g.DrawString(PrintLapTime(lap.Total, false), f, lapBrush, columns["Time"], y);

                        var gapToBest = lap.Total - bestLap.Total;
                        g.DrawString(PrintLapTime(gapToBest, true), f, lapBrush, columns["Gap"], y);

                    }

                    ind--;
                }

                sind++;
                g.DrawString("Fastest lap:", f, Brushes.DarkGray,35f, 10f + sind * lineheight);

                if (personalBestSoFar > 0)
                {
                    g.DrawString(PrintLapTime(bestLap.Sector1, true), f, Brushes.Magenta, 100f,10f + sind * lineheight);
                    g.DrawString(PrintLapTime(bestLap.Sector2, true), f, Brushes.Magenta, 150f,10f + sind * lineheight);
                    g.DrawString(PrintLapTime(bestLap.Sector3, true), f, Brushes.Magenta, 200f,10f + sind * lineheight);

                    g.DrawString(PrintLapTime(personalBestSoFar, false), f, Brushes.Magenta, 250f, 10f + sind * lineheight);
                }

                sind++;

                g.DrawString("Combined lap:", f, Brushes.DarkGray, 20f, 10f + sind * lineheight);

                if (personalBestS3SoFar > 0)
                {
                    g.DrawString(PrintLapTime(personalBestS1SoFar, true), f, Brushes.Magenta, 100f,10f + sind*lineheight);
                    g.DrawString(PrintLapTime(personalBestS2SoFar, true), f, Brushes.Magenta, 150f,10f + sind*lineheight);
                    g.DrawString(PrintLapTime(personalBestS3SoFar, true), f, Brushes.Magenta, 200f,10f + sind*lineheight);

                    g.DrawString(PrintLapTime(personalBestS1SoFar + personalBestS2SoFar + personalBestS3SoFar, false), f,Brushes.Magenta, 250f, 10f + sind*lineheight);
                }

                Font sf = new Font("Arial", 8f);

                g.DrawString("Engine", sf, DimBrush, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, DimBrush, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", sf, Brushes.White, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);
            }
            catch (Exception ex)
            {
                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);

            }
        }

        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

    }
}
