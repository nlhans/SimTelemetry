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
using System.Drawing.Imaging;
using System.Windows.Forms;
using LiveTelemetry.Gauges;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Services;
using SimTelemetry.Domain.Telemetry;

namespace LiveTelemetry
{
    public class LiveTrackMap : TrackMap
    {
        public const float Bubblesize = 34f;
        public const float ArrowSize = Bubblesize / 2;
        private const float ArrowAngle = (float) (50.0f / 180.0f * Math.PI);

        public LiveTrackMap()
        {
            BackgroundImage = _BackgroundTrackMap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!TelemetryApplication.SessionAvailable) return;
            base.OnPaint(e);
            if (!TelemetryApplication.TelemetryAvailable) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var pDarkRed = new Pen(Color.DarkRed, 3f);
            var pDarkGreen = new Pen(Color.DarkGreen, 3f);

            try
            {
                lock (TelemetryApplication.Data.Drivers)
                {
                    foreach (var driver in TelemetryApplication.Data.Drivers)
                    {
                        if (driver.Position == 0 || driver.Position > 120 || !(Math.Abs(driver.CoordinateX) >= 0.1))
                            continue;

                        var a1 = GetImageX(driver.CoordinateX);
                        var a2 = GetImageY(driver.CoordinateY);

                        Brush c;
                        if (driver.Position == TelemetryApplication.Data.Player.Position)      // Player
                            c = Brushes.Magenta;
                        else if (driver.Speed < 5)                                                  // Stopped
                            c = Brushes.Red;
                        else if (driver.FlagYellow)                                                 // Local yellow flag
                            c = Brushes.Gold;
                        else if (TelemetryApplication.Data.Session.Info.Type == SessionType.RACE &&
                                 driver.GetSplitTime(TelemetryApplication.Data.Player) >= 10000)
                            c = new SolidBrush(Color.FromArgb(80, 80, 80));                         // InRace && lapped vehicle
                        else if (driver.Position > TelemetryApplication.Data.Player.Position)
                            c = Brushes.YellowGreen;                                                // In front of player.
                        else
                            c = new SolidBrush(Color.FromArgb(90, 120, 120));                       // Behind player, but not lapped.

                        var arrow = new PointF[3];
                        arrow[0] = new PointF(Convert.ToSingle(a1 + Math.Sin(driver.Heading)*(ArrowSize + 10)),
                                              Convert.ToSingle(a2 + Math.Cos(driver.Heading)*(ArrowSize + 10)));
                        arrow[1] =
                            new PointF(Convert.ToSingle(a1 + Math.Sin(driver.Heading + ArrowAngle)*ArrowSize),
                                       Convert.ToSingle(a2 + Math.Cos(driver.Heading + ArrowAngle)*ArrowSize));
                        arrow[2] =
                            new PointF(Convert.ToSingle(a1 + Math.Sin(driver.Heading - ArrowAngle)*ArrowSize),
                                       Convert.ToSingle(a2 + Math.Cos(driver.Heading - ArrowAngle)*ArrowSize));

                        g.FillPolygon(Brushes.White, arrow, FillMode.Winding);

                        a1 -= Bubblesize/2f;
                        a2 -= Bubblesize/2f;

                        g.FillEllipse(c, a1, a2, Bubblesize, Bubblesize);
                        g.DrawEllipse(new Pen(Color.White, 1f), a1, a2, Bubblesize, Bubblesize);

                        g.DrawString(driver.Position.ToString("00"), tf12, Brushes.White, a1 + 5, a2 + 2);

                        // Brake bar
                        if (driver.InputBrake > 0)
                            g.DrawLine(pDarkRed,
                                       a1 + Bubblesize/2f - 10,
                                       a2 + 3 + Bubblesize/2f,
                                       a1 + Bubblesize/2f - 10 + Convert.ToInt32(driver.InputBrake*20),
                                       a2 + 3 + Bubblesize/2f);

                        // Throttle bar
                        if (driver.InputThrottle > 0)
                            g.DrawLine(pDarkGreen,
                                       a1 + Bubblesize/2f - 10,
                                       a2 + 3 + Bubblesize/2f,
                                       a1 + Bubblesize/2f - 10 + Convert.ToInt32(driver.InputThrottle*20),
                                       a2 + 3 + Bubblesize/2f);

                        // Speed
                        g.DrawString((driver.Speed*3.6).ToString("000"), tf8, Brushes.White,
                                     a1 + Bubblesize/2f - 10,
                                     a2 + Bubblesize/2f + 5);
                    }
                }
            }
            catch (Exception ex)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);

                Font f = new Font("Arial", 10f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 40);

            }
            //base.OnPaint(e);
        }
    }
}