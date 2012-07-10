using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects
{
    public interface ISimTelemetry
    {
        void Report_SimStart(ISimulator me);
        void Report_SimStop(ISimulator me);
    }
}
