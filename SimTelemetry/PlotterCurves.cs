using System.Collections.Generic;
using System.Drawing;

namespace SimTelemetry
{
    public class PlotterCurves
    {
        public Dictionary<double, double> Data = new Dictionary<double, double>();
        public string Legend = "";
        public Color LineColor;
        public int LineThickness;
        public int YAxis;

        public PlotterCurves()
        {
            Data = new Dictionary<double, double>();
            LineThickness = 0;
            YAxis = 0;
        }
    }
}