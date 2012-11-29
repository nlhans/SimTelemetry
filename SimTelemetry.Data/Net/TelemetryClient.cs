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
using System.Net.Sockets;
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

        private byte[] rx_buffer = new byte[128 * 1024];
        private byte[] tx_buffer = new byte[128 * 1024];

        private TcpClient _mClient;

        private NetworkStream _mStream { get { return _mClient.GetStream(); } }
        public TelemetryClient()
        {


        }

        public bool Connect()
        {
            try
            {
                _mClient = new TcpClient(IP, Port);
                _mStream.BeginRead(rx_buffer, 0, rx_buffer.Length, RxDone, this);
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


        private void RxDone(IAsyncResult ar)
        {
            byte[] rx_data = new byte[0];
            int read = 0;
            try
            {

                if (ar != null)
                {
                    read = _mStream.EndRead(ar);

                    rx_data = new byte[read];
                    Array.Copy(rx_buffer, rx_data, read);

                }
            }
            catch (Exception ex)
            {

            }

            if (read > 0)
            {
                int last_i = 0;
                // Process.
                for (int i = 0; i < read - 8; i++)
                {
                    if (rx_data[i] == '^' && rx_data[i + 1] == '%')
                    {
                        try
                        {
                            short type = BitConverter.ToInt16(rx_data, i + 2);
                            int length = BitConverter.ToInt32(rx_data, i + 4);

                            byte[] packet = new byte[length];
                            Array.Copy(rx_data, i + 8, packet, 0, length);
                            i += length;
                            last_i = i;
                            NetworkPacket np = new NetworkPacket(type, packet);
                            if (Packet != null)
                                Packet(np);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                if (last_i < read)
                {
                    int left = read - last_i;
                    // We haven't read till end of buffer.
                    // So copy bytes back into rx_buffer.
                    Array.Copy(rx_data, last_i, rx_buffer, 0, left);


                    if (_mStream != null)
                        _mStream.BeginRead(rx_buffer, left, rx_buffer.Length-left, RxDone, this);
                }
                else if (_mStream != null)
                    _mStream.BeginRead(rx_buffer, 0, rx_buffer.Length, RxDone, this);
            }
            else if (_mStream != null)
                _mStream.BeginRead(rx_buffer, 0, rx_buffer.Length, RxDone, this);
        }

        public void Disconnect()
        {
            _mClient.Close();

            if (Disconnected != null)
                Disconnected();
        }
    }
}