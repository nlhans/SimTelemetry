using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimTelemetry.Objects.Garage;
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

        private ICar car;
        private CarEngineTools engineinfo;

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
            ucEngine.CurveUpdated += new AnonymousSignal(ucEngine_CurveUpdated);
            splitContainer1.Panel2.Controls.Add(ucEngine);

        }

        void ucEngine_CurveUpdated()
        {
            engineinfo.Scan(ucEngine.Settings_Speed, ucEngine.Settings_Throttle,
                            ucEngine.Settings_Mode);
            UpdateLabels();
        }

        void _models_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (_models.SelectedItems.Count > 0)
            {
                string file = _models.SelectedItems[0].SubItems[2].Text;

                car = fGarage.Sim.Garage.CarFactory(fGarage.Mod, file);

                engineinfo = new CarEngineTools(car);
                engineinfo.Scan();

                UpdateLabels();

                ucEngine.Load(car, engineinfo);
                Resize();
            }
        }

        private void UpdateLabels()
        {


            lbl_Team.Text = car.Driver + " [" + car.Team + "]";

            lbl_info1.Text = "[Team]\n";
            lbl_info1.Text += "Start number: " + car.Number.ToString() + "\n";
            lbl_info1.Text += "Engine: " + car.Info_Engine_Manufacturer.ToString() + "\n";
            lbl_info1.Text += "\n";
            lbl_info1.Text += "Team Founded: " + car.Info_YearFounded.ToString() + "\n";
            lbl_info1.Text += "Team Headquarters: " + car.Info_HQ + "\n";
            if (car.Info_Starts > 0)
            {
                lbl_info1.Text += "Team Starts: " + car.Info_Starts.ToString() + "\n";
                lbl_info1.Text += "Team Pole positions: " + car.Info_Poles.ToString() + " (" +
                                  (100.0 * car.Info_Poles / car.Info_Starts).ToString("000.0") + "%)\n";
                lbl_info1.Text += "Team Race Wins: " + car.Info_Wins.ToString() + " (" +
                                  (100.0 * car.Info_Wins / car.Info_Starts).ToString("000.0") + "%)\n";
                lbl_info1.Text += "Team Championship Wins: " + car.Info_Championships.ToString() + "\n";
            }

            lbl_info1.Text += "\n";
            lbl_info1.Text += "[Car]\n";

            lbl_info2.Text = "[Engine]\n";
            lbl_info2.Text += "Maximum RPM: " + car.Engine.MaxRPM.ToString("00000") + "rpm\n";
            lbl_info2.Text += "Idle RPM: " + car.Engine.IdleRPM.ToString("00000") + "rpm\n";

            lbl_info2.Text += "Maximum torque: " + engineinfo.MaxTorque_NM.ToString("0000.0") + "nm  at " + engineinfo.MaxTorque_RPM.ToString("00000") + " rpm\n";
            lbl_info2.Text += "Maximum power: " + engineinfo.MaxPower_HP.ToString("0000.0") + "hp at " + engineinfo.MaxPower_RPM.ToString("00000") + " rpm\n";

            lbl_info2.Text += "Boost steps: " + car.Engine.EngineModes.Count.ToString() + "\n";
        }

        public void Draw()
        {
            models = new List<ListViewItem>();
            _models.Items.Clear();

            foreach (ICar car in fGarage.Mod.Models)
            {
                car.Scan();
                string driver = car.Description;

                if (car.Number > 0 && driver.StartsWith("#") == false)
                    driver = "#" + car.Number.ToString("000") + " " + driver;
                models.Add(
                    new ListViewItem(new string[3]
                                         {
                                             driver, car.Team, car.File
                                         }));
            }

            models.Sort(
                delegate(ListViewItem lvi1, ListViewItem lvi2)
                { return lvi1.SubItems[0].Text.CompareTo(lvi2.SubItems[0].Text); });

            _models.Items.AddRange(models.ToArray());

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