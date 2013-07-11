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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveTelemetry.Garage;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Entities;
using SimTelemetry.Domain.Plugins;

namespace LiveTelemetry
{
    public partial class fGarage : Form
    {
        private GarageWindow Window;

        private ucSelectGame ucGame;
        private ucSelectTrackCars ucTrackCars;
        private ucSelectModel ucMod;

        private Button btBack;

        public static IPluginSimulator Sim { get; set; }
        public static Mod Mod { get; set; }

        public fGarage()
        {
            Window = GarageWindow.GameSelect;
            InitializeComponent();

            Padding = new Padding(0, 50, 0, 0);

            // Window: Game
            ucGame = new ucSelectGame();
            ucGame.Chosen += ucGame_Chosen;

            ucTrackCars = new ucSelectTrackCars();
            ucTrackCars.Chosen += ucTrackCars_Chosen;

            ucMod = new ucSelectModel();

            // Button Back
            btBack = new Button();
            btBack.Text = "Back";
            btBack.Size = new Size(50, 25);
            btBack.Location = new Point(10, 10);
            btBack.BackColor = Color.White;
            btBack.Click += btBack_Click;

            Resize += fGarage_Resize;
            Redraw();
        }

        private void ucTrackCars_Chosen(object sender)
        {
            // TODO: ADd track selection as well!
            var smod = sender.ToString();
            Mod = TelemetryApplication.Simulator.Mods.FirstOrDefault(x => x.Name.Equals(smod));

            Window = Mod == null ? GarageWindow.TrackCars : GarageWindow.Mod;
            Redraw();
        }

        private void ucGame_Chosen(object sim)
        {
            Sim = TelemetryApplication.Plugins.Simulators.FirstOrDefault(x => x.Name.Equals(sim.ToString()));
            //Telemetry.m.Sims.Sims.Find(delegate(ISimulator s) { return s.Name.Equals(sim.ToString()); });
            if (Sim == null || (Sim.TrackProvider == null && Sim.CarProvider == null))
            {
                // TODO: Display errors.
                // TODO: Check if sim is installed.
                Window = GarageWindow.GameSelect;
            }
            else
            {
                Window = GarageWindow.TrackCars;

                // TODO: Setup garage & repo's
            }

            Redraw();
        }

        private void ActionBack()
        {
            switch(Window)
            {
                case GarageWindow.GameSelect:
                    this.Close();
                    break;

                case GarageWindow.TrackCars:
                    Window = GarageWindow.GameSelect;
                    break;

                case GarageWindow.Mod:
                    Window = GarageWindow.TrackCars;
                    break;
            }
            if (Window == GarageWindow.GameSelect)
                Sim = null;
            Redraw();
        }

        private void btBack_Click(object sender, EventArgs e)
        {
            ActionBack();
        }

        private void ucGame_Close()
        {
            ActionBack();
        }

        private void Redraw()
        {
            if (Controls.Count > 0)
                ((IGarageUserControl) Controls[0]).Close -= ucGame_Close;
            Controls.Clear();
            BackColor = Color.Black;
            switch(Window)
            {
                case GarageWindow.GameSelect:
                    Controls.Add(ucGame);
                    break;

                case GarageWindow.TrackCars:
                    Controls.Add(ucTrackCars);
                    break;

                case GarageWindow.Mod:
                    Controls.Add(ucMod);
                    break;
            }

            ((IGarageUserControl)Controls[0]).Close += ucGame_Close;
            Controls[0].Dock = DockStyle.Fill;
            ((IGarageUserControl)Controls[0]).Draw();
            ((IGarageUserControl)Controls[0]).Resize();

            if(GarageWindow.GameSelect != Window)
                Controls.Add(btBack);
        }

        void fGarage_Resize(object sender, EventArgs e)
        {
            var uc = (IGarageUserControl) Controls[0];
            uc.Resize();
        }

    }
}
