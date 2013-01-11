using System;
using System.Collections.Generic;
using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Repositories
{
    public class CarRepository : LazyInMemoryRepository<Car, string>, ICarRepository
    {
        public CarRepository(ILazyRepositoryDataSource<Car, string> source) : base(source)
        {

        }

        public Car GetByName(string name)
        {
            return data.Where(x => x.Name == name).FirstOrDefault();
        }

        public Car GetByFile(string file)
        {
            return data.Where(x => x.ID == file).FirstOrDefault();
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
