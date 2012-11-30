using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SimTelemetry.Data.Logger;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net
{
    public class TelemetryServerData
    {
        public int Bandwidth { get; set; }

        private TelemetryServer _mServer;
        private Thread _mData;

        public TelemetryServerData(TelemetryServer mServer, int bandwidth)
        {
            _mServer = mServer;
            Bandwidth = bandwidth;
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

            List<TelemetryLoggerSubscribedInstance> data_instances =
                new List<TelemetryLoggerSubscribedInstance>();
            List<TelemetryLoggerSubscribedInstance> driver_instances =
                new List<TelemetryLoggerSubscribedInstance>();
            List<string> Events = new List<string>();

            while (_mServer.Running)
            {
                if (!Telemetry.m.Active_Sim)
                    Packet_WaitingForSimulator();
                else
                    Packet_WaitingForSession();

                if (Telemetry.m.Active_Sim && !Telemetry.m.Active_Session)
                    NoSession = true;

                if (Telemetry.m.Active_Sim && Telemetry.m.Active_Session)
                {
                    while (Telemetry.m.Active_Session && _mServer.Running)
                    {
                        ResetOnChange = false;
                        if (NoSession)
                        {
                            data_instances.Clear();
                            data_instances.Add(new TelemetryLoggerSubscribedInstance("Session", typeof(ISession),
                                                                                     Telemetry.m.Sim.Session));
                            data_instances[0].ID = 1;

                            data_instances.Add(new TelemetryLoggerSubscribedInstance("Player", typeof(IDriverPlayer),
                                                                                     Telemetry.m.Sim.Player));
                            data_instances[1].ID = 2;

                            foreach (TelemetryServerClient tsc in _mServer.ClientList)
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
                                                                                           typeof(IDriverGeneral),
                                                                                           drv));
                                driver_instances[0].ID = 3;
                            }

                            LastCars = Telemetry.m.Sim.Session.Cars;

                            foreach (TelemetryServerClient tsc in _mServer.ClientList)
                                tsc.Configured = false;

                            ResetOnChange = true;
                        }

                        foreach(TelemetryServerClient tsc in _mServer.ClientList.Where(x => !x.Configured))
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
                            tsc.Configured = true;
                        }

                        Packet_WaitingForSession();

                        // TODO: Add Simulator packets
                        // Session data
                        data_instances[0].RunFrequency = Bandwidth;
                        data_instances[1].RunFrequency = Bandwidth;

                        data_instances[0].ResetOnchange = ResetOnChange;
                        data_instances[1].ResetOnchange = ResetOnChange;

                        byte[] Data_Session = CreatePacket(data_instances[0].Dump(Events), (short)NetworkTypes.SESSION);
                        byte[] Data_Player = CreatePacket(data_instances[1].Dump(Events), (short)NetworkTypes.PLAYER);


                        _mServer.PushGameData(Data_Session);
                        _mServer.PushGameData(Data_Player);
                        _mServer.FlushGameData();

                        short i = 0;
                        foreach (TelemetryLoggerSubscribedInstance drv in driver_instances)
                        {
                            drv.RunFrequency = Bandwidth;
                            drv.ResetOnchange = ResetOnChange;
                            byte[] Data_Driver = CreatePacket(drv.Dump(Events),
                                                              (short)(((short)NetworkTypes.DRIVER)));
                            _mServer.PushGameData(Data_Driver);
                            Console.WriteLine("Pushing " + Data_Player.Length + " bytes of player data");
                            i++;
                        }

                        _mServer.FlushGameData();

                        Thread.Sleep(1000 / Bandwidth);
                    }
                }
                Thread.Sleep(1000 / Bandwidth);
            }

        }

        private void Packet_WaitingForSession()
        {
            byte[] SimDescriptor;
            List<byte> bf = new List<byte>();
            bf.Add((byte)NetworkAppState.WAITING_SESSION);
            bf.AddRange(ASCIIEncoding.ASCII.GetBytes(Telemetry.m.Sim.Name + "," + Telemetry.m.Sim.ProcessName));

            SimDescriptor = CreatePacket(bf.ToArray(), (short)NetworkTypes.SIMULATOR);

            _mServer.PushGameData(SimDescriptor);
            _mServer.FlushGameData();
        }

        private void Packet_WaitingForSimulator()
        {
            List<byte> bf = new List<byte>();
            bf.Add((byte)NetworkAppState.WAITING_SIM);

            foreach (ISimulator sim in Telemetry.m.Sims.Sims)
                bf.AddRange(ASCIIEncoding.ASCII.GetBytes(sim.Name + "," + sim.ProcessName + "\r\n"));

            byte[] Sims = CreatePacket(bf.ToArray(), (short)NetworkTypes.SIMULATOR);
            _mServer.PushGameData(Sims);
            _mServer.FlushGameData();
        }

        public bool Start()
        {
            if (_mData == null)
            {
                _mData = new Thread(DataCreator);
                _mData.IsBackground = true;
                _mData.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            if (_mData!= null)
            {
                _mData.Abort();
                _mData = null;
            }
        }
    }
}