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

        private void ParseGameData(NetworkPacket packet)
        {
            try
            {
                int instance_id = BitConverter.ToInt16(packet.Data, 0);
                int last_typeid = 0;
                for (int i = 2; i < packet.Data.Length - 4;)
                {

                    int id_type = BitConverter.ToInt32(packet.Data, i);
                    object v = 0;
                    i += 4;
                    if (Instances.ContainsKey(instance_id) && Types.ContainsKey(Instances[instance_id]))
                    {
                        Dictionary<int, int> TypeArray = Types[Instances[instance_id]];
                        if (TypeArray.ContainsKey(id_type))
                        {
                            switch (TypeArray[id_type])
                            {
                                case 0: //double
                                    v = BitConverter.ToDouble(packet.Data, i);
                                    i += 8;
                                    break;

                                case 1: //float
                                    v = BitConverter.ToSingle(packet.Data, i);
                                    i += 4;
                                    break;

                                case 2: // int32
                                    v = BitConverter.ToInt32(packet.Data, i);
                                    i += 4;
                                    break;

                                case 3: // int16
                                    v = BitConverter.ToInt16(packet.Data, i);
                                    i += 2;
                                    break;

                                case 4: // int8
                                    v = packet.Data[i];
                                    i += 1;
                                    break;

                                case 5: // uint32
                                    v = BitConverter.ToUInt32(packet.Data, i);
                                    i += 4;
                                    break;

                                case 6: // uint16
                                    v = BitConverter.ToUInt16(packet.Data, i);
                                    i += 2;
                                    break;

                                case 7: // uint8
                                    v = packet.Data[i];
                                    i += 1;
                                    break;

                                case 8: // string
                                    int k = i;
                                    for (; k < packet.Data.Length; k++)
                                    {
                                        if (packet.Data[k] == 0) break;
                                    }
                                    v = ASCIIEncoding.ASCII.GetString(packet.Data, i, k - i);
                                    i = k + 1;
                                    break;

                                case 9: // bool
                                    v = ((packet.Data[i] == 1) ? true : false);
                                    i += 1;
                                    break;
                                default:
                                    v = 0;
                                    break;
                            }

                            string instance_name = Instances[instance_id];
                            string property_key = Properties[instance_name][id_type];

                            if (Descriptor.ContainsKey(instance_name) &&
                                Descriptor[instance_name].ContainsKey(property_key))
                            {

                                PropertyDescriptor pd = Descriptor[instance_name][property_key];

                                switch (instance_name)
                                {
                                    case "Session":
                                        pd.SetValue(Session, v);
                                        break;


                                    case "Player":
                                        pd.SetValue(Player, v);
                                        break;

                                    case "Driver":
                                        // Temporary:
                                        pd.SetValue(Drivers.Player, v);
                                        break;

                                }
                            }

                            //Console.WriteLine( + "=" + v.ToString());
                            //Sample.Data[instance_id][id_type] = (object)v;
                            last_typeid = id_type;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error parsing network game data");
            }

        }

        private void Listener_Packet(object sender)
        {
            NetworkPacket packet = (NetworkPacket) sender;

            // TODO: Make two fields instead.
            int lsb = ((ushort) packet.Type) & 0xFF;
            NetworkTypes type = (NetworkTypes) ((ushort) packet.Type & 0xFF00);

            switch(type)
            {
                    case NetworkTypes.SIMULATOR:
                    NetworkAppState state = (NetworkAppState) packet.Data[0];
                    if (state == NetworkAppState.WAITING_SIM)
                        Attached = false;
                    if (state == NetworkAppState.WAITING_SESSION)
                    {
                        Attached = true;
                        string SimName = ASCIIEncoding.ASCII.GetString(packet.Data, 1, packet.Data.Length - 1);
                        string[] SimData =SimName.Split(",".ToCharArray());

                        _name = SimData[0];
                        _processname = SimData[1];

                    }
                    break;

                case NetworkTypes.DRIVER:
                    ParseGameData(packet);
                    break;

                case NetworkTypes.PLAYER:
                    ParseGameData(packet);
                    break;

                case NetworkTypes.SESSION:
                    ParseGameData(packet);
                    break;

                case NetworkTypes.HEADER:
                    ParseGameHeader(packet);
                    break;

                    // Others.. do later
            }
        }

        private void ParseGameHeader(NetworkPacket packet)
        {
            //
            byte[] data = packet.Data;
            string last_type = "";
            for (int i = 0; i < data.Length-8; i++)
            {
                if(data[i] == '$' && data[i+1] == '#')
                {
                    ushort packet_type = BitConverter.ToUInt16(data, i + 4);// name or member?
                    ushort instance = BitConverter.ToUInt16(data, i + 6);// name or member?
                    ushort length = BitConverter.ToUInt16(data, i + 8);// name or member?
                    if (length > 0)
                    {

                        byte[] tmp_data = new byte[length];
                        Array.Copy(data, i + 10, tmp_data, 0, length);


                        if ((TelemetryLogPacket) packet_type == TelemetryLogPacket.InstanceName)
                        {
                            last_type = ASCIIEncoding.ASCII.GetString(tmp_data).Replace('\0', ' ').Trim();
                            int last_type_id = instance;
                            Instances.Add(last_type_id, last_type);
                            Types.Add(last_type, new Dictionary<int, int>());
                            Properties.Add(last_type, new Dictionary<int, string>());
                            ParseDescriptors(last_type);
                        }
                        else
                        {
                            int id = BitConverter.ToInt16(tmp_data, 2);
                            int type_id = BitConverter.ToInt16(tmp_data, 0);

                            if (Types[last_type].ContainsKey(id) == false)
                            {
                                Types[last_type].Add(id, type_id);

                                string key = ASCIIEncoding.ASCII.GetString(tmp_data, 4, tmp_data.Length - 4).Replace('\0', ' ').Trim();
                                Properties[last_type].Add(id, key);
                            }
                        }
                    }
                }

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