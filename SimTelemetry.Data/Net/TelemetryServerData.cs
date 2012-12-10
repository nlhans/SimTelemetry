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
using System.Threading;
using SimTelemetry.Data.Net.Objects;
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
        private void DataCreator()
        {
            int SecTimer = 0;
            while (_mServer.Running)
            {
                SecTimer++;
                try
                {
                    NetworkPacket packet = new NetworkPacket();
                    packet.Type = NetworkTypes.SIMULATOR;
                    if(!Telemetry.m.Active_Sim)
                    {
                        List<string> Sims = new List<string>();
                        Telemetry.m.Sims.Sims.ForEach(x => Sims.Add(x.ProcessName));
                        NetworkStateReport report = new NetworkStateReport(NetworkAppState.WAITING_SIM, Sims);
                        packet.Data = ByteMethods.SerializeToBytes(report);
                        _mServer.Write(packet);
                    }
                    else if (!Telemetry.m.Active_Session)
                    {
                        NetworkStateReport report = new NetworkStateReport(NetworkAppState.WAITING_SESSION, Telemetry.m.Sim.Name, Telemetry.m.Sim.ProcessName);
                        packet.Data = ByteMethods.SerializeToBytes(report);
                        _mServer.Write(packet);
                    }

                    // TODO: Add packet for game.
                    if (Telemetry.m.Active_Sim && Telemetry.m.Active_Session)
                    {
                        NetworkStateReport report = new NetworkStateReport(NetworkAppState.RUNNING, Telemetry.m.Sim.Name, Telemetry.m.Sim.ProcessName);
                        packet.Data = ByteMethods.SerializeToBytes(report);
                        _mServer.Write(packet);

                        packet.Type = NetworkTypes.SESSION;
                        packet.Data = ByteMethods.SerializeToBytes(NetworkSession.Create(Telemetry.m.Sim.Session));
                        _mServer.Write(packet);

                        packet.Type = NetworkTypes.DRIVER;
                        packet.Data = ByteMethods.SerializeToBytes(NetworkDrivers.Create(Telemetry.m.Sim.Drivers));
                        _mServer.Write(packet);

                        packet.Type = NetworkTypes.PLAYER;
                        packet.Data = ByteMethods.SerializeToBytes(NetworkDriverPlayer.Create(Telemetry.m.Sim.Player));
                        _mServer.Write(packet);

                        if (SecTimer % (5*Bandwidth) == 0 && Telemetry.m.Track.Route != null) // every 5 seconds
                        {
                            // Send complete track route
                            packet.Type = NetworkTypes.TRACKMAP;
                            packet.Data = ByteMethods.SerializeToBytes(Telemetry.m.Track.Route);
                            _mServer.Write(packet);

                            packet.Type = NetworkTypes.TRACK;
                            NetworkTrackInformation trackInfo = new NetworkTrackInformation();
                            trackInfo.Type = Telemetry.m.Track.Type;
                            trackInfo.Location = Telemetry.m.Track.Location;
                            trackInfo.Name = Telemetry.m.Track.Name;
                            packet.Data = ByteMethods.SerializeToBytes(trackInfo);
                            _mServer.Write(packet);
                        }
                    }
                }
                catch(Exception ex )
                {
                    
                }
                Thread.Sleep(1000/Bandwidth);
            }

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