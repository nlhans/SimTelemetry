namespace SimTelemetry.Domain.Memory
{
    public class MemoryPointerSignature : MemoryPointer
    {
        public string Signature { get; private set; }

        public MemoryPointerSignature(string signature, bool additive)
            : base(0, additive)
        {
            Signature = signature;
            Additive = additive;
            MarkDirty();
        }

        public override void Refresh(MemoryProvider master)
        {
            if (IsDirty && master.Scanner.Enabled && Signature != string.Empty)
            {
                Offset = master.Scanner.Scan<int>(MemoryRegionType.EXECUTE, Signature);
                IsDirty = false;
            }
        }
    }
}