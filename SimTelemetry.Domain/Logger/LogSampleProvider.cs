using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Logger
{
    public class LogSampleProvider
    {
        protected LogFileReader Reader { get; set; }

        public IEnumerable<string> Groups { get { return _groups; } }
        private List<string> _groups = new List<string>();
        public int StartTime { get; protected set; }
        public int EndTime { get; protected set; }

        private List<int> TimeLine = new List<int>();

        private LogSample Sample;

        public LogSampleProvider(LogFileReader reader, IEnumerable<string> groups, int startTime, int endTime)
        {
            Reader = reader;
            _groups = groups.ToList();
            StartTime = startTime;
            EndTime = endTime;

            // Make base sample.
            Sample = new LogSample(this, startTime, groups.Select(reader.GetGroup));


            TimeLine = reader.Groups.SelectMany(x => x.Timeline.Keys).Distinct().ToList();

            InitializeSample(Sample, startTime);

        }

        protected void UpdateSampleValues(LogSample sample, int timestamp)
        {
            Sample.UpdateTime(timestamp);
            foreach (var group in sample.Groups.Values)
                ((LogSampleGroup)group).Update(timestamp, true);
        }

        protected void InitializeSample(LogSample sample, int timestamp)
        {
            Sample.UpdateTime(timestamp);
            foreach (var group in sample.Groups.Values)
                ((LogSampleGroup)group).Update(timestamp, false);
        }

        public IEnumerable<LogSample> GetSamples()
        {
            foreach(var time in TimeLine.Where(x => x >= StartTime && x <= EndTime))
            {
                UpdateSampleValues(Sample, time);
                yield return Sample;
            }
        }
    }
}