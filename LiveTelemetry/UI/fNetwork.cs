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
using System.Windows.Forms;
using SimTelemetry.Domain.Aggregates;

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
                                           if (TelemetryApplication.NetworkHost)
                                           {
                                               // TODO: Add traffic.
                                               txt = string.Format("Running server - {0} clients connected - {1}kB/s upload", TelemetryApplication.Telemetry.Session.Cars, Math.Round(0/3/1024.0, 1));
                                               }
                                           else if (TelemetryApplication.NetworkHost == false)
                                               txt = string.Format("Client connected to {0}:{1}", "", ""); //Telemetry.m.Net.Listener.IP, Telemetry.m.Net.Listener.Port);
                                           else
                                           {
                                               txt = "Offline mode";
                                           }
                                           lbl_Status.Text = txt;
                                           lbl_Status.ForeColor = Color.Black;
                                       };
            _mStatusUpdate.Interval = 1;
            _mStatusUpdate.Start();

            // TODO: Add error event
            //Telemetry.m.Net.Error += new Triton.Signal(Net_Error);
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
            if (TelemetryApplication.NetworkHost)
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
            else if (!TelemetryApplication.NetworkHost)
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
            if (TelemetryApplication.NetworkHost)
            {
                // Already started
                //Telemetry.m.Net.AbortServer();

            }
            else if (!TelemetryApplication.NetworkHost)
                MessageBox.Show("Must close client connection first.");
            else
            {
                // TODO: Adjust settings.
                //Telemetry.m.Net.ConfigureServer(tb_Server_Port.Text, tb_Server_Clients.Text, tb_Server_Bandwidth.Value);

            }

            UpdateFormComponents();
        }

        private void bt_Client_StartStop_Click(object sender, EventArgs e)
        {

            if (!TelemetryApplication.NetworkHost)
            {
                // Already started, abort
                //Telemetry.m.Net.AbortClient();

            }
            else if (TelemetryApplication.NetworkHost)
                MessageBox.Show("Must close server first.");
            else
            {
                // TODO: Adjust settings.
                //Telemetry.m.Net.ConfigureClient(tb_Client_IP.Text, tb_Client_Port.Text);

            }

            UpdateFormComponents();
        }

        private void tb_Server_Bandwidth_ValueChanged(object sender, EventArgs e)
        {
            int cars = 20;
            if (TelemetryApplication.TelemetryAvailable)
                cars = TelemetryApplication.Telemetry.Session.Cars;

            if (cars == 1)
                cars = 20;
            
            double traffic_1car = 1500/1024.0 + tb_Server_Bandwidth.Value*650.0/1024.0;
            double traffic_xcars = traffic_1car + 80*cars*tb_Server_Bandwidth.Value/1024.0;
            lbl_Server_BandwidthSetting.Text = tb_Server_Bandwidth.Value + " Hz\r\n1 car: " + Math.Round(traffic_1car, 1) + "kB/s  - " + cars + " cars: " + Math.Round(traffic_xcars, 1) + "kB/s";

            // We're a host?
            if (TelemetryApplication.NetworkHost)
            {
                // TODO: Re-enable
                //Telemetry.m.Net.HostData.Bandwidth = tb_Server_Bandwidth.Value;
            }
        }
    }
}
