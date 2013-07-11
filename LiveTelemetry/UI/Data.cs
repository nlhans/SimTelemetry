using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveTelemetry.UI
{
    public partial class Data : Form
    {
        private Timer t = new Timer();

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
            labels.Controls.Clear();

            // Display:
            // Sim.Session
            // Sim.Drivers.Player
            // Sim.Player
            if (TelemetryApplication.TelemetryAvailable)
            {
                //List<string> session = DumpToLabels(Telemetry.m.Sim.Session, typeof(ISession));
                //List<string> driver = DumpToLabels(Telemetry.m.Sim.Drivers.Player, typeof(IDriverGeneral));
                //List<string> player = DumpToLabels(Telemetry.m.Sim.Player, typeof(IDriverPlayer));

                var session = new List<string>(new[]{":)"});
                var driver = new List<string>(new[] { ":)" });
                var player = new List<string>(new[] { ":)" });

                session[0] = "[Telemetry.m.Sim.Session]\r\n" + session[0];
                driver[0] = "[Telemetry.m.Sim.Drivers.Player]\r\n" + driver[0];
                player[0] = "[Telemetry.m.SIm.Player]\r\n" + player[0];

                int colsize = (this.Width - 20)/(session.Count + driver.Count + player.Count) - 5;
                int l = 10;
                colsize = 230;
                this.AutoScroll = true;
                foreach (string d in session)
                {
                    if (d.Trim() != "")
                    {
                        var lbl = new Label
                                        {Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f)};
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
                foreach (string d in driver)
                {
                    if (d.Trim() != "")
                    {
                        var lbl = new Label { Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f) };
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
                foreach (string d in player)
                {
                    if (d.Trim() != "")
                    {
                        var lbl = new Label { Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f) };
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
            }
            else
            {

                Label lbl = new Label { Size = new Size(this.Width, this.Height), Location = new Point(10, 10), Font = new Font("Tahoma", 36.0f) };
                if (!TelemetryApplication.SimulatorAvailable)
                {
                    lbl.Text = "No simulator running";
                }
                else
                {
                    lbl.Text = "No session running in simulator " + TelemetryApplication.Simulator.Name;
                }
                labels.Controls.Add(lbl);
            }


        }

    }
}
