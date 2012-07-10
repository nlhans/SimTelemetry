namespace SimTelemetry.Objects.Peripherals
{
    public interface IDevice
    {
        event DevicePacketEvent RX;
        string Name { get; set; }
        void TX(DevicePacket packet);
    }
}