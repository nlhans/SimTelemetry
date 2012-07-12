namespace SimTelemetry.Objects.Peripherals
{
    public delegate void DevicePacketEvent(DevicePacket packet, object sender);
    public class DevicePacket
    {
        public int ID { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }
    }
}