using System;

namespace SimTelemetry.Objects.Peripherals
{
    public struct DashboardPacket
    {
        public char Sync1;
        public char Sync2;
        public UInt16 length;
        public byte id;
        public byte crc;

    }
}