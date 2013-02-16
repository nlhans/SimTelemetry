namespace SimTelemetry.Tests.Events
{
    public class MemoryReadAction
    {
        public int Address;
        public int Size;

        public MemoryReadAction(int address, int size)
        {
            Address = address;
            Size = size;
        }
    }
}