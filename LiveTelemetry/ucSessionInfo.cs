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

            try
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (!Telemetry.m.Active_Session) return;

                // What session type?
                SessionInfo i = Telemetry.m.Sim.Session.Type;

                string sSessionType = "Race";
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
                }

                System.Drawing.Font f24 = new Font("Calibri", 26f);
                System.Drawing.Font f16 = new Font("Calibri", 16f);
                System.Drawing.Font f14 = new Font("Calibri", 14f);
                System.Drawing.Font f12 = new Font("Calibri", 12f);
                System.Drawing.Font f10 = new Font("Calibri", 10f);
                System.Drawing.Font f8 = new Font("Calibri", 8f);

                g.DrawString(sSessionType, f24, Brushes.White, 8, 5);
                if (i.Type != SessionType.RACE)
                {
                    double ftime = Telemetry.m.Sim.Session.Time-30;
                    int hours = Convert.ToInt32(Math.Floor(ftime / 3600));
                    int minutes = Convert.ToInt32(Math.Floor((ftime - hours * 3600) / 60));
                    int seconds = Convert.ToInt32((ftime - hours * 3600 - minutes * 60));

                    if (i.Type == SessionType.TEST_DAY)
                        i.Length = 30+24*3600;
                    i.Length -= 30;
                    int hours_l = Convert.ToInt32(Math.Floor(i.Length / 3600));
                    int minutes_l = Convert.ToInt32(Math.Floor((i.Length - hours_l * 3600) / 60));
                    int seconds_l = Convert.ToInt32((i.Length - hours_l * 3600 - minutes_l * 60));

                    if(ftime>0)
                    g.DrawString(String.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,
                                 20);
                    else
                    g.DrawString(String.Format("--:--:-- / {3:00}:{4:00}:{5:00}", hours, minutes, seconds, hours_l, minutes_l, seconds_l), f16, Brushes.White, 200,
                                 20);
                }
                else
                {
                    // display time/laps

                    double ftime = Telemetry.m.Sim.Session.Time - 30;
                    int hours = Convert.ToInt32(Math.Floor(ftime / 3600));
                    int minutes = Convert.ToInt32(Math.Floor((ftime - hours * 3600) / 60));
                    int seconds = Convert.ToInt32((ftime - hours * 3600 - minutes * 60));

                    if (i.Type == SessionType.TEST_DAY)
                        i.Length = 30 + 24 * 3600;
                    i.Length -= 30;
                    int hours_l = Convert.ToInt32(Math.Floor(i.Length / 3600));
                    int minutes_l = Convert.ToInt32(Math.Floor((i.Length - hours_l * 3600) / 60));
                    int seconds_l = Convert.ToInt32((i.Length - hours_l * 3600 - minutes_l * 60));

                    int total_laps = Telemetry.m.Sim.Session.RaceLaps;
                    if (total_laps > 0)
                    {
                        if (ftime > 0)
                            g.DrawString(
                                String.Format("{0:00}:{1:00}:{2:00} / {3:00}:{4:00}:{5:00}", hours, minutes, seconds,
                                              hours_l, minutes_l, seconds_l), f14, Brushes.White, 200,
                                15);
                        else
                            g.DrawString(
                                String.Format("--:--:-- / {3:00}:{4:00}:{5:00}", hours, minutes, seconds, hours_l,
                                              minutes_l, seconds_l), f14, Brushes.White, 200,
                                15);

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
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 30);


            }
        }
    }
}
