using System.Collections.Generic;
using SimTelemetry.Core.Aggregates;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.Repositories
{
    public interface ICarRepository : IRepository<Car>
    {
        Car GetByName(string name);
        Car GetByFile(string file);
        IEnumerable<Car> GetByClass(string cls);
        IEnumerable<Car> GetByClasses(IEnumerable<string> cls);
    }
}