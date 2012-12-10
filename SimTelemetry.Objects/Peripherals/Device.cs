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
using System.Threading;
using SimTelemetry.Objects.Peripherals;

namespace SimTelemetry.Objects
{
    public class Device : IDevice
    {
        private Semaphore SerialPort_Lock = new Semaphore(1, 1);
        public event DevicePacketEvent RX;
        public SerialPort SP;
        private List<byte> RX_Buffer = new List<byte>();
        private List<DevicePacket> TX_Buffer = new List<DevicePacket>();
        private Thread TX_Timer;
        public string Name { get; set; }

        public Device(string name, int com, int baud)
        {
            this.Name = name;

            SP = new SerialPort("COM" + com.ToString(), baud);
            SP.DataReceived += new SerialDataReceivedEventHandler(SP_DataReceived);
            SP.Open();
            TX_Timer = new Thread(TX_Thread);
            TX_Timer.IsBackground = true;
            TX_Timer.Priority = ThreadPriority.Highest;
            TX_Timer.Start();
            Console.WriteLine("Device added: " + Name + " at (COM"+com.ToString()+")");
        }

        private void TX_Thread()
        {
            while (true)
            {
                while (TX_Buffer.Count != 0)
                {
                    TX_Sync(TX_Buffer[0]);
                    TX_Buffer.RemoveAt(0);
                }
                Thread.Sleep(1);
            }
        }
         
        private void TX_Sync(DevicePacket devicePacket)
        {
            SerialPort_Lock.WaitOne();
            try
            {
                DashboardPacket packet = new DashboardPacket();
                packet.Sync1 = '$';
                packet.Sync2 = '&';
                packet.length = (UInt16)(devicePacket.Length);
                packet.id = (byte)devicePacket.ID;
                packet.crc = 0xAA;
                for (int i = 0; i < devicePacket.Data.Length; i++)
                    packet.crc += devicePacket.Data[i];

                // Write header
                byte[] bPacket = ByteMethods.ToBytes(packet);
                SP.Write(bPacket, 0, 6);

                // Write packet data
                SP.Write(devicePacket.Data, 0, devicePacket.Length);

                // Write junk incase bytes are missed on embedded system.
                // This way there is a better chance the next package is received.
                byte[] bf = new byte[2];
                bf[0] = 0;
                bf[1] = 0;
                SP.Write(bf, 0, 2);
            }
            catch (Exception ex)
            { }
            SerialPort_Lock.Release();
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (SP.BytesToRead > 0)
                {
                    byte[] bf = new byte[SP.BytesToRead];
                    SP.Read(bf, 0, bf.Length);
                    RX_Buffer.AddRange(bf);
                }
                // Search for packages
                for (int i = 0; i < RX_Buffer.Count; i++)
                {
                    if (RX_Buffer[i] == '$' && RX_Buffer[i + 1] == '&' && RX_Buffer.Count - i > 4)
                    {

                        int length = BitConverter.ToUInt16(RX_Buffer.ToArray(), i + 2);
                        int id = RX_Buffer[i + 4];
                        byte[] data = new byte[length];
                        if (length + i > RX_Buffer.Count && length < 128)
                            break;

                        ByteMethods.memcpy(data, RX_Buffer.ToArray(), length, 0, i + 5);

                        DevicePacket packet = new DevicePacket{Data=data, ID=id, Length=length};
                        if (RX != null) RX(packet, this);

                        // Done
                        if (length + i > RX_Buffer.Count)
                            RX_Buffer.Clear();
                        else
                            RX_Buffer.RemoveRange(i, length + 5);

                    }

                }
                if (RX_Buffer.Count > 300)
                    RX_Buffer.Clear();
            }
            catch (Exception ex) { }
        }

        public void TX(DevicePacket packet)
        {
                TX_Buffer.Add(packet);
        }

    }
}
