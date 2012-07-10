using System;

namespace SimTelemetry.Objects.Peripherals
{
    public delegate void DevicePacketEvent(DevicePacket packet, object sender);
    public class DevicePacket
    {
        public int ID { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }
    }

    public struct DashboardPacket
    {
        public char Sync1;
        public char Sync2;
        public UInt16 length;
        public byte id;
        public byte crc;

    }

    public enum DashboardPackages
    {
        PACK_GAMEDATA_LS = 0,
        PACK_GAMEDATA_HS,
        PACK_PREFS_RACE,
        PACK_PREFS_DISPLAY,
        PACK_PREFS_SHIFTBAR,
        PACK_CARDATA,
        PACK_POTSINFO,
        PACK_PREFS_ENGINEREVS,
        PACK_PREFS_CRUISECONTROL
    }
}