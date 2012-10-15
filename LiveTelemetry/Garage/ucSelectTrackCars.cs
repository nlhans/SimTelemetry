using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects.Garage;
using Triton;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectTrackCars : UserControl, IGarageUserControl
    {
        private bool ControlsAdded = false;
        private Label t;
        private FlowLayoutPanel panel;
        public ucSelectTrackCars()
        {
            InitializeComponent();
            this.Padding = new Padding(35, 85, 35, 35);
            panel = new FlowLayoutPanel();
            panel.HorizontalScroll.Enabled = false;
            panel.VerticalScroll.Enabled = true;
            panel.BorderStyle = BorderStyle.None;


            t = new Label { Text = "Select mod/track" };
            t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
            t.ForeColor = Color.White;
            t.TextAlign = ContentAlignment.MiddleCenter;
        }

        public event AnonymousSignal Close;
        public event AnonymousSignal Chosen;
        public void Draw()
        {
            this.BackColor = Color.Black;

            // TODO: Fix bug when simulator is not active!!
                int columns = (int) Math.Ceiling(Math.Sqrt(Telemetry.m.Sim.Garage.Mods.Count))+2;
                if (columns == 0) columns = 1;
                if (Telemetry.m.Sim.Garage.Mods.Count%columns == 1)
                    columns++;
                if (this.Width > 213)
                {
                    while (213*columns > this.Width)
                        columns--;
                }
                int rows = (int) Math.Ceiling(Telemetry.m.Sim.Garage.Mods.Count*1.0/columns) + 1;

            panel.Size = new Size(233 * columns+40, rows * 140+20);
            panel.Location = new Point((this.Width - panel.Size.Width) / 2, (this.Height - panel.Size.Height) / 2);

            t.Size = new Size(panel.Size.Width, 50);
            if (ControlsAdded == false)
            {
                Controls.Clear();

                panel.Controls.Add(t);

                foreach (IMod mod in Telemetry.m.Sim.Garage.Mods)
                {
                    // If required, scan:
                    mod.Scan();

                    if (mod.Image != "" && File.Exists(mod.Image))
                    {

                        ucResizableImage pb = new ucResizableImage(mod.Image);
                        pb.Caption = mod.Name;
                        pb.Margin = new Padding(10);
                        pb.Name = mod.Name;
                        if (mod.Models.Count == 0)
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
                        Label l = new Label();
                        l.Text = mod.Name;
                        l.Font = new Font("Tahoma", 24.0f, FontStyle.Bold);
                        l.Size = new Size(213, 120);
                        if (mod.Models.Count == 0)
                        {
                            l.ForeColor = Color.Gray;
                        }
                        else
                        {
                            l.ForeColor = Color.White;
                            l.Cursor = Cursors.Hand;
                            l.Click += new EventHandler(pb_Click);
                        }
                        panel.Controls.Add(l);

                    }
                }

                Controls.Add(panel);
                ControlsAdded = true;
            }
        }

        void pb_Click(object sender, EventArgs e)
        {

        }
    }
}
