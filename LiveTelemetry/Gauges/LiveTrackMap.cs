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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Controls;
using SimTelemetry.Objects;

namespace LiveTelemetry
{
    public class LiveTrackMap : TrackMap
    {
        public LiveTrackMap()
        {
            BackgroundImage = _EmptyTrackMap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {

                Graphics g = e.Graphics;
                if (!Telemetry.m.Active_Session) return;
                if (_EmptyTrackMap == null)
                {
                    g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
                }
                else
                {
                    CompositingMode compMode = g.CompositingMode;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(_EmptyTrackMap, 0, 0);
                    g.CompositingMode = compMode;
                }
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var f = new Font("Arial", 12f);
                var ft = new Font("Arial", 7f);

                Pen pDarkRed = new Pen(Color.DarkRed, 3f);
                Pen pDarkGreen = new Pen(Color.DarkGreen, 3f);
                float bubblesize = 34f;
                // get all drivers and draw a dot!
                lock (Telemetry.m.Sim.Drivers.AllDrivers)
                {
                    foreach (IDriverGeneral driver in Telemetry.m.Sim.Drivers.AllDrivers)
                    {
                        if (driver.Position != 0 && driver.Position <= 120 && Math.Abs( driver.CoordinateX)>=0.1)
                        {
                            float a1 = Convert.ToSingle(10 + ((driver.CoordinateX - pos_x_min) / (pos_x_max - pos_x_min)) * (map_width - 20));
                            float a2 = Convert.ToSingle(100 + (1 - (driver.CoordinateY - pos_y_min) / (pos_y_max - pos_y_min)) * (map_height - 20));

                            a1 -= bubblesize / 2f;
                            a2 -= bubblesize / 2f;

                            Brush c;
                            if (driver.Position == Telemetry.m.Sim.Drivers.Player.Position) // Player
                                c = Brushes.Magenta;
                            else if (driver.Speed < 5) // Stopped
                                c = Brushes.Red;
                            else if (driver.Flag_Yellow) // Local yellow flag
                                c = Brushes.Yellow;
                            else if (Telemetry.m.Sim.Session.Type.Type == SessionType.RACE && driver.GetSplitTime(Telemetry.m.Sim.Drivers.Player) >= 10000) // InRace && lapped vehicle
                                c = new SolidBrush(Color.FromArgb(80, 80, 80));
                            else if (driver.Position > Telemetry.m.Sim.Drivers.Player.Position) // In front of player.
                                c = Brushes.YellowGreen;
                            else // Behind player, but not lapped.
                                c = new SolidBrush(Color.FromArgb(90, 120, 120));

                            g.FillEllipse(c, a1, a2, bubblesize, bubblesize);
                            g.DrawEllipse(new Pen(Color.White, 1f), a1, a2, bubblesize, bubblesize);
                            g.DrawString(driver.Position.ToString(), f, Brushes.White, a1 + 5, a2 + 2);

                            g.DrawLine(pDarkRed, a1 + bubblesize / 2f - 10, a2 + 3 + bubblesize / 2f,
                                       a1 + bubblesize / 2f - 10 + Convert.ToInt32(driver.Brake * 20),
                                       a2 + 3 + bubblesize / 2f);
                            g.DrawLine(pDarkGreen, a1 + bubblesize / 2f - 10, a2 + 3 + bubblesize / 2f,
                                       a1 + bubblesize / 2f - 10 + Convert.ToInt32(driver.Throttle * 20),
                                       a2 + 3 + bubblesize / 2f);
                            g.DrawString((driver.Speed * 3.6).ToString("000"), ft, Brushes.White, a1 + bubblesize / 2f - 10,
                                         a2 + bubblesize / 2f + 5);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);

                Font f = new Font("Arial", 10f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 40);

            }
            //base.OnPaint(e);
        }
    }
}