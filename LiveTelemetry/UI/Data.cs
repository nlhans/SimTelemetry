using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SimTelemetry.Domain.Services;
using SimTelemetry.Domain.Telemetry;

namespace LiveTelemetry.UI
{
    public partial class Data : Form
    {
        private Timer t = new Timer();

        private bool hasPaintedTelLabels = false;

        public Data()
        {
            InitializeComponent();

            t = new Timer {Interval =50};
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
            bool renderTelemetryLabels = false;

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

                //All detailed tabs
                if (TelemetryApplication.SessionAvailable == false)
                {
                    if (hasPaintedTelLabels)
                    {
                        ResetLabels(tabSession);
                        ResetLabels(tabPlayer1);
                        ResetLabels(tabPlayer2);
                        ResetLabels(tabPlayer3);

                        var err = new Dictionary<string, string>();
                        err.Add("Plug-in hasn't detected a session running!", string.Empty);
                        var errLabels = new List<Tuple<int, Dictionary<string, string>>>();
                        errLabels.Add(new Tuple<int, Dictionary<string, string>>(1, err));

                        RenderLabels(tabSession, errLabels, 500);
                        RenderLabels(tabPlayer1, errLabels, 500);
                        RenderLabels(tabPlayer2, errLabels, 500);
                        RenderLabels(tabPlayer3, errLabels, 500);
                    }
                    hasPaintedTelLabels = false;
                    renderTelemetryLabels = false;
                }
                else
                {
                    if (!hasPaintedTelLabels)
                    {
                        ResetLabels(tabSession);
                        ResetLabels(tabPlayer1);
                        ResetLabels(tabPlayer2);
                        ResetLabels(tabPlayer3);
                    }
                    renderTelemetryLabels = true;
                }

                if (renderTelemetryLabels)
                {
                    // Tab: Session
                    var sessLabels = new List<Tuple<int, Dictionary<string, string>>>();
                    var sessCol1 = new Dictionary<string, string>();

                    var sessData = TelemetryApplication.Data.Session;
                    sessCol1.Add("Session Info", string.Empty);
                    sessCol1.Add("Type", string.Format("{0}({1})", sessData.Info.Name, sessData.Info.Type.ToString()));
                    sessCol1.Add("Duration", string.Format("{0} seconds", sessData.Info.Duration.TotalSeconds));
                    sessCol1.Add("Time", string.Format("{0} seconds", sessData.Time));
                    sessCol1.Add("No. of cars", sessData.Cars.ToString());
                    sessCol1.Add("of which: on track",
                                 TelemetryApplication.Data.Drivers.Count(x => !x.IsPits).ToString());
                    sessCol1.Add("of which: in pits",
                                 TelemetryApplication.Data.Drivers.Count(x => x.IsPits).ToString());
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
                    }
                    else
                    {
                        sessCol1.Add("Name", "Unavailable");
                        sessCol1.Add("Length", "Unavailable");
                        sessCol1.Add("Location", "Unavailable");
                    }

                    sessLabels.Add(new Tuple<int, Dictionary<string, string>>(1, sessCol1));

                    RenderLabels(tabSession, sessLabels, 400);

                    // Tab: player1 
                    var plr1Labels = new List<Tuple<int, Dictionary<string, string>>>();
                    var plr1Col1 = new Dictionary<string, string>();

                    plr1Col1.Add("Car", string.Empty);
                    plr1Col1.Add("Loaded?", (TelemetryApplication.CarAvailable ? "Yes" : "No"));
                    plr1Col1.Add("Game File", TelemetryApplication.Data.Player.CarFile);

                    var carObj = TelemetryApplication.Car;
                    if (carObj != null)
                    {
                        plr1Col1.Add("Model", carObj.Name);
                        plr1Col1.Add("Driver", carObj.Driver);
                        if (carObj.Team != null)
                            plr1Col1.Add("Team", carObj.Team.Name);
                        else
                            plr1Col1.Add("Team", "Unavailable");

                        plr1Col1.Add("Start number", carObj.StartNumber.ToString());
                        plr1Col1.Add("Classes", string.Join(", ", carObj.CarClass));

                        plr1Col1.Add("General", string.Empty);
                        plr1Col1.Add("Weight", carObj.Chassis.Weight.ToString("0000.0kg"));
                        plr1Col1.Add("Fuel Tank", carObj.Chassis.FuelTankSize.ToString("000.0L"));

                        plr1Col1.Add("Engine", string.Empty);
                        plr1Col1.Add("Configuration",
                                     carObj.Engine.Displacement + "cc " + carObj.Engine.Cilinders + " cilinder");
                        plr1Col1.Add("Idle RPM", carObj.Engine.IdleRpm.Average.ToString("0000 rpm"));
                        plr1Col1.Add("Maximum Stock RPM", carObj.Engine.MaximumRpm.Average.ToString("0000 rpm"));
                        plr1Col1.Add("Torque",
                                     carObj.Engine.MaximumTorque.ToString("000 nm") + "@" +
                                     carObj.Engine.MaximumTorqueRpm.ToString("0000 rpm"));
                        plr1Col1.Add("Power",
                                     carObj.Engine.MaximumPower.ToString("000 hp") + "@" +
                                     carObj.Engine.MaximumPowerRpm.ToString("0000 rpm"));
                        plr1Col1.Add("Typical lifetime",
                                     string.Format("{0} seconds @ {1} / {2}",
                                                   carObj.Engine.Lifetime.Time.Optimum.ToString(),
                                                   carObj.Engine.Lifetime.EngineRpm.Optimum.ToString("0000 rpm"),
                                                   carObj.Engine.Lifetime.OilTemperature.Optimum.ToString("000 C oil")));
                    }
                    plr1Labels.Add(new Tuple<int, Dictionary<string, string>>(1, plr1Col1));

                    RenderLabels(tabPlayer1, plr1Labels, 400);

                    // Tab: player2 
                    var plr2Labels = new List<Tuple<int, Dictionary<string, string>>>();
                    var plr2Col1 = new Dictionary<string, string>();
                    var plr2Col2 = new Dictionary<string, string>();

                    var plr = TelemetryApplication.Data.Player;

                    plr2Col1.Add("Competition", string.Empty);
                    plr2Col1.Add("Position", "P" + plr.Position);
                    plr2Col1.Add("Flags",
                                 "Yellow: " + (plr.FlagYellow ? "Yes" : "No") + " / Blue: " +
                                 (plr.FlagBlue ? "Yes" : "No") + " / Black: " + (plr.FlagBlack ? "Yes" : "No"));
                    // TODO: Driving aids

                    plr2Col1.Add("Laps driven", plr.Laps.ToString() + " laps");
                    plr2Col1.Add("Pitstops", plr.Pitstops + " pitstops");
                    plr2Col1.Add("Fastest lap",
                                 string.Format("S1 {0:00.000} / S2 {1:00.000} / S3 {2:00.000} / Lap {3:000.000}",
                                               plr.BestLap.Sector1, plr.BestLap.Sector2, plr.BestLap.Sector3,
                                               plr.BestLap.Total));
                    plr2Col1.Add("Last lap",
                                 string.Format("S1 {0:00.000} / S2 {1:00.000} / S3 {2:00.000} / Lap {3:000.000}",
                                               plr.LastLap.Sector1, plr.LastLap.Sector2, plr.LastLap.Sector3,
                                               plr.LastLap.Total));
                    plr2Col1.Add("Current lap",
                                 string.Format("S1 {0:00.000} / S2 {1:00.000} / S3 {2:00.000} / Lap {3:000.000}",
                                               plr.CurrentLap.Sector1, plr.CurrentLap.Sector2, plr.CurrentLap.Sector3,
                                               plr.CurrentLap.Total));
                    plr2Col1.Add("Fastest sectors",
                                 string.Format("S1 {0:00.000} / S2 {1:00.000} / S3 {2:00.000} / Lap {3:000.000}",
                                               plr.BestS1, plr.BestS2, plr.BestS3, plr.BestS1 + plr.BestS2 + plr.BestS3));
                    plr2Col1.Add("Sector", plr.TrackPosition.ToString());
                    plr2Col1.Add("Distance on lap", plr.Meter.ToString("00000.000m"));
                    plr2Col1.Add("Fuel", plr.Fuel.ToString("000.000L") + " / " + plr.FuelCapacity.ToString("000L"));

                    plr2Col2.Add("Driving", string.Empty);
                    plr2Col2.Add("Coordinate",
                                 string.Format("X {0:0000.00} Y {1:0000.00} Z {2:0000.00}", plr.CoordinateX,
                                               plr.CoordinateY, plr.CoordinateZ));
                    plr2Col2.Add("Acceleration",
                                 string.Format("X {0:0000.00} Y {1:0000.00} Z {2:0000.00}", plr.AccelerationX,
                                               plr.AccelerationY, 0));
                    plr2Col2.Add("Speed", Math.Round(plr.Speed, 2) + "m/s  / " + Math.Round(plr.Speed*3.6, 1) + "km/h");
                    plr2Col2.Add("Heading", plr.Heading.ToString() + " rad");
                    plr2Col2.Add("Live Weight", plr.Mass.ToString("0000.000kg"));

                    plr2Col2.Add("Drivetrain", string.Empty);
                    plr2Col2.Add("Engine RPM",
                                 plr.EngineRpm.ToString("000 rpm") + " / " + plr.EngineRpmMax.ToString("000 rpm (max)"));
                    plr2Col2.Add("Engine Fluids",
                                 Math.Round(plr.WaterTemperature, 2) + "C water / " + Math.Round(plr.OilTemperature, 2) +
                                 " C oil");
                    plr2Col2.Add("Engine Lifetime", Math.Round(plr.EngineLifetime, 2) + " seconds at optimal conditions");
                    plr2Col2.Add("Gear", plr.Gear + " / " + plr.Gears);
                    if (plr.GearRatios != null && plr.GearRatios.Any())
                        plr2Col2.Add("Gear Ratios", string.Join(", ", plr.GearRatios.Select(x => x.ToString("0.000"))));
                    else
                        plr2Col2.Add("Gear Ratios", "Unavailable");

                    plr2Labels.Add(new Tuple<int, Dictionary<string, string>>(1, plr2Col1));
                    plr2Labels.Add(new Tuple<int, Dictionary<string, string>>(2, plr2Col2));

                    RenderLabels(tabPlayer2, plr2Labels, 400);

                    // Tab: player3
                    var plr3Labels = new List<Tuple<int, Dictionary<string, string>>>();
                    var plr3Col1 = new Dictionary<string, string>();
                    var plr3Col2 = new Dictionary<string, string>();

                    UpdateWheelLabels(plr3Col1, "LF", "Wheel Front-Left (LF)", plr.WheelLF);
                    UpdateWheelLabels(plr3Col2, "RF", "Wheel Front-Right (RF)", plr.WheelRF);
                    UpdateWheelLabels(plr3Col1, "LR", "Wheel Rear-Left (LR)", plr.WheelLR);
                    UpdateWheelLabels(plr3Col2, "RR", "Wheel Rear-Right (RR)", plr.WheelRR);

                    plr3Labels.Add(new Tuple<int, Dictionary<string, string>>(1, plr3Col1));
                    plr3Labels.Add(new Tuple<int, Dictionary<string, string>>(2, plr3Col2));

                    RenderLabels(tabPlayer3, plr3Labels, 400);

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

        private static void UpdateWheelLabels(Dictionary<string, string> col, string sAdd, string header, TelemetryWheel wheel)
        {
            col.Add(header, string.Empty);
            col.Add(sAdd + " Tyre Rotational Speed", string.Format("{0:000.00}rad/s", wheel.Speed));
            col.Add(sAdd + " Tyre Temperature", string.Format("{0:000.00}C inside / {1:000.00}C center / {2:000.00}C outside", wheel.TemperatureInside, wheel.TemperatureMiddle, wheel.TemperatureOutside));
            col.Add(sAdd + " Tyre Pressure", string.Format("{0:000.00} kPa", wheel.TyrePressure));
            col.Add(sAdd + " Tyre Wear", string.Format("{0:000.00}%", wheel.TyreWear * 100));
            col.Add(sAdd + " Tyre Contact Patch", string.Format("{0:0.00000} [m]m^2?", wheel.TyreContactPatch));
            col.Add(sAdd + " RideHeight", string.Format("{0:000.00}cm", wheel.Rideheight * 100));
            col.Add(sAdd + " Brake temperature", string.Format("{0:0000.0}C", wheel.BrakeTemperature));
            col.Add(sAdd + " Brake thickness", string.Format("{0:0.00}cm", wheel.BrakeThickness*100));
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
                    else progressingColHeight.Add(col, 10);

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
