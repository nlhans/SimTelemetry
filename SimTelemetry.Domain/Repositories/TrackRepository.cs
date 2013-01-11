using System.Linq;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Repositories
{
    public class TrackRepository : LazyInMemoryRepository<Track,string>, ITrackRepository
    {
        public TrackRepository(ILazyRepositoryDataSource<Track, string> source) : base(source)
        {
        }

        public Track GetByName(string name)
        {
            return data.Where(x => x.Name == name).FirstOrDefault();
        }

        public Track GetByFile(string file)
        {
            return data.Where(x => x.ID == file).FirstOrDefault();
        }
    }
}
