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
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects;
    using Triton;
    using Timer = System.Windows.Forms.Timer;

namespace SimTelemetry.Controls
{
    public partial class TrackMap : UserControl
    {
        public static bool StaticTrackMap = false; // only static atm

        public string AIW_File = "";

        // Settings for inherited classes
        protected bool AutoPosition = true;
        protected bool AccurateTrackWidth = false;

        protected double pos_x_max = 1000000000.0;
        protected double pos_x_min = -1000000000.0;
        protected double pos_y_max = 1000000000.0;
        protected double pos_y_min = -1000000000.0;
        protected double map_width = 0;
        protected double map_height = 0;
        protected WayPoint[] waypoints = new WayPoint[25001];
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            if (_EmptyTrackMap == null)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }
            else
            {
                g.DrawImage(_EmptyTrackMap, 0, 0);
            }
        }

        protected Bitmap _EmptyTrackMap;


        #region Settings
        float track_width = 6f;
        float pitlane_width = 4f;

        Pen brush_start = new Pen(Color.FromArgb(200, 50, 30), 6f); // 6f=track_width
        Brush brush_sector1 = new SolidBrush(Color.FromArgb(105, 105, 105));
        Brush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
        Brush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));
        Brush brush_pitlane = new SolidBrush(Color.FromArgb(100, Color.Orange));

        Font tf24 = new Font("calibri", 24f);
        Font tf16 = new Font("calibri", 16f);
        Font tf12 = new Font("calibri", 12f);
        Font tf18 = new Font("calibri", 18f);
        #endregion

        public void UpdateTrackmap()
        {
            _EmptyTrackMap = new Bitmap(10 + this.Size.Width, 10 + this.Size.Height);
            Graphics g = Graphics.FromImage(_EmptyTrackMap);
            g.FillRectangle(Brushes.Black, 0, 0, this.Size.Width, this.Size.Height);

            if (Telemetry.m.Track == null || Telemetry.m.Track.Route == null || Telemetry.m.Track.Route.Racetrack == null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (AutoPosition)
            {

                pos_x_max = -100000;
                pos_x_min = 100000;
                pos_y_max = -100000;
                pos_y_min = 1000000;

                if (Telemetry.m.Track.Route.Pitlane != null)
                {
                    foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Pitlane)
                    {
                        pos_x_max = Math.Max(wp.X, pos_x_max);
                        pos_x_min = Math.Min(wp.X, pos_x_min);
                        pos_y_max = Math.Max(wp.Z, pos_y_max);
                        pos_y_min = Math.Min(wp.Z, pos_y_min);
                    }
                }
                foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Racetrack)
                {
                    pos_x_max = Math.Max(wp.X, pos_x_max);
                    pos_x_min = Math.Min(wp.X, pos_x_min);
                    pos_y_max = Math.Max(wp.Z, pos_y_max);
                    pos_y_min = Math.Min(wp.Z, pos_y_min);
                }


            }

            if (this.Height > this.Width)
            {
                map_width = this.Width;
                map_height = this.Height - 150;
                if (map_height / map_width > 1.2)
                    map_height = Math.Min(this.Height, this.Width);
            }
            else
            {
                map_width = Math.Min(this.Height, this.Width);
                map_height = this.Height-200;
            }

            List<PointF> pitlane_a = new List<PointF>();
            List<PointF> pitlane_b = new List<PointF>();
            if (Telemetry.m.Track.Route.Pitlane != null)
            {
                foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Pitlane)
                {
                    float x1 =
                        Convert.ToSingle(10 + ((wp.CoordinateL[0] - pos_x_min)/(pos_x_max - pos_x_min))*(map_width - 20));
                    float y1 =
                        Convert.ToSingle(100 +
                                         (1 - (wp.CoordinateL[1] - pos_y_min)/(pos_y_max - pos_y_min))*(map_height - 20));
                    float x2 =
                        Convert.ToSingle(10 + ((wp.CoordinateR[0] - pos_x_min)/(pos_x_max - pos_x_min))*(map_width - 20));
                    float y2 =
                        Convert.ToSingle(100 +
                                         (1 - (wp.CoordinateR[1] - pos_y_min)/(pos_y_max - pos_y_min))*(map_height - 20));

                    // This is for your own safety :)
                    x1 = Limits.Clamp(x1, -100000, 100000);
                    y1 = Limits.Clamp(y1, -100000, 100000);
                    x2 = Limits.Clamp(x2, -100000, 100000);
                    y2 = Limits.Clamp(y2, -100000, 100000);

                    //g.FillEllipse(brush_pitlane, x1, y1, 6.0f, 6.0f);


                }
            }
            double wp_lastMeters = 0;
            Font f = new Font("Tahoma", 9f);
            List<PointF> sector1a = new List<PointF>();
            List<PointF> sector2a = new List<PointF>();
            List<PointF> sector3a = new List<PointF>();
            List<PointF> sector1b = new List<PointF>();
            List<PointF> sector2b = new List<PointF>();
            List<PointF> sector3b = new List<PointF>();
            
            foreach (TrackWaypoint wp in Telemetry.m.Track.Route.Racetrack)
            {
                float x1 = Convert.ToSingle(10 + ((wp.CoordinateL[0] - pos_x_min) / (pos_x_max - pos_x_min)) * (map_width - 20));
                float y1 = Convert.ToSingle(100 + (1 - (wp.CoordinateL[1] - pos_y_min) / (pos_y_max - pos_y_min)) * (map_height - 20));
                float x2 = Convert.ToSingle(10 + ((wp.CoordinateR[0] - pos_x_min) / (pos_x_max - pos_x_min)) * (map_width - 20));
                float y2 = Convert.ToSingle(100 + (1 - (wp.CoordinateR[1] - pos_y_min) / (pos_y_max - pos_y_min)) * (map_height - 20));

                // This is for your own safety :)
                x1 = Limits.Clamp(x1, -100000, 100000);
                y1 = Limits.Clamp(y1, -100000, 100000);
                x2 = Limits.Clamp(x2, -100000, 100000);
                y2 = Limits.Clamp(y2, -100000, 100000);

                switch(wp.Sector+1)
                {
                    case 1:
                        sector1a.Add(new PointF(x1, y1));
                        sector1b.Add(new PointF(x2, y2));
                        break;
                    case 2:
                        sector2a.Add(new PointF(x1, y1));
                        sector2b.Add(new PointF(x2, y2));
                        break;
                    case 3:
                        sector3a.Add(new PointF(x1, y1));
                        sector3b.Add(new PointF(x2, y2));
                        break;
                }
                wp_lastMeters = wp.Meters;
            }

            // Combine both a&b polygon , but in reverse (i.e. a + reverse b)
            sector1b.Reverse();
            sector2b.Reverse();
            sector3b.Reverse();
            pitlane_b.Reverse();
            sector1a.AddRange(sector1b);
            sector2a.AddRange(sector2b);
            sector3a.AddRange(sector3b);
            pitlane_a.AddRange(pitlane_b);

            // Draw polygons!
            if (pitlane_a.Count > 0) g.FillPolygon(brush_pitlane, pitlane_a.ToArray());
            if(sector1a.Count > 0)g.FillPolygon(brush_sector1, sector1a.ToArray());
            if (sector2a.Count > 0) g.FillPolygon(brush_sector2, sector2a.ToArray());
            if (sector3a.Count > 0) g.FillPolygon(brush_sector3, sector3a.ToArray());

            // draw track name
            try
            {
                g.DrawString(Telemetry.m.Track.Name, tf24, Brushes.White, 10f, 10f);
                g.DrawString(Telemetry.m.Track.Location, tf18, Brushes.White, 10f, 40f);
                g.DrawString(Telemetry.m.Track.Length.ToString("0000.0m") + " , " + Telemetry.m.Track.Type, tf12, Brushes.White, 10f, 65f);

            }
            catch (Exception ex)
            {


            }
            
            this.Invalidate();

        }

        public TrackMap()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SizeChanged += new EventHandler(TrackMap_SizeChanged);

            Telemetry.m.Track_Loaded += new Triton.Signal(m_Track_Load);
        }

        void TrackMap_SizeChanged(object sender, EventArgs e)
        {
            UpdateTrackmap();
        }

        void m_Track_Load(object sender)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Signal(m_Track_Load), new object[1] { sender });
                return;
            }
            UpdateTrackmap();
        }
    }
}