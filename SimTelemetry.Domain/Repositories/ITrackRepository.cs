using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Repositories
{
    public interface ITrackRepository
    {
        Track GetByName(string name);
        Track GetByFile(string file);
    }
}