using System;
using SimTelemetry.Core.Common;
using SimTelemetry.Core.Enumerations;

namespace SimTelemetry.Core.ValueObjects
{
    public class TrackPoint : IValueObject<TrackPoint>
    {
        public float Meter { get; private set; }
        public TrackPointType Type { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public double[] BoundsL { get; private set; }
        public double[] BoundsR { get; private set; }

        public bool Equals(TrackPoint other)
        {
            return other.Meter == Meter && other.X == X && other.Y == Y && other.Z == Z;
        }
    }
}