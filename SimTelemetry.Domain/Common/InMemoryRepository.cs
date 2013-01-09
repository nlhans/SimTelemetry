using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        protected IList<T> data = new List<T>();

        public void Add(T entity)
        {
            if (!Contains(entity))
            {
                data.Add(entity);
            }
        }

        public void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(T entity)
        {
            return data.Any(x => x.Equals(entity));
        }

        public void Remove(T entity)
        {
            if (Contains(entity))
                data.Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return data;
        }
    }
}