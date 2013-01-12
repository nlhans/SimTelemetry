using System.Collections.Generic;

namespace SimTelemetry.Domain.Common
{
    public interface IRepository<T> 
    {
        bool Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Clear();
        bool Contains(T entity);
        bool Store(T entity);
        bool Remove(T entity);
        IEnumerable<T> GetAll();
    }
}