using System.Collections.Generic;
using SimTelemetry.Domain.Aggregates;

namespace SimTelemetry.Domain.Entities
{
    public class Mod
    {
        public string Name { get; private set; }
        public IEnumerable<string> Classes { get; private set; }
        public IEnumerable<Season> Seasons { get; private set; }

        public IEnumerable<Car> Cars { get; private set; }

        public Mod(string name, IEnumerable<string> classes, IEnumerable<Season> seasons, IEnumerable<Car> cars)
        {
            Name = name;
            Classes = classes;
            Seasons = seasons;
            Cars = cars;
        }
    }
}