using System;
using SimTelemetry.Domain.Common;

namespace SimTelemetry.Domain.ValueObjects
{
    public class Time : IValueObject<Time>
    {
        public float Timestamp { get; private set; }
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }
        public int Millisecond { get; private set; }

        public Time(int hour, int minute, int second, int millisecond)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
            Millisecond = millisecond;

            Timestamp = hour*3600 + minute*60 + second + millisecond*1.0f;
        }

        public bool Equals(Time other)
        {
            return Math.Abs(other.Timestamp - Timestamp) < 0.001; // difference less than 1ms
        }

        public Time(float time)
        {
            Timestamp = time;
            ParseTimestamp();

        }

        public TimeSpan Subtract(Time t)
        {
            var diff = Timestamp - t.Timestamp;
            return new TimeSpan(0, 0, 0, 0, Convert.ToInt32(diff*1000.0f));
        }

        public void Subtract(float time)
        {
            Timestamp -= time;
            ParseTimestamp();
        }

        public void Add(float time)
        {
            Timestamp += time;
            ParseTimestamp();
        }

        private void ParseTimestamp()
        {
            var time = Timestamp;

            Hour = ((int)time) / 3600;
            time -= Hour * 3600.0f;
            Minute = ((int)time) / 60;
            time -= Minute * 60.0f;
            Second = ((int)time);
            time -= Second * 1.0f;
            Millisecond = Convert.ToInt32(time * 1000.0f);
        }
    }
}