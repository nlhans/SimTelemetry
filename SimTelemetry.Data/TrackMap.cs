
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using SimTelemetry.Data;
    using SimTelemetry.Data.Track;
    using Triton;
    using Timer = System.Windows.Forms.Timer;

namespace SimTelemetry.Controls
{
    public partial class TrackMap : UserControl
    {
        public static bool StaticTrackMap=false;
        public string AIW_File = "";

        protected double pos_x_max = 1000000000.0;
        protected double pos_x_min = -1000000000.0;
        protected double pos_y_max = 1000000000.0;
        protected double pos_y_min = -1000000000.0;
        protected WayPoint[] waypoints = new WayPoint[25001];
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            if (BackgroundImage == null)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }
            else
            {
                g.DrawImage(BackgroundImage, 0, 0);
            }
            base.OnPaint(e);
        }

        protected double x_offset, y_offset, x_scale, y_scale;

        protected Bitmap BackgroundImage;



        public void UpdateTrackmap()
        {
            BackgroundImage = new Bitmap(10+this.Size.Width, 10+this.Size.Height);
            Graphics g = Graphics.FromImage(BackgroundImage);
            g.FillRectangle(Brushes.Black, 0, 0, this.Size.Width, this.Size.Height);
            if (!Telemetry.m.Active_Session) return;
            if (Telemetry.m.Track == null || Telemetry.m.Track.Route == null || Telemetry.m.Track.Route.Racetrack==null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

float track_width = 6f;
            float pitlane_width =4f;

            Pen brush_start = new Pen(Color.FromArgb(200, 50, 30), track_width);
            Brush brush_sector1 = new SolidBrush(Color.FromArgb(105, 105, 105));
            Brush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
            Brush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));
            Brush brush_sector1_pits = new SolidBrush(Color.FromArgb(105 * 10 / 15, 105 * 10 / 15, 105 * 10 / 15));
            Brush brush_sector2_pits = new SolidBrush(Color.FromArgb(47 * 10 / 15, 79 * 10 / 15, 79 * 10 / 15));
            Brush brush_sector3_pits = new SolidBrush(Color.FromArgb(85 * 10 / 15, 107 * 10 / 15, 47 * 10 / 15));
            double ZoomFactor = 20 + Math.Abs(Math.Max(Math.Max(Telemetry.m.Sim.Player.Speed, Telemetry.m.Sim.Drivers.Player.Speed), Telemetry.m.Sim.Player.SpeedSlipping) * 3.6 * 1.5);
            ZoomFactor = 200 + Math.Max(25, ZoomFactor);
                track_width = Convert.ToSingle(200/ZoomFactor*30);
                pitlane_width = Convert.ToSingle(200/ZoomFactor*20);
                if (!StaticTrackMap)
                {
                    Telemetry.m.Track.Route.x_max = Telemetry.m.Sim.Drivers.Player.CoordinateX + ZoomFactor;
                    Telemetry.m.Track.Route.x_min = Telemetry.m.Sim.Drivers.Player.CoordinateX - ZoomFactor;

                    Telemetry.m.Track.Route.y_max = Telemetry.m.Sim.Drivers.Player.CoordinateZ + ZoomFactor;
                    Telemetry.m.Track.Route.y_min = Telemetry.m.Sim.Drivers.Player.CoordinateZ - ZoomFactor;
                }

            x_scale = (this.Size.Width - 40) / (Telemetry.m.Track.Route.x_max - Telemetry.m.Track.Route.x_min);
            y_scale = -1 * (this.Size.Width - 40) / (Telemetry.m.Track.Route.y_max - Telemetry.m.Track.Route.y_min);
            x_offset = 20+x_scale * (0 - Telemetry.m.Track.Route.x_min);
            y_offset = this.Size.Height -20+y_scale * (0 - Telemetry.m.Track.Route.y_min);
            if (StaticTrackMap)
            {
                // TODO: Dit is verrot.
                double max = Math.Abs((Telemetry.m.Track.Route.y_max - Telemetry.m.Track.Route.y_min) * y_scale);
                if (max + 100 > this.Size.Height)
                {
                    y_scale *= this.Size.Height / (max + 200);
                    x_scale *= this.Size.Height / (max + 200);
                }

                if (this.Size.Height > this.Size.Width)
                {
                    double offset = 100 - (this.Size.Height - this.Size.Width);

                    y_scale = y_scale / this.Size.Height * (this.Size.Height + offset);
                    x_scale = x_scale / this.Size.Height * (this.Size.Height + offset);

                    y_offset += offset;
                }
                else
                {
                    double offset = 100;

                    y_offset += 100;
                }
            }
            x_scale = Math.Min(x_scale, -1 * y_scale);
            y_scale = -1*Math.Min(x_scale, -1 * y_scale);

            double zmax = double.MinValue, zmin = double.MaxValue;
            foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Pitlane)
            {
                zmax = Math.Max(wp.Y, zmax);
                zmin = Math.Min(wp.Y, zmin);
            }
            foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Racetrack)
            {
                zmax = Math.Max(wp.Y, zmax);
                zmin = Math.Min(wp.Y, zmin);
            }


            PointF pv = new PointF(float.MinValue, float.MinValue);
            PointF fpv = new PointF(float.MinValue, float.MinValue);

            List<PointF> sector1 = new List<PointF>();
            List<PointF> sector2 = new List<PointF>();
            List<PointF> sector3 = new List<PointF>();
            foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Pitlane)
            {
                float x = Convert.ToSingle(x_offset + x_scale * wp.X);
                float y = Convert.ToSingle(y_offset + y_scale * wp.Z);

                PointF p = new PointF(x, y);
                Brush b = brush_sector1;
                switch (wp.Sector)
                {
                    case 0:
                        b = brush_sector1_pits;
                        break;
                    case 1:
                        b = brush_sector2_pits;
                        break;
                    case 2:
                        b = brush_sector3_pits;
                        break;
                }

                if (pv.X != float.MinValue)
                {
                            g.FillEllipse(b, p.X, p.Y, pitlane_width, pitlane_width);

                }
                else fpv = p;
                pv = p;
            }


            sector1 = new List<PointF>();
            sector2 = new List<PointF>();
            sector3 = new List<PointF>();
            fpv = new PointF(float.MinValue, float.MinValue);
            pv = new PointF(float.MinValue, float.MinValue);

            double wp_lastMeters = 0;
            //Lap CurrentLap = Telemetry.m.Track.GetCurrentLap();
            //Lap LastLap = Telemetry.m.Track.GetLastLap();
            Font f = new Font("Tahoma",9f);
            foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Racetrack)
            {
                float x = Convert.ToSingle(x_offset + x_scale * wp.X);
                float y = Convert.ToSingle(y_offset + y_scale * wp.Z);

                /*Pen mypen = new Pen(Color.FromArgb(100, Convert.ToInt32(Math.Round((wp.Y - zmin) / (zmax - zmin) * -110)) + 130, 20), 6);
                brush_sector1_pits = mypen;
                brush_sector2_pits = mypen;
                brush_sector3_pits = mypen;
                brush_sector1 = mypen;
                brush_sector2 = mypen;
                brush_sector3 = mypen;*/
                PointF p = new PointF(x, y);
                Brush b = brush_sector1;
                switch (wp.Sector)
                {
                    case 0:
                        b = brush_sector1;
                        break;
                    case 1:
                        b = brush_sector2;
                        break;
                    case 2:
                        b = brush_sector3;
                        break;
                }
                Pen pennetje = new Pen(b, 10f);


                if (pv.X != float.MinValue)
                {
                    //g.FillEllipse(b, p.X - track_width / 2, p.Y - track_width / 2, track_width, track_width);
                    g.DrawLine(pennetje, p, pv);

                }
                else fpv = p;


                
                // Is there a checkpoint of apexpoint here?
                /*int i = 0;
                foreach (KeyValuePair<double, string> chkpoint in Telemetry.m.Track.Sections.Lines)
                {
                    if (wp_lastMeters < chkpoint.Key && wp.Meters >= chkpoint.Key)
                    {
                        g.FillEllipse(Brushes.Purple, pv.X - track_width / 2, pv.Y - track_width / 2, track_width, track_width);
                        g.DrawString(chkpoint.Value, f, Brushes.Yellow, pv.X- 5*track_width, pv.Y - 48);
                        if (CurrentLap.Checkpoints != null && CurrentLap.Checkpoints[i] != -1 && i > 1)
                            g.DrawString(
                                String.Format("{0:00.000}", CurrentLap.Checkpoints[i] - CurrentLap.Checkpoints[i - 1]),
                                f, Brushes.White, pv.X -4*track_width, pv.Y - 35);
                        if (LastLap.Checkpoints != null && LastLap.Checkpoints[i] != -1)
                            g.DrawString(
                                String.Format("{0:00.000}", LastLap.Checkpoints[i] - LastLap.Checkpoints[i - 1]),
                                f, Brushes.DarkGray, pv.X -4*track_width, pv.Y - 20);

                    }
                    i++;

                }
                i = 0;
                foreach (KeyValuePair<double, string> apex in Telemetry.m.Track.Apexes.Positions)
                {
                    if (wp_lastMeters < apex.Key && wp.Meters >= apex.Key)
                    {
                        g.FillEllipse(Brushes.Blue, pv.X - track_width / 2, pv.Y - track_width / 2, track_width, track_width);

                        g.DrawString(apex.Value, f, Brushes.Yellow, pv.X-5*track_width, pv.Y - 48);
                        if (CurrentLap.ApexSpeeds != null  && CurrentLap.ApexSpeeds[i] != -1)
                            g.DrawString(
                                String.Format("{0:000.0}km/h", CurrentLap.ApexSpeeds[i] * 3.6),
                                f, Brushes.White, pv.X -4* track_width, pv.Y - 35);
                        if (LastLap.ApexSpeeds != null && LastLap.ApexSpeeds[i] != -1)
                            g.DrawString(
                                String.Format("{0:000.0}km/h", LastLap.ApexSpeeds[i] * 3.6),
                                f, Brushes.DarkGray, pv.X -4* track_width, pv.Y - 20);

                    }
                    i++;
                }*/

                pv = p;
                wp_lastMeters = wp.Meters;
            }

            g.DrawPolygon(brush_start, new PointF[3] { fpv, pv, fpv });
            // draw track name);
            try
            {

                System.Drawing.Font tf24 = new Font("calibri", 24f);
                System.Drawing.Font tf16 = new Font("calibri", 16f);
                System.Drawing.Font tf12 = new Font("calibri", 12f);
                System.Drawing.Font tf18 = new Font("calibri", 18f);

                g.DrawString(Telemetry.m.Track.Name, tf24, Brushes.White, 10f, 10f);
                g.DrawString(Telemetry.m.Track.Location, tf18, Brushes.White, 10f, 40f);
                g.DrawString(Telemetry.m.Track.Length.ToString("0000.0m") + " , " + Telemetry.m.Track.Type, tf12, Brushes.White, 10f, 65f);

            }
            catch (Exception ex)
            {


            }
            //BackgroundImage.Save("current.png");

            this.Invalidate();

        }

        public TrackMap()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SizeChanged += new EventHandler(TrackMap_SizeChanged);
            Telemetry.m.Track_Load += new Triton.Signal(m_Track_Load);

            Timer t = new Timer();
            t.Interval = 250;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            UpdateTrackmap();
            this.Invalidate();
        }

        void TrackMap_SizeChanged(object sender, EventArgs e)
        {
            UpdateTrackmap();
        }

        void m_Track_Load(object sender)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Signal(m_Track_Load), new object[1] {sender});
                return;
            }
            UpdateTrackmap();
        }
    }
}