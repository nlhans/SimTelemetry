using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Game.Rfactor;
using Triton;
using Triton.Joysticks;

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

        private Joystick Joy;

        private string Track_LastAIW = "";

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
            if(this.InvokeRequired)
            {
                this.Invoke(new EventHandler(SetStatusControls), new object[2] {sender, e});
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

            switch(StatusMenu)
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
                SetStatusControls(null,null);
                    break;
            }

        }

        public LiveTelemetry()
        {
            rFactor r = new rFactor();

            myself = this;

            this.FormClosing += LiveTelemetry_FormClosing;

            List<JoystickDevice> devices = JoystickDevice.GetDevices();
            if (devices.Count == 0)
                MessageBox.Show("Cycling through panels disabled, no suitable joystick found?");
            else
            {
                Joy = new Joystick(devices[0]);
                Joy.Release += Joy_Release;
            }

            BackColor = Color.Black;
            InitializeComponent();

            ucLaps = new Gauge_Laps();
            ucSplits = new Gauge_Splits();
            ucA1GP = new Gauge_A1GP(Joy);
            ucTyres = new Gauge_Tyres();
            SessionData = new ucSessionInfo();

            ucTrackmap = new LiveTrackMap();
            ucLapChart = new LapChart();

            Controls.Add(ucTrackmap);
            Controls.Add(ucLapChart);
            Controls.Add(SessionData);

            // Timers
            Tmr_HiSpeed = new Timer();
            Tmr_MdSpeed = new Timer();
            Tmr_LwSpeed = new Timer();

            Tmr_HiSpeed.Interval = 10;   // 10Hz
            Tmr_MdSpeed.Interval = 250;  // 4 Hz
            Tmr_LwSpeed.Interval = 1000;  // 1 Hz

            Tmr_HiSpeed.Start();
            Tmr_MdSpeed.Start();
            Tmr_LwSpeed.Start();

            Tmr_HiSpeed.Tick += Tmr_HiSpeed_Tick;
            Tmr_MdSpeed.Tick += Tmr_MdSpeed_Tick;
            Tmr_LwSpeed.Tick += Tmr_LwSpeed_Tick;
            SizeChanged += LiveTelemetry_SizeChanged;

            LiveTelemetry_SizeChanged(null, null);
            SetStatusControls(null, null);
        }

        void LiveTelemetry_FormClosing(object sender, FormClosingEventArgs e)
        {
            TritonBase.TriggerExit();
        }


        void Joy_Release(string joystick, int button)
        {
            if (button == TheButton)
            {
                StatusMenu++;

            }
        }

        void LiveTelemetry_SizeChanged(object sender, EventArgs e)
        {
            int tmp = StatusMenu;
            StatusMenu = 0;

            ucA1GP.Size = new Size(450, 325);
            this.ucA1GP.Location = new Point(this.Size.Width - ucA1GP.Size.Width-20, this.Size.Height-ucA1GP.Height-40);
            

            this.ucLapChart.Location = new Point(this.Size.Width - ucLapChart.Size.Width-30, 10);
            this.ucTrackmap.Size = new Size(ucLapChart.Location.X-20,
                                                      Math.Min(ucLapChart.Location.X-20,this.Size.Height));
            this.ucTrackmap.Location = new Point(10, 10);
            ucTrackmap.ReadAIW(rFactor.rFactor_Directory + rFactor.Track_AIW);

            SessionData.Location = new Point(ucA1GP.Location.X, ucA1GP.Location.Y - SessionData.Size.Height - 10);

            this.ucTyres.Size = ucA1GP.Size;
            this.ucTyres.Location = ucA1GP.Location;

            this.ucLaps.Size = ucA1GP.Size;
            this.ucLaps.Location = ucA1GP.Location;

            ucSplits.Size = ucA1GP.Size;
            ucSplits.Location = ucA1GP.Location;
            StatusMenu = tmp;
        }

        void Tmr_HiSpeed_Tick(object sender, EventArgs e)
        {
            ucA1GP.Update();
            if (Controls.Contains(ucA1GP)) ucA1GP.Invalidate();
            if (Controls.Contains(ucTyres)) ucTyres.Invalidate();
        }

        void Tmr_MdSpeed_Tick(object sender, EventArgs e)
        {
            SessionData.Invalidate();
            ucTrackmap.Invalidate();

        }

        void Tmr_LwSpeed_Tick(object sender, EventArgs e)
        {
            if(Track_LastAIW !=rFactor.Track_AIW && rFactor.Track_AIW != "")
            {
                ucTrackmap.ReadAIW(rFactor.rFactor_Directory + rFactor.Track_AIW);
                Track_LastAIW = rFactor.Track_AIW;
            }

            ucLapChart.Invalidate();
             if (Controls.Contains(ucLaps))ucLaps.Invalidate();
            if (Controls.Contains(ucSplits)) ucSplits.Invalidate();
        }

    }
}
