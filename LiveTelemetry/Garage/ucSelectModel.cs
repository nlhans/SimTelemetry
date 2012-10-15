using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Objects.Garage;
using Triton;
using Triton.Controls;

namespace LiveTelemetry.Garage
{
    public partial class ucSelectModel : UserControl, IGarageUserControl
    {
        private VisualListDetails _models;
        public ucSelectModel()
        {
            InitializeComponent();
            _models = new VisualListDetails(true);

            _models.Columns.Add("id", "Driver",150);
            _models.Columns.Add("team","Team",140);

            splitContainer1.Panel1.Controls.Add(_models);


        }

        public event AnonymousSignal Close;
        public event Signal Chosen;
        public void Draw()
        {
            _models.Items.Clear();

            List<ListViewItem> i = new List<ListViewItem>();
            foreach(ICar car in fGarage.Mod.Models)
            {
                car.Scan();
                i.Add(
                    new ListViewItem(new string[2]
                                         {
                                             car.Number.ToString("#000 ") + car.Driver, car.Team
                                         }));
            }

            i.Sort(
                delegate(ListViewItem lvi1, ListViewItem lvi2)
                    { return lvi1.SubItems[0].Text.CompareTo(lvi2.SubItems[0].Text); });
            _models.Items.AddRange(i.ToArray());

        }
    }
}
