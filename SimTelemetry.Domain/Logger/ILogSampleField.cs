namespace SimTelemetry.Domain.Logger
{
    public interface ILogSampleField
    {
        void SetOffset(int offset);
        int Offset { get; set; }
    }
}