using System.Collections.Generic;

namespace SimTelemetry.Domain.Common
{
    public interface IRepositoryDataProvider<T> where T : class
    {
        IEnumerable<T> SearchByName(string name);
        IEnumerable<T> SearchByFile(string file);
        IEnumerable<T> GetAll();
        void Clear();
    }
}