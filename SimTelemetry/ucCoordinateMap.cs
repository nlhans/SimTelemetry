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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Controls;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;

namespace SimTelemetry
{
    public partial class ucCoordinateMap : TrackMap
    {
        private TelemetryViewer _mMaster;
        public ucCoordinateMap(TelemetryViewer master)
        {
            InitializeComponent();
            _mMaster = master;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
         {
            if (_mMaster.Data == null)
            {
                e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
                return;
            }

            try
            {
                // Search for buonds
                if (_mMaster.Data != null)
                {
                    lock (_mMaster.Data)
                    {
                        foreach (KeyValuePair<double, TelemetrySample> s in _mMaster.Data.Samples)
                        {
                            if (_mMaster.TimeLine[1] >= s.Key/1000.0 && s.Key/1000.0 >= _mMaster.TimeLine[0])
                            {
                                pos_x_max = (float)Math.Max(_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateX"), pos_x_max);
                                pos_x_min = (float)Math.Min(_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateX"), pos_x_min);

                                pos_y_max = (float)Math.Max(_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateZ"), pos_y_max);
                                pos_y_min = (float)Math.Min(_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateZ"), pos_y_min);
                            }
                        }

                        //:???
                        float x_d = pos_x_max - pos_x_min;
                        float y_d = pos_y_max - pos_y_min;
                        float d = Math.Max(y_d, x_d);

                        pos_x_min -= (x_d - d)/2.0f;
                        pos_x_min += (x_d - d)/2.0f;

                        pos_y_min -= (x_d - d)/2.0f;
                        pos_y_min += (x_d - d)/2.0f;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            base.OnPaint(e);
            try
            {
                Graphics g = e.Graphics;
                Rectangle bounds = e.ClipRectangle;
                Pen PenBlack = new Pen(Color.Black, 1f);


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
                    map_height = this.Height - 200;
                }

                //g.FillRectangle(new SolidBrush(new PlotterPalette().Background), bounds);
                g.DrawImage(_EmptyTrackMap, 0, 0);

                double px = 0;
                double py = 0;
                Pen whPen = new Pen(Color.FromArgb(200, 200, 200), 1.0f);
                double LeastTime = 0;
                double Leastdt = 200000000;
                int i = 0;
                double TimeOffset = double.NegativeInfinity;

                if (_mMaster.Data != null)
                {
                    lock (_mMaster.Data.Samples)
                    {

                        foreach (KeyValuePair<double, TelemetrySample> s in _mMaster.Data.Samples)
                        {
                            if (TimeOffset == double.NegativeInfinity)
                                TimeOffset = s.Key / 1000.0;
                            if (_mMaster.TimeLine[1] >= s.Key / 1000.0 && s.Key / 1000.0 >= _mMaster.TimeLine[0])
                            {
                                double dt = _mMaster.TimeCursor[1] - (s.Key / 1000.0 - TimeOffset);

                                if (Math.Abs(dt) < Leastdt)
                                {
                                    Leastdt = Math.Abs(dt);
                                    LeastTime = s.Key;

                                }

                                double x = 10 + ((_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateX") - pos_x_min) / (pos_x_max - pos_x_min)) * (map_width - 20);
                                double y = 100 + (1 - (_mMaster.Data.GetDouble(s.Key, "Driver.CoordinateZ") - pos_y_min) / (pos_y_max - pos_y_min)) * (map_height - 20);

                                if (px == 0 || Math.Abs(x - px) > 4 || Math.Abs(y - py) > 4)
                                {
                                    if (px != 0 && py != 0)
                                    {
                                        g.DrawLine(new Pen(Color.FromArgb(Convert.ToInt32(_mMaster.Data.GetDouble(s.Key, "Player.Pedals_Brake") * 255), Convert.ToInt32(_mMaster.Data.GetDouble(s.Key, "Player.Pedals_Throttle") * 255), 0), 3f), x, y, px, py);
                                    }

                                    px = x;
                                    py = y;
                                }
                            }
                            i++;
                        }

                        if (_mMaster.TimeCursor[1] > 0 && Math.Abs(Leastdt) < 2000)
                        {
                            double x = 10 + ((_mMaster.Data.GetDouble(LeastTime, "Driver.CoordinateX") - pos_x_min) / (pos_x_max - pos_x_min)) * (map_width - 20);
                            double y = 100 + (1 - (_mMaster.Data.GetDouble(LeastTime, "Driver.CoordinateZ") - pos_y_min) / (pos_y_max - pos_y_min)) * (map_height - 20);
                            g.FillEllipse(new SolidBrush(Color.Yellow), x - 3, y - 3, 6, 6);

                        }
                    }
                }
            }

            catch (Exception ex)
            {
            }
         }
    }
}