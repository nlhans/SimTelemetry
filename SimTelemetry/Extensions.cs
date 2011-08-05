using System;
using System.Drawing;

namespace SimTelemetry
{
    public  static partial class Extensions
    {
        public static void FillEllipse(this Graphics g, Brush b, double x, double y, double sx, double sy)
        {
            g.FillEllipse(b, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(sx), Convert.ToSingle(sy));
        }

        public static void DrawLine(this Graphics g, Pen p, double x, double y, double x2, double y2)
        {
            g.DrawLine(p, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(x2), Convert.ToSingle(y2));
        }
        public static void DrawEllipse(this Graphics g, Pen p, double x, double y, double sx, double sy)
        {
            g.DrawEllipse(p, Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(sx), Convert.ToSingle(sy));
        }

        public static void DrawString(this Graphics g, string s, System.Drawing.Font f, Brush b, double x, double y)
        {
            g.DrawString(s, f, b, Convert.ToSingle(x), Convert.ToSingle(y));
        }
    }
}