using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Repositories
{
    public interface ITrackRepository
    {
        Track GetById(int id);
        Track GetByName(string name);
        Track GetByFile(string file);
    }
}