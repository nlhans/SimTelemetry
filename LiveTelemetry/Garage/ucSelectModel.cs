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
using System.Linq;
using System.Windows.Forms;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Repositories;
using Triton;
using Triton.Controls;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectModel : UserControl, IGarageUserControl
    {

        private VisualListDetails _models;

        public event AnonymousSignal Close;
        public event Signal Chosen;
        private List<ListViewItem> models = new List<ListViewItem>();

        private Car car;

        private ucSelectModel_EngineCurve ucEngine;

        public ucSelectModel()
        {
            InitializeComponent();
            _models = new VisualListDetails(true);

            _models.Columns.Add("id", "Description", 130);
            _models.Columns.Add("team", "Team", 160);
            _models.Columns.Add("file", "File", 1);
            _models.MultiSelect = false;
            _models.ItemSelectionChanged += _models_ItemSelectionChanged;

            splitContainer1.Panel1.Controls.Add(_models);

            ucEngine = new ucSelectModel_EngineCurve();
            ucEngine.CurveUpdated += ucEngine_CurveUpdated;
            splitContainer1.Panel2.Controls.Add(ucEngine);

        }

        void ucEngine_CurveUpdated()
        {
            car.Engine.Apply(ucEngine.Settings_Speed, ucEngine.Settings_Throttle, ucEngine.Settings_Mode);

            UpdateLabels();
        }

        void _models_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string file = "";
            if (_models.SelectedItems.Count > 0)
                file = _models.SelectedItems[0].SubItems[2].Text;
            else if (_models.Items.Count == 0)
                return;
            else
                file = _models.Items[0].SubItems[2].Text;


            //car = fGarage.Sim.Garage.CarFactory(fGarage.Mod, file);

            car = TelemetryApplication.Cars.GetByFile(file);
            UpdateLabels();

            ucEngine.Load(car);
            Resize();

        }

        private void UpdateLabels()
        {
            if (car != null)
            {
                string lbl1 = "", lbl2 = "";
                lbl_Team.Text = car.Driver + " [" + car.Team + "]";

                lbl1 = "[Team]\n";
                lbl1 += "Start number: " + car.Team.Position.ToString() + "\n";
                lbl1 += "Engine: " + car.Engine.Manufacturer.ToString() + "\n";
                lbl1 += "\n";
                lbl1 += "Team Founded: " + car.Team.Founded.ToString() + "\n";
                lbl1 += "Team Headquarters: " + car.Team.Headquarters + "\n";
                if (car.Team.Races > 0)
                {
                    lbl1 += "Team Starts: " + car.Team.Races.ToString() + "\n";
                    lbl1 += "Team Pole positions: " + car.Team.Poles.ToString() + " (" +
                                      (100.0 * car.Team.Poles / car.Team.Races).ToString("000.0") + "%)\n";
                    lbl1 += "Team Race Wins: " + car.Team.Wins.ToString() + " (" +
                                      (100.0 * car.Team.Wins / car.Team.Races).ToString("000.0") + "%)\n";
                    lbl1 += "Team Championship Wins: " + car.Team.Championships.ToString() + "\n";
                }

                lbl1 += "\n";
                lbl1 += "[Car]\n";

                if (car.Engine != null)
                {
                    lbl2 = "[Engine]\n";
                    lbl2 += "Maximum RPM: " + car.Engine.MaximumRpm.Maximum.ToString("00000") + "rpm\n";
                    lbl2 += "Idle RPM: " + car.Engine.IdleRpm.Minimum.ToString("00000") + "rpm\n";

                    lbl2 += string.Format("Maximum torque: {0}nm  at {1} rpm\n", car.Engine.MaximumTorque.ToString("0000.0"), car.Engine.MaximumTorqueRpm.ToString("00000"));
                    lbl2 += string.Format("Maximum power: {0}hp at {1} rpm\n", car.Engine.MaximumPower.ToString("0000.0"), car.Engine.MaximumPowerRpm.ToString("00000"));

                    lbl2 += "Boost steps: " + car.Engine.Modes.Count().ToString() + "\n";
                }

                lbl_info1.Text = lbl1;
                lbl_info2.Text = lbl2;
            }
        }

        public void Draw()
        {
            models = new List<ListViewItem>();
            _models.Items.Clear();

            foreach (Car car in fGarage.Mod.Cars)
            {
                string driver = car.Description;

                if (car.Team.Position > 0 && driver.StartsWith("#") == false)
                    driver = "#" + car.Team.Position.ToString("000") + " " + driver;
                models.Add(
                    new ListViewItem(new string[3]
                                         {
                                             driver, car.Team.Name, car.ID
                                         }));
            }

            models.Sort((lvi1, lvi2) => lvi1.SubItems[0].Text.CompareTo(lvi2.SubItems[0].Text));

            _models.Items.AddRange(models.ToArray());
            _models_ItemSelectionChanged(null, null);
        }

        public void Resize()
        {

            ucEngine.Size = new Size(splitContainer1.Panel2.Width, splitContainer1.Panel2.Height - Math.Max(lbl_info2.Height, lbl_info1.Height) - lbl_info2.Location.Y);
            ucEngine.Location = new Point(0, splitContainer1.Panel2.Height - ucEngine.Size.Height);
            ucEngine.Resize();
            ucEngine.Invalidate();
        }
    }
}