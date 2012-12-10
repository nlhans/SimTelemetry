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
using System.Text;
using System.Windows.Forms;
using SimTelemetry.Data;

namespace LiveTelemetry.UI
{
    public partial class fNetwork : Form
    {
        private Timer _mStatusUpdate = new Timer();

        public fNetwork()
        {
            InitializeComponent();
            UpdateFormComponents();

            tb_Server_Bandwidth.Value = 10;

            _mStatusUpdate.Tick += (e, sender) =>
                                       {
                                           _mStatusUpdate.Interval = 3000;
                                           string txt = "";
                                           if (Telemetry.m.Net.IsHost)
                                           {
                                               txt = "Running server - " + Telemetry.m.Net.Host.Clients +
                                                     " clients connected - " +
                                                     Math.Round(Telemetry.m.Net.Host.Traffic/3/1024.0, 1)
                                                     + "kB/s upload";
                                               Telemetry.m.Net.Host.Traffic = 0;
                                           }
                                           else if (Telemetry.m.Net.IsClient)
                                               txt = "Client connected to " + Telemetry.m.Net.Listener.IP + ":" +
                                                     Telemetry.m.Net.Listener.Port;
                                           else
                                           {
                                               txt = "Offline mode";
                                           }
                                           lbl_Status.Text = txt;
                                           lbl_Status.ForeColor = Color.Black;
                                       };
            _mStatusUpdate.Interval = 1;
            _mStatusUpdate.Start();

            Telemetry.m.Net.Error += new Triton.Signal(Net_Error);
        }

        void Net_Error(object sender)
        {
            lbl_Status.Text = sender.ToString();
            lbl_Status.ForeColor = Color.Red;
            _mStatusUpdate.Stop();
            //reset timer to 0ms, set interval to 5s
            _mStatusUpdate.Interval = 5000;
            _mStatusUpdate.Start();
        }

        private void UpdateFormComponents()
        {
            if (Telemetry.m.Net.IsHost)
            {
                bt_Server_StartStop.Text = "Stop";
                bt_Server_StartStop.Enabled = true;

                tb_Client_Port.Enabled = false;
                tb_Client_IP.Enabled = false;
                tb_Server_Port.Enabled = false;
                tb_Server_Clients.Enabled = false;
                tb_Server_Bandwidth.Enabled = true;

                bt_Client_StartStop.Text = "Start";
                bt_Client_StartStop.Enabled = false;

            }
            else if (Telemetry.m.Net.IsClient)
            {
                bt_Client_StartStop.Text = "Stop";
                bt_Client_StartStop.Enabled = true;

                tb_Client_Port.Enabled = false;
                tb_Client_IP.Enabled = false;
                tb_Server_Port.Enabled = false;
                tb_Server_Clients.Enabled = false;
                tb_Server_Bandwidth.Enabled = false;

                bt_Server_StartStop.Text = "Start";
                bt_Server_StartStop.Enabled = false;

            }
            else
            {

                bt_Client_StartStop.Text = "Start";
                bt_Client_StartStop.Enabled = true;

                bt_Server_StartStop.Text = "Start";
                bt_Server_StartStop.Enabled = true;

                tb_Client_Port.Enabled = true;
                tb_Client_IP.Enabled = true;

                tb_Server_Port.Enabled = true;
                tb_Server_Clients.Enabled = true;

                tb_Server_Bandwidth.Enabled = true;
            }
        }

        private void bt_Server_StartStop_Click(object sender, EventArgs e)
        {
            if (Telemetry.m.Net.IsHost)
            {
                // Already started
                Telemetry.m.Net.AbortServer();

            }
            else if (Telemetry.m.Net.IsClient)
                MessageBox.Show("Must close client connection first.");
            else
            {
                // TODO: Adjust settings.
                Telemetry.m.Net.ConfigureServer(tb_Server_Port.Text, tb_Server_Clients.Text, tb_Server_Bandwidth.Value);

            }

            UpdateFormComponents();
        }

        private void bt_Client_StartStop_Click(object sender, EventArgs e)
        {

            if (Telemetry.m.Net.IsClient)
            {
                // Already started, abort
                Telemetry.m.Net.AbortClient();

            }
            else if (Telemetry.m.Net.IsHost)
                MessageBox.Show("Must close server first.");
            else
            {
                // TODO: Adjust settings.
                Telemetry.m.Net.ConfigureClient(tb_Client_IP.Text, tb_Client_Port.Text);

            }

            UpdateFormComponents();
        }

        private void tb_Server_Bandwidth_ValueChanged(object sender, EventArgs e)
        {
            int cars = 20;
            if (Telemetry.m.Active_Session)
                cars = Telemetry.m.Sim.Session.Cars;
            if (cars == 1)
                cars = 20;
            double traffic_1car = 1500/1024.0 + tb_Server_Bandwidth.Value*650.0/1024.0;
            double traffic_xcars = traffic_1car + 80*cars*tb_Server_Bandwidth.Value/1024.0;
            lbl_Server_BandwidthSetting.Text = tb_Server_Bandwidth.Value + " Hz\r\n1 car: " + Math.Round(traffic_1car, 1) + "kB/s  - " + cars + " cars: " + Math.Round(traffic_xcars, 1) + "kB/s";

            if (Telemetry.m.Net.Host != null)
            {
                Telemetry.m.Net.HostData.Bandwidth = tb_Server_Bandwidth.Value;
            }
        }
    }
}
