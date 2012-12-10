﻿/*************************************************************************
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
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using SimTelemetry.Objects;
using Triton;

namespace SimTelemetry.Data.Net
{
    public class TelemetryClient
    {
        public event AnonymousSignal Connected;
        public event AnonymousSignal Disconnected;
        public event AnonymousSignal ConnectFailure;
        public event Signal Packet;

        public int Port { get; set; }
        public string IP { get; set; }

        private TcpClient _mClient;

        private NetworkStream _mStream { get { return _mClient.GetStream(); } }
        private Thread _mThread;

        public bool Connect()
        {
            try
            {
                _mClient = new TcpClient(IP, Port);
                _mThread = new Thread(AcceptPackets);
                _mThread.IsBackground = true;
                _mThread.Start();
                if (Connected != null)
                    Connected();
                return true;
            }
            catch (Exception ex)
            {
                if (ConnectFailure != null)
                    ConnectFailure();

                return false;
            }
        }

        public void AcceptPackets()
        {
            List<byte> RxBuffer = new List<byte>();
            while(_mClient.Connected)
            {

                try
                {
                    // TODO: This is really really messy.
                    byte[] rxbuf = new byte[256*1024];
                    int available = _mStream.Read(rxbuf, 0, 256 * 1024);
                    byte[] rxbuf2 = new byte[available];
                    Array.Copy(rxbuf, rxbuf2, available);
                    RxBuffer.AddRange(rxbuf2);

                    while (RxBuffer.Count >= 6)
                    {
                        if (RxBuffer.Count > 2*1024*1024) // Cap buffer at 2MiB
                            RxBuffer.RemoveRange(0, RxBuffer.Count - 2*1024*1024); // remove x bytes so 2 MiB remains.

                        try
                        {
                            if (RxBuffer[0] == (byte) '$' && RxBuffer[1] == (byte) '#') /// 0x24 + 0x23
                            {
                                int size = BitConverter.ToInt32(RxBuffer.ToArray(), 2);
                                if (RxBuffer.Count > size)
                                {
                                    RxBuffer.RemoveRange(0, 6);

                                    NetworkPacket pack =
                                        (NetworkPacket) ByteMethods.DeserializeFromBytes(RxBuffer.ToArray());
                                    if (Packet != null)
                                        Packet(pack);

                                    RxBuffer.RemoveRange(0, size);
                                }
                                else
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        int next = RxBuffer.IndexOf((byte) '$');
                        if (next >= 2)
                            RxBuffer.RemoveRange(0, next - 1);
                    }


                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(5);
            }
        }

        public void Disconnect()
        {
            _mClient.Close();

            if (Disconnected != null)
                Disconnected();
        }
    }
}