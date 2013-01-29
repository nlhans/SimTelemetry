namespace SimTelemetry.Domain.Memory
{
    public enum MemoryRefreshLevel
    {
        Always = 0xFF,
        OneTime = 0x7F,
        SlowParameter = 0x1,
        Parameter = 0x0,
    }
}