using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using Triton;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectGame : UserControl, IGarageUserControl
    {
        public event AnonymousSignal Close;
        public event AnonymousSignal Chosen;
        public ucSelectGame()
        {
            InitializeComponent();

            Draw();
        }

        public void Draw()
        {
            this.BackColor = Color.Black;
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

            Label t = new Label { Text = "Select simulator" };
            t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
            t.ForeColor = Color.White;
            t.Size = new Size(panel.Size.Width, 50);
            t.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(t);

            foreach (ISimulator sim in Telemetry.m.Sims.Sims)
            {
                if (File.Exists("Simulators/" + sim.ProcessName + ".png"))
                {
                    ucResizableImage pb = new ucResizableImage("Simulators/" + sim.ProcessName + ".png");
                    pb.Margin = new Padding(10);
                    pb.Name = sim.ProcessName;
                    if (sim.Garage == null)
                    {
                        pb.Disabled = true;
                    }
                    else
                    {
                        pb.Cursor = Cursors.Hand;
                        pb.Click += new EventHandler(pb_Click);
                    }
                    pb.Crop(213, 120);
                    panel.Controls.Add(pb);
                }
                else
                {
                    Label l = new Label {Text = sim.Name};
                    l.Size = new Size(213, 120);
                    l.Font = new Font("Tahoma", 24.0f, FontStyle.Bold);
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Name = sim.ProcessName;
                    if (sim.Garage == null)
                    {
                        l.ForeColor = Color.Gray;
                    }
                    else
                    {
                        l.ForeColor = Color.White;
                        l.Click += new EventHandler(pb_Click);
                        l.Cursor = Cursors.Hand;
                    }
                    panel.Controls.Add(l);
                }
            }
            Controls.Add(panel);
        }

        void pb_Click(object sender, EventArgs e)
        {
            Control pb = (Control)sender;

            if (Chosen != null)
                Chosen();
        }
    }
}
