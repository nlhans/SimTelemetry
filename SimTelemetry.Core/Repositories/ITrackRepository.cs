using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Repositories
{
    public interface ITrackRepository
    {
        Track GetById(int id);
        Track GetByName(string name);
        Track GetByFile(string file);
    }
}