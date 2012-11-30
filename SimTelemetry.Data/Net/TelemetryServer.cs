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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Threading;

namespace SimTelemetry.Data.Net
{
    public class TelemetryServer
    {
        private Thread _mServer;
        private Thread _mKeeper;
        private bool _mServerRunning;
        private TcpListener _mTcpServer;
        private List<TelemetryServerClient> _mClients;
        private ManualResetEvent _mTcpServerClientAccepted;
        public List<TelemetryServerClient> ClientList { get { return _mClients; } set { _mClients = value; } }
        public int Clients { get { return _mClients.Count; } }
        public int Traffic { get; set; }

        public int Port { get; set; }

        public bool Running { get { return _mServerRunning; } }

        public TelemetryServer()
        {
            // Temporary:
            Port = 12345;

            _mClients = new List<TelemetryServerClient>();
            _mTcpServerClientAccepted = new ManualResetEvent(false);

            _mServerRunning = false;
        }

        public bool Start()
        {
            try
            {
                if (_mServer == null)
                {
                    _mServerRunning = true;
                    _mServer = new Thread(ClientAcceptThread);
                    _mServer.IsBackground = true;
                    _mServer.Start();

                    _mKeeper = new Thread(ServerKeeper);
                    _mKeeper.IsBackground = true;
                    _mKeeper.Start();
                }
                return true;
            }
            catch (Exception ex)
            {
                _mServerRunning = false;
                _mServer = null;
                _mKeeper = null;
                //
                return false;
            }
        }

        private void ServerKeeper()
        {
            while (_mServerRunning)
            {
                List<TelemetryServerClient> DisconnectedClients = new List<TelemetryServerClient>();
                foreach(TelemetryServerClient client in _mClients)
                {
                    if (client.Connected == false)
                        DisconnectedClients.Add(client);
                }
                DisconnectedClients.ForEach(delegate(TelemetryServerClient cl) { _mClients.Remove(cl); });
                if (DisconnectedClients.Count > 0)
                {
                    Console.WriteLine("Removed " + DisconnectedClients.Count + " clients");
                }
                Thread.Sleep(100);
            }
        }

        private void ClientAcceptThread()
        {
            try
            {
                _mTcpServer = new TcpListener(Port);
                _mTcpServer.Start();

                while (_mServerRunning)
                {
                    _mTcpServerClientAccepted.Reset();
                    _mTcpServer.BeginAcceptTcpClient(ClientConnect, this);
                    _mTcpServerClientAccepted.WaitOne();
                }

                _mTcpServer.Stop();
                _mTcpServer = null;

            }
            catch (Exception ex)
            {
                //
            }

        }

        public void ClientConnect(IAsyncResult iar)
        {
            try
            {
                if (iar != null && _mTcpServer != null && _mTcpServer.Server != null && _mTcpServer.Server.IsBound)
                {
                    TcpClient client = _mTcpServer.EndAcceptTcpClient(iar);

                    _mClients.Add(new TelemetryServerClient(client));
                    _mTcpServerClientAccepted.Set();

                    Console.WriteLine(client.Client.RemoteEndPoint.ToString() + " client connected");
                }
            }
            catch(Exception ex)
            {
                if (_mTcpServer != null)
                    _mTcpServerClientAccepted.Set();

                Console.WriteLine("Failed to connect client!");
            }
        }

        public void PushGameData(byte[] data)
        {
            if (_mClients != null)
            {
                foreach (TelemetryServerClient sc in _mClients)
                {
                    Traffic += data.Length;
                    sc.PushGameData(data);
                }
            }
        }

        public void FlushGameData()
        {
            if (_mClients != null)
            {
                foreach (TelemetryServerClient sc in _mClients)
                    sc.FlushGameData();
            }
        }

        public void Stop()
        {
            if (_mServer != null)
            {
                foreach (TelemetryServerClient sc in _mClients)
                    sc.EndConnection();

                _mServerRunning = false;
                _mTcpServerClientAccepted.Set(); // fake client connect to force exit
                Thread.Sleep(1);

                _mServer.Abort();
                _mKeeper.Abort();
                _mServer = null;
                _mKeeper = null;
                _mClients = new List<TelemetryServerClient>();
            }
        }
    }
}