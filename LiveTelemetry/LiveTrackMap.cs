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
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {

                Graphics g = e.Graphics;
                if (!Telemetry.m.Active_Session) return;
                if (BackgroundImage == null)
                {
                    g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
                }
                else
                {
                    g.DrawImage(BackgroundImage, 0, 0);
                }
                g.SmoothingMode = SmoothingMode.AntiAlias;

                System.Drawing.Font f = new Font("Arial", 12f);
                System.Drawing.Font ft = new Font("Arial", 7f);

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
                            //if (driver.Name.Trim() == "") continue;
                            float a1 =
                                Convert.ToSingle(x_offset + 
                                                 driver.CoordinateX * x_scale );
                            float a2 =
                                 Convert.ToSingle(y_offset +
                                                       driver.CoordinateZ * y_scale);
                            a1 -= bubblesize / 2f;
                            a2 -= bubblesize / 2f;
                            if (driver.Position == Telemetry.m.Sim.Drivers.Player.Position) // YOU
                                g.FillEllipse(Brushes.Magenta, a1, a2, bubblesize, bubblesize);
                            else if (driver.Speed < 5) // speed < 
                                g.FillEllipse(Brushes.Red, a1, a2, bubblesize, bubblesize);
                            else if (driver.GetSplitTime(Telemetry.m.Sim.Drivers.Player) >= 10000) // lap>
                                g.FillEllipse(new SolidBrush(Color.FromArgb(80, 80, 80)), a1, a2, bubblesize, bubblesize);
                            else if (driver.Position > Telemetry.m.Sim.Drivers.Player.Position) // positie<
                                g.FillEllipse(Brushes.YellowGreen, a1, a2, bubblesize, bubblesize);
                            else // positie>
                                g.FillEllipse(new SolidBrush(Color.FromArgb(90, 120, 120)), a1, a2, bubblesize,
                                              bubblesize);
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
                        else
                        {
                            int a = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Graphics g = e.Graphics;
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);

                System.Drawing.Font f = new Font("Arial", 10f);

                g.DrawString(ex.Message, f, Brushes.White, 10, 10);
                g.DrawString(ex.StackTrace, f, Brushes.White, 10, 40);

            }
            //base.OnPaint(e);
        }
    }
}