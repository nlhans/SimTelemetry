using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SimTelemetry.Data;
using Triton;
using Triton.Controls;

namespace SimTelemetry
{
    public partial class MainForm : Form
    {


        private bool DR_ReadingDone = false;
        private int Plot_SelectedLap = -1;

        /**** TIMELINE ****/
        private double _timeScaleFactor=1; // % of the lap displayed. 1% - 100%
        private double _timeScaleOffset=0; // % of the lap offseted.

        private double _timeLineStart = 0;
        private double _timeLineEnd = 30;

        private bool _timeLineIgnoreEvents = false;

        /**** DATA COLLECTOR & READERS ****/
        private DataCollector _dC;
        private DataReader _dR;

        private PlotterConfigurations _PlotterConfiguration;

        public MainForm()
        {
            this.FormClosing += new FormClosingEventHandler(_FormClosing);
            InitializeComponent();


            // TRACK MAP
            cTrackMap = new TrackMap {Dock = DockStyle.Fill};
            this.cSidePanelSplit.Panel1.Controls.Add(cTrackMap);

            // LAPS LIST
            cLaps = new VisualListDetails(false) {Size = new Size(1, 200), Dock = DockStyle.Bottom};
            cLaps.Columns.Add("number", "Lap #");
            cLaps.Columns.Add("number", "Time");
            cLaps.ItemSelectionChanged += LapsList_Changed;
            this.cSidePanelSplit.Panel2.Controls.Add(cLaps);

            // RECORD
            cButtonRecord = new Button {Text = "Record", Location = new Point(105, 5), Size = new Size(100, 50)};
            cButtonRecord.Click += cButtonRecord_Click;
            this.cSidePanelSplit.Panel2.Controls.Add(cButtonRecord);

            // REPAINT
            cButtonRepaint = new Button {Text = "Repaint", Location = new Point(5, 5), Size = new Size(100, 50)};
            cButtonRepaint.Click += cButtonRepaint_Click;
            this.cSidePanelSplit.Panel2.Controls.Add(cButtonRepaint);

            // Plotter
            cPlotter = new Plotter {Dock = DockStyle.Fill};
            cPlotter.Drawn += cPlotter_Drawn;
            cPlotter.MouseWheel += cPlotter_Scroll;
            cPlotter.timeline.Scroll += cPlotter_TimelineScroll;

            // Plotter configuration
            _PlotterConfiguration = new PlotterConfigurations();
            _PlotterConfiguration.Load("ChartSetup.txt");
            _PlotterConfiguration.Configure(cPlotter);
            

            cScreenSplit.Panel2.Controls.Add(cPlotter);
        }

        void cPlotter_TimelineScroll(object sender, ScrollEventArgs e)
        {
            if(_timeLineIgnoreEvents==false)
            {
                _timeScaleOffset = e.NewValue/10000.0;

                DrawPlotbounds();

            }
        }

        void cPlotter_Scroll(object sender, MouseEventArgs e)
        {
            if(e.Delta != 0)
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
                double cursorDutycycle = (cursor-cPlotter.x_min)/(cPlotter.x_max - cPlotter.x_min);

                _timeScaleOffset = cPlotter.TimeCursor / timespan - _timeScaleFactor  * cursorDutycycle;


                DrawPlotbounds();

            }
        }


        void LapsList_Changed(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(e.IsSelected)
            {
                int lap = 0;
                if(Int32.TryParse(e.Item.Text, out lap))
                {
                    Plot_SelectedLap = lap-1;
                    DrawPlot(-0);
                    DrawPlotbounds();
                }

            }

        }

        void cButtonRecord_Click(object sender, EventArgs e)
        {
            if (_dC == null)
            {
                if (_dR != null)
                    _dR.Stop();
                _dC = new DataCollector();
                _dC.Run();
                cButtonRecord.Text = "STOP and DISPLAY";
            }
            else
            {
                _dC.Stop();
                _dC = null;
                _dR = new DataReader("testje.txt");

                cButtonRecord.Text = "Record";

            }
        }

        void cPlotter_Drawn()
        {
            cTrackMap.TimeCursor = cPlotter.TimeCursor;
            cTrackMap.Draw();
        }

        private void ReadDataAsync(object n)
        {

            _dR.Read();
        }

        void cButtonRepaint_Click(object sender, EventArgs e)
        {
            cButtonRepaint.Text = "Reading file..";
            if (_dR == null)
            {
                DR_ReadingDone = false;
                _dR = new DataReader("testje.txt");
                _dR.DataReceived += Dr_DataReceived;
                _dR.LapsReceived += Dr_LapsReceived;
                _dR.SpecifiedLap_Received += Dr_SpecifiedLap_Received;
                ThreadPool.QueueUserWorkItem(ReadDataAsync);
            }

        
        }

        void Dr_SpecifiedLap_Received()
        {
            //
        }

        void Dr_LapsReceived()
        {
            if(cLaps.InvokeRequired)
            {
                Invoke(new AnonymousSignal(Dr_LapsReceived));
                return;
            }
            Plot_SelectedLap = -1;
            cLaps.Items.Clear();
            int i = 0;
            foreach(KeyValuePair<int, double> lap in _dR.Laps)
            {
                i++;
                if (i == 1) continue;
                ListViewItem il = new ListViewItem(new string[2] {lap.Key.ToString(), lap.Value.ToString()});
                cLaps.Items.Add(il);

            }
        }

        void Dr_DataReceived()
        {
            DR_ReadingDone = true;
        }

        private void DrawPlot(object o)
        {
            double timeMin = Int32.MaxValue,  timeMax = Int32.MinValue;

            if (DR_ReadingDone == false)
            {
                Thread.Sleep(50);
                ThreadPool.QueueUserWorkItem(DrawPlot);
                return;
            }

            cTrackMap.Samples.Clear();

            foreach (PlotterGraph g in cPlotter.Graphs)
            {
                foreach (PlotterCurves c in g.Curves)
                {
                    c.Data.Clear();
                }
            }

            if (Plot_SelectedLap == -1)
            {
                cPlotter.Draw();
                cTrackMap.Draw();
                return;
            }
            try
            {
                foreach (KeyValuePair<int, DataSample> s in _dR.Samples)
                {

                    if (cPlotter.Graphs[0].Curves[0].Data.ContainsKey(s.Value.Time) == false && s.Value.Drivers[0].Laps == Plot_SelectedLap)
                    {
                        timeMin = Math.Min(timeMin, s.Value.Time);
                        timeMax = Math.Max(timeMax, s.Value.Time);

                        cTrackMap.Samples.Add(s.Value);

                        int i_g = 0;
                        foreach(PlotterGraph g in cPlotter.Graphs)
                        {
                            int i_c = 0;
                            foreach(PlotterCurves c in g.Curves)
                            {
                                cPlotter.Graphs[i_g].Curves[i_c].Data.Add(s.Value.Time-timeMin, DataChannels.Parse(c.Legend, s.Value));
                                i_c++;
                            }
                            i_g++;
                        }
                        /*cPlotter.Graphs[1].Curves[0].Data.Add(s.Value.Time - t2, -1 * s.Value.Player.Tyre_Speed_LF * s.Value.Player.Wheel_Radius_LF - s.Value.Drivers[0].Speed);
                        cPlotter.Graphs[1].Curves[1].Data.Add(s.Value.Time - t2, -1 * s.Value.Player.Tyre_Speed_RF * s.Value.Player.Wheel_Radius_RF - s.Value.Drivers[0].Speed);
                        cPlotter.Graphs[1].Curves[2].Data.Add(s.Value.Time - t2, -1 * s.Value.Player.Tyre_Speed_LR * s.Value.Player.Wheel_Radius_LR - s.Value.Drivers[0].Speed);
                        cPlotter.Graphs[1].Curves[3].Data.Add(s.Value.Time - t2, -1 * s.Value.Player.Tyre_Speed_RR * s.Value.Player.Wheel_Radius_RR - s.Value.Drivers[0].Speed);*/


                    }

                }

                _timeLineStart = timeMin;
                _timeLineEnd = timeMax;

                DrawPlotbounds();

            }
            catch (Exception e)
            {
                cTrackMap.Samples.Clear();
                cPlotter.Draw();
            }
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
            cPlotter.timeline.Value = Convert.ToInt32(Math.Round(10000*(_timeScaleOffset + _timeScaleFactor/2)));
            cPlotter.timeline.SmallChange = 100;
            cPlotter.timeline.LargeChange = Convert.ToInt32(Math.Round(10000*_timeScaleFactor));
            _timeLineIgnoreEvents = false;

            TimeStart += TimeSpan*_timeScaleOffset;
            TimeEnd = _timeScaleFactor*TimeSpan + TimeStart;

            cTrackMap.TimeOffset = _timeLineStart;
            cTrackMap.Time_Bounds_Min = TimeStart - _timeLineStart;
            cTrackMap.Time_Bounds_Max = TimeEnd - _timeLineStart;

            cPlotter.x_min = cTrackMap.Time_Bounds_Min;
            cPlotter.x_max = cTrackMap.Time_Bounds_Max;

            cPlotter.AutoScale();
            cPlotter.Draw();
            cTrackMap.Draw();
        }

        void _FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_dC != null)
            {
                _dC.Stop();
                _dC.Close();
                _dC = null;
            }
            if (_dR != null)
            {
                _dR.Stop();
                _dR = null;
            }
            Application.Exit();
            e.Cancel = false;
        }
    }
}