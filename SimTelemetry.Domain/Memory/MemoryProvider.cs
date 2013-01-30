using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryProvider
    {
        public int BaseAddress { get; protected set; }
        public MemoryReader Reader { get; protected set; }

        public IList<MemoryPool> Pools { get { return _pools; } }
        private readonly IList<MemoryPool> _pools = new List<MemoryPool>();

        public MemoryProvider(MemoryReader reader)
        {
            BaseAddress = reader.Process.MainModule.BaseAddress.ToInt32();
            Reader = reader;
        }

        public void Add(MemoryPool pool)
        {
            _pools.Add(pool);
            pool.SetProvider(this);
        }

        public void Remove(MemoryPool pool)
        {
            _pools.Remove(pool);
        }

        public MemoryPool Get(string name)
        {
            return _pools.Where(x => x.Name == name).FirstOrDefault();
        }

        public bool Contains(string name)
        {
            return _pools.Any(x => x.Name == name);
        }

        public void Refresh()
        {
            foreach (var pool in _pools)
            {
                pool.Refresh();
            }
        }
    }
}
