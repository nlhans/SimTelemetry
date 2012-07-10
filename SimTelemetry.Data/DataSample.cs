using System.Collections.Generic;
using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class DataSample
    {
        public double Time = 0;
        public SampledSession Session;
        public SampledDriverPlayer Player;
        public List<SampledDriverGeneral> Drivers;

        public DataSample()
        {
            Player = new SampledDriverPlayer();
            Session = new SampledSession();
            Drivers = new List<SampledDriverGeneral>();
        }
    }
}