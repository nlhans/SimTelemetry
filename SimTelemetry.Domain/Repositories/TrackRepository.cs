using System.IO;
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
            return GetIds().Select(GetById).Where(x => x.Name == name).FirstOrDefault();
        }

        public Track GetByFile(string file)
        {
            file = Path.GetFileName(file.ToLower());
            return GetById(GetIds().Where(x => Path.GetFileName(x) == file).FirstOrDefault());
            //return data.Where(x => x.ID == file).FirstOrDefault();
        }
    }
}
