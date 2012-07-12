using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;

namespace SimTelemetry
{
    public partial class ucCoordinateMap : UserControl
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
            try
            {
                Graphics g = e.Graphics;
                Rectangle bounds = e.ClipRectangle;
                Pen PenBlack = new Pen(Color.Black, 1f);

                bounds.Width = Math.Min(bounds.Width, bounds.Height);
                bounds.Height = bounds.Width;

                g.FillRectangle(new SolidBrush(new PlotterPalette().Background), bounds);

                double px = 0;
                double py = 0;


                double x_max = -100000;
                double x_min = 100000;
                double y_max = -100000;
                double y_min = 1000000;

                // Search for buonds
                lock (_mMaster.Data)
                {
                foreach (KeyValuePair<double, TelemetrySample> s in _mMaster.Data)
                {
                    if (_mMaster.TimeLine[1] >= s.Key / 1000.0 && s.Key / 1000.0 >= _mMaster.TimeLine[0])
                    {
                        x_max = Math.Max(s.Value["Drivers.Coordinate_Z"], x_max);
                        x_min = Math.Min(s.Value["Drivers.Coordinate_Z"], x_min);

                        y_max = Math.Max(s.Value["Drivers.Coordinate_X"], y_max);
                        y_min = Math.Min(s.Value["Drivers.Coordinate_X"], y_min);
                    }
                }

                double x_d = x_max - x_min;
                double y_d = y_max - y_min;
                double d = Math.Max(y_d, x_d);

                x_min -= (x_d - d) / 2;
                x_min += (x_d - d) / 2;

                y_min -= (x_d - d) / 2;
                y_min += (x_d - d) / 2;

                    Pen whPen = new Pen(Color.FromArgb(200,200,200), 1.0f);
                double  LeastTime = 0;
                double Leastdt = 200000000;
                    double TimeOffset = double.NegativeInfinity;
                int i = 0;
                    foreach (KeyValuePair<double, TelemetrySample> s in _mMaster.Data)
                    {
                        if(TimeOffset == double.NegativeInfinity)
                            TimeOffset = s.Key/1000.0;
                        if (_mMaster.TimeLine[1] >= s.Key / 1000.0 && s.Key / 1000.0 >= _mMaster.TimeLine[0])
                        {
                            double dt = _mMaster.TimeCursor[1] - (s.Key/1000.0 - TimeOffset);

                            if (Math.Abs(dt) < Leastdt)
                            {
                                Leastdt = Math.Abs(dt);
                                LeastTime = s.Key;

                            }

                            double x = 10 + (1 - (s.Value["Drivers.Coordinate_Z"] - x_min) / (x_max - x_min)) * (bounds.Width - 20);
                            double y = 10 + (1 - (s.Value["Drivers.Coordinate_X"] - y_min) / (y_max - y_min)) * (bounds.Height - 20);

                            if (px == 0 || Math.Abs(x - px) > 4 || Math.Abs(y - py) > 4)
                            {
                            if (px != 0 && py != 0)
                            {
                                g.DrawLine(new Pen(Color.FromArgb(Convert.ToInt32(s.Value["Drivers.Pedals_Brake"] * 255), Convert.ToInt32(s.Value["Drivers.Pedals_Throttle"] * 255), 0), 3f), x, y, px, py);
                            }

                                px = x;
                                py = y;
                            }
                        }
                        i++;
                    }

                    if (_mMaster.TimeCursor[1] > 0 && Math.Abs(Leastdt) < 2000)
                    {
                        TelemetrySample cursor = _mMaster.Data[LeastTime];

                        double x = 10 + (1 - (cursor["Drivers.Coordinate_Z"] - x_min) / (x_max - x_min)) * (bounds.Width - 20);
                        double y = 10 + (1 - (cursor["Drivers.Coordinate_X"] - y_min) / (y_max - y_min)) * (bounds.Height - 20);
                        g.FillEllipse(new SolidBrush(Color.Yellow), x - 3, y - 3, 6, 6);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

            base.OnPaint(e);
        }
    }
}
