using System;
using System.Threading;
using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public sealed class SimTelemetry : ISimTelemetry
    {
        private static SimTelemetry _m = new SimTelemetry();
        public static SimTelemetry m { get { return _m; } }

        public Simulators Sims;

        public SimTelemetry()
        {
            if(m != null)
                throw new Exception("Already initialized");

            ThreadPool.QueueUserWorkItem(new WaitCallback(Bootup));
        }

        public void Bootup(object no)
        {
            Sims = new Simulators();
        }

        // All code destioned for plugins.
        public void Report_SimStart(ISimulator me)
        {
            Sims.FireStart(me);
        }

        public void Report_SimStop(ISimulator me)
        {
            Sims.FireStop(me);
        }
    }
}