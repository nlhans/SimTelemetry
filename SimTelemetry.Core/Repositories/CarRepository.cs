using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Core.Aggregates;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.Repositories
{
    public class CarRepository : InMemoryRepository<Car>, ICarRepository
    {
        public Car GetById(long id)
        {
            return data.Where(x => x.ID == id).FirstOrDefault();
        }

        public Car GetByName(string name)
        {
            return data.Where(x => x.Name == name).FirstOrDefault();
        }

        public Car GetByFile(string file)
        {
            return data.Where(x => x.File == file).FirstOrDefault();
        }

        public IEnumerable<Car> GetByClass(string cls)
        {
            return data.Where(x => x.BelongsTo(cls));
        }

        public IEnumerable<Car> GetByClasses(IEnumerable<string> cls)
        {
            return data.Where(x => x.BelongsTo(cls));
        }
    }
}
