namespace SimTelemetry.Objects.Peripherals
{
    public interface IDevices
    {
        event DevicePacketEvent RX;
        void TX(DevicePacket packet, string destination);
    }

}
