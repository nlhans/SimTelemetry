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
            // Try looking for..
            string[] com_ports = SerialPort.GetPortNames();

            foreach (string port in com_ports)
            {
                string name = "";
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