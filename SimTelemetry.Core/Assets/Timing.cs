using SimTelemetry.Core.Events;

namespace SimTelemetry.Core.Assets
{
    public class Timing
    {
        
        public Timing()
        {
            // This has to attach to:
            GlobalEvents.Hook<SessionStart>(StartTiming, false);
            GlobalEvents.Hook<SessionStop>(StopTiming, false);
        }

        protected void StopTiming(SessionStop st)
        {
            //

        }

        protected void StartTiming(SessionStart st)
        {
            //

        }
    }
}
