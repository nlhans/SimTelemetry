using System.Collections.Generic;
using System.Drawing;

namespace SimTelemetry
{
    public class PlotterGraph
    {
        public List<PlotterCurves> Curves = new List<PlotterCurves>();

        public List<PlotterYAxis> Y_Axis = new List<PlotterYAxis>();

        public void CreateCurve(string name, Color color, int yaxis)
        {
            if(Y_Axis.Count <= yaxis)
            {
                yaxis = 0;

            }

            if(Y_Axis.Count == 0)
                Y_Axis.Add(new PlotterYAxis());

            PlotterCurves curve = new PlotterCurves();
            curve.LineColor = color;
            curve.Legend = name;
            curve.YAxis = yaxis;

            Curves.Add(curve);

        }
    }
}