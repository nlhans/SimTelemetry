using System.Collections.Generic;

namespace SimTelemetry.Domain.Common
{
    public interface ILazyRepositoryDataSource<TType, TId> where TType : IEntity<TId>
    {
        IList<TId> GetIds();
        bool Add(TType obj);
        bool Store(TType obj);
        TType Get(TId id);
        bool Remove(TId Id);
        bool Clear();
    }
}