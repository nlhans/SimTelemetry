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