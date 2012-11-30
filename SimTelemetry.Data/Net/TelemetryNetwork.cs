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
using System.Linq;
using System.Text;
using Triton;

namespace SimTelemetry.Data.Net
{
    public class TelemetryNetwork
    {
        public bool IsHost { get; protected set; }
        public bool IsClient { get; protected set; }

        public TelemetryServerData HostData { get; set; }
        public TelemetryServer Host { get; set; }
        public TelemetryClient Listener { get; set; }

        public event AnonymousSignal Change;
        public event Signal Error;

        public TelemetryNetwork()
        {
            IsHost = false;
            IsClient = false;
            Host = null;
        }

        private void FireError(string sError)
        {
            if (Error != null)
                Error(sError);
        }

        private void FireChange()
        {
            if (Change != null)
                Change();
        }

        public void AbortServer()
        {
            HostData.Stop();
            Host.Stop();
            Host = null;
            HostData = null;

            IsHost = false;
            IsClient = false;

            FireChange();
        }

        public void AbortClient()
        {
            Listener.Disconnect();
            Listener = null;


            IsHost = false;
            IsClient = false;

            FireChange();
        }

        public void ConfigureServer(string sPort, string sClients, int iBandwidth)
        {
            int port, clients;
            if (Int32.TryParse(sPort, out port) && Int32.TryParse(sClients, out clients))
            {
                Host = new TelemetryServer();
                HostData  = new TelemetryServerData(Host, iBandwidth);
                if (Host.Start() && HostData.Start())
                {
                    IsHost = true;
                    IsClient = false;
                }
                else
                {
                    FireError("Failed to start server at port " + port);
                }

                FireChange();
            }
            else
            {
                FireError("Invalid server parameters");
            }
        }

        public void ConfigureClient(string ip, string sPort)
        {
            int port;
            if (Int32.TryParse(sPort, out port))
            {
                Listener = new TelemetryClient();
                Listener.IP = ip;
                Listener.Port = port;
                if (Listener.Connect())
                {
                    IsHost = false;
                    IsClient = true;
                }
                else
                {
                    FireError("Failed to connect to " + ip + ":" + port);
                }

                FireChange();
            }
            else
            {
                FireError("Invalid server port");
            }
        }
    }
}
