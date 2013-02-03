using System;

namespace SimTelemetry.Domain.Memory
{
    public interface IMemoryObject : ICloneable
    {
        string Name { get; }
        MemoryProvider Memory { get; }
        MemoryAddress AddressType { get; }

        bool IsDynamic { get; }
        bool IsStatic { get; }
        bool IsConstant { get; }

        MemoryPool Pool { get; }
        int Offset { get; }
        int Address { get; }
        int Size { get; }

        TOut ReadAs<TOut>();

        void Refresh();
        void SetProvider(MemoryProvider provider);
        void SetPool(MemoryPool pool);
    }
}