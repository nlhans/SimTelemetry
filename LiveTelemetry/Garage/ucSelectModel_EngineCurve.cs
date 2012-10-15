using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Objects.Garage;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectModel_EngineCurve : UserControl
    {
        private ICarEngine eng;
        private CarEngineTools tools;
        public ucSelectModel_EngineCurve()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        public void Load(ICar car)
        {
            CarEngineTools t = new CarEngineTools(car);
            t.Scan();
            Load(car, t);
        }
        public void Load(ICar car, CarEngineTools t)
        {
            this.BackColor = Color.Black;

            eng = car.Engine;
            tools = t;
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);
            
            Font label_font = new Font("Tahoma", 10.0f);
            Pen gridPen = new Pen(Color.Gray, 1.0f);
            Pen axisPen = new Pen(Color.White, 1.0f);
            if(eng !=null)
            {
                // Draw grid
                double max_y = Math.Max(tools.MaxPower_HP, tools.MaxTorque_NM);
                double max_x = eng.MaxRPM;

                max_y = Math.Ceiling(max_y / 15) * 15; // 6 ticks
                max_x = Math.Ceiling(max_x / 500.0) * 500.0; // to 500rpm

                double step_y = max_y/15;
                double step_x = 500;
                if (max_x > 10000)
                    step_x = 1000;

                int labelsLeft = 50;
                int labelsBot = 50;

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
                                 e.ClipRectangle.Height - 40);
                }
                for (double v = 0; v <= max_y; v += step_y)
                {
                    float y = Convert.ToSingle(10 + v/max_y* graph_y);
                    g.DrawLine(gridPen, labelsLeft, y, e.ClipRectangle.Width - 10, y);
                    string a_str = (max_y-v).ToString();

                    g.DrawString(a_str, label_font, Brushes.LightGray, 10,
                                 y-7);
                }

                Dictionary<double, double> curve_power = eng.GetPowerCurve(0,1,0);
                Dictionary<double, double> curve_torque = eng.GetTorqueCurve(0, 1, 0);

                List<PointF> graph1 = new List<PointF>();
                List<PointF> graph2 = new List<PointF>();

                int index = -1;
                for (int rpm = 0; rpm <= eng.MaxRPM; rpm += 100)
                {
                    double x = labelsLeft + rpm / max_x * graph_x;
                    if (x < labelsLeft) x = labelsLeft;
                    double y1 = 10+(1 - Math.Max(0,curve_power[rpm]) / max_y) * graph_y;
                    double y2 = 10 + (1 - Math.Max(0, curve_torque[rpm]) / max_y) * graph_y;

                    graph1.Add(new PointF(Convert.ToSingle(x), Convert.ToSingle(y1)));
                    graph2.Add(new PointF(Convert.ToSingle(x), Convert.ToSingle(y2)));

                    index++;
                }

                for (; index >= 0;index-- )
                {
                    graph1.Add(graph1[index]);
                    graph2.Add(graph2[index]);
                }

                g.DrawPolygon(new Pen(Color.Red, 1.0f), graph1.ToArray());
                g.DrawPolygon(new Pen(Color.Green, 1.0f), graph2.ToArray());

                // Draw axis
                g.DrawLine(axisPen, labelsLeft, 10, labelsLeft, e.ClipRectangle.Height - labelsBot);
                g.DrawLine(axisPen, labelsLeft, e.ClipRectangle.Height - labelsBot, e.ClipRectangle.Width - 10, e.ClipRectangle.Height - labelsBot);

                g.DrawString("[Green] Torque", label_font, Brushes.Green,55, 14);
                g.DrawString("[Red] Power", label_font, Brushes.Red, 55, 36);

            }
        }
    }
}
