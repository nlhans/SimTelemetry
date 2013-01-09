using System.Collections.Generic;
using SimTelemetry.Core.Aggregates;

namespace SimTelemetry.Core.Entities
{
    public class Mod
    {
        public string Name { get; private set; }
        public IEnumerable<string> Classes { get; private set; }
        public IEnumerable<Season> Seasons { get; private set; }

        public IEnumerable<Car> Cars { get; private set; }
    }
}