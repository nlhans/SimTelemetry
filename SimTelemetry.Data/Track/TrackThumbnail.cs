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
using System.Text;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Track
{
    public class TrackThumbnail
    {
        public TrackThumbnail()
        {


        }

        protected double pos_x_max = 1000000000.0;
        protected double pos_x_min = -1000000000.0;
        protected double pos_y_max = 1000000000.0;
        protected double pos_y_min = -1000000000.0;
        protected double map_width = 0;
        protected double map_height = 0;

        #region Settings
        protected bool AutoPosition = true;

        float track_width = 5f;
        float pitlane_width = 0f;

        Pen brush_start = new Pen(Color.FromArgb(200, 50, 30), 6f); // 6f=track_width
        Brush brush_sector1 = new SolidBrush(Color.FromArgb(105, 105, 105));
        Brush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
        Brush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));
        Brush brush_pitlane = new SolidBrush(Color.FromArgb(100, Color.Orange));

        Font tf24 = new Font("calibri", 24f);
        Font tf16 = new Font("calibri", 16f);
        Font tf12 = new Font("calibri", 12f);
        Font tf10 = new Font("calibri", 10f);
        Font tf18 = new Font("calibri", 18f);
        Font font_version = new Font("calibri", 24f, FontStyle.Bold | FontStyle.Italic);
        #endregion

        public void Create(string file, string name, string version, IEnumerable<TrackPoint> route, int width, int height)
        {
            Pen pen_track = new Pen(brush_sector1, track_width);
            try
            {
                Image track_img = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(track_img);
                g.FillRectangle(Brushes.Black, 0, 0, width, height);

                if (route == null) return;

                g.SmoothingMode = SmoothingMode.AntiAlias;

                pos_x_max = -100000;
                pos_x_min = 100000;
                pos_y_max = -100000;
                pos_y_min = 1000000;

                foreach (var wp in route)
                {
                    if (wp.Type != TrackPointType.GRID)
                    {
                        pos_x_max = Math.Max(wp.X, pos_x_max);
                        pos_x_min = Math.Min(wp.X, pos_x_min);
                        pos_y_max = Math.Max(wp.Y, pos_y_max);
                        pos_y_min = Math.Min(wp.Y, pos_y_min);
                    }
                }


                double scale = Math.Max(pos_x_max - pos_x_min, pos_y_max - pos_y_min);
                double map_width = width - 12;
                double map_height = height - 12;

                double offset_x = map_width/2 - (pos_x_max - pos_x_min)/scale*map_width/2;
                double offset_y = 0-(scale - pos_y_max + pos_y_min)/scale*map_height/2;
                bool swap_xy = pos_x_max + pos_x_min < pos_y_max + pos_y_min;
                var track = new List<PointF>();

                int i = 0;
                foreach (var wp in route)
                {
                    if (wp.Type != TrackPointType.GRID)
                    {
                        float x1 = Convert.ToSingle(6 + ((wp.X - pos_x_min)/scale*map_width) + offset_x);
                        float y1 = Convert.ToSingle(6 + (1 - (wp.Y - pos_y_min)/scale)*map_height + offset_y);

                        x1 = Limits.Clamp(x1, -1000, 1000);
                        y1 = Limits.Clamp(y1, -1000, 1000);

                        if (swap_xy)
                            track.Add(new PointF(y1, x1));
                        else
                            track.Add(new PointF(x1, y1));
                    }
                }

                // Draw polygons!
                if (track.Count > 0) g.DrawPolygon(pen_track, track.ToArray());

                g.DrawString(version, font_version, Brushes.DarkRed, 5.0f, 5.0f);
                //g.DrawString(name, tf18, Brushes.White, 3.0f, Convert.ToSingle(map_height - 19.0f));

                track_img.Save(file);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
