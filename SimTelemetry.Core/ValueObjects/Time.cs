namespace SimTelemetry.Core.ValueObjects
{
    public class Time
    {
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }
        public int Millisecond { get; private set; }
    }
}