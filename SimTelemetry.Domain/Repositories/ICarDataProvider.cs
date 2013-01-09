using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Repositories
{
    public interface ICarDataProvider : IDataProvider<Car>
    {
    }
}