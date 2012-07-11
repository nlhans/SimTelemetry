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
using Triton;

namespace SimTelemetry
{
    public partial class TelemetryViewer : Form
    {

        private DataChannels Channels;
        private Plotter cPlotter;
        private TrackMap cTrackMap;

        /**** TIMELINE ****/
        private double _timeScaleFactor = 1; // % of the lap displayed. 1% - 100%
        private double _timeScaleOffset = 0; // % of the lap offseted.

        private double _timeLineStart = 0;
        private double _timeLineEnd = 30;

        private bool _timeLineIgnoreEvents = false;

        private PlotterConfigurations _PlotterConfiguration;



        private TelemetryLogReader _logReader;

        public TelemetryViewer()
        {
            DataChannels c = new DataChannels();
            //c.ShowDialog();
            InitializeComponent();

            // Trackmap
            //cTrackMap = new TrackMap();



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
            _logReader = new TelemetryLogReader(@"C:\Users\Hans\Documents\GitHub\SimTelemetry\LiveTelemetry\bin\Debug\test.txt");
            _logReader.Read();
            while (_logReader.Progress == 0) ;
            while (_logReader.Progress != 1000) ;

            double timeMin = 100000, timeMax = 0;

            double lastX = 0, lastY = 0, lastZ = 0,spd=0, mySpeed = 0, lastSpeed=0, lastTime=0, myAcc=0;
            Triton.Maths.IIR spd_filter = new Triton.Maths.IIR(new double[25]{
        0.03986147865151436200,
        0.03989606880123134500,
        0.03992766857748184000,
        0.03995627322145577800,
        0.03998187842539897100,
        0.04000448033326193400,
        0.04002407554128045000,
        0.04004066109848827800,
        0.04005423450716150100,
        0.04006479372319470700,
        0.04007233715640879500,
        0.04007686367079047600,
        0.04007837258466332600,
        0.04007686367079047600,
        0.04007233715640879500,
        0.04006479372319470700,
        0.04005423450716150100,
        0.04004066109848827800,
        0.04002407554128045000,
        0.04000448033326193400,
        0.03998187842539897100,
        0.03995627322145577800,
        0.03992766857748184000,
        0.03989606880123134500,
        0.03986147865151436200}, new double[1]{
            0
        });
            Triton.Maths.IIR acc_filter = new Triton.Maths.IIR(new double[5]{
                
        0.19974388710184859000,
        0.20012802619262748000,
        0.20025617341104796000,
        0.20012802619262748000,
        0.19974388710184859000}, new double[1] { 0 });

            foreach(KeyValuePair<double, TelemetrySample> sp in _logReader.Samples)
            {
                timeMin = Math.Min(sp.Key / 1000.0, timeMin);
                timeMax = Math.Max(sp.Key / 1000.0, timeMax);
                TelemetrySample sample = sp.Value;
                //X=7
                //Y=8
                //Z=9

                double x=(double)sample.Data[3][7], y=(double)sample.Data[3][8], z=(double)sample.Data[3][9];
                double dt = sp.Key - lastTime;
                if (lastX != x && dt> 0)
                {
                    spd = Math.Pow(x - lastX, 2) + Math.Pow(y - lastY, 2) + Math.Pow(z - lastZ, 2);
                    spd = Math.Sqrt(spd)/(dt/1000);


                    mySpeed = spd_filter.Add(spd);
                    if (mySpeed > 320 / 3.6) mySpeed = 320 / 3.6;
                    if (mySpeed < -320 / 3.6) mySpeed = -320 / 3.6;
                    double acc = (mySpeed - lastSpeed) / (dt / 1000);
                    myAcc = acc_filter.Add(acc);
                    lastSpeed = mySpeed;
                    if (myAcc > 2 * 9.81) myAcc = 2 * 9.81;
                    if (myAcc < -7 * 9.81) myAcc = -7 * 9.81;
                }
                lastX = x; lastY = y; lastZ = z; lastTime = sp.Key; lastSpeed = mySpeed;

                cPlotter.Graphs[0].Curves[0].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][48]);
                cPlotter.Graphs[1].Curves[1].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][47] * 3.6);
                cPlotter.Graphs[1].Curves[0].Data.Add(sample.Time / 1000.0, (double)mySpeed*3.6);
                cPlotter.Graphs[2].Curves[0].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][10] * 100);
                cPlotter.Graphs[2].Curves[1].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][11] * 100);
                cPlotter.Graphs[3].Curves[0].Data.Add(sample.Time / 1000.0, (double)myAcc/9.81 );

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
