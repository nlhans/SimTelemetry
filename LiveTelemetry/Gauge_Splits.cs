using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Game.Rfactor;
using SimTelemetry.Objects;

namespace LiveTelemetry
{
    public partial class Gauge_Splits : UserControl
    {
        public Gauge_Splits()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private int sortDriver(DriverGeneral d1, DriverGeneral d2)
        {
            if (d1.Position == d2.Position) return 0;
            if (d1.Position > 120 || d1.Position == 0) return 1;
            if (d2.Position > 120 || d2.Position == 0) return -1;
            if (d1.Position > d2.Position) return 1;
            else return -1;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);
                if (rFactor.Session.Cars == 0) return;

                System.Drawing.Font f = new Font("Arial", 8, FontStyle.Regular);

                g.DrawString("Pos", f, Brushes.DarkGray, 10f, 10f);
                g.DrawString("Name", f, Brushes.DarkGray, 40f, 10f);
                g.DrawString("Gap", f, Brushes.DarkGray, 160f, 10f);
                g.DrawString("Best Lap", f, Brushes.DarkGray, 230f, 10f);
                g.DrawString("Last Lap", f, Brushes.DarkGray, 300f, 10f);
                g.DrawString("Pits", f, Brushes.DarkGray, 380f, 10f);
                
                List<DriverGeneral> drivers = rFactor.Drivers.AllDrivers;
                drivers.Sort(sortDriver);

                int ind =1;
                float LineHeight = 16f;

                // Go through all drivers
                for (int p = Math.Max(0, rFactor.Drivers.Player.Position - 6); p <= rFactor.Drivers.Player.Position + 12; p++ )
                {
                    if (ind == 16 || p >= drivers.Count)
                        break;
                    DriverGeneral driver = drivers[p];

                    Brush OntrackBrush = ((!driver.Pits && driver.Speed > 5) ? Brushes.White : Brushes.Red);
                    if(rFactor.Drivers.Player.Position == driver.Position) OntrackBrush = Brushes.Yellow;
                    g.DrawString(driver.Position.ToString(), f, Brushes.White, 10f, 10f + ind * LineHeight);
                    string[] name = driver.Name.ToUpper().Split(" ".ToCharArray());
                    if (name.Length == 1)
                        g.DrawString(name[0], f, OntrackBrush, 38f, 10f + ind * LineHeight);
                    else if (name.Length > 1)
                        g.DrawString(name[0].Substring(0, 1) + ". " + name[name.Length - 1], f, OntrackBrush, 38f,
                                     10f + ind * LineHeight);

                    if (!driver.Pits && driver.Speed < 5)
                    {
                        g.DrawString("[STOP]", f, Brushes.Red, 345f, 10f + ind * LineHeight);

                    }
                    if (driver.Pits)
                    {
                        g.DrawString("[PITS]", f, Brushes.Red, 345f, 10f + ind * LineHeight);

                    }

                    double split_leader = driver.GetSplitTime(drivers[0]);
                    if (split_leader >= 0 && split_leader < 10000)
                        g.DrawString(Math.Round(split_leader, 1).ToString(), f, Brushes.White, 160f, 10f + ind * LineHeight);
                    else if (split_leader >= 10000)
                    {
                        int laps = Convert.ToInt32(split_leader / 10000);
                        g.DrawString("+" + laps + "L", f, Brushes.Yellow, 160f, 10f + ind * LineHeight);
                    }
                    else if (drivers[0].Position == driver.Position)
                        g.DrawString("LAP " + drivers[0].Laps, f, Brushes.Yellow, 160f, 10f + ind * LineHeight);

                    /*if (driver.Position > 3)
                    {
                        DriverGeneral drv = driver;
                        foreach(DriverGeneral drv_ in drivers)
                        {
                            if (drv_.Position == driver.Position - 1 && drv_.__LapsData.ToInt32() > 0)
                                drv = drv_;
                        }
                        if (drv != null)
                        {
                            double split_next = driver.GetSplitTime(drv);
                            if (split_next < 10000)
                                g.DrawString(Math.Round(split_next, 1).ToString(), f,
                                             Brushes.White, 190f, 10f + ind*LineHeight);
                            else
                            {
                                int laps = Convert.ToInt32(split_next/10000);
                                g.DrawString("+" + laps + "L", f, Brushes.Yellow, 190f, 10f + ind*LineHeight);
                            }
                        }
                    }*/

                    g.DrawString(PrintLapTime(driver.LapTime_Best, false), f, Brushes.Yellow, 230f, 10f + ind * LineHeight);
                    if (driver.LapTime_Best == driver.LapTime_Last)
                        g.DrawString(PrintLapTime(driver.LapTime_Last, false), f, Brushes.Green, 300f, 10f + ind * LineHeight);
                    else
                    g.DrawString(PrintLapTime(driver.LapTime_Last, false), f, Brushes.Yellow, 300f, 10f + ind * LineHeight);

                    g.DrawString(driver.PitStopRuns.ToString(), f, Brushes.White, 380f, 10f + ind * LineHeight);


                    ind++;

                }

                Font sf = new Font("Arial", 8f);

                g.DrawString("Engine", sf, DimBrush, this.Width - 42, this.Height - 60);
                g.DrawString("Tyres", sf, DimBrush, this.Width - 35, this.Height - 45);
                g.DrawString("Laptimes", sf, DimBrush, this.Width - 51, this.Height - 30);
                g.DrawString("Split", sf,  Brushes.White, this.Width - 30, this.Height - 15);
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, e.ClipRectangle);

                System.Drawing.Font f = new Font("Arial", 8f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace.Replace("in ","\r\n"), f, Brushes.White, 10, 30);


            }
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

        private Color DimColor = Color.FromArgb(70, 70, 70);
        private SolidBrush DimBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

    }
}
