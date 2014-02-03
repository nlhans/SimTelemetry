using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryPointer : IMemoryPointer
    {
        public int Offset { get; protected set; }
        public bool Additive { get; protected set; }
        public bool IsDirty { get; protected set; }

        public MemoryPointer(int offset, bool additive)
        {
            Offset = offset;
            Additive = additive;
            IsDirty = false;
        }

        public void MarkDirty()
        {
            IsDirty = true;
        }

        public virtual void Refresh(MemoryProvider master)
        {
            IsDirty = false;
        }

    }
}
