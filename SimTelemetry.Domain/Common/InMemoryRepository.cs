using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class InMemoryRepository<TType, TId> : IRepository<TType> where TType : IEntity<TId>
    {
        protected ConcurrentDictionary<TId, TType> data = new ConcurrentDictionary<TId, TType>();

        public virtual bool Add(TType entity)
        {
            if (!this.Contains(entity))
            {
                return data.TryAdd(entity.ID, entity);
                //data.Add(entity);
                return true;
            }else
            {
                return false;
            }
        }

        public virtual void AddRange(IEnumerable<TType> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public virtual void Clear()
        {
            lock (data)
            {
                data.Clear();
            }

        }

        public virtual bool Contains(TType entity)
        {
            lock (data)
            {
                return data.Any(x => x.Key.Equals(entity.ID));
                //return data.Any(x => x.Equals(entity.ID));
            }

        }

        public virtual bool Store(TType entity)
        {
            if (Contains(entity) == false)
                return false;
            else
            {
                lock (data)
                {
                    data[entity.ID] = entity;
                    //var index = data.IndexOf(entity);
                    //data[index] = entity;
                }

                return true;
            }
        }

        public virtual bool Remove(TType entity)
        {
            TType tmp = default(TType);

            if (Contains(entity))
            {
                lock (data)
                {
                    data.TryRemove(entity.ID, out tmp);
                    //data.Remove(entity);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual IEnumerable<TType> GetAll()
        {
            return new List<TType>(data.Values);
        }
    }
}