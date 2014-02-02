namespace SimTelemetry.Domain.Memory
{
    public interface IMemoryPointer
    {
        int Offset { get; }
        bool Additive { get; }
        bool IsDirty { get; }

        void MarkDirty();
        void Refresh(MemoryProvider master);

    }
}