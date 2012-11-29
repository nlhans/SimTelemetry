using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net
{
    public class TelemetryServer
    {
        private Thread _mServer;
        private Thread _mData;
        private Thread _mKeeper;
        private bool _mServerRunning;
        private TcpListener _mTcpServer;
        private List<TelemetryServerClient> _mClients;
        private ManualResetEvent _mTcpServerClientAccepted;
        public int Clients { get { return _mClients.Count; } }
        public int Traffic { get; set; }

        public int Port { get; set; }
        public int Bandwidth { get; set; }

        public TelemetryServer()
        {
            // Temporary:
            Port = 12345;
            Bandwidth = 1;

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

                    _mData = new Thread(DataCreator);
                    _mData.IsBackground = true;
                    _mData.Start();

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
                _mData = null;
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

        private byte[] CreatePacket(byte[] data, short instance)
        {
            NetworkPacket np = new NetworkPacket(instance, data);
            return np.ToBytes();
        }

        private void DataCreator()
        {
            bool ResetOnChange = true;
            bool NoSession = true;
            int LastCars = -1;

            byte[] SimDescriptor = new byte[0];
            List<TelemetryLoggerSubscribedInstance> data_instances =
                new List<TelemetryLoggerSubscribedInstance>();
            List<TelemetryLoggerSubscribedInstance> driver_instances =
                new List<TelemetryLoggerSubscribedInstance>();
            List<string> Events = new List<string>();

            while (_mServerRunning)
            {
                if (!Telemetry.m.Active_Sim)
                {
                    List<byte> bf = new List<byte>();
                    bf.Add((byte) NetworkAppState.WAITING_SIM);

                    foreach (ISimulator sim in Telemetry.m.Sims.Sims)
                        bf.AddRange(ASCIIEncoding.ASCII.GetBytes(sim.Name + "," + sim.ProcessName + "\r\n"));

                    byte[] Sims = CreatePacket(bf.ToArray(), (short) NetworkTypes.SIMULATOR);
                    PushGameData(Sims);
                    FlushGameData();

                }
                else if (Telemetry.m.Active_Sim)
                {

                    List<byte> bf = new List<byte>();
                    bf.Add((byte) NetworkAppState.WAITING_SESSION);
                    bf.AddRange(ASCIIEncoding.ASCII.GetBytes(Telemetry.m.Sim.Name + "," + Telemetry.m.Sim.ProcessName));

                    SimDescriptor = CreatePacket(bf.ToArray(), (short) NetworkTypes.SIMULATOR);

                }
                if (Telemetry.m.Active_Sim && !Telemetry.m.Active_Session)
                    NoSession = true;

                if (Telemetry.m.Active_Sim && Telemetry.m.Active_Session)
                {
                    while (Telemetry.m.Active_Session && _mServerRunning)
                    {
                        ResetOnChange = false;
                        if (NoSession)
                        {
                            data_instances.Clear();
                            data_instances.Add(new TelemetryLoggerSubscribedInstance("Session", typeof (ISession),
                                                                                     Telemetry.m.Sim.Session));
                            data_instances[0].ID = 1;
                            data_instances.Add(new TelemetryLoggerSubscribedInstance("Player", typeof (IDriverPlayer),
                                                                                     Telemetry.m.Sim.Player));
                            data_instances[1].ID = 2;

                            foreach (TelemetryServerClient tsc in _mClients)
                                tsc.Configured = false;

                            NoSession = false;
                            ResetOnChange = true;
                        }
                        if (Telemetry.m.Sim.Session.Cars != LastCars)
                        {
                            driver_instances.Clear();

                            foreach (IDriverGeneral drv in Telemetry.m.Sim.Drivers.AllDrivers)
                            {
                                driver_instances.Add(new TelemetryLoggerSubscribedInstance("Driver",
                                                                                           typeof (IDriverGeneral),
                                                                                           drv));
                                driver_instances[0].ID = 3;
                            }

                            LastCars = Telemetry.m.Sim.Session.Cars;

                            foreach (TelemetryServerClient tsc in _mClients)
                                tsc.Configured = false;

                            ResetOnChange = true;
                        }

                        ResetOnChange = true;
                        _mClients.FindAll(delegate(TelemetryServerClient tsc) { return tsc.Configured == false; }).
                            ForEach(
                                delegate(TelemetryServerClient tsc)
                                    {

                                        foreach (TelemetryLoggerSubscribedInstance tlsi in data_instances)
                                        {
                                            byte[] packet = CreatePacket(tlsi.ExportHeader(),
                                                                         (short) NetworkTypes.HEADER);
                                            tsc.PushGameData(packet);
                                        }
                                        if (driver_instances.Count > 0)
                                        {
                                            byte[] packet = CreatePacket(driver_instances[0].ExportHeader(),
                                                                         (short) NetworkTypes.HEADER);
                                            tsc.PushGameData(packet);
                                        }

                                        tsc.FlushGameData();

                                        //tsc.Configured = true;
                                    });

                        // TODO: Add Simulator packets
                        // Session data
                        data_instances[0].RunFrequency = Bandwidth;
                        data_instances[1].RunFrequency = Bandwidth;

                        data_instances[0].ResetOnchange = ResetOnChange;
                        data_instances[1].ResetOnchange = ResetOnChange;

                        byte[] Data_Session = CreatePacket(data_instances[0].Dump(Events), (short) NetworkTypes.SESSION);
                        byte[] Data_Player = CreatePacket(data_instances[1].Dump(Events), (short)NetworkTypes.PLAYER);

                        PushGameData(SimDescriptor);
                        FlushGameData();

                        PushGameData(Data_Session);
                        PushGameData(Data_Player);
                        FlushGameData();

                        short i = 0;
                        foreach (TelemetryLoggerSubscribedInstance drv in driver_instances)
                        {
                            drv.RunFrequency = Bandwidth;
                            drv.ResetOnchange = ResetOnChange;
                            byte[] Data_Driver = CreatePacket(drv.Dump(Events),
                                                              (short) (((short) NetworkTypes.DRIVER) ));
                            PushGameData(Data_Driver);
                            Console.WriteLine("Pushing " + Data_Player.Length + " bytes of player data");
                            i++;
                        }

                        FlushGameData();

                        Thread.Sleep(1000/Bandwidth);
                    }
                }
                Thread.Sleep(1000/Bandwidth);
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

                _mData.Abort();
                _mServer.Abort();
                _mKeeper.Abort();
                _mData = null;
                _mServer = null;
                _mKeeper = null;
                _mClients = new List<TelemetryServerClient>();
            }
        }
    }
}