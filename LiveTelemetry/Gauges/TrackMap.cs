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
using System.Linq;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.ValueObjects;
using Triton;

namespace LiveTelemetry.Gauges
{
    public partial class TrackMap : UserControl
    {
        protected string TrackMapPainted = "";

        protected float pos_x_max;
        protected float pos_x_min;
        protected float pos_y_max;
        protected float pos_y_min;

        protected double map_width;
        protected double map_height;

        protected Bitmap _BackgroundTrackMap;

        private Timer _mUpdateBackground;


        #region Settings

        Brush brush_sector1 = new SolidBrush(Color.FromArgb(105, 105, 105));
        Brush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
        Brush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));

        Font tf24 = new Font("Calibri", 24f);
        Font tf16 = new Font("Calibri", 16f);
        Font tf12 = new Font("Calibri", 12f);
        Font tf18 = new Font("Calibri", 18f);

        #endregion


        public TrackMap()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            SizeChanged += TrackMap_SizeChanged;

            GlobalEvents.Hook<SessionStarted>(e => m_Track_Load(0), true);
            GlobalEvents.Hook<DrivingStarted>(e => m_Track_Load(0), true);

            _mUpdateBackground = new Timer {Interval = 1000};
            _mUpdateBackground.Tick += new EventHandler(_mUpdateBackground_Tick);
            _mUpdateBackground.Start();
        }

        void _mUpdateBackground_Tick(object sender, EventArgs e)
        {
            if (!IsValidTrackmap())
                UpdateTrackmap();
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            if (_BackgroundTrackMap == null)
            {
                g.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }
            else
            {
                g.DrawImage(_BackgroundTrackMap, 0, 0);
            }
        }

        protected bool IsValidTrackmap()
        {
            if (TelemetryApplication.TrackAvailable == false)
                return true;

            return (TrackMapPainted == TelemetryApplication.Track.ID);
        }

        public void UpdateTrackmap()
        {
            _BackgroundTrackMap = new Bitmap(10 + this.Size.Width, 10 + this.Size.Height);
            Graphics g = Graphics.FromImage(_BackgroundTrackMap);
            g.FillRectangle(Brushes.Black, 0, 0, this.Size.Width, this.Size.Height);

            if (TelemetryApplication.TrackAvailable == false) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            pos_x_max = TelemetryApplication.Track.TrackCoordinateMaxX;
            pos_y_max = TelemetryApplication.Track.TrackCoordinateMaxY;

            pos_x_min = TelemetryApplication.Track.TrackCoordinateMinX;
            pos_y_min = TelemetryApplication.Track.TrackCoordinateMinY;

            double scale_x = pos_x_max - pos_x_min;
            double scale_y = pos_y_max - pos_y_min;
            double scale = Math.Max(scale_x, scale_y);

            if (this.Height > this.Width)
            {
                map_width = this.Width;
                map_height = this.Height - 200;
                if (map_height / map_width > 1.2)
                    map_height = Math.Min(this.Height, this.Width);
            }
            else
            {
                map_height = this.Height - 200;
                map_width = this.Width;

            }

            Font f = new Font("Tahoma", 9f);
            List<PointF> sector1a = new List<PointF>();
            List<PointF> sector2a = new List<PointF>();
            List<PointF> sector3a = new List<PointF>();
            List<PointF> sector1b = new List<PointF>();
            List<PointF> sector2b = new List<PointF>();
            List<PointF> sector3b = new List<PointF>();

            TrackMapPainted = TelemetryApplication.Track.ID;

            // Create sector arrays.
            foreach (TrackPoint wp in TelemetryApplication.Track.Route)
            {
                if (wp.BoundsL == null || wp.BoundsR == null) continue;

                // Left side
                float x1 =
                    Convert.ToSingle(10 + ((wp.BoundsL[0] - pos_x_min) / scale_x) * (map_width - 20));
                float y1 =
                    Convert.ToSingle(100 +
                                     (1 - (wp.BoundsL[1] - pos_y_min) / scale_y) * (map_height - 20));
                // Right side
                float x2 =
                    Convert.ToSingle(10 + ((wp.BoundsR[0] - pos_x_min) / scale_x) * (map_width - 20));
                float y2 =
                    Convert.ToSingle(100 +
                                     (1 - (wp.BoundsR[1] - pos_y_min) / scale_y) * (map_height - 20));

                // Add by sector
                switch (wp.Type)
                {
                    case TrackPointType.SECTOR1:
                        sector1a.Add(new PointF(x1, y1));
                        sector1b.Add(new PointF(x2, y2));
                        break;
                    case TrackPointType.SECTOR2:
                        sector2a.Add(new PointF(x1, y1));
                        sector2b.Add(new PointF(x2, y2));
                        break;
                    case TrackPointType.SECTOR3:
                        sector3a.Add(new PointF(x1, y1));
                        sector3b.Add(new PointF(x2, y2));
                        break;
                }
            }

            // Add overlapping sections.
            var sector1aLast = sector1a.LastOrDefault();
            var sector2aLast = sector2a.LastOrDefault();
            var sector3aLast = sector3a.LastOrDefault();

            var sector1bLast = sector1b.LastOrDefault();
            var sector2bLast = sector2b.LastOrDefault();
            var sector3bLast = sector3b.LastOrDefault();

            // Insert them at the beginning of each.
            sector2a.Insert(0, sector1aLast);
            sector3a.Insert(0, sector2aLast);
            sector1a.Insert(0, sector3aLast);

            sector2b.Insert(0, sector1bLast);
            sector3b.Insert(0, sector2bLast);
            sector1b.Insert(0, sector3bLast);

            // Reverse 'right side' of the track and add it opposite to left side, so a polygon is created which can be filled.
            sector1b.Reverse();
            sector2b.Reverse();
            sector3b.Reverse();
            sector1a.AddRange(sector1b);
            sector2a.AddRange(sector2b);
            sector3a.AddRange(sector3b);

            // Draw the track itself.
            if (sector1a.Count > 0) g.FillPolygon(brush_sector1, sector1a.ToArray());
            if (sector2a.Count > 0) g.FillPolygon(brush_sector2, sector2a.ToArray());
            if (sector3a.Count > 0) g.FillPolygon(brush_sector3, sector3a.ToArray());

            // Draw track details.
            g.DrawString(TelemetryApplication.Track.Name, tf24, Brushes.White, 10f, 10f);
            g.DrawString(TelemetryApplication.Track.Location, tf18, Brushes.White, 10f, 40f);
            g.DrawString(TelemetryApplication.Track.Length.ToString("0000.0m") + " , " + TelemetryApplication.Track.Version, tf12, Brushes.White, 10f, 65f);

            Invalidate();
        }

        private void TrackMap_SizeChanged(object sender, EventArgs e)
        {
            UpdateTrackmap();
        }

        private void m_Track_Load(object sender)
        {
            if (InvokeRequired)
            {
                Invoke(new Signal(m_Track_Load), new object[1] { sender });
                return;
            }
            UpdateTrackmap();
        }
    }
}
