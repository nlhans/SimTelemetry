using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using SimTelemetry.Objects.Peripherals;
using Timer = System.Timers.Timer;

namespace SimTelemetry.Peripherals
{
    public class Device : IDevice
    {
        private Semaphore SerialPort_Lock = new Semaphore(1, 1);
        public event DevicePacketEvent RX;
        public SerialPort SP;
        private List<byte> RX_Buffer = new List<byte>();
        private List<DevicePacket> TX_Buffer = new List<DevicePacket>();
        private Timer TX_Timer;

        public string Name { get; set; }

        public Device(string name, int com, int baud)
        {
            this.Name = name;

            SP = new SerialPort("COM" + com.ToString(), baud);
            SP.DataReceived += new SerialDataReceivedEventHandler(SP_DataReceived);
            SP.Open();

            TX_Timer = new Timer{AutoReset = true, Enabled=true, Interval=5};
            TX_Timer.Elapsed += new ElapsedEventHandler(TX_Timer_Elapsed);
        }

        private void TX_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SerialPort_Lock.WaitOne();
            if(TX_Buffer.Count != 0)
            {
                lock (TX_Buffer)
                {
                    TX_Sync(TX_Buffer[0]);
                    TX_Buffer.RemoveAt(0);
                }
            }
            SerialPort_Lock.Release();
        }

        private void TX_Sync(DevicePacket devicePacket)
        {
            DashboardPacket packet = new DashboardPacket();
            packet.Sync1 = '$';
            packet.Sync2 = '&';
            packet.length = (UInt16)(devicePacket.Length);
            packet.id = (byte)devicePacket.ID;

            // Write header
            byte[] bPacket = ByteMethods.ToBytes(packet);
            SP.Write(bPacket, 0, 5);

            // Write packet data
            SP.Write(devicePacket.Data, 0, devicePacket.Length);

            // Write junk incase bytes are missed on embedded system.
            // This way there is a better chance the next package is received.
            byte[] bf = new byte[2];
            bf[0] = 0;
            bf[1] = 0;
            SP.Write(bf, 0, 2);
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] bf = new byte[SP.BytesToRead];
                SP.Read(bf, 0, bf.Length);
                RX_Buffer.AddRange(bf);

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
            lock (TX_Buffer)
            {
                TX_Buffer.Add(packet);
            }
        }

    }
}
