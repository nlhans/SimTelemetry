using System.Collections.Generic;

namespace SimTelemetry.Objects
{
    public interface IDriverCollection
    {
        List<IDriverGeneral> AllDrivers { get; }


        IDriverGeneral Player { get; }
    }
}