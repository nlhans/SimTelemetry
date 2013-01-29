using System.Collections.Generic;
using Triton.Memory;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryProvider
    {
        public MemoryProvider(MemoryReader reader)
        {
            BaseAddress = reader.ReadProcess.MainModule.BaseAddress.ToInt32();
            Reader = reader;
        }

        public int BaseAddress { get; protected set; }
        public MemoryReader Reader { get; protected set; }

        public IEnumerable<MemoryPool> Pools { get { return _pools; } }
        private readonly IList<MemoryPool> _pools = new List<MemoryPool>();

        public void Add(MemoryPool pool)
        {
            _pools.Add(pool);
            pool.SetProvider(this);
        }
        public void Remove(MemoryPool pool)
        {
            _pools.Remove(pool);
        }

        public void Refresh(MemoryRefreshLevel level)
        {
            foreach (var pool in _pools)
                //    .Where(x => x.Level <= level)) 
                if (pool.Level <= level)
                    pool.Refresh(level);

        }
        public void Refresh()
        {
            foreach (var pool in _pools) pool.Refresh(MemoryRefreshLevel.Always);
        }
    }
}
