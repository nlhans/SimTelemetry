using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Common
{
    public class LazyInMemoryRepository<TType, TId> : InMemoryRepository<TType> where TType : IEntity<TId>
    {
        protected Lazy<IList<TId>> idList;
        protected ILazyRepositoryDataSource<TType, TId> dataSource;

        public LazyInMemoryRepository(ILazyRepositoryDataSource<TType, TId> source)
        {
            dataSource = source;
            idList = new Lazy<IList<TId>>(dataSource.GetIds);
        }

        public virtual bool Contains(TId id)
        {
            return idList.Value.Contains(id);
        }

        public virtual TType GetById(TId id)
        {
            if (data.Any(x => x.ID.Equals(id)))
                return data.Where(x => x.ID.Equals(id)).FirstOrDefault();
            else
            {
                var obj = dataSource.Get(id);
                if (obj == null || obj.ID == null || !obj.ID.Equals(id))
                    return obj;
                else
                {
                    Add(obj);
                    return obj;
                }
            }
        }

        public IEnumerable<TId> GetIds()
        {
            return idList.Value;
        }

        public override bool Contains(TType entity)
        {
            return Contains(entity.ID) && base.Contains(entity);
        }

        public override bool Add(TType entity)
        {

            // Check if our ID list contains the object.
            if (!Contains(entity.ID))
            {
                // Can our DataSource support Add operations?
                if (dataSource.Add(entity))
                {
                    idList.Value.Add(entity.ID);
                }
                else
                {
                    return false;
                }
            }
            base.Add(entity);

            return true;
        }

        public override bool Remove(TType entity)
        {
            if (dataSource.Remove(entity.ID))
            {
                base.Remove(entity);
                idList.Value.Remove(entity.ID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Clear()
        {
            if (dataSource.Clear())
            {
                base.Clear();
                idList = new Lazy<IList<TId>>(dataSource.GetIds);                
            }
        }
    }
}