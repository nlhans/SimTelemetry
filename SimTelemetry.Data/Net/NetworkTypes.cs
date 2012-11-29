namespace SimTelemetry.Data.Net
{
    public enum NetworkTypes
    {
        TIME = 0x0100,
        METOHDS = 0x0200,
        SIMULATOR = 0x0300,
        TRACK = 0x0400,
        TRACKMAP = 0x0500,
        SESSION = 0x0600,
        PLAYER =0x0700,

        ENGINECURVE = 0x0800,

        DRIVER = 0x0900,
        HEADER = 0x0F00
    }
}