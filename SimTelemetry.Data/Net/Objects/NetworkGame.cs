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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using SimTelemetry.Data;
using SimTelemetry.Data.Net;
using SimTelemetry.Data.Net.Objects;
using SimTelemetry.Objects;
using SimTelemetry.Objects.Game;
using SimTelemetry.Objects.Garage;
using SimTelemetry.Objects.HyperType;
using SimTelemetry.Objects.Utilities;

namespace SimTelemetry.Game.Network
{
    [Export(typeof(ISimulator))]
    public class NetworkGame : ISimulator
    {
        public bool Attached { get; private set; }
        public bool UseMemoryReader { get { return false; } }

        public ISetup Setup
        {
            get { return null; }
        }

        public ITelemetry Host { get; set; }
        public void Initialize()
        {
            SimulatorModules _Modules = new SimulatorModules();
            _Modules.Time_Available = false;             // The plug-in knows the session time.
            _Modules.Track_Coordinates = true;
            _Modules.Track_MapFile = false;
            _Modules.Times_LapsBasic = false;
            _Modules.Times_LastSectors = true;
            _Modules.Times_History_LapTimes = false;
            _Modules.Times_History_SectorTimes = false;
            _Modules.Engine_Power = false;
            _Modules.Engine_PowerCurve = false;
            _Modules.Aero_Drag = false;
            Modules = _Modules;

            Drivers = new NetworkDrivers();
            Player = new NetworkDriverPlayer();
            Session = new NetworkSession();
            Garage = new NetworkGarage();
            Memory = null;

            // For speed:

            HyperTypeDescriptionProvider.Add(typeof(NetworkDrivers));
            HyperTypeDescriptionProvider.Add(typeof(NetworkDriverPlayer));
            HyperTypeDescriptionProvider.Add(typeof(NetworkDriverGeneral));
            HyperTypeDescriptionProvider.Add(typeof(NetworkSession));

            Types = new Dictionary<string, Dictionary<int, int>>();
            Properties = new Dictionary<string, Dictionary<int, string>>();
            Instances = new Dictionary<int, string>();
            Descriptor = new Dictionary<string, Dictionary<string, PropertyDescriptor>>();

            Telemetry.m.Net.Change += Net_Change;
        }

        public void Deinitialize()
        {

        }

        /// <summary>
        /// Something changed in the network configuration. For us it only matters when it is going to act as a client.
        /// </summary>
        private void Net_Change()
        {
            if (Telemetry.m.Net.IsClient && !Attached)
            {
                Connected();
            }
            else if (!Telemetry.m.Net.IsClient && Attached)
            {
                Disconnected();
            }
        }

        private void Connected()
        {

            Telemetry.m.Net.Listener.Packet += Listener_Packet;
            // Do what must be done to catch events.

        }

        public Dictionary<int, string> Instances { get; set; }
        public Dictionary<string, Dictionary<int, string>> Properties { get; set; }
        public Dictionary<string, Dictionary<int, int>> Types { get; set; }
        public Dictionary<string, Dictionary<string, PropertyDescriptor>> Descriptor { get; set; }

        private void ParseDescriptors(string sType)
        {
            Type type = typeof (int);
            switch(sType.ToLower())
            {
                case "session":
                    type = typeof (NetworkSession);
                    break;

                case "driver":
                    type = typeof (NetworkDriverGeneral);
                    break;

                case "player":
                    type = typeof (NetworkDriverPlayer);
                    break;
            }

            Descriptor.Add(sType, new Dictionary<string, PropertyDescriptor>());
            PropertyDescriptorCollection PropertyDescriptors;

            PropertyDescriptors = TypeDescriptor.GetProperties(type);
            PropertyInfo[] pic = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = PropertyDescriptors[pi.Name];
                if (fi == null || fi.Attributes.Contains(new Unloggable()))
                    continue;

                Descriptor[sType].Add(pi.Name, fi);
            }
        }


        private void Listener_Packet(object sender)
        {
            NetworkPacket packet = (NetworkPacket) sender;

            // TODO: Make two fields instead.
            int lsb = ((ushort) packet.Type) & 0xFF;
            NetworkTypes type = (NetworkTypes) ((ushort) packet.Type & 0xFF00);

            switch (type)
            {
                case NetworkTypes.SIMULATOR:
                    NetworkStateReport report = (NetworkStateReport)ByteMethods.DeserializeFromBytes(packet.Data);

                    switch(report.State)
                    {
                        case NetworkAppState.RUNNING:
                            Attached = true;
                            _name = report.SimRunning;
                            _processname = report.ProcessRunning;
                            break;

                            case NetworkAppState.WAITING_SIM:
                            Attached = false;
                            break;

                        case NetworkAppState.WAITING_SESSION:
                            Attached = true;
                            _name = report.SimRunning;
                            _processname = report.ProcessRunning;
                            break;
                    }

                    break;

                case NetworkTypes.DRIVER:
                    Drivers = (IDriverCollection) ByteMethods.DeserializeFromBytes(packet.Data);
                    break;

                case NetworkTypes.PLAYER:
                    Player = (IDriverPlayer) ByteMethods.DeserializeFromBytes(packet.Data);
                    break;

                case NetworkTypes.SESSION:
                    Session = (ISession) ByteMethods.DeserializeFromBytes(packet.Data);
                    break;

                case NetworkTypes.HEADER:
                    break;

                    // Others.. do later
            }
        }

        private void Disconnected()
        {
            Attached = false;

        }

        private string _name = "Network";
        private string _processname = "Network";
        public string Name { get { return _name; } }
        public string ProcessName { get { return _processname; } }
        public SimulatorModules Modules { get; private set; }
        public IDriverCollection Drivers { get; private set; }
        public IDriverPlayer Player { get; private set; }
        public ISession Session { get; private set; }
        public IGarage Garage { get; private set; }
        private MemoryPolledReader Memory { get; set; }
    }
}