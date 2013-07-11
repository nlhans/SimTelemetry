using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class LazyInMemoryRepository<TType, TId> : InMemoryRepository<TType, TId> where TType : class, IEntity<TId>
    {
        protected Lazy<IList<TId>> IdList;
        protected ILazyRepositoryDataSource<TType, TId> DataSource;

        public LazyInMemoryRepository(ILazyRepositoryDataSource<TType, TId> source)
        {
            DataSource = source;
            IdList = new Lazy<IList<TId>>(DataSource.GetIds);
        }

        public virtual bool Contains(TId id)
        {
            return IdList.Value.Contains(id);
        }

        public override bool Contains(TType entity)
        {
            return Contains(entity.ID) && base.Contains(entity);
        }

        public virtual TType GetById(TId id)
        {
            lock (data)
            {
                if (data.Any(x => x.ID.Equals(id)))
                {
                    return data.Where(x => x.ID.Equals(id)).FirstOrDefault();
                }
                else
                {
                    if (Contains(id) == false)
                        return null;

                    var obj = DataSource.Get(id);
                    if (obj == null || obj.ID == null || !obj.ID.Equals(id))
                    {
                        return obj;
                    }
                    else
                    {
                        Add(obj);
                        return obj;
                    }
                }
            }
        }

        public IEnumerable<TId> GetIds()
        {
            return IdList.Value.ToList();
        }

        public override IEnumerable<TType> GetAll()
        {
            return GetIds().Select(GetById);
        }

        public override bool Add(TType entity)
        {
            if (!Contains(entity.ID))
            {
                if (DataSource.Add(entity))
                {
                    IdList.Value.Add(entity.ID);
                }
                else
                {
                    return false;
                }
            }
            base.Add(entity);
            return true;
        }

        public override bool Store(TType entity)
        {
            if (!Contains(entity.ID))
                return false;
            if (!DataSource.Store(entity))
                return false;
            base.Store(entity);
            return true;
        }
        public override bool Remove(TType entity)
        {
            if (DataSource.Remove(entity.ID))
            {
                base.Remove(entity);
                IdList.Value.Remove(entity.ID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Clear()
        {
            if (DataSource.Clear())
            {
                base.Clear();
                IdList = new Lazy<IList<TId>>(DataSource.GetIds);                
            }
        }

#if DEBUG
        public int GetDataCount()
        {
            return data.Count;
        }
        public int GetIdCount()
        {
            return IdList.Value.Count;
        }
#endif
        public int GetCount()
        {
            return IdList.Value.Count;
        }
    }
}