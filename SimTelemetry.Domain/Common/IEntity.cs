namespace SimTelemetry.Domain.Common
{
    public interface IEntity<T>
    {
        T ID { get; }
    }
}