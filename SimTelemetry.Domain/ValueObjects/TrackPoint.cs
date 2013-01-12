using System;
using SimTelemetry.Domain.Common;
using SimTelemetry.Domain.Enumerations;

namespace SimTelemetry.Domain.ValueObjects
{
    public class TrackPoint : IValueObject<TrackPoint>
    {
        public float Meter { get; private set; }
        public TrackPointType Type { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public float[] BoundsL { get; private set; }
        public float[] BoundsR { get; private set; }

        public TrackPoint(float meter, TrackPointType type, float x, float y, float z, float[] boundsL, float[] boundsR)
        {
            Meter = meter;
            Type = type;
            X = x;
            Y = y;
            Z = z;
            BoundsL = boundsL;
            BoundsR = boundsR;
        }

        public bool Equals(TrackPoint other)
        {
            return other.Meter == Meter && other.X == X && other.Y == Y && other.Z == Z;
        }
    }
}