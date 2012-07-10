using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SimTelemetry.Data;
using Triton;
using Triton.Controls;

namespace SimTelemetry
{
    public partial class MainForm : Form
    {
      //  private string LogsMap = @"I:\Dropbox\Tools\Telemetry\SimTelemetry\LiveTelemetry\bin\Debug\Logs";
        private string LogsMap = "Logs/";
        private bool DR_ReadingDone = false;
        private int Plot_SelectedLap = -1;

        /**** TIMELINE ****/
        private double _timeScaleFactor=1; // % of the lap displayed. 1% - 100%
        private double _timeScaleOffset=0; // % of the lap offseted.

        private double _timeLineStart = 0;
        private double _timeLineEnd = 30;

        private bool _timeLineIgnoreEvents = false;

        /**** DATA COLLECTOR & READERS ****/
        private DataReader _dR;

        private PlotterConfigurations _PlotterConfiguration;

        public MainForm()
        {
            this.FormClosing += new FormClosingEventHandler(_FormClosing);
            InitializeComponent();


            // TRACK MAP
            cTrackMap = new TrackMap {Dock = DockStyle.Fill};
            this.cSidePanelSplit.Panel1.Controls.Add(cTrackMap);

            // TRACKS LIST
            cTracks = new VisualListDetails(false) { Size = new Size(1, 120), Dock = DockStyle.Bottom };
            cTracks.Columns.Add("track", "Track", 200);
            cTracks.MultiSelect = false;
            cTracks.ItemSelectionChanged += cTracks_ItemSelectionChanged;
            this.cSidePanelSplit.Panel2.Controls.Add(cTracks);

            // SESSIONS LIST
            cSessions = new VisualListDetails(false) { Size = new Size(1, 120), Dock = DockStyle.Bottom };
            cSessions.Columns.Add("session", "Session", 150);
            cSessions.Columns.Add("date", "Date", 100);
            cSessions.Columns.Add("iter", "#", 100);
            cSessions.MultiSelect = false;
            cSessions.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(cSessions_ItemSelectionChanged);
            this.cSidePanelSplit.Panel2.Controls.Add(cSessions);

            // LAPS LIST
            cLaps = new VisualListDetails(false) { Size = new Size(1, 120), Dock = DockStyle.Bottom };
            cLaps.Columns.Add("lap", "Lap #");
            cLaps.Columns.Add("time", "Time");
            cLaps.Columns.Add("file", "file",1);
            cLaps.ItemSelectionChanged += LapsList_Changed;
            this.cSidePanelSplit.Panel2.Controls.Add(cLaps);

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

            // Select files
            try
            {
                List<string> files =
                    Directory.GetFiles(LogsMap).ToList();
                List<string> tracks = new List<string>();
                foreach (string log in files)
                {
                    string track = log.Substring(0, log.IndexOf("+"));
                    int slash = track.IndexOf("Logs");
                    track = track.Substring(slash + 5);
                    if (tracks.Contains(track) == false)
                        tracks.Add(track);

                }
                foreach (string track in tracks)
                {
                    cTracks.Items.Add(new ListViewItem(track));
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
        private string PrintLapTime(double time, bool sector)
        {
            if (time == -1)
                return "--:--:--";
            if (time < 60)
            {
                if (sector) return time.ToString("00.000");
                else
                {
                    return "0:" + time.ToString("00.000");
                }
            }
            else
            {
                int minutes = Convert.ToInt32(Math.Floor(Convert.ToDouble(time / 60f)));

                return minutes + ":" + (time % 60f).ToString("00.000");
            }

        }
        #region track selectors
        void cSessions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(cSessions.SelectedItems.Count == 1)
            {
                string session = cSessions.SelectedItems[0].Text;
                string sessiondate = cSessions.SelectedItems[0].SubItems[1].Text;
                string sessionindex = cSessions.SelectedItems[0].SubItems[2].Text;
                string track = cTracks.SelectedItems[0].Text;
                cLaps.Items.Clear();
                // get all laps
                List<string> files =
                    Directory.GetFiles(LogsMap).ToList();
                List<string> laps = new List<string>();
                foreach (string log in files)
                {
                    string[] data = log.Replace(".dat","").Split("+".ToCharArray());

                    if (data[0].EndsWith(track) && data[1] == session && data[2] == sessiondate && data[3] == sessionindex)
                    {
                        // open file
                        try
                        {
                            string[] lines = File.ReadAllLines(log);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                string line = lines[i].Trim();
                                if (line == "[Lap]")
                                {
                                    int lapID = Convert.ToInt32(lines[i + 1].Substring(7));
                                    double lapTime = Convert.ToDouble(lines[i + 2].Substring(8));
                                    cLaps.Items.Add(new ListViewItem(new string[3] { lapID.ToString(), PrintLapTime(lapTime, false), log }));
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                        int lap = Convert.ToInt32(data[4].Substring(4).Trim());
                        // laptime
                        double laptime = 10;
                    }
                }

            }
        }

        void cTracks_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //
            if (cTracks.SelectedItems.Count == 1)
            {
                string track = cTracks.SelectedItems[0].Text;
                cSessions.Items.Clear();
                List<string> files =
                    Directory.GetFiles(LogsMap).ToList();
                List<string> sessions = new List<string>();
                foreach (string log in files)
                {
                    string[] data = log.Split("+".ToCharArray());
                    if (data[0].EndsWith(track))
                    {
                        // this is our track
                        if (sessions.Contains(data[1] + "|" + data[2] + "|" + data[3]) == false)
                        sessions.Add(data[1] + "|" + data[2]+"|"+data[3]);
                    }
                }
                foreach (string sess in sessions)
                {
                    string[] data = sess.Split("|".ToCharArray());

                    cSessions.Items.Add(new ListViewItem(data));
                }
            }
        }
        #endregion
        #region Plot
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

        void cPlotter_Drawn()
        {
            cTrackMap.TimeCursor = cPlotter.TimeCursor;
            cTrackMap.Draw();
        }
        private void DrawPlot(object o)
        {
            double timeMin = Int32.MaxValue, timeMax = Int32.MinValue;

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
                DataChannels.Reset();
                foreach (KeyValuePair<int, DataSample> s in _dR.Samples)
                {

                    if (cPlotter.Graphs[0].Curves[0].Data.ContainsKey(s.Value.Time) == false)// && s.Value.Drivers[0].Laps == Plot_SelectedLap)
                    {
                        timeMin = Math.Min(timeMin, s.Value.Time);
                        timeMax = Math.Max(timeMax, s.Value.Time);

                        cTrackMap.Samples.Add(s.Value);

                        int i_g = 0;
                        foreach (PlotterGraph g in cPlotter.Graphs)
                        {
                            int i_c = 0;
                            foreach (PlotterCurves c in g.Curves)
                            {
                                cPlotter.Graphs[i_g].Curves[i_c].Data.Add(s.Value.Time - timeMin, DataChannels.Parse(c.Legend, s.Value, s.Value.Time));
                                i_c++;
                            }
                            i_g++;
                        }

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
            cPlotter.timeline.Value = Convert.ToInt32(Math.Round(10000 * (_timeScaleOffset + _timeScaleFactor / 2)));
            cPlotter.timeline.SmallChange = 100;
            cPlotter.timeline.LargeChange = Convert.ToInt32(Math.Round(10000 * _timeScaleFactor));
            _timeLineIgnoreEvents = false;

            TimeStart += TimeSpan * _timeScaleOffset;
            TimeEnd = _timeScaleFactor * TimeSpan + TimeStart;

            cTrackMap.TimeOffset = _timeLineStart;
            cTrackMap.Time_Bounds_Min = TimeStart - _timeLineStart;
            cTrackMap.Time_Bounds_Max = TimeEnd - _timeLineStart;

            cPlotter.x_min = cTrackMap.Time_Bounds_Min;
            cPlotter.x_max = cTrackMap.Time_Bounds_Max;

            cPlotter.AutoScale();
            cPlotter.Draw();
            cTrackMap.Draw();
        }
        #endregion
        #region LapList
        void LapsList_Changed(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(e.IsSelected)
            {
                int lap = 0;
                if(Int32.TryParse(e.Item.Text, out lap))
                {
                    string file = cLaps.SelectedItems[0].SubItems[2].Text;
                    _dR = new DataReader(file);
                    _dR.DataReceived += Dr_DataReceived;
                    Plot_SelectedLap = lap;
                    // get the file corresponding
                    cLaps.Enabled = false;
                    cSessions.Enabled = false;
                    cTracks.Enabled = false;
                    ThreadPool.QueueUserWorkItem(ReadDataAsync);

                }

            }

        }
        #endregion
        #region DataReader
        private void ReadDataAsync(object n)
        {
            _dR.Read();
        }
        void Dr_DataReceived()
        {
            if(this.cLaps.InvokeRequired)
            {
                this.Invoke(new AnonymousSignal(Dr_DataReceived));
                return;
            }
            cLaps.Enabled = true;
            cSessions.Enabled = true;
            cTracks.Enabled = true;

            DR_ReadingDone = true;
            DrawPlot(-0);
            DrawPlotbounds();
        }
        #endregion
        
        void _FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_dR != null)
            {
                _dR.Stop();
                _dR = null;
            }
            Application.Exit();
            e.Cancel = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            _dR = new DataReader("Logs/Monza2009+Free practice 3+9-11-2011+1+Lap 5.dat");
            _dR.DataReceived += Dr_DataReceived;
            Plot_SelectedLap = 5;
            // get the file corresponding
            cLaps.Enabled = false;
            cSessions.Enabled = false;
            cTracks.Enabled = false;
            ThreadPool.QueueUserWorkItem(ReadDataAsync);
        }
    }
}