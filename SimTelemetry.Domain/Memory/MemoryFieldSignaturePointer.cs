namespace SimTelemetry.Domain.Memory
{
    public class MemoryFieldSignaturePointer
    {
        public int Offset;
        // Reserved for future expansion.
        public MemoryFieldSignaturePointer(int offset)
        {
            Offset = offset;
        }
    }
}