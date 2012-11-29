/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Data.Logger;
using SimTelemetry.Data.Track;
using SimTelemetry.Objects;
using SimTelemetry.Properties;
using Triton;
using Timer = System.Windows.Forms.Timer;

namespace SimTelemetry
{
    public partial class TelemetryViewer : Form
    {

        //private TelemetryLogReader _logReader;
        private TelemetryLogReplay _logReader;
        private ReplaySFX _logSound ;

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

        /**** TELEMETRY ****/
        protected string TelemetryFile { get; set; }
        private Timer _mTelemetryLoading = new Timer {Interval = 10};


        public double[] TimeLine
        {
            get
            {
                return new double[2] { _timeScaleOffset * (_timeLineEnd - _timeLineStart), (_timeScaleOffset + _timeScaleFactor) * +(_timeLineEnd - _timeLineStart) };
            }
        }
        public TelemetryLogReader Data
        {
            get
            {
                if (_logReader == null)
                    return null;
                else
                return _logReader;
            }
        }

        public double[] TimeCursor
        {
            get { return new double[2] {0, cPlotter.TimeCursor }; }
        }



        public TelemetryViewer()
        {
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
            _PlotterConfiguration.Load("ChartSetup.txt"); // TODO: Make this independant of configuration files.
            _PlotterConfiguration.Configure(cPlotter);

            this.GraphSplit.Panel2.Controls.Add(cPlotter);

            // Loading.
            _mTelemetryLoading.Tick += new EventHandler(_mTelemetryLoading_Tick);
            _mTelemetryLoading.Start();

            this.FormClosing += new FormClosingEventHandler(TelemetryViewer_FormClosing);
        }

        void TelemetryViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReplayStop();
        }

        void _mTelemetryLoading_Tick(object sender, EventArgs e)
        {
            if(_logReader != null && _logReader.Progress != 1000)
            {
                this.lbLoading.Visible = true;
                this.lbLoadingbar.Visible = true;

                this.lbLoadingbar.Value = Limits.Clamp((int)_logReader.Progress, 0, 1000);
                this.btOpen.Enabled = false;
            }
            else
            {
                this.lbLoading.Visible = false;
                this.lbLoadingbar.Visible = false;
                this.btOpen.Enabled = true;
            }
        }


        /**** PLOTTER FUNCTIONS ****/ 
        void GraphFill()
        {


            double timeMin = 100000, timeMax = 0;
            if(_logReader != null)
            {
                foreach(PlotterGraph graph in cPlotter.Graphs)
                {
                    foreach(PlotterCurves curve in graph.Curves)
                    {
                        curve.Data.Clear();
                    }
                }
            lock (_logReader.Samples)
            {
                //double px = Double.MinValue, py= Double.MinValue, pz = Double.MinValue, pt = -1;
                //Triton.Maths.Filter spdFilter = new Triton.Maths.Filter(25);
                foreach (KeyValuePair<double, TelemetrySample> sp in _logReader.Samples)
                {
                    try
                    {
                        timeMin = Math.Min(sp.Key / 1000.0, timeMin);
                        timeMax = Math.Max(sp.Key / 1000.0, timeMax);
                        TelemetrySample sample = sp.Value;

                        // TODO: Make this work from the configuration data (or files)
                        cPlotter.Graphs[0].Curves[0].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Driver.RPM"));
                        cPlotter.Graphs[1].Curves[0].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Driver.Speed") * 3.6);
                        cPlotter.Graphs[2].Curves[0].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Pedals_Throttle")*100);
                        cPlotter.Graphs[2].Curves[1].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Pedals_Brake") * 100);
                        cPlotter.Graphs[3].Curves[0].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Brake_Temperature_LF"));
                        cPlotter.Graphs[3].Curves[1].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Brake_Temperature_LR"));
                        cPlotter.Graphs[3].Curves[2].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Brake_Temperature_RF"));
                        cPlotter.Graphs[3].Curves[3].Data.Add(sample.Time / 1000.0, _logReader.GetDouble(sample, "Player.Brake_Temperature_RR"));

                        /*double x = (double)sample.Data[3][7];
                        double y = (double)sample.Data[3][8];
                        double z = (double)sample.Data[3][9];
                        if (pt > 0)
                        {
                            double dx = x - px, dy = y - py, dz = z - pz, spd = 0;
                            dx /= (sample.Time - pt) / 1000;
                            dy /= (sample.Time - pt) / 1000;
                            dz /= (sample.Time - pt) / 1000;
                            if(Math.Abs(dx)>200) dx = 0;
                                                    if(Math.Abs(dy)>200) dy = 0;
                                                    if(Math.Abs(dz)>200) dz = 0;
                            //cPlotter.Graphs[2].Curves[0].Data.Add(sample.Time / 1000.0, (double)dx);
                            //cPlotter.Graphs[2].Curves[1].Data.Add(sample.Time / 1000.0, (double)dz);
                            spd = Math.Sqrt(dx*dx+dy*dy+dz*dz);
                            spdFilter.Add(spd);
                            cPlotter.Graphs[2].Curves[2].Data.Add(sample.Time / 1000.0, spdFilter.Average*3.6);
                        }

                        pt = sample.Time;
                        px = x;
                        py = y;
                        pz = z;*/
                        //cPlotter.Graphs[2].Curves[0].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][10] * 100);
                        //cPlotter.Graphs[2].Curves[1].Data.Add(sample.Time / 1000.0, (double)sample.Data[3][11] * 100);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Data conversion in TelemetryViewer error!");
                    }
                }
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


            btPlayPause.Enabled = true;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            FileManager fileman = new FileManager(TelemetryFile);
            fileman.ShowDialog();

            if(fileman.DatafileNew)
            {
                OpenFile(fileman.Datafile);
            }
        }

        private bool ReplayRunning = false;
        private void ReplaySoundStart(object n)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WaitCallback(ReplaySoundStart), new object[1] {n});
                return;
            }

            if (_logReader != null)
            {
                _logSound = new ReplaySFX(this, _logReader);
                _logReader.Start();

                Timer t = new Timer();
                t.Interval = 50;
                t.Tick += (e, b) =>
                              {

                              };
                t.Start();
            }
        }

        private void OpenFile(string datafile)
        {
            ReplayStop();

            Task t = new Task(() =>
                                  {
                                      try
                                      {
                                          //_logReader = new TelemetryLogReader(datafile);
                                          _logReader = new TelemetryLogReplay(datafile);
                                          _logReader.Read();
                                          while (_logReader.Progress == 0) ;
                                          while (_logReader.Progress != 1000) ;
                                          TelemetryFile = datafile;
                                          Telemetry.m.Track_Load(
                                              _logReader.GetString(0.0, "Session.GameDirectory"),
                                              _logReader.GetString(0.0, "Session.GameData_TrackFile"));
                                          GraphFill();
                                          DrawPlotbounds();
                                          _logReader.Start();
                                      }
                                      catch(Exception ex)
                                      {
                                          _logReader = null;
                                      }
                                  });
            t.Start();
        }

        private void ReplayStop()
        {

            if (_logSound != null)
            {
                _logSound.Stop();
                _logSound = null;
            }
            if (_logReader != null)
                _logReader.Stop();

            ReplayRunning = false;
            btPlayPause.Image = Resources.Play_icon;
        
    }
        private void ReplayStart()
        {
            ReplayRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ReplaySoundStart));
            btPlayPause.Image = Resources.Pause_Pressed_icon;
        }
        private void btPlayPause_Click(object sender, EventArgs e)
        {
            if(ReplayRunning)
            {
                ReplayStop();
            }
            else
            {
                ReplayStart();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DataChannels ch = new DataChannels();
            ch.ShowDialog();
        }
    }
}
