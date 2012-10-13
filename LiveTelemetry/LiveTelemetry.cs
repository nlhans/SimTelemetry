using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimTelemetry;
using SimTelemetry.Data;
using Triton;
using Triton.Joysticks;
using SimTelemetry.Objects;
using System.Globalization;

namespace LiveTelemetry
{
    public partial class LiveTelemetry : Form
    {
        private static LiveTelemetry _liveTelemetry;
        private Simulators Sims;
        
        // Joystick data for cycling through panels.
        private Joystick Joystick_Instance;
        public static int Joystick_Button;

        // Interface update timers.
        // See end of init function for speed settings.
        private Timer Tmr_HiSpeed; 
        private Timer Tmr_MdSpeed;
        private Timer Tmr_LwSpeed;

        // User interface controls.
        private LapChart ucLapChart;
        private LiveTrackMap ucTrackmap;
        private Gauge_A1GP ucA1GP;
        private Gauge_Tyres ucTyres;
        private Gauge_Laps ucLaps;
        private Gauge_Splits ucSplits;
        private ucSessionInfo ucSessionData;

        /// <summary>
        /// Gets or sets the current menu panel it's in (0-3). If set the interface will be updated.
        /// Static property allows it to be changed from external sources as well.
        /// </summary>
        public static int StatusMenu
        {
            get { return _StatusMenu; }
            set
            {
                _StatusMenu = value;
                _liveTelemetry.SetStatusControls(null, null);

            }
        }
        private static int _StatusMenu;

        /// <summary>
        /// Initializes for the window. This setups the data back-end framework, as well searches for joystick
        /// configuration. Set-ups interface controls.
        /// </summary>
        public LiveTelemetry()
        {
            // Use EN-US for compatibility with functions as Convert.ToDouble etc.
            // This is mainly used within track parsers.
            Application.CurrentCulture = new CultureInfo("en-US");
            
            if(false)
            {
                TelemetryViewer t = new TelemetryViewer();
                t.ShowDialog();
                return;
            }

            // Boot up the telemetry service. Wait for it to start and register events that trigger interface changes.
            Telemetry.m.Run();
            while (Telemetry.m.Sims == null) System.Threading.Thread.Sleep(1);
            Telemetry.m.Sim_Start += mUpdateUI;
            Telemetry.m.Sim_Stop += mUpdateUI;
            Telemetry.m.Session_Start += mUpdateUI;
            Telemetry.m.Session_Stop += mUpdateUI;

            System.Threading.Thread.Sleep(500);
            if (Telemetry.m.Sim != null)
                Telemetry.m.Sim.Garage.Scan();

            // TODO: Detect hardware devices (COM-ports or USB devices)
            // GameData is used for my own hardware extension projects.
            // Race dashboard:
            // https://dl.dropbox.com/u/207647/IMAG0924.jpg
            // https://dl.dropbox.com/u/207647/IMAG1204.jpg
            // Switchboard:
            // https://dl.dropbox.com/u/207647/IMAG0928.jpg
            // https://dl.dropbox.com/u/207647/IMAG0934.jpg
            //GameData g = new GameData();

            // Form of singleton for public StatusMenu access.
            _liveTelemetry = this;

            // Read joystick configuration.
            // TODO: Needs fancy dialogs to first-time setup.
            string[] lines = File.ReadAllLines("config.txt");
            string controller = "";
            bool controlleruseindex = false;
            int controllerindex = 0;
            foreach (string line in lines)
            {
                string[] p = line.Trim().Split("=".ToCharArray());
                if (line.StartsWith("button"))Joystick_Button = Convert.ToInt32(p[1]);
                if (line.StartsWith("index"))
                {
                    controlleruseindex = true;
                    controllerindex = Convert.ToInt32(p[1]);
                }
                if (line.StartsWith("controller"))
                    controller = p[1];

                if (line.StartsWith("trackmap"))
                {
                    if (p[1] == "static")
                        LiveTrackMap.StaticTrackMap = true;
                    else
                        LiveTrackMap.StaticTrackMap = false;
                }

            }
            
            // Search for the joystick.
            List<JoystickDevice> devices = JoystickDevice.Search();
            if (devices.Count == 0)
            {
                //MessageBox.Show("No (connected) joystick found for display panel control.\r\nTo utilize this please connect a joystick, configure and restart this program.");
            }
            else
            {
                if (controlleruseindex)
                {
                    Joystick_Instance = new Joystick(devices[controllerindex]);
                    Joystick_Instance.Release += Joy_Release;
                }
                else
                {
                    int i = 0;
                    foreach (JoystickDevice jd in devices)
                    {
                        if (jd.Name.Contains(controller.Trim()))
                        {
                            Joystick_Instance = new Joystick(jd);
                            Joystick_Instance.Release += Joy_Release;
                        }
                        i++;
                    }
                }
            }

            // Set-up the main interface.
            FormClosing += LiveTelemetry_FormClosing;
            SizeChanged += Telemetry_ResizePanels;

            InitializeComponent();

            this.SuspendLayout();
            BackColor = Color.Black;
            ucLaps = new Gauge_Laps();
            ucSplits = new Gauge_Splits();
            ucA1GP = new Gauge_A1GP(Joystick_Instance);
            ucTyres = new Gauge_Tyres();
            ucSessionData = new ucSessionInfo();
            ucTrackmap = new LiveTrackMap();
            ucLapChart = new LapChart();

            // Timers
            Tmr_HiSpeed = new Timer{Interval=33}; // 30fps
            Tmr_MdSpeed = new Timer{Interval = 450}; // ~2fps
            Tmr_LwSpeed = new Timer{Interval=1000}; // 1fps

            Tmr_HiSpeed.Tick += Tmr_HiSpeed_Tick;
            Tmr_MdSpeed.Tick += Tmr_MdSpeed_Tick;
            Tmr_LwSpeed.Tick += Tmr_LwSpeed_Tick;

            Tmr_HiSpeed.Start();
            Tmr_MdSpeed.Start();
            Tmr_LwSpeed.Start();

            System.Threading.Thread.Sleep(500);

            SetupUI();
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Reinitializes the panel changable with user defined button/joystick. The function invokes execution
        /// to the main interface thread. 
        /// 0: general data/wear A1GP Style (Default)
        /// 1: Tyre temperature, wear and brakes.
        /// 2: Lap times list
        /// 3: Split times.
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">EventArgs of event</param>
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

        /// <summary>
        /// Completely re updates user interfaces upon sim start/stop or session start/stop.
        /// It will call SetupUI(true) in the windows UI context.
        /// </summary>
        /// <param name="sender">Parameter fed from anonymous signal. Unused</param>
        private void mUpdateUI(object sender)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Signal(mUpdateUI), new object[1] { sender });
                return;
            }
            SetupUI();
        }

        /// <summary>
        /// Completely redraws the user interface. It will bring this window into 3 modes:
        /// A) Waiting for simulator.
        /// B) Waiting for session.
        /// C) Telemetry window.
        /// 
        /// The simulator panel displays all installed modules of simulators (if an image is found).
        /// The session panel displays the full-size simulator image (if exists) with "waiting for session".
        /// The telemetry window adds the controls track map, lap chart, session status. It initializes panel sizes
        /// via Telemetry_ResizePanels and sets the user panel via SetStatusControls.
        /// </summary>
        private void SetupUI()
        {
            if (Telemetry.m.Active_Session)
            {
                this.Controls.Clear();
                this.Padding = new Padding(0);

                Controls.Add(ucTrackmap);
                Controls.Add(ucLapChart);
                Controls.Add(ucSessionData);
                //Controls.Add(FuelData);
                Telemetry_ResizePanels(null, null);
                SetStatusControls(null, null);

            }
            else if (Telemetry.m.Active_Sim)
            {
                this.Controls.Clear();
                this.Padding = new Padding(0);
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
                    pb.Crop(Size.Width - 80, Size.Height - 70);
                    panel.Controls.Add(pb);
                    panel.Location = new Point(40, (Height - pb.Size.Height - 60) / 2);
                    panel.Size = new Size(Width - 80, 60 + pb.Size.Height);

                }
                else
                {
                    panel.Size = new Size(Width, 60);
                    panel.Location = new Point(40, (Height - 60) / 2);
                }
                panel.Controls.Add(t);

                Controls.Add(panel);

            }
            else
            {
                Controls.Clear();

                // draw sim gallery
                FlowLayoutPanel panel = new FlowLayoutPanel();
                this.Padding = new Padding(35);

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
                        pb.Margin = new Padding(10);
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

        /// <summary>
        /// Close the Triton framework properly. This stops all services and what not is running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiveTelemetry_FormClosing(object sender, FormClosingEventArgs e)
        { 
            TritonBase.TriggerExit();
        }

        /// <summary>
        /// This method listens for joystick button releases and updates the StatusMenu property.
        /// This will in turn update the user interface.
        /// </summary>
        /// <param name="joystick">Joystick class from which the event was wired.</param>
        /// <param name="button">The button that was released.</param>
        private void Joy_Release(Joystick joystick, int button)
        {
            if (button == Joystick_Button)
            {
                StatusMenu++;

            }
        }

        /// <summary>
        /// This method resizes the interface to accommodate different window sizes possible on different systems and monitors.
        /// This method is dependant on the different display modes as described at the method SetupUI().
        /// 
        /// The exact details still need further testing across various resolutions and aspect ratio's.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Telemetry_ResizePanels(object sender, EventArgs e)
        {
            try
            {
                if (Telemetry.m.Active_Session)
                {
                    if (ucA1GP != null)
                    {
                        int tmp = StatusMenu;
                        StatusMenu = 0;

                        ucA1GP.Size = new Size(450, 325);
                        ucA1GP.Location = new Point(this.Size.Width - ucA1GP.Size.Width - 20,
                                                    this.Size.Height - ucA1GP.Height - 40);


                        this.ucLapChart.Location = new Point(this.Size.Width - ucLapChart.Size.Width - 30, 10);
                        if (this.ucLapChart.Height + this.ucA1GP.Height > this.Height - 40)
                            this.ucLapChart.Size = new Size(this.ucLapChart.Width, this.Height - this.ucA1GP.Height - 40);

                        this.ucTrackmap.Size = new Size(ucLapChart.Location.X - 20, this.Size.Height);
                        this.ucTrackmap.Location = new Point(10, 10);
                        ucSessionData.Location = new Point(ucA1GP.Location.X,
                                                           ucA1GP.Location.Y - ucSessionData.Size.Height - 10);

                        this.ucTyres.Size = ucA1GP.Size;
                        this.ucTyres.Location = ucA1GP.Location;

                        this.ucLaps.Size = ucA1GP.Size;
                        this.ucLaps.Location = ucA1GP.Location;

                        ucSplits.Size = ucA1GP.Size;
                        ucSplits.Location = ucA1GP.Location;
                        StatusMenu = tmp;
                    }
                }
                else
                {
                    SetupUI();
                }
            }
            catch (Exception ex)
            {
                // This exception is often fired because the resize event is fired before the panels are placed.
            }
        }

        /// <summary>
        /// High-speed user interface updates. This runs gauges that need fluent updates like the A1GP(general data) and tyres.
        /// The execution is invoked into the windows interface context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmr_HiSpeed_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Tmr_HiSpeed_Tick), new object[2] { sender, e });
                return;
            }

            ucA1GP.Update();
            if (Controls.Contains(ucA1GP)) ucA1GP.Invalidate();
            if (Controls.Contains(ucTyres)) ucTyres.Invalidate();
        }

        /// <summary>
        /// Medium-speed interface updates. This is the time keeper (Session status) and track map. The track map
        /// could also been placed into high-speed, but takes up too much CPU-time for little gain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tmr_MdSpeed_Tick(object sender, EventArgs e)
        {
            ucSessionData.Invalidate();
            ucTrackmap.Invalidate();

        }

        /// <summary>
        /// Low-speed interface updater. All things that keep track of lap times which don't have to be super fast.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tmr_LwSpeed_Tick(object sender, EventArgs e)
        {
            ucLapChart.Invalidate();
            if (Controls.Contains(ucLaps)) ucLaps.Invalidate();
            if (Controls.Contains(ucSplits)) ucSplits.Invalidate();
        }

    }
}
