namespace SimTelemetry
{
    public class PlotterYAxis
    {
        public double min = 0;
        public double max = 10;
        public double step = 2;
        public double step_divider = 1;
        public bool auto=false;
        public int digits_comma=1;

        public PlotterYAxis()
        {
            min = 0;
            max = 10;
            step = 2;
            step_divider = 1;
            auto = true;

        }
    }
}