namespace SimTelemetry.Domain.Common
{
    public interface IDataNode
    {
        T ReadAs<T>(string field);
    }
}
