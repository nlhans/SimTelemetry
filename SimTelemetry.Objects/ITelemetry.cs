using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects
{
    public interface ITelemetry
    {
        ISimulator Sim { get; }
        bool Active_Sim { get; }
        bool Active_Session { get; }

        //void Report_SimStart(ISimulator me);
        //void Report_SimStop(ISimulator me);
    }
}
