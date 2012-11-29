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
            Host.Stop();
            Host = null;

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

        public void ConfigureServer(string sPort, string sClients)
        {
            int port, clients;
            if (Int32.TryParse(sPort, out port) && Int32.TryParse(sClients, out clients))
            {
                Host = new TelemetryServer();
                if (Host.Start())
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
