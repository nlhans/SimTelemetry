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
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimTelemetry.Data.Track;
using SimTelemetry.Objects.Garage;
using Triton;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectTrackCars : UserControl, IGarageUserControl
    {
        private bool ControlsAdded = false;
        private Label t;
        private BufferedFlowLayoutPanel panel;

        private Label txt_loading;
        private bool Loading = false;

        private List<Control> mods_list = new List<Control>();

        public ucSelectTrackCars()
        {
            InitializeComponent();
            this.Padding = new Padding(35, 85, 35, 35);
            panel = new BufferedFlowLayoutPanel();

            t = new Label { Text = "Select mod/track" };
            t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
            t.ForeColor = Color.White;
            t.TextAlign = ContentAlignment.MiddleCenter;

            txt_loading = new Label();
            txt_loading.Text = "Loading cars & tracks...";
            txt_loading.Dock = DockStyle.Fill;
            txt_loading.TextAlign = ContentAlignment.MiddleCenter;
            txt_loading.ForeColor = Color.White;
            txt_loading.Font = new Font("Tahoma", 24.0f, FontStyle.Bold);

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        public event AnonymousSignal Close;
        public event Signal Chosen;
        public void Draw()
        {

            if (fGarage.Sim == null)
            {
                // TODO: Display errors.
                // TODO: Check if sim is installed.
                if (Close != null)
                    Close();
                return;
            }
            ControlsAdded = false;
            if (ControlsAdded == false)
            {
                Controls.Clear();;
                Controls.Add(txt_loading);
                ControlsAdded = true;
                mods_list = new List<Control>();
                panel.Controls.Clear();
                panel.Controls.Add(t);

                this.BackColor = Color.Black;
                Loading = true;

                Task load = new Task(() =>
                                         {
                                             if (fGarage.Sim != null && fGarage.Sim.Garage != null && fGarage.Sim.Garage.Mods != null)
                                             {
                                                 foreach (IMod mod in fGarage.Sim.Garage.Mods)
                                                 {
                                                     // If required, scan:
                                                     mod.Scan();

                                                     if (mod.Image != "" &&
                                                         File.Exists(mod.Image))
                                                     {

                                                         ucResizableImage pb =
                                                             new ucResizableImage(mod.Image);
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
                                                             pb.Click +=
                                                                 new EventHandler(pb_Click);
                                                         }
                                                         pb.Crop(220, 220);
                                                         mods_list.Add(pb);
                                                     }
                                                     else
                                                     {
                                                         Label l = new Label();
                                                         l.Text = mod.Name;
                                                         l.Name = mod.Name;
                                                         l.Font = new Font("Tahoma", 24.0f,
                                                                           FontStyle.Bold);
                                                         l.Size = new Size(213, 120);
                                                         if (mod.Models.Count == 0)
                                                         {
                                                             l.ForeColor = Color.Gray;
                                                         }
                                                         else
                                                         {
                                                             l.ForeColor = Color.White;
                                                             l.Cursor = Cursors.Hand;
                                                             l.Click += pb_Click;
                                                         }
                                                         mods_list.Add(l);

                                                     }
                                                 }
                                             }
                                         });
                load.ContinueWith((result) =>
                                      {
                                          DrawPanel();
                                      });

                Task loadtracks = new Task(() =>
                                               {
                                                   TrackThumbnail thumbnail_generator = new TrackThumbnail();
                                                   fGarage.Sim.Garage.Scan();
                                                   if (fGarage.Sim.Garage.Tracks != null){
                                                       foreach (ITrack track in fGarage.Sim.Garage.Tracks)
                                                       {
                                                           track.Scan();
                                                           if (File.Exists(track.Thumbnail) == false)
                                                           {
                                                               track.ScanRoute();
                                                               thumbnail_generator.Create(track.Thumbnail, track.Name,
                                                                                          track.Version, track.Route,
                                                                                          220,
                                                                                          220);
                                                           }

                                                           if (File.Exists(track.Thumbnail))
                                                           {
                                                               ucResizableImage pb =
                                                                   new ucResizableImage(track.Thumbnail);
                                                               pb.Caption = track.Name;
                                                               pb.Margin = new Padding(10);
                                                               pb.Name = track.Name;
                                                               pb.Cursor = Cursors.Hand;
                                                               //pb.Click +=pb_Click;
                                                               pb.Crop(220, 220);
                                                               mods_list.Add(pb);
                                                           }
                                                       }
                                                   }
                                               });
                loadtracks.ContinueWith((r) => {
                                                   load.Start();
                });
                loadtracks.Start();
                //load.Start();
            }
        }
        private void DrawPanel()
        {
            if(InvokeRequired)
            {
                Invoke(new AnonymousSignal(DrawPanel));
                return;
            }
            Loading = false;
            Controls.Remove(txt_loading);
            panel.Controls.AddRange(mods_list.ToArray());
            Controls.Add(panel);
            Resize();
        }

        public void Resize()
        {
            try
            {
                int grid_content_size = fGarage.Sim.Garage.Mods.Count + fGarage.Sim.Garage.Tracks.Count;
                int columns = (int) Math.Ceiling(Math.Sqrt(grid_content_size)) + 2;
                if (grid_content_size%columns == 1)
                    columns++;
                if (this.Width + 40 >= 240)
                {
                    while (240*columns > this.Width - 40 && columns > 0)
                        columns--;
                }
                if (columns <= 0) columns = 1;
                int rows = (int) Math.Ceiling(grid_content_size*1.0/columns) + 1;

                panel.Size = new Size(240*columns + 40,
                                      Math.Min(this.Height - 50, rows*240 + 20));
                panel.Location = new Point((this.Width - panel.Size.Width)/2,
                                           (this.Height - panel.Size.Height)/2);

                t.Size = new Size(panel.Size.Width - 40, 50);
                panel.Rebuffer();
            }catch(Exception){}
        }

        void pb_Click(object sender, EventArgs e)
        {
            Control c = (Control) sender;

            if (Chosen != null)
                Chosen(c.Name);
        }
    }
}