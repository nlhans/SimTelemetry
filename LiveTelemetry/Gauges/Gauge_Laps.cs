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
            base.OnPaint(e);

            try
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (!TelemetryApplication.TelemetryAvailable) return;

                System.Drawing.Font f = new Font("Arial", 8, FontStyle.Regular);

                g.DrawString("Lap", f, Brushes.DarkGray, 70f, 10f);
                g.DrawString("S1", f, Brushes.DarkGray, 100f, 10f);
                g.DrawString("S2", f, Brushes.DarkGray, 150f, 10f);
                g.DrawString("S3", f, Brushes.DarkGray, 200f, 10f);
                g.DrawString("Time", f, Brushes.DarkGray, 250f, 10f);
                g.DrawString("Diff", f, Brushes.DarkGray, 320f, 10f);

                float lineheight = 16f;
                int ind = 16;
                int sind = ind;
                var laps = TelemetryApplication.Telemetry.Player.GetLaps().ToList();

                float best_lap = 1000f;
                float best_sector1 = 1000f;
                float best_sector2 = 1000f;
                float best_sector3 = 1000f;

                // TODO: Determine laptimes
                float absolute_best_lap = laps.Min(x => x.Total);
                float absolute_best_sector1 = laps.Min(x=>x.Sector1);
                float absolute_best_sector2 = laps.Min(x=>x.Sector2);
                float absolute_best_sector3 = laps.Min(x=>x.Sector3);

                for (int lap = laps.Count - ind; lap <= laps.Count - 1;  lap++)
                {
                    if (lap <= 0) continue;
                    g.DrawString(lap.ToString(), f, ((lap == laps.Count) ? Brushes.White : Brushes.Yellow),
                                80f, 10f + lineheight * ind);

                    if (laps[lap].Total > 0)
                        best_lap = Math.Min(best_lap, laps[lap].Total);
                    if (laps[lap].Sector1 > 0)
                        best_sector1 = Math.Min(best_sector1, laps[lap].Sector1);
                    if (laps[lap].Sector2 > 0)
                        best_sector2 = Math.Min(best_sector2, laps[lap].Sector2);
                    if (laps[lap].Sector3 > 0)
                        best_sector3 = Math.Min(best_sector3, laps[lap].Sector3);


                    if (laps[lap].Sector1 > 0)
                    {
                        Brush br1 = ((lap == laps.Count) ? Brushes.White : Brushes.Yellow);
                        if (laps[lap].Sector1 == best_sector1)
                            br1 = Brushes.DarkGreen;
                        if (laps[lap].Sector1 == absolute_best_sector1)
                            br1 = Brushes.Magenta;

                        g.DrawString(PrintLapTime(laps[lap].Sector1, true), f, br1, 100f, 10f + ind * lineheight);

                    }

                    if (laps[lap].Sector2 > 0)
                    {
                        Brush br1 = ((lap == laps.Count) ? Brushes.White : Brushes.Yellow);
                        if (laps[lap].Sector2 == best_sector2)
                            br1 = Brushes.DarkGreen;
                        if (laps[lap].Sector2 == absolute_best_sector2)
                            br1 = Brushes.Magenta;

                        g.DrawString(PrintLapTime(laps[lap].Sector2, true), f, br1, 150f, 10f + ind * lineheight);

                    }

                    if (laps[lap].Sector3 > 0)
                    {
                        Brush br1 = ((lap == laps.Count) ? Brushes.White : Brushes.Yellow);
                        if (laps[lap].Sector3 == best_sector3)
                            br1 = Brushes.DarkGreen;
                        if (laps[lap].Sector3 == absolute_best_sector3)
                            br1 = Brushes.Magenta;

                        g.DrawString(PrintLapTime(laps[lap].Sector3, true), f, br1, 200f, 10f + ind * lineheight);

                    }

                    if (laps[lap].Total > 0)
                    {
                        Brush br1 = ((lap == laps.Count) ? Brushes.White : Brushes.Yellow);
                        if (laps[lap].Total == best_lap)
                            br1 = Brushes.DarkGreen;
                        if (laps[lap].Total == absolute_best_lap)
                            br1 = Brushes.Magenta;

                        g.DrawString(PrintLapTime(laps[lap].Total, true), f, br1, 250f, 10f + ind * lineheight);

                    }
                    if (laps[lap].Sector1 < 1 && laps[lap].Sector2 < 1 && laps[lap].Sector3 < 1)
                    {
                        if (laps[lap - 1].Sector3 < 0 || (laps.Count == lap && TelemetryApplication.Telemetry.Player.TrackPosition != TrackPointType.SECTOR1))
                        g.DrawString("Out lap - no sector data", f, Brushes.Red, 100f, 10f + ind * lineheight);
                    }
                    if (laps[lap].Sector1 > 1 && laps[lap].Sector2 > 1 && laps[lap].Sector3 < 1 && !(TelemetryApplication.Telemetry.Player.TrackPosition == TrackPointType.SECTOR3 && lap + 1 == laps.Count))
                    {
                        g.DrawString("Pits", f, Brushes.Red,200f, 10f + ind * lineheight);
                    }
                    if (laps[lap].Total > 0)
                        // - Telemetry.m.Sim.Drivers.Player.LapTime_Best -> Total - 0
                        // TODO: Add laptimes
                        g.DrawString(PrintLapTime(laps[lap].Total - 0, true), f, Brushes.White, 310f, 10f + ind * lineheight);

                    ind--;


                }
                sind++;
                g.DrawString("Fastest lap:", f, Brushes.DarkGray,35f, 10f + sind * lineheight);

                // TODO: Add laptimes
                //g.DrawString(PrintLapTime(Telemetry.m.Sim.Drivers.Player.LapTime_Best_Sector1, true), f, Brushes.Magenta, 100f, 10f + sind * lineheight);
                //g.DrawString(PrintLapTime(Telemetry.m.Sim.Drivers.Player.LapTime_Best_Sector2, true), f, Brushes.Magenta, 150f, 10f + sind * lineheight);
                //g.DrawString(PrintLapTime(Telemetry.m.Sim.Drivers.Player.LapTime_Best_Sector3, true), f, Brushes.Magenta, 200f, 10f + sind * lineheight);

                //g.DrawString(PrintLapTime(Telemetry.m.Sim.Drivers.Player.LapTime_Best, false), f, Brushes.Magenta, 250f, 10f + sind * lineheight);

                sind++;
                g.DrawString("Combined lap:", f, Brushes.DarkGray, 20f, 10f + sind * lineheight);

                g.DrawString(PrintLapTime(absolute_best_sector1, true), f, Brushes.Magenta, 100f, 10f + sind * lineheight);
                g.DrawString(PrintLapTime(absolute_best_sector2, true), f, Brushes.Magenta, 150f, 10f + sind * lineheight);
                g.DrawString(PrintLapTime(absolute_best_sector3, true), f, Brushes.Magenta, 200f, 10f + sind * lineheight);

                g.DrawString(PrintLapTime(absolute_best_sector1 + absolute_best_sector2 + absolute_best_sector3, false), f, Brushes.Magenta, 250f, 10f + sind * lineheight);

                // TODO: Add laptimes
                //g.DrawString(PrintLapTime(absolute_best_sector1 + absolute_best_sector2 + absolute_best_sector3 -Telemetry.m.Sim.Drivers.Player.LapTime_Best, true), f, Brushes.White, 310f, 10f + sind * lineheight);


                Font sf = new Font("Arial", 8f);

                g.DrawString("Engine", sf, DimBrush, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, DimBrush, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", sf, Brushes.White, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf, DimBrush, this.Width - 30, this.Height - 15);
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);


            }
        }

        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

    }
}
