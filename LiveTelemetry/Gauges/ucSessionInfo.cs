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
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace LiveTelemetry
{
    public partial class ucSessionInfo : UserControl
    {
        public ucSessionInfo()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            try
            {
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (!Telemetry.m.Active_Session) return;

                // What session type?
                SessionInfo i = Telemetry.m.Sim.Session.Type;

                string sSessionType = "";
                switch (i.Type)
                {
                    case SessionType.WARMUP:
                        sSessionType = "Warmup";
                        break;
                    case SessionType.QUALIFY:
                        sSessionType = "Qualifying " + i.Number;
                        break;
                    case SessionType.TEST_DAY:
                        sSessionType = "Test Day";
                        break;
                    case SessionType.PRACTICE:
                        sSessionType = "Practice " + i.Number;
                        break;
                    case SessionType.RACE:
                        sSessionType = "Race";
                        break;
                    default:
                        sSessionType = "???";
                        break;
                }

                string FontFamily = "Calibri";
                Font f24 = new Font(FontFamily, 26f);
                Font f16 = new Font(FontFamily, 16f);
                Font f14 = new Font(FontFamily, 14f);
                Font f12 = new Font(FontFamily, 12f);
                Font f10 = new Font(FontFamily, 10f);
                Font f8 = new Font(FontFamily, 8f);

                g.DrawString(sSessionType, f24, Brushes.White, 8, 5);

                if (i.Type != SessionType.RACE)
                {
                    // Timed session.
                    // 30 seconds is typical offset used in rFactor.
                    // TODO: Fix time offsets.
                    double ftime = Telemetry.m.Sim.Session.Time-30;

                    int hours = Convert.ToInt32(Math.Floor(ftime / 3600));
                    int minutes = Convert.ToInt32(Math.Floor((ftime - hours * 3600) / 60));
                    int seconds = Convert.ToInt32((ftime - hours * 3600 - minutes * 60));

                    // Test-days often don't have time limits, so display 24 hours.
                    if (i.Type == SessionType.TEST_DAY)
                        i.Length = 30+24*3600;
                    i.Length -= 30;

                    int hours_l = Convert.ToInt32(Math.Floor(i.Length / 3600));
                    int minutes_l = Convert.ToInt32(Math.Floor((i.Length - hours_l * 3600) / 60));
                    int seconds_l = Convert.ToInt32((i.Length - hours_l * 3600 - minutes_l * 60));

                    // Display time.
                    // If session not yet started, display --:--:--
                    if(ftime>0)
                        g.DrawString(String.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", 
                            hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,20);
                    else
                        g.DrawString(String.Format("--:--:-- / {3:00}:{4:00}:{5:00}", 
                            hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,20);
                }
                else
                {
                    // Race session. Time/laps based.
                    double ftime = Telemetry.m.Sim.Session.Time - 30;
                    int hours = Convert.ToInt32(Math.Floor(ftime / 3600));
                    int minutes = Convert.ToInt32(Math.Floor((ftime - hours * 3600) / 60));
                    int seconds = Convert.ToInt32((ftime - hours * 3600 - minutes * 60));

                    int hours_l = Convert.ToInt32(Math.Floor(i.Length / 3600));
                    int minutes_l = Convert.ToInt32(Math.Floor((i.Length - hours_l * 3600) / 60));
                    int seconds_l = Convert.ToInt32((i.Length - hours_l * 3600 - minutes_l * 60));

                    int total_laps = Telemetry.m.Sim.Session.RaceLaps;
                    
                    if (total_laps > 0)
                    {
                        // Display laps + time.
                        // If session not yet started, display --:--:--
                        if (ftime > 0)
                            g.DrawString(
                                String.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", 
                                hours, minutes, seconds, hours_l, minutes_l, seconds_l), f14, Brushes.White, 200,15);
                        else
                            g.DrawString(
                                String.Format("--:--:-- / {3:00}:{4:00}:{5:00}", 
                                hours, minutes, seconds, hours_l, minutes_l, seconds_l), f14, Brushes.White, 200,15);

                        // TODO: Add LeaderLaps property to frame-work.
                        int leader_laps = 0;
                        foreach (IDriverGeneral dg in Telemetry.m.Sim.Drivers.AllDrivers)
                        {
                            if (dg.Position == 1)
                            {
                                leader_laps = dg.Laps;
                                break;
                            }
                        }
                        g.DrawString(string.Format("{0:000}/{1:000} laps", leader_laps, total_laps), f24, Brushes.White,
                                     195, 35);
                    }
                    else
                    {
                        // Display time.
                        // If session not yet started, display --:--:--
                        if (ftime > 0)
                            g.DrawString(String.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,
                                         20);
                        else
                            g.DrawString(String.Format("--:--:-- / {3:00}:{4:00}:{5:00}", hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,
                                         20);
                    }
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
