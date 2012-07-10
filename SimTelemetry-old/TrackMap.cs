using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data;

namespace SimTelemetry
{
    public partial class TrackMap : UserControl
    {
        public List<DataSample> Samples = new List<DataSample>();

        public double TimeCursor = 0;
        public double TimeOffset = 0;

        public double Time_Bounds_Max = 0;
        public double Time_Bounds_Min = 0;

        public TrackMap()
        {
            InitializeComponent();

            this.map.Paint += new PaintEventHandler(map_Paint);
        }

        void map_Paint(object sender, PaintEventArgs e)
        {
            try{
            Graphics g = e.Graphics;
            Rectangle bounds = e.ClipRectangle;
            Pen PenBlack = new Pen(Color.Black, 1f);

            bounds.Width = Math.Min(bounds.Width, bounds.Height);
            bounds.Height = bounds.Width;

            g.FillRectangle(Brushes.White, bounds);

            double px = 0;
            double py = 0;


            double x_max = -100000;
            double x_min = 100000;
            double y_max = -100000;
            double y_min = 1000000;

            // Search for buonds
            foreach(DataSample s in Samples)
            {
                if (Time_Bounds_Max >= s.Time - TimeOffset && s.Time - TimeOffset >= Time_Bounds_Min)
                {
                    x_max = Math.Max(s.Drivers[0].CoordinateZ, x_max);
                    x_min = Math.Min(s.Drivers[0].CoordinateZ, x_min);

                    y_max = Math.Max(s.Drivers[0].CoordinateX, y_max);
                    y_min = Math.Min(s.Drivers[0].CoordinateX, y_min);
                }
            }

            double x_d = x_max - x_min;
            double y_d = y_max - y_min;
            double d = Math.Max(y_d, x_d);

            x_min -= (x_d - d) / 2;
            x_min += (x_d - d) / 2;

            y_min -= (x_d - d) / 2;
            y_min += (x_d - d) / 2;

            int LeastIndex = 0;
            double Leastdt = 20000;
            int i = 0;
            lock (Samples)
            {
            foreach(DataSample s in Samples)
            {
                if (Time_Bounds_Max >= s.Time - TimeOffset && s.Time - TimeOffset >= Time_Bounds_Min)
                {
                    double CorrectedTime = s.Time - TimeOffset;

                    double dt = TimeCursor - CorrectedTime;

                    if (Math.Abs(dt) < Leastdt)
                    {
                        Leastdt = Math.Abs(dt);
                        LeastIndex = i;

                    }

                    double x = 10 + (1 - (s.Drivers[0].CoordinateZ - x_min)/(x_max - x_min))*(bounds.Width - 20);
                    double y = 10 + (1 - (s.Drivers[0].CoordinateX - y_min)/(y_max - y_min))*(bounds.Height - 20);

                    if (px != 0 && py != 0)
                    {
                        g.DrawLine(new Pen(Color.FromArgb(Convert.ToInt32(s.Player.Pedals_Brake*255), Convert.ToInt32(s.Player.Pedals_Throttle*255), 0),3f), x, y, px, py);

                    }

                    px = x;
                    py = y;
                }
                i++;
            }

            if (TimeCursor > 0)
            {
                    DataSample cursor = Samples[LeastIndex];

                    double x = 10 + (1 - (cursor.Drivers[0].CoordinateZ - x_min)/(x_max - x_min))*(bounds.Width - 20);
                    double y = 10 + (1 - (cursor.Drivers[0].CoordinateX - y_min)/(y_max - y_min))*(bounds.Height - 20);
                    g.FillEllipse(new SolidBrush(Color.DarkBlue), x - 3, y - 3, 6, 6);
                
            }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        public void Draw()
        {
            map.Invalidate();
        }
    }
}
