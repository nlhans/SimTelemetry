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
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net
{
    public class NetworkPacket
    {
        public NetworkTypes Type { get; set; }
        public int Length { get { return Data.Length; } }
        public byte[] Data { get; set; }

        public NetworkPacket(NetworkTypes type, byte[] data)
        {
            this.Data = data;
            this.Type = type;
        }

        public NetworkPacket(short type, byte[] data)
        {
            this.Data = data;
            this.Type = (NetworkTypes)type;
        }

        public byte[] ToBytes()
        {
            byte[] packet = new byte[Data.Length + 8];
            packet[0] = (byte)'^';
            packet[1] = (byte)'%';
            ByteMethods.memcpy(packet, BitConverter.GetBytes((short)Type), 2, 2, 0);
            ByteMethods.memcpy(packet, BitConverter.GetBytes(Data.Length), 4, 4, 0);
            ByteMethods.memcpy(packet, Data, Data.Length, 8, 0);

            return packet;
        }
    }
}