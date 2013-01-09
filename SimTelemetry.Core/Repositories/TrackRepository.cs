using System.Linq;
using SimTelemetry.Core.Aggregates;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.Repositories
{
    public class TrackRepository : InMemoryRepository<Track>, ITrackRepository
    {
        public Track GetById(int id)
        {
            return data.Where(x => x.ID == id).FirstOrDefault();
        }

        public Track GetByName(string name)
        {
            return data.Where(x => x.Name == name).FirstOrDefault();
        }

        public Track GetByFile(string file)
        {
            return data.Where(x => x.File == file).FirstOrDefault();
        }
    }
}
