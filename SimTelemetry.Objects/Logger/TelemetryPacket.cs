namespace SimTelemetry.Objects
{
    public class TelemetryPacket
    {
        public ushort ID { get; set; }
        public ushort InstanceID { get; set; }
        public ushort Size { get; set; }
        public byte[] Data { get; set; }

        public TelemetryLogPacket Type { get; set; }

    }
}