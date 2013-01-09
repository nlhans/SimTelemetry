using System.Collections.Generic;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Repositories
{
    public interface ICarRepository : IRepository<Car>
    {
        Car GetByName(string name);
        Car GetByFile(string file);
        IEnumerable<Car> GetByClass(string cls);
        IEnumerable<Car> GetByClasses(IEnumerable<string> cls);
    }
}