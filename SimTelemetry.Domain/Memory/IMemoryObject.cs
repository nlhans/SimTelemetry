namespace SimTelemetry.Domain.Memory
{
    public interface IMemoryObject
    {
        string Name { get; }
        MemoryProvider Memory { get; }
        MemoryRefreshLevel Level { get; }
        MemoryAddressType AddressType { get; }

        bool IsDynamic { get; }
        bool IsStatic { get; }

        MemoryPool Pool { get; }
        int Offset { get; }
        int Address { get; }
        int Size { get; }

        TOut ReadAs<TOut>();

        void Refresh(MemoryRefreshLevel level);
        void SetProvider(MemoryProvider provider);
        void SetPool(MemoryPool pool);
    }
}