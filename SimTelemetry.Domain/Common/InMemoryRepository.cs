using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class InMemoryRepository<TType, TId> : IRepository<TType> where TType : IEntity<TId>
    {
        protected IList<TType> data = new List<TType>();

        public virtual bool Add(TType entity)
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

        public virtual void AddRange(IEnumerable<TType> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public virtual void Clear()
        {
            data.Clear();
        }

        public virtual bool Contains(TType entity)
        {
            return data.Any(x => x.Equals(entity));
        }

        public virtual bool Store(TType entity)
        {
            if (Contains(entity) == false)
                return false;
            else
            {
                var index = data.IndexOf(entity);
                data[index] = entity;

                return true;
            }
        }

        public virtual bool Remove(TType entity)
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

        public virtual IEnumerable<TType> GetAll()
        {
            return data;
        }
    }
}