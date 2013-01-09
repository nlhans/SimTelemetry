using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.Aggregates
{
    public class RecordedLap : IEntity, IEquatable<RecordedLap>
    {
        public int ID { get; private set; }
        public Track track { get; private set; }

        // etc.



        public bool Equals(RecordedLap other)
        {
            return other.ID == ID;
        }
    }
}