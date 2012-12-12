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
        public event Signal Chosen;

        private BufferedFlowLayoutPanel panel;
        private Label lbl_selectsim;
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
            panel = new BufferedFlowLayoutPanel();
            panel.AutoScroll = false;
            Padding = new Padding(35);

            lbl_selectsim = new Label { Text = "Select simulator" };
            lbl_selectsim.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
            lbl_selectsim.ForeColor = Color.White;
            lbl_selectsim.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(lbl_selectsim);

            foreach (ISimulator sim in Telemetry.m.Sims.Sims)
            {
                bool validSim = (sim.Garage == null || sim.Garage.Available == false ||
                                 !Directory.Exists(sim.Garage.InstallationDirectory));
                if (File.Exists("Simulators/" + sim.ProcessName + ".png"))
                {
                    ucResizableImage pb = new ucResizableImage("Simulators/" + sim.ProcessName + ".png");
                    pb.Margin = new Padding(10);
                    pb.Name = sim.Name;
                    if (validSim)
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
                    l.Name = sim.Name;
                    if (validSim)
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

        public void Resize()
        {

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


            panel.Size = new Size(233 * columns+20, rows * 140);
            panel.Location = new Point((this.Width - panel.Size.Width) / 2, (this.Height - panel.Size.Height) / 2);

            lbl_selectsim.Size = new Size(panel.Size.Width, 50);

        }

        void pb_Click(object sender, EventArgs e)
        {
            Control pb = (Control)sender;

            if (Chosen != null)
                Chosen(pb.Name);
        }
    }
}
