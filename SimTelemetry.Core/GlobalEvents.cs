using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Core
{
    public class GlobalEvents
    {
        private static List<Delegate> _handlers2 = new List<Delegate>();
        private static List<GlobalEventDelegate> _handlers = new List<GlobalEventDelegate>();
        private static List<Action<ILoggableEvent>> loggers = new List<Action<ILoggableEvent>>();

#if DEBUG
        public static int Count { get { return _handlers2.Count; } }
        public static IEnumerable<GlobalEventDelegate> List { get { return _handlers; } }
        // For testing only:
        public static void Reset()
        {
            _handlers2.Clear();
        }
#endif

        public static void ConnectNetwork()
        {

        }

        public static void DisconnectNetwork()
        {

        }

        public static void HookLogger(Action<ILoggableEvent> logger)
        {
            if (loggers.Contains(logger) == false)
                loggers.Add(logger);
        }

        public static void Hook<T>(Action<T> handler, bool includeNetwork)
        {
            // Allow multiple inclusion??
            //if (_handlers.Select(x => x.Action).Contains(handler) == false)
            //    _handlers.Add(new GlobalEventDelegate { Action = handler, Network = includeNetwork });
            if (_handlers2.Contains(handler) == false)
            _handlers2.Add(handler);
        }


        public static void Unhook<T>(Action<T> handler)
        {
            var handlerHash = handler.GetHashCode();

            while (_handlers.Count(x => x.Action.GetHashCode() == handlerHash) > 0)
                _handlers.Remove(_handlers.Where(x => x.Action.GetHashCode() == handlerHash).First());
        }

        public static void Fire<T>(T Data, bool includeNetwork)
        {
            foreach (var handelr in _handlers2.OfType<Action<T>>())
                handelr(Data);
            foreach (var logger in loggers)
                logger((ILoggableEvent)Data);

        }
    }
}