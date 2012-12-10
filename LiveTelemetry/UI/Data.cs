using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data;
using SimTelemetry.Objects;
using SimTelemetry.Objects.HyperType;

namespace LiveTelemetry.UI
{
    public partial class Data : Form
    {
        private Timer t = new Timer();

        public Data()
        {
            InitializeComponent();

            t = new Timer {Interval = 100};
            t.Tick += new EventHandler(t_Tick);
            t.Start();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        void t_Tick(object sender, EventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new EventHandler(t_Tick), new object[2] {sender, e});
                return;
            }
            labels.Controls.Clear();

            // Display:
            // Sim.Session
            // Sim.Drivers.Player
            // Sim.Player
            if (Telemetry.m.Active_Session)
            {
                List<string> session = DumpToLabels(Telemetry.m.Sim.Session, typeof(ISession));
                List<string> driver = DumpToLabels(Telemetry.m.Sim.Drivers.Player, typeof(IDriverGeneral));
                List<string> player = DumpToLabels(Telemetry.m.Sim.Player, typeof(IDriverPlayer));

                session[0] = "[Telemetry.m.Sim.Session]\r\n" + session[0];
                driver[0] = "[Telemetry.m.Sim.Drivers.Player]\r\n" + driver[0];
                player[0] = "[Telemetry.m.SIm.Player]\r\n" + player[0];

                int colsize = (this.Width - 20)/(session.Count + driver.Count + player.Count) - 5;
                int l = 10;
                colsize = 230;
                this.AutoScroll = true;
                foreach (string d in session)
                {
                    if (d.Trim() != "")
                    {
                        Label lbl = new Label
                                        {Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f)};
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
                foreach (string d in driver)
                {
                    if (d.Trim() != "")
                    {
                        Label lbl = new Label { Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f) };
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
                foreach (string d in player)
                {
                    if (d.Trim() != "")
                    {
                        Label lbl = new Label { Text = d, Size = new Size(colsize, this.Height), Location = new Point(l, 10), Font = new Font("Tahoma", 7.0f) };
                        labels.Controls.Add(lbl);
                        l += colsize + 5;
                    }
                }
            }
            else
            {

                Label lbl = new Label { Size = new Size(this.Width, this.Height), Location = new Point(10, 10), Font = new Font("Tahoma", 36.0f) };
                if(!Telemetry.m.Active_Sim)
                {
                    lbl.Text = "No simulator running";
                }
                else
                {
                    lbl.Text = "No session running in simulator " + Telemetry.m.Sim.Name;
                }
                labels.Controls.Add(lbl);
            }


        }

        private List<string> DumpToLabels(object inst, Type type)
        {
            List<StringBuilder> builders = new List<StringBuilder>();
                    builders.Add(new StringBuilder());
            int row = 0;
            int column = 0;
            int rows = this.Height/12-6;
            // Make it go FAST
            HyperTypeDescriptionProvider.Add(type);
                
            PropertyDescriptorCollection PropertyDescriptors = TypeDescriptor.GetProperties(type);
            PropertyInfo[] pic = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = PropertyDescriptors[pi.Name];
                object  value = fi.GetValue(inst);
                string txt = "";
                if(fi.PropertyType == typeof(double))
                {
                    txt = string.Format("{0} = {1} ", pi.Name, Math.Round((double)value, 3));
                }
                else if (fi.PropertyType == typeof(float))
                {
                    txt = string.Format("{0} = {1} ", pi.Name, Math.Round((float)value,3));
                }
                else
                {
                    txt = string.Format("{0} = {1} ", pi.Name, value.ToString());
                }
                builders[column].AppendLine(txt);
                if (txt.Length > 30)
                    row++; // double rows
                row++;

                // Go to next side.
                if(row == rows)
                {
                    builders.Add(new StringBuilder());
                    column++;
                    row = 0;
                }
            }

            return builders.Select(x => x.ToString()).ToList();
        }
    }
}
