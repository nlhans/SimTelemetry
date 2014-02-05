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
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.ValueObjects;
using Triton;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectModel_EngineCurve : UserControl
    {
        private Car _car;

        private int _Settings_mode = 0;
        private double _Settings_speed = 0;
        private double _Settings_throttle = 1.0;

        public int Settings_Mode { get { return _Settings_mode; } }
        public double Settings_Speed { get { return _Settings_speed; } }
        public double Settings_Throttle { get { return _Settings_throttle; } }

        public event AnonymousSignal CurveUpdated;

        public ucSelectModel_EngineCurve()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            //cb_mode.SelectedIndex = 0;
            tb_throttle.Value = 100;
            tb_speed.Text = "0";

        }

        public void Load(Car car)
        {
            if (car != null && car.Engine != null)
            {
                _car = car;
                this.BackColor = Color.Black;

                _Settings_mode = 0;
                _Settings_speed = 0;
                _Settings_throttle = 1;

                this.Invalidate();
                cb_mode.Items.Clear();
                cb_mode.DisplayMember = "mode";
                cb_mode.ValueMember = "index";
                foreach (EngineMode mode in car.Engine.Modes)
                {
                    cb_mode.Items.Add(mode);
                }

                if (cb_mode.Items.Count >= _Settings_mode)
                    cb_mode.SelectedIndex = _Settings_mode;
            }
        }
        public void Resize()
        {
            this.panel1.Size = new Size(this.Width, 40);
            this.panel1.Location = new Point(0, this.Height - 40);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            
            Font label_font = new Font("Tahoma", 10.0f);
            Pen gridPen = new Pen(Color.Gray, 1.0f);
            Pen axisPen = new Pen(Color.White, 1.0f);
            if(_car !=null)
            {
                _car.Engine.Apply(_Settings_speed, _Settings_throttle, _Settings_mode);
                // Draw grid
                double max_y = Math.Max(_car.Engine.MaximumPower, _car.Engine.MaximumTorque);
                double max_rpm = Math.Max(_car.Engine.MaximumRpm.Maximum, 0.0);

                var curve_power = _car.Engine.GetPowerCurve();
                var curve_torque = _car.Engine.GetTorqueCurve();

                if (_Settings_mode != 0)
                {
                    foreach (KeyValuePair<double, double> kvp in curve_power)
                        max_y = Math.Max(kvp.Value, max_y);
                    foreach (KeyValuePair<double, double> kvp in curve_torque)
                        max_y = Math.Max(kvp.Value, max_y);
                }

                double max_x = Math.Ceiling(max_rpm / 500.0) * 500.0; // steps of 500rpm

                double step_y = 100;
                if (max_y < 100)
                    step_y = 10;
                if (max_y < 200 && max_y >= 100)
                    step_y = 20;
                if (max_y < 400 && max_y >= 200)
                    step_y = 25;
                if (max_y < 700 && max_y >= 400)
                    step_y = 50;
                max_y = Math.Ceiling(max_y / step_y) * step_y; // steps of 50
                double step_x = 500;
                if (max_x > 10000)
                    step_x = 1000;

                int labelsLeft = 50;
                int labelsBot = 30;

                double graph_x = e.ClipRectangle.Width - 10 - labelsLeft;
                double graph_y = e.ClipRectangle.Height - 10 - labelsBot;
                // Draw grid
                for (double rpm = 0; rpm <= max_x; rpm += step_x)
                {
                    // https://www.google.nl/search?q=rotated+string+draw+C%23&oq=rotated+string+draw+C%23&sugexp=chrome,mod=0&sourceid=chrome&ie=UTF-8
                    float x = Convert.ToSingle(labelsLeft + rpm / max_x * graph_x);
                    g.DrawLine(gridPen, x, 10, x, e.ClipRectangle.Height - labelsBot);;
                    string r_str = "";
                    if (step_x == 1000)
                        r_str = (rpm / 1000.0).ToString("00");
                    else
                        r_str = (rpm / 1000.0).ToString("0.0");
                    
                    g.DrawString(r_str, label_font, Brushes.LightGray, x-10,
                                 e.ClipRectangle.Height - labelsBot + 10);
                }
                for (double v = 0; v <= max_y; v += step_y)
                {
                    float y = Convert.ToSingle(10 + v/max_y* graph_y);
                    if (y < 10 || float.IsNaN(y) || float.IsInfinity(y))
                        y = 10;
                    g.DrawLine(gridPen, labelsLeft, y, e.ClipRectangle.Width - 10, y);
                    string a_str = (max_y-v).ToString();

                    g.DrawString(a_str, label_font, Brushes.LightGray, 10,
                                 y-7);
                }

                List<PointF> graph1 = new List<PointF>();
                List<PointF> graph2 = new List<PointF>();

                int index = -1;
                for (int rpm = 0; rpm <= max_rpm; rpm += 100)
                {
                    double x = labelsLeft + rpm / max_x * graph_x;
                    if (x < labelsLeft) x = labelsLeft;
                    double y1 = 10+(1 - Math.Max(0,curve_power[rpm]) / max_y) * graph_y;
                    double y2 = 10 + (1 - Math.Max(0, curve_torque[rpm]) / max_y) * graph_y;
                    if (y1 < 10 || double.IsNaN(y1) || double.IsInfinity(y1)) y1 = 10 + graph_y;
                    if (y2 < 10 || double.IsNaN(y2) || double.IsInfinity(y2)) y2 = 10 + graph_y;

                    graph1.Add(new PointF(Convert.ToSingle(x), Convert.ToSingle(y1)));
                    graph2.Add(new PointF(Convert.ToSingle(x), Convert.ToSingle(y2)));

                    index++;
                }

                for (; index >= 0;index-- )
                {
                    graph1.Add(graph1[index]);
                    graph2.Add(graph2[index]);
                }

                // Maximum RPM
                float x_max = Convert.ToSingle(labelsLeft + _car.Engine.MaximumRpm.Maximum / max_x * graph_x);
                g.DrawLine(new Pen(Color.DarkBlue, 2.0f), x_max, 10, x_max, e.ClipRectangle.Height - labelsBot);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Power/torque curves
                g.DrawPolygon(new Pen(Color.Red, 2.0f), graph1.ToArray());
                g.DrawPolygon(new Pen(Color.Green, 2.0f), graph2.ToArray());

                g.SmoothingMode = SmoothingMode.HighSpeed;

                // Draw axis
                g.DrawLine(axisPen, labelsLeft, 10, labelsLeft, e.ClipRectangle.Height - labelsBot);
                g.DrawLine(axisPen, labelsLeft, e.ClipRectangle.Height - labelsBot, e.ClipRectangle.Width - 10, e.ClipRectangle.Height - labelsBot);

                g.DrawString("[Green] Torque", label_font, Brushes.Green,55, 14);
                g.DrawString("[Red] Power", label_font, Brushes.Red, 55, 36);

                if (CurveUpdated != null)
                    CurveUpdated();
            }
        }

        private void tb_throttle_ValueChanged(object sender, EventArgs e)
        {
            _Settings_throttle = tb_throttle.Value/100.0;
            lbl_throttle.Text = "Throttle " + tb_throttle.Value.ToString() + "%";
            this.Invalidate();
        }

        private void tb_speed_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(tb_speed.Text, out _Settings_speed))
            {
                if (_Settings_speed > 1000)
                    tb_speed.Text = "1000";
                else
                {
                    _Settings_speed /= 3.6;
                    this.Invalidate();
                }
            }

        }

        private void cb_mode_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((cb_mode.SelectedItem) != null)
            {
                _Settings_mode = ((EngineMode) cb_mode.SelectedItem).Index;
                this.Invalidate();
            }
        }
    }
}
