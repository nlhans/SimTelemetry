using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Controls;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry
{
    public partial class TelemetryViewer : Form
    {

        private TelemetryLogReader _logReader;

        private DataChannels Channels;
        private Plotter cPlotter;
        private ucCoordinateMap cTrackMap;

        /**** TIMELINE ****/
        private double _timeScaleFactor = 1; // % of the lap displayed. 1% - 100%
        private double _timeScaleOffset = 0; // % of the lap offseted.

        private double _timeLineStart = 0;
        private double _timeLineEnd = 30;

        private bool _timeLineIgnoreEvents = false;

        private PlotterConfigurations _PlotterConfiguration;



        public double[] TimeLine
        {
            get
            {
                return new double[2] { _timeScaleOffset * (_timeLineEnd - _timeLineStart), (_timeScaleOffset + _timeScaleFactor) * +(_timeLineEnd - _timeLineStart) };
            }
        }
        public Dictionary<double, TelemetrySample> Data
        {
            get
            {
                return _logReader.Samples;
            }
        }

        public double[] TimeCursor
        {
            get { return new double[2] {0, cPlotter.TimeCursor }; }
        }

        public TelemetryViewer()
        {
            DataChannels c = new DataChannels();
            c.ShowDialog();
            InitializeComponent();

            // Trackmap
            cTrackMap = new ucCoordinateMap(this);
            cTrackMap.Dock = DockStyle.Fill;
            this.GraphSplit.Panel1.Controls.Add(cTrackMap);



            // Plotter
            cPlotter = new Plotter { Dock = DockStyle.Fill };
            cPlotter.Drawn += cPlotter_Drawn;
            cPlotter.MouseWheel += cPlotter_Scroll;
            cPlotter.timeline.Scroll += cPlotter_TimelineScroll;

            // Plotter configuration
            _PlotterConfiguration = new PlotterConfigurations();
            _PlotterConfiguration.Load("ChartSetup.txt");
            _PlotterConfiguration.Configure(cPlotter);

            this.GraphSplit.Panel2.Controls.Add(cPlotter);
            _logReader = new TelemetryLogReader(@"C:\Users\Hans\Documents\GitHub\SimTelemetry\LiveTelemetry\bin\Debug\Logs\rfactor\Silverstone-TEST_DAY-2012-07-13-33\Lap 8.gz");
            _logReader.Read();
            while (_logReader.Progress == 0) ;
            while (_logReader.Progress != 1000) ;

            double timeMin = 100000, timeMax = 0;
            lock (_logReader.Samples)
            {
                foreach (KeyValuePair<double, TelemetrySample> sp in _logReader.Samples)
                {
                    timeMin = Math.Min(sp.Key / 1000.0, timeMin);
                    timeMax = Math.Max(sp.Key / 1000.0, timeMax);
                    TelemetrySample sample = sp.Value;

                    cPlotter.Graphs[0].Curves[0].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][48]);
                    cPlotter.Graphs[1].Curves[1].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][47] * 3.6);
                    cPlotter.Graphs[2].Curves[0].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][10] * 100);
                    cPlotter.Graphs[2].Curves[1].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][11] * 100);

                }
            }
            _timeLineStart = timeMin;
            _timeLineEnd = timeMax;

            DrawPlotbounds();
        }
        void cPlotter_TimelineScroll(object sender, ScrollEventArgs e)
        {
            if (_timeLineIgnoreEvents == false)
            {
                _timeScaleOffset = e.NewValue / 10000.0;

                DrawPlotbounds();

            }
        }

        void cPlotter_Scroll(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {

                // Zooming in/out
                if (e.Delta > 0)
                {
                    _timeScaleFactor *= 0.88;
                }
                if (e.Delta < 0)
                {
                    _timeScaleFactor /= 0.88;
                }

                if (_timeScaleFactor <= 0.01)
                    _timeScaleFactor = 0.01;
                if (_timeScaleFactor >= 1)
                    _timeScaleFactor = 1;

                double timespan = _timeLineEnd - _timeLineStart;
                double cursor = cPlotter.TimeCursor;
                double cursorDutycycle = (cursor - cPlotter.x_min) / (cPlotter.x_max - cPlotter.x_min);

                _timeScaleOffset = cPlotter.TimeCursor / timespan - _timeScaleFactor * cursorDutycycle;


                DrawPlotbounds();

            }
        }

        void cPlotter_Drawn()
        {

            cTrackMap.Invalidate();
        }
        private void DrawPlotbounds()
        {
            if (cPlotter.timeline.InvokeRequired)
            {
                cPlotter.timeline.Invoke(new AnonymousSignal(DrawPlotbounds));
                return;
            }
            if (_timeScaleOffset < 0) _timeScaleOffset = 0;
            if (_timeScaleOffset + _timeScaleFactor > 1) _timeScaleOffset = 1 - _timeScaleFactor;

            //double t = ;
            double TimeSpan = _timeLineEnd - _timeLineStart;
            double TimeStart = _timeLineStart, TimeEnd = 0;

            cPlotter.timeline.Minimum = 0;
            cPlotter.timeline.Maximum = 10000;


            _timeLineIgnoreEvents = true;
            cPlotter.timeline.Value = Convert.ToInt32(Math.Round(10000 * (_timeScaleOffset + _timeScaleFactor / 2)));
            cPlotter.timeline.SmallChange = 100;
            cPlotter.timeline.LargeChange = Convert.ToInt32(Math.Round(10000 * _timeScaleFactor));
            _timeLineIgnoreEvents = false;

            TimeStart += TimeSpan * _timeScaleOffset;
            TimeEnd = _timeScaleFactor * TimeSpan + TimeStart;

            /* cTrackMap.TimeOffset = _timeLineStart;
             cTrackMap.Time_Bounds_Min = TimeStart - _timeLineStart;
             cTrackMap.Time_Bounds_Max = TimeEnd - _timeLineStart;*/

            cPlotter.x_min = TimeStart - _timeLineStart;
            cPlotter.x_max = TimeEnd - _timeLineStart;

            cPlotter.AutoScale();
            cPlotter.Draw();
           //cTrackMap.Draw();
        }
    }
}
