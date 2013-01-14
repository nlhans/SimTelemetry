using System.Collections.Generic;
using System.IO;
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

        public Car GetByFile(string file)
        {
            file = file.ToLower();

            // Search for an ID that's identical >filename<)
            var id = GetIds().Where(x => Path.GetFileName(x) == file);
            return GetById(id.FirstOrDefault());
        }

        public Car GetByName(string name)
        {
            return GetIds().Select(GetById).Where(x => x.Name == name).FirstOrDefault();
        }

        public IEnumerable<Car> GetByClass(string cls)
        {
            return GetIds().Select(GetById).Where(x => x.BelongsTo(cls));
        }

        public IEnumerable<Car> GetByClasses(IEnumerable<string> cls)
        {
            return GetIds().Select(GetById).Where(x => x.BelongsTo(cls));
        }
    }

}
