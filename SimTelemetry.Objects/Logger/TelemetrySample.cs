using System;
using System.Collections.Generic;

namespace SimTelemetry.Objects
{
    public class TelemetrySample
    {
        public double Time { get; set; }
        public Dictionary<int, Dictionary<int, object>> Data { get; set; }

        public TelemetrySample()
        {
            Data = new Dictionary<int, Dictionary<int, object>>();
        }

        public TelemetrySample Clone()
        {
            TelemetrySample sample = new TelemetrySample
                                         {
                                             Time = Time,
                                             Data = new Dictionary<int, Dictionary<int, object>>()
                                         };

            foreach(KeyValuePair<int, Dictionary<int, object>> kvp in this.Data)
            {
                sample.Data.Add(kvp.Key, new Dictionary<int, object>(kvp.Value));
            }
            return sample;
        }
    }
}