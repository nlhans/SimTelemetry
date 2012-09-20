using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SimTelemetry;
using SimTelemetry.Data;
using SimTelemetry.Peripherals.Dashboard;
using Triton;
using Triton.Joysticks;
using SimTelemetry.Objects;
using SimTelemetry.Data;
using System.Globalization;

namespace LiveTelemetry
{
    public partial class LiveTelemetry : Form
    {
        private static LiveTelemetry myself;
        public static int TheButton = 6;


        private Timer Tmr_HiSpeed;
        private Timer Tmr_MdSpeed;
        private Timer Tmr_LwSpeed;

        private LapChart ucLapChart = new LapChart();

        private LiveTrackMap ucTrackmap;
        private Gauge_A1GP ucA1GP;
        private Gauge_Tyres ucTyres;
        private Gauge_Laps ucLaps;
        private Gauge_Splits ucSplits;

        private ucSessionInfo SessionData;
        private ucFuel FuelData;

        private Joystick Joy;


        private Simulators Sims;

        public static int StatusMenu
        {
            get { return _StatusMenu; }
            set
            {
                _StatusMenu = value;
                myself.SetStatusControls(null, null);

            }
        }

        private static int _StatusMenu;

        internal void SetStatusControls(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(SetStatusControls), new object[2] { sender, e });
                return;
            }
            if (Controls.Contains(ucA1GP))
                Controls.Remove(ucA1GP);
            if (Controls.Contains(ucTyres))
                Controls.Remove(ucTyres);
            if (Controls.Contains(ucLaps))
                Controls.Remove(ucLaps);
            if (Controls.Contains(ucSplits))
                Controls.Remove(ucSplits);

            switch (StatusMenu)
            {
                case 0:
                    Controls.Add(ucA1GP);
                    break;
                case 1:
                    Controls.Add(ucTyres);
                    break;
                case 2:
                    Controls.Add(ucLaps);
                    break;
                case 3:
                    Controls.Add(ucSplits);
                    break;
                default:
                    StatusMenu = 0;
                    SetStatusControls(null, null);
                    break;
            }

        }

        public LiveTelemetry()
        {


            Application.CurrentCulture = new CultureInfo("en-US");
            //SimTelemetry.TelemetryViewer fileman = new TelemetryViewer();
            //fileman.ShowDialog();
            //Application.Exit();
           // return;
            SimTelemetry.Data.Telemetry.m.Run();
            while (SimTelemetry.Data.Telemetry.m.Sims == null) System.Threading.Thread.Sleep(1);
            SimTelemetry.Data.Telemetry.m.Sim_Start += mUpdateUI;
            SimTelemetry.Data.Telemetry.m.Sim_Stop += mUpdateUI;
            SimTelemetry.Data.Telemetry.m.Session_Start += mUpdateUI;
            SimTelemetry.Data.Telemetry.m.Session_Stop += mUpdateUI;

            GameData g = new GameData();


            myself = this;

            this.FormClosing += LiveTelemetry_FormClosing;

            string[] lines = File.ReadAllLines("config.txt");
            string controller = "";
            bool controlleruseindex = false;
            int controllerindex = 0;
            foreach (string line in lines)
            {
                string[] p = line.Trim().Split("=".ToCharArray());
                if (line.StartsWith("button"))
                {
                    TheButton = Convert.ToInt32(p[1]);
                }
                if (line.StartsWith("index"))
                {
                    controlleruseindex = true;
                    controllerindex = Convert.ToInt32(p[1]);
                }
                if (line.StartsWith("controller"))
                {
                    controller = p[1];
                }
                if (line.StartsWith("trackmap"))
                {
                    if (p[1] == "static")
                        LiveTrackMap.StaticTrackMap = true;
                    else
                        LiveTrackMap.StaticTrackMap = false;
                }

            }

            List<JoystickDevice> devices = JoystickDevice.Search();
            if (devices.Count == 0)
            {
                MessageBox.Show("Cycling through panels disabled, no suitable joystick found?");
            }
            else
            {
                if (controlleruseindex)
                {
                    Joy = new Joystick(devices[controllerindex]);
                    Joy.Release += Joy_Release;
                }
                else
                {
                    int i = 0;
                    foreach (JoystickDevice jd in devices)
                    {
                        if (jd.Name.Contains(controller.Trim()))
                        {
                            Joy = new Joystick(jd);
                            Joy.Release += Joy_Release;


                        }
                        i++;
                    }
                }
            }

            BackColor = Color.Black;
            InitializeComponent();

            ucLaps = new Gauge_Laps();
            ucSplits = new Gauge_Splits();
            ucA1GP = new Gauge_A1GP(Joy);
            ucTyres = new Gauge_Tyres();
            SessionData = new ucSessionInfo();
            //FuelData = new ucFuel();

            ucTrackmap = new LiveTrackMap();
            ucLapChart = new LapChart();

            // Timers
            Tmr_HiSpeed = new Timer();
            Tmr_MdSpeed = new Timer();
            Tmr_LwSpeed = new Timer();

            Tmr_HiSpeed.Interval = 75;   // 10Hz
            Tmr_MdSpeed.Interval = 600;  // 4 Hz
            Tmr_LwSpeed.Interval = 1000;  // 1 Hz

            Tmr_HiSpeed.Start();
            Tmr_MdSpeed.Start();
            Tmr_LwSpeed.Start();

            Tmr_HiSpeed.Tick += Tmr_HiSpeed_Tick;
            Tmr_MdSpeed.Tick += Tmr_MdSpeed_Tick;
            Tmr_LwSpeed.Tick += Tmr_LwSpeed_Tick;
            SizeChanged += LiveTelemetry_SizeChanged;

            System.Threading.Thread.Sleep(500);

            SetupUI(true);
        }

        void mUpdateUI(object sender)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Signal(mUpdateUI), new object[1] { sender });
                return;
            }
            SetupUI(true);
        }


        void SetupUI(bool update)
        {
            if (Telemetry.m.Active_Session)
            {
                this.Controls.Clear();
                this.Padding = new System.Windows.Forms.Padding(0);

                Controls.Add(ucTrackmap);
                Controls.Add(ucLapChart);
                Controls.Add(SessionData);
                //Controls.Add(FuelData);
                LiveTelemetry_SizeChanged(null, null);
                SetStatusControls(null, null);

            }
            else if (Telemetry.m.Active_Sim)
            {
                this.Controls.Clear();
                this.Padding = new System.Windows.Forms.Padding(0);
                // draw sim pic.

                Label t = new Label { Text = "Waiting for session" };
                t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
                t.ForeColor = Color.White;
                t.Size = new Size(this.Size.Width - 80, 60);
                t.TextAlign = ContentAlignment.MiddleCenter;

                FlowLayoutPanel panel = new FlowLayoutPanel();

                if (File.Exists("Simulators/" + Telemetry.m.Sim.ProcessName + ".png") && this.Width > 80 && this.Height > 100)
                {
                    ucResizableImage pb = new ucResizableImage("Simulators/" + Telemetry.m.Sim.ProcessName + ".png");
                    pb.Crop(this.Size.Width - 80, this.Size.Height - 70);
                    panel.Controls.Add(pb);
                    panel.Location = new Point(40, (this.Height - pb.Size.Height - 60) / 2);
                    panel.Size = new System.Drawing.Size(this.Width - 80, 60 + pb.Size.Height);

                }
                else
                {
                    panel.Size = new System.Drawing.Size(this.Width, 50);
                }
                panel.Controls.Add(t);

                Controls.Add(panel);

            }
            else
            {
                Controls.Clear();

                // draw sim gallery
                FlowLayoutPanel panel = new FlowLayoutPanel();
                this.Padding = new System.Windows.Forms.Padding(35);

                int columns = (int)Math.Ceiling(Math.Sqrt(Telemetry.m.Sims.Sims.Count));
                if (columns == 0) columns = 1;
                if (Telemetry.m.Sims.Sims.Count % columns == 1)
                    columns++;
                if (this.Width > 233)
                {
                    while (233 * columns > this.Width)
                        columns--;
                }
                int rows = (int)Math.Ceiling(Telemetry.m.Sims.Sims.Count * 1.0 / columns) + 1;

                panel.Size = new Size(233 * columns, rows * 140);
                panel.Location = new Point((this.Width - panel.Size.Width) / 2, (this.Height - panel.Size.Height) / 2);

                foreach (ISimulator sim in Telemetry.m.Sims.Sims)
                {
                    if (File.Exists("Simulators/" + sim.ProcessName + ".png"))
                    {
                        ucResizableImage pb = new ucResizableImage("Simulators/" + sim.ProcessName + ".png");
                        pb.Margin = new System.Windows.Forms.Padding(10);
                        pb.Crop(213, 120);
                        panel.Controls.Add(pb);
                    }
                }

                Label t = new Label { Text = "Waiting for simulator" };
                t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
                t.ForeColor = Color.White;
                t.Size = new Size(panel.Size.Width, 50);
                t.TextAlign = ContentAlignment.MiddleCenter;
                panel.Controls.Add(t);


                Controls.Add(panel);
            }

        }

        private Label tmp;

        void LiveTelemetry_FormClosing(object sender, FormClosingEventArgs e)
        {
            TritonBase.TriggerExit();
        }


        void Joy_Release(Joystick joystick, int button)
        {
            if (button == TheButton)
            {
                StatusMenu++;

            }
        }

        void LiveTelemetry_SizeChanged(object sender, EventArgs e)
        {
            if (Telemetry.m.Active_Session)
            {
                int tmp = StatusMenu;
                StatusMenu = 0;

                ucA1GP.Size = new Size(450, 325);
                this.ucA1GP.Location = new Point(this.Size.Width - ucA1GP.Size.Width - 20, this.Size.Height - ucA1GP.Height - 40);


                this.ucLapChart.Location = new Point(this.Size.Width - ucLapChart.Size.Width - 30, 10);
                this.ucTrackmap.Size = new Size(ucLapChart.Location.X - 20, this.Size.Height);
                this.ucTrackmap.Location = new Point(10, 10);
                SessionData.Location = new Point(ucA1GP.Location.X, ucA1GP.Location.Y - SessionData.Size.Height - 10);

                this.ucTyres.Size = ucA1GP.Size;
                this.ucTyres.Location = ucA1GP.Location;

                this.ucLaps.Size = ucA1GP.Size;
                this.ucLaps.Location = ucA1GP.Location;

                //this.FuelData.Location = new Point(110, 10);

                ucSplits.Size = ucA1GP.Size;
                ucSplits.Location = ucA1GP.Location;
                StatusMenu = tmp;
            }
            else
            {
                SetupUI(false);
            }
        }

        private bool TrackResetDone = false;
        private double ApexSpeed = 0;

        void Tmr_HiSpeed_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(Tmr_HiSpeed_Tick), new object[2] { sender, e });
                return;
            }

            ucA1GP.Update();
            if (Controls.Contains(ucA1GP)) ucA1GP.Invalidate();
            if (Controls.Contains(ucTyres)) ucTyres.Invalidate();

            //FuelData.Update();
        }

        void Tmr_MdSpeed_Tick(object sender, EventArgs e)
        {
            SessionData.Invalidate();
            ucTrackmap.Invalidate();

        }

        void Tmr_LwSpeed_Tick(object sender, EventArgs e)
        {
            ucLapChart.Invalidate();
            if (Controls.Contains(ucLaps)) ucLaps.Invalidate();
            if (Controls.Contains(ucSplits)) ucSplits.Invalidate();
        }

    }
}
