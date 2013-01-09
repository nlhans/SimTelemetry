namespace SimTelemetry.Core.ValueObjects
{
    public class EngineLifetime
    {
        public int Time { get; private set; }

        public Range EngineRpm { get; private set; }
        public Range OilTemperature { get; private set; }
        public Range WaterTemperature { get; private set; }

        public EngineLifetime(int time, Range engineRpm, Range oilTemperature, Range waterTemperature)
        {
            Time = time;
            EngineRpm = engineRpm;
            OilTemperature = oilTemperature;
            WaterTemperature = waterTemperature;
        }
    }
}