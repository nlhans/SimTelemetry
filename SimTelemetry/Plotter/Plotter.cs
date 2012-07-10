using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Triton;

namespace SimTelemetry
{
    public class PlotterPalette
    {
        public Color Background { get { return Color.FromArgb(255,20,20,20); } }
        public Color AxisLine { get { return Color.FromArgb(255, 230,230, 230); } }
        public Color AxisText { get { return Color.FromArgb(255, 230, 230, 230); } }
        public Color Text { get { return Color.YellowGreen; } }
        public Color Window { get { return Color.LightGray; } }
        public Color Cursor { get { return Color.DarkRed; } }
        public Color[] Graphs
        {
            get
            {
                return new Color[4]
                           {
                               Color.Cyan,
                               Color.LightGreen,
                               Color.LightYellow,
                               Color.LightBlue
                           };
            }
        }
    }
    public partial class Plotter : UserControl
    {
        public List<PlotterGraph> Graphs = new List<PlotterGraph>();

        private int Mouse_X = 0;
        private int Mouse_Y = 0;

        public event AnonymousSignal Drawn;
        public double TimeCursor = 0;

        public double x_min = 0;
        public double x_max = 30;

        private double Best_Time;
        private PlotterPalette Palette { get { return new PlotterPalette(); } }

        private DateTime LastMoveDraw = DateTime.Now;

        public Plotter()
        {
            InitializeComponent();

            this.Resize += new EventHandler(Plotter_Resize);

            this.chart.Paint += new PaintEventHandler(chart_Paint);
            this.chart.Invalidate();

            this.chart.MouseLeave += new EventHandler(chart_MouseLeave);
            this.chart.MouseMove += new MouseEventHandler(chart_MouseMove);
        }



        void chart_MouseLeave(object sender, EventArgs e)
        {
            Mouse_X = -1;
            this.chart.Invalidate();
        }

        void chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.Now.Subtract(LastMoveDraw).TotalMilliseconds < 100) return;
            LastMoveDraw = DateTime.Now;
            Mouse_X = e.X;
            Mouse_Y = e.Y;
            this.chart.Invalidate();
            this.Focus();
        }

        void Plotter_Resize(object sender, EventArgs e)
        {
            this.chart.Invalidate();
        }

        void chart_Paint(object sender, PaintEventArgs e)
        {
            //ComputeAxis();
            try
            {
                Graphics gfx = e.Graphics;
                Rectangle bounds = e.ClipRectangle;
                gfx.FillRectangle(new SolidBrush(Palette.Background), bounds);
                Font f = new Font("Tahoma", 8);
                Pen AxisPen = new Pen(Palette.AxisLine, 1f);
                Pen WindowPen = new Pen(Palette.Window, 1f);
                int graphs = Graphs.Count;
                int BottomSize = 80;
                int TopSize = 15;
                int Rightsize = 30;

                int g_index = 0;

                // Max axis index
                int MaxAxisIndex = 0;
                foreach (PlotterGraph g in Graphs)
                {
                    if (g.Y_Axis.Count > MaxAxisIndex)
                        MaxAxisIndex = g.Y_Axis.Count;
                }

                double x_axis_min = x_min;
                double x_axis_max = x_max;
                double x_axis_step = 0;

                // How many charts?
                foreach (PlotterGraph g in Graphs)
                {
                    double height = bounds.Height - TopSize - BottomSize;

                    height /= graphs;

                    double base_y = height * g_index + TopSize;
                    height -= 9;

                    double base_x = 10 + 50 * MaxAxisIndex - 50 * g.Y_Axis.Count;

                    // Draw all y-axis
                    foreach (PlotterYAxis axis in g.Y_Axis)
                    {
                        double Max_Min = axis.max - axis.min;

                        double Draw_Min = Math.Ceiling(axis.min * 1.0 / axis.step) * axis.step;
                        double Draw_Max = Math.Floor(axis.max * 1.0 / axis.step) * axis.step;

                        axis.step = Math.Abs(axis.step);
                        if (Draw_Min == Draw_Max)
                        {

                            if (axis.step > Draw_Max - Draw_Min)
                            {
                                axis.step = 1;
                                axis.step_divider = axis.step / 5.0;
                            }
                            Draw_Max += axis.step;
                        }
                        else
                        {
                            if (axis.step > Draw_Max - Draw_Min)
                            {
                                axis.step = Draw_Max - Draw_Min;
                                axis.step_divider = axis.step / 5.0;
                            }
                        }
                        for (double v = Draw_Min; v <= Draw_Max; v += axis.step)
                        {
                            double __y = base_y + height * (1 - (v - axis.min) / (Max_Min));

                            if (v != Draw_Max)
                            {
                                for (double v2 = v; v2 <= v + axis.step; v2 += axis.step_divider)
                                {
                                    double __y2 = base_y + height * (1 - (v2 - axis.min) / (Max_Min));
                                    gfx.DrawLine(AxisPen, base_x + 43, __y2, base_x + 48, __y2);
                                }
                            }

                            gfx.DrawLine(AxisPen, base_x + 38, __y, base_x + 48, __y);

                            gfx.DrawString(Math.Round(v, axis.digits_comma).ToString(), f, new SolidBrush(Palette.AxisText), base_x + 10, __y - 5);
                        }

                        gfx.DrawLine(AxisPen, base_x + 48, base_y, base_x + 48, base_y + height); // left
                        base_x += 50;
                    }


                    // Draw line alongside..
                    gfx.DrawLine(WindowPen, bounds.Width - Rightsize, base_y, bounds.Width - Rightsize, base_y + height); // right
                    gfx.DrawLine(WindowPen, base_x - 50 + 48, base_y, bounds.Width - Rightsize, base_y); // top
                    gfx.DrawLine(WindowPen, base_x - 50 + 48, base_y + height, bounds.Width - Rightsize, base_y + height); // bottom

                    // Plot data
                    foreach (PlotterCurves c in g.Curves)
                    {
                        PlotterYAxis yx = g.Y_Axis[c.YAxis];

                        Best_Time = 0;
                        double Best_Value = 0;
                        double Best_Y = 0;
                        double Best_X = 0;
                        double Best_dX = 78990;
                        double pp_x = 0, pp_y = 0;
                        Pen line = new Pen(c.LineColor, Convert.ToSingle(c.LineThickness));
                        foreach (KeyValuePair<double, double> point in c.Data)
                        {

                            if (point.Key >= x_axis_min && point.Key <= x_axis_max)
                            {
                                double p_x = (point.Key - x_axis_min) / (x_axis_max - x_axis_min);
                                double p_y = (point.Value - yx.min) / (yx.max - yx.min);
                                if (p_y > 1) p_y = 1;
                                if (p_y < 0) p_y = 0;
                                if (double.IsInfinity(p_y)) p_y = 0.5;
                                if (double.IsNaN(p_y)) p_y = 0.5;
                                if (double.IsInfinity(p_x)) p_x = 0.5;
                                if (double.IsNaN(p_x)) p_x = 0.5;
                                p_y = 1 - p_y;

                                p_y *= height;
                                p_x *= (bounds.Width - Rightsize) - (base_x - 50 + 48);

                                p_x += base_x;
                                p_y += base_y;

                                double dx = Math.Abs(p_x - Mouse_X);


                                // clip outside boundaries.)
                                if (p_y <= base_y) p_y = base_y;
                                if (p_y >= base_y + height) p_y = base_y + height;


                                if (dx < Best_dX)
                                {
                                    Best_dX = dx;
                                    Best_X = p_x;
                                    Best_Time = point.Key;
                                    Best_Value = point.Value;
                                    Best_Y = p_y;
                                }

                                if (pp_x != 0 && pp_y != 0)
                                {
                                    gfx.DrawLine(line, pp_x, pp_y, p_x, p_y);
                                }

                                pp_x = p_x;
                                pp_y = p_y;

                            }


                        }

                        if (Mouse_X > base_x && Mouse_X < bounds.Width - Rightsize && Best_X != 0)
                        {
                            gfx.DrawString(Math.Round(Best_Value, yx.digits_comma).ToString(), f, new SolidBrush(c.LineColor), Best_X, Best_Y - 15);
                            gfx.FillEllipse(new SolidBrush(Palette.Cursor), Best_X - 3, Best_Y - 3, 6, 6);
                            TimeCursor = Best_Time;
                        }
                    }

                    g_index++;
                    if (Mouse_X > base_x && Mouse_X < bounds.Width - Rightsize)
                    {
                        gfx.DrawLine(new Pen(Palette.Cursor, 1f), Mouse_X, TopSize, Mouse_X, bounds.Height - BottomSize - Graphs.Count * 5);
                    }
                    
                }

                double bx = 10 + 50 * MaxAxisIndex;

                double time_stepsize = 5;
                double dt = x_max - x_min;

                if (dt > 0.1) time_stepsize = 0.01;
                if (dt > 0.5) time_stepsize = 0.05;
                if (dt > 1) time_stepsize = 0.1;
                if (dt > 2.5) time_stepsize = 0.25;
                if (dt > 5) time_stepsize = 0.5;
                if (dt > 10) time_stepsize = 1;
                if (dt > 20) time_stepsize = 2;
                if (dt > 30) time_stepsize = 2.5;
                if (dt > 60) time_stepsize = 5;
                if (dt > 2 * 60) time_stepsize = 10;
                if (dt > 3 * 60) time_stepsize = 20;

                for (double time = x_min; time <= x_max; time += time_stepsize)
                {

                    if (x_min == x_max) continue;
                    float dutycycle = Convert.ToSingle(((time - x_min) / (x_max - x_min)) * ((bounds.Width - Rightsize) - (bx - 50 + 48)) + bx);
                    
                    StringFormat strf = new StringFormat();
                    strf.FormatFlags = StringFormatFlags.DirectionVertical;

                    gfx.DrawString(time.ToString("00.00"), f, new SolidBrush(Palette.Text), dutycycle - 8f,
                                   Convert.ToSingle(bounds.Height - BottomSize + 5), strf);
                    gfx.DrawLine(AxisPen, dutycycle, Convert.ToSingle(bounds.Height - BottomSize - 3), dutycycle,
                                 Convert.ToSingle(bounds.Height - BottomSize - 9));

                }

                ThreadPool.QueueUserWorkItem(new WaitCallback(_FireDrawn));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void _FireDrawn(object o)
        {
            if (Drawn != null)
                Drawn();

        }

        public void Draw()
        {
            this.chart.Invalidate();

        }



        public void AutoScale()
        {
            try
            {
                int i_g = 0;

                foreach (PlotterGraph g in Graphs)
                {
                    List<double> axisMaxMap = new List<double>();
                    List<double> axisMinMap = new List<double>();
                    int i_c = 0;
                    foreach (PlotterCurves c in g.Curves)
                    {
                        if (g.Y_Axis[c.YAxis].auto == false) continue;
                        if (c.Data.Values.Count == 0) continue;
                        if (axisMaxMap.Count - c.YAxis >= 0)
                        {
                            for (int i = axisMaxMap.Count - 1; i < c.YAxis; i++)
                            {
                                axisMaxMap.Add(-100000);
                                axisMinMap.Add(100000);
                            }
                        }
                        foreach (double val in c.Data.Values)
                        {
                            if (axisMaxMap[c.YAxis] < val)
                                axisMaxMap[c.YAxis] = val;

                            if (axisMinMap[c.YAxis] > val)
                                axisMinMap[c.YAxis] = val;
                        }

                        i_c++;

                    }

                    int i_y = 0;
                    foreach (PlotterYAxis y in g.Y_Axis)
                    {
                        if (y.auto && axisMinMap.Count > i_y)
                        {
                            if (axisMaxMap[i_y] == axisMinMap[i_y])
                                axisMaxMap[i_y] = axisMinMap[i_y] + 1;

                            Graphs[i_g].Y_Axis[i_y].max = AutoScale_Autoranging(axisMaxMap[i_y], axisMinMap[i_y], true);
                            Graphs[i_g].Y_Axis[i_y].min = AutoScale_Autoranging(axisMaxMap[i_y], axisMinMap[i_y], false);
                            Graphs[i_g].Y_Axis[i_y].step = (y.max - y.min) / 10.0;
                            Graphs[i_g].Y_Axis[i_y].step_divider = y.step / 5;

                            // up to 5 significant digits
                            Graphs[i_g].Y_Axis[i_y].digits_comma = Convert.ToInt32(3-Math.Round(Math.Log(AutoScale_DetermineFactor(y.step))));
                            if (y.digits_comma < 0) Graphs[i_g].Y_Axis[i_y].digits_comma = 0;

                        }
                        i_y++;
                    }


                    i_g++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
        
        private double AutoScale_DetermineFactor(double v)
        {
            return AutoScale_DetermineFactor(v, v);
        }
        private double AutoScale_DetermineFactor(double v, double v2)
        {
            double factor = 0.0001;
            while (factor < v || factor < v2)
            {
                factor *= 10;
            }
            
            return factor;

        }

        private double AutoScale_Autoranging(double v, double v2, bool p2)
        {
            double factor = AutoScale_DetermineFactor(v, v2);
            factor /= 100;
            double division = 0;
            if (p2)
                division = v / factor;
            else
            {
                division = v2 / factor;
            }
            if (p2)
                return Math.Ceiling(division) * factor;
            else
            {
                return Math.Floor(division) * factor;
            }
        }
    }
}
