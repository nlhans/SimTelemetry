using System;
using SimTelemetry.Core.Common;

namespace SimTelemetry.Core.Aggregates
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