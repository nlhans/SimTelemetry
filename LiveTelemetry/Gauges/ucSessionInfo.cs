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
using System.Windows.Forms;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Telemetry;
using SimTelemetry.Domain.ValueObjects;

namespace LiveTelemetry
{
    public partial class ucSessionInfo : UserControl
    {
        private Font f24, f16, f14, f12, f10, f8;

        public ucSessionInfo()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            string FontFamily = "Calibri";
            f24 = new Font(FontFamily, 26f);
            f16 = new Font(FontFamily, 16f);
            f14 = new Font(FontFamily, 14f);
            f12 = new Font(FontFamily, 12f);
            f10 = new Font(FontFamily, 10f);
            f8 = new Font(FontFamily, 8f);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            if (!TelemetryApplication.TelemetryAvailable) return;

            try
            {
                var sessionInfo = TelemetryApplication.Telemetry.Session.Info;

                g.DrawString(sessionInfo.Name, f24, Brushes.White, 8, 5);

                // Compute session time left / total.
                double ftime = TelemetryApplication.Telemetry.Session.Time;

                int hours = Convert.ToInt32(Math.Floor(ftime / 3600));
                int minutes = Convert.ToInt32(Math.Floor((ftime - hours * 3600) / 60));
                int seconds = Convert.ToInt32((ftime - hours * 3600 - minutes * 60));

                double duration = sessionInfo.Duration.TotalSeconds;

                int hours_l = Convert.ToInt32(Math.Floor(duration / 3600));
                int minutes_l = Convert.ToInt32(Math.Floor((duration - hours_l * 3600) / 60));
                int seconds_l = Convert.ToInt32((duration - hours_l * 3600 - minutes_l * 60));

                // Display --:--:-- when session is not under way
                var timeToDisplay = ftime > 0
                                        ? string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds)
                                        : "--:--:--";

                // Don't display session length if the duration is invalid (sometimes valid for test sessions)
                timeToDisplay += duration > 0
                                     ? string.Format(" / {0:00}:{1:00}:{2:00}", hours_l, minutes_l, seconds_l)
                                     : "";

                // Figure out what to-do
                int total_laps = TelemetryApplication.Telemetry.Session.RaceLaps;
                int leader_laps = TelemetryApplication.Telemetry.Session.LeaderLaps;

                if (sessionInfo.Type != SessionType.RACE || (sessionInfo.Type == SessionType.RACE && total_laps <= 0))
                {
                    // Timed session (24h races, practice, etc.) ; only display time.
                    g.DrawString(timeToDisplay, f16, Brushes.White, 200, 20);
                }
                else
                {
                    var lapsToDisplay = string.Format("{0:000}/{1:000} laps", leader_laps, total_laps);

                    // Lapped sessions (possibly with time limit)
                    g.DrawString(timeToDisplay, f14, Brushes.White, 200, 15);
                    g.DrawString(lapsToDisplay, f24, Brushes.White, 195, 35);
                }
            }
            catch (Exception ex)
            {
                Font f = new Font("Arial", 8f);

                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);
            }
        }
    }
}
