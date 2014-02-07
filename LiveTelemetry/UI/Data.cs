using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiveTelemetry.UI
{
    public partial class Data : Form
    {
        private Timer t = new Timer();

        private bool hasPaintedTelLabels = false;

        public Data()
        {
            InitializeComponent();

            t = new Timer {Interval =500};
            t.Tick += t_Tick;
            t.Start();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        void t_Tick(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new EventHandler(t_Tick), new object[2] {sender, e});
                return;
            }

            if (TelemetryApplication.TelemetryAvailable)
            {
                // Tab: plug-in
                var simPlg = TelemetryApplication.Simulator;
                var simLabels = new List<Tuple<int, Dictionary<string, string>>>();
               
                // Col 1
                var simCol1 = new Dictionary<string, string>();
                simCol1.Add("Active Plug-in", string.Empty);
                simCol1.Add("Name", simPlg.Name);
                simCol1.Add("Version", simPlg.GetSimulatorVersions());
                simCol1.Add("Plug-in Version", simPlg.Version);
                simCol1.Add("Author", simPlg.Author);
                simCol1.Add("Technical Plug-in Info", string.Empty);
                simCol1.Add("Location", simPlg.Location + " ");
                simCol1.Add("Simulator Process Name", simPlg.ProcessName);
                simCol1.Add("Tracks Found", simPlg.Tracks.Count().ToString());
                simCol1.Add("Car Mods Found", simPlg.Mods.Count().ToString());

                simLabels.Add(new Tuple<int, Dictionary<string, string>>(1, simCol1));

                // Display
                RenderLabels(tabEnv, simLabels, 250);

                // Tab: Session
                var sessLabels = new List<Tuple<int, Dictionary<string, string>>>();
                if (TelemetryApplication.SessionAvailable == false)
                {
                    if (hasPaintedTelLabels)
                    {
                        ResetLabels(tabSession);

                        var sessColE = new Dictionary<string, string>();
                        sessColE.Add("Plug-in hasn't detected a session running!", string.Empty);
                        sessLabels.Add(new Tuple<int, Dictionary<string, string>>(1, sessColE));

                        RenderLabels(tabSession, sessLabels, 500);
                    }
                    hasPaintedTelLabels = false;
                }
                else
                {
                    if (!hasPaintedTelLabels)
                        ResetLabels(tabSession);

                    var sessData = TelemetryApplication.Telemetry.Session;
                    var sessCol1 = new Dictionary<string, string>();

                    sessCol1.Add("Session Info", string.Empty);
                    sessCol1.Add("Type", string.Format("{0}({1})", sessData.Info.Name, sessData.Info.Type.ToString()));
                    sessCol1.Add("Duration", string.Format("{0} seconds", sessData.Info.Duration.TotalSeconds));
                    sessCol1.Add("Time", string.Format("{0} seconds", sessData.Time));
                    sessCol1.Add("No. of cars", sessData.Cars.ToString());
                    sessCol1.Add("of which: on track", TelemetryApplication.Telemetry.Drivers.Count(x=> !x.IsPits).ToString());
                    sessCol1.Add("of which: in pits", TelemetryApplication.Telemetry.Drivers.Count(x => x.IsPits).ToString());
                    sessCol1.Add("Flags",
                                 string.Format("Active:{0} Offline:{1} Replay: {2} Loading: {3}",
                                               (sessData.IsActive ? "Yes" : "No"), (sessData.IsOffline ? "Yes" : "No"),
                                               (sessData.IsReplay ? "Yes" : "No"), (sessData.IsLoading ? "Yes" : "No")));

                    sessCol1.Add("Race info", string.Empty);
                    sessCol1.Add("Race Laps", string.Format("{0} Laps", sessData.RaceLaps.ToString()));
                    sessCol1.Add("Leader Laps", string.Format("{0} Laps", sessData.LeaderLaps.ToString()));

                    sessCol1.Add("Track Info", string.Empty);
                    sessCol1.Add("Game File", sessData.Track);
                    sessCol1.Add("Track Loaded?", TelemetryApplication.TrackAvailable ? "Yes" : "No");
                    sessCol1.Add("Temperature",
                                 string.Format("Track {0}C / Ambient {1}C", Math.Round(sessData.TrackTemperature, 1),
                                               Math.Round(sessData.AmbientTemperature, 1)));
                    
                    if (TelemetryApplication.TrackAvailable)
                    {
                        var track = TelemetryApplication.Track;
                        sessCol1.Add("Name", track.Name);
                        sessCol1.Add("Length", string.Format("{0} metres", track.Length));
                        sessCol1.Add("Location", track.Location);
                    }else
                    {
                        sessCol1.Add("Name", "Unavailable");
                        sessCol1.Add("Length", "Unavailable");
                        sessCol1.Add("Location", "Unavailable");
                    }

                    sessLabels.Add(new Tuple<int, Dictionary<string, string>>(1, sessCol1));

                    // Display
                    RenderLabels(tabSession, sessLabels, 400);
                    hasPaintedTelLabels = true;
                }
            }
            else
            {

                Label lbl = new Label { Size = new Size(this.Width, this.Height), Location = new Point(10, 10), Font = new Font("Tahoma", 36.0f) };
                if (!TelemetryApplication.SimulatorAvailable)
                {
                    lbl.Text = "No simulator running";
                }
                else if (!TelemetryApplication.SessionAvailable)
                {
                    lbl.Text = "No session running in simulator " + TelemetryApplication.Simulator.Name;
                }else
                {
                    lbl.Text = "Unknown reason why no Telemetry data is present";
                }
                tabEnv.Controls.Add(lbl);
            }


        }

        private void ResetLabels(TabPage tabPage)
        {
            tabPage.Controls.Clear();
        }

        private void RenderLabels(TabPage tab, List<Tuple<int, Dictionary<string, string>>> simLabels, int colWidth)
        {
            var columnLabelWidthPerc = 30;

            if(tab.Controls.Count == 0)
            {
                Dictionary<int, int> progressingColHeight = new Dictionary<int, int>();

                // Initial render
                foreach(var t in simLabels)
                {
                    var col = t.Item1-1;
                    var lbls = t.Item2;

                    var x = 10 + colWidth*col;
                    var y = 10;
                    if (progressingColHeight.ContainsKey(col)) y = progressingColHeight[col];
                    else progressingColHeight.Add(10, 10);

                    foreach(var lblInfo in lbls)
                    {
                        var friendlyName = lblInfo.Key.GetHashCode().ToString();

                        var lblH = new Label()
                                      {
                                          Name = "lblH" + friendlyName,
                                          Text = lblInfo.Key,
                                          Location = new Point(x, y),
                                          Size = new Size(colWidth * columnLabelWidthPerc/100, 19)
                                      };
                        if (string.IsNullOrEmpty(lblInfo.Value))
                        {
                            lblH.Font = new Font(lblH.Font.FontFamily, lblH.Font.Size + 4, FontStyle.Bold);
                            lblH.Size = new Size(colWidth, 30);
                            y += 8;
                        }
                        else
                        {
                            var lblV = new Label()
                                           {
                                               Name = "lblV" + friendlyName,
                                               Text = lblInfo.Value,
                                               Location = new Point(x + lblH.Size.Width, y),
                                               Size = new Size(colWidth - 1 - lblH.Size.Width, 19)
                                           };

                            tab.Controls.Add(lblV);
                        }
                        tab.Controls.Add(lblH);
                        y += 20; // 20px label height
                    }

                    progressingColHeight[col] = y;
                }
            }
            else
            {
                // Update labels
                foreach(var t in simLabels)
                {
                    var col = t.Item1-1;
                    var lbls = t.Item2;

                    foreach (var lblInfo in lbls)
                    {
                        var friendlyName = lblInfo.Key.GetHashCode().ToString();
                        var lblValueName = "lblV" + friendlyName;
                        var c = tab.Controls.Find(lblValueName, false);
                        if (c.Length == 0) continue;
                        var l = c[0] as Label;
                        l.Text = lblInfo.Value;
                    }
                }
            }
        }
    }
}
