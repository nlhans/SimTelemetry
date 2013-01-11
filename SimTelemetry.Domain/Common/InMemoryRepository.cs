using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        protected IList<T> data = new List<T>();

        public virtual bool Add(T entity)
        {
            if (!this.Contains(entity))
            {
                data.Add(entity);
                return true;
            }else
            {
                return false;
            }
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public virtual void Clear()
        {
            data.Clear();
        }

        public virtual bool Contains(T entity)
        {
            return data.Any(x => x.Equals(entity));
        }

        public virtual bool Remove(T entity)
        {
            if (Contains(entity))
            {
                data.Remove(entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            return data;
        }
    }
}