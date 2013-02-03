using System;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Domain.Telemetry
{
    public class TelemetryDriver : ITelemetryObject
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }
        public int BaseAddress { get { return Pool.Address; }}

        internal MemoryPool Pool { get; set; }

        public float RPM { get; private set; }

        public void Update()
        {
            RPM = Pool.ReadAs<float>("RPM");

        }

        public TelemetryDriver(Aggregates.Telemetry telemetry, MemoryPool pool)
        {
            Pool = pool;
            Telemetry = telemetry;
        }
    }
}