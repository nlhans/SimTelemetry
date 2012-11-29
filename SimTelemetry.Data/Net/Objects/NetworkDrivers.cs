using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net.Objects
{
    public class NetworkDrivers : IDriverCollection
    {
        private IDriverGeneral p = new NetworkDriverGeneral();
        public List<IDriverGeneral> AllDrivers
        {
            get { List<IDriverGeneral> drg = new List<IDriverGeneral>();
                drg.Add(p);
                return drg;
            }
        }

        public IDriverGeneral Player
        {
            get { return p; }
        }
    }
}
