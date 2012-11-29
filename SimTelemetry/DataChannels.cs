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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Objects;
using Triton.Controls;

namespace SimTelemetry
{
    public partial class DataChannels : Form
    {
        private VisualListDetails channels;
        public DataChannels()
        {
            InitializeComponent();

            channels = new VisualListDetails(true);
            channels.Location = new Point(10, 10);
            channels.Size = new Size(350, 500);

            channels.Columns.Add("id", "#", 50);
            channels.Columns.Add("name", "Name", 150);
            channels.Columns.Add("freq", "", 50);
            
            Controls.Add(channels);



            PropertyAnalyser(typeof(IDriverGeneral));
            PropertyAnalyser(typeof(IDriverPlayer));
            PropertyAnalyser(typeof(ISession));

            channels.ShowGroups = true;
        }

        private Dictionary<string, int> PropertyAnalyser(Type type)
        {
            channels.Groups.Add(new ListViewGroup(type.Name));
            Dictionary<string, int> Properties = new Dictionary<string, int>();
            PropertyDescriptorCollection PropertyDescriptors;
            
            PropertyDescriptors = TypeDescriptor.GetProperties(type);
            PropertyInfo[] pic = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            List<ListViewItem> MyItems = new List<ListViewItem>();
            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = PropertyDescriptors[pi.Name];
                if (fi == null || fi.Attributes.Contains(new Unloggable()))
                    continue;

                // ---------- Unloggable ----------
                object[] attrs = pi.GetCustomAttributes(typeof (Unloggable), false);
                if (attrs.Length == 1) // is unloggable?
                    continue;

                string name = fi.Name;

                // Description
                attrs = pi.GetCustomAttributes(typeof(LogProperty), false);
                if (attrs.Length == 1)
                    name = ((LogProperty)attrs[0]).Name;

                Properties.Add(fi.Name, 0);

                // ---------- Frequency ----------
                double Frequency = 0; // 100Hz normal
                attrs = pi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;

                i++;

                MyItems.Add(new ListViewItem(new string[3] { i.ToString(), name, Frequency.ToString("000Hz") }, channels.Groups[channels.Groups.Count-1]));
            }
            MyItems.Sort(delegate(ListViewItem lvi1, ListViewItem lvi2)
                             {
                                 return lvi1.SubItems[1].Text.CompareTo(lvi2.SubItems[1].Text);
                             });
            channels.Items.AddRange(MyItems.ToArray());
            return Properties;
        }
    }
}
