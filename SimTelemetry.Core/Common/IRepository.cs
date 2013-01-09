using System.Collections.Generic;

namespace SimTelemetry.Core.Common
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddOrUpdate(T entity);
        void AddOrUpdateRange(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
        void Clear();
        bool Contains(T entity);
        T GetById(long id);
        void Remove(T entity);
        IEnumerable<T> GetAll();
    }
}