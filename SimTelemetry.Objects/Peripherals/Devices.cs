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
using System.IO.Ports;
using SimTelemetry.Objects.Peripherals;

namespace SimTelemetry.Objects
{
    public class Devices : IDevices
    {
        public event DevicePacketEvent RX;
        private List<Device> devices = new List<Device>();

        public Devices()
        {
            // Try looking for devices (Serial Ports)
            string[] com_ports = SerialPort.GetPortNames();

            foreach (string port in com_ports)
            {
                string name = "";
                // TODO: Auto detect device type
                if (port == "COM1") continue;
                if (port == "COM4")
                    name = "Dashboard";
                //if (port == "COM18")
                else
                    name = "Switchboard";
                try
                {
                    Device ph = new Device(name, Convert.ToInt32(port.Substring(3)), 115200);
                    ph.RX += ph_RX;
                    devices.Add(ph);
                }
                catch (Exception ex)
                { }
            }

        }

        private void ph_RX(DevicePacket packet, object sender)
        {
            if (RX != null)
                RX(packet, sender);
        }

        public void TX(DevicePacket packet, string destination)
        {
            if (destination == "")
            {
                foreach (Device ph in devices)
                    ph.TX(packet);
            }
            else
            {
                foreach (Device ph in devices)
                    if (ph.Name == destination)
                        ph.TX(packet);
            }

        }

    }
}