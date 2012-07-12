using System.Drawing;

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
}