using System.Collections.Generic;

namespace SimTelemetry.Domain.Common
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Clear();
        bool Contains(T entity);
        void Remove(T entity);
        IEnumerable<T> GetAll();
    }
}