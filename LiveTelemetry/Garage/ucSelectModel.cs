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

        public ucSelectModel()
        {
            InitializeComponent();
            _models = new VisualListDetails(true);

            _models.Columns.Add("id", "Description", 130);
            _models.Columns.Add("team", "Team", 160);

            splitContainer1.Panel1.Controls.Add(_models);

        }

        public void Draw()
        {
            _models.Items.Clear();

            foreach (ICar car in fGarage.Mod.Models)
            {
                car.Scan();
                string driver = car.Description;

                if (car.Number > 0 && driver.StartsWith("#") == false)
                    driver = "#" + car.Number.ToString("000") + " " + driver;
                models.Add(
                    new ListViewItem(new string[2]
                                         {
                                             driver, car.Team
                                         }));
            }

            models.Sort(
                delegate(ListViewItem lvi1, ListViewItem lvi2)
                { return lvi1.SubItems[0].Text.CompareTo(lvi2.SubItems[0].Text); });

            _models.Items.AddRange(models.ToArray());

        }

        public void Resize()
        {
            

        }
    }
}