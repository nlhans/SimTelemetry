using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain
{
    public class GlobalEvents
    {
        private static List<GlobalEventDelegate> _handlers = new List<GlobalEventDelegate>();

#if DEBUG
        public static int Count { get { return _handlers.Count; } }
        public static IEnumerable<GlobalEventDelegate> List { get { return _handlers; } }
        // For testing only:
        public static void Reset()
        {
            _handlers.Clear();
        }
#endif

        public static void ConnectNetwork()
        {

        }

        public static void DisconnectNetwork()
        {

        }


        public static void Hook<T>(Action<T> handler, bool includeNetwork)
        {
            // Allow multiple inclusion??
            if (_handlers.Any(x => x.Action.Equals(handler)) == false)
                _handlers.Add(new GlobalEventDelegate { Action = handler, Network = includeNetwork });
        }


        public static void Unhook<T>(Action<T> handler)
        {
            var handlerHash = handler.GetHashCode();

            while (_handlers.Count(x => x.Action.GetHashCode() == handlerHash) > 0)
                _handlers.Remove(_handlers.Where(x => x.Action.GetHashCode() == handlerHash).First());
        }

        public static void Fire<T>(T Data, bool includeNetwork)
        {
            foreach (var handler in _handlers
                .Where(x => includeNetwork || (includeNetwork == false && x.Network==false))
                .Select(x => x.Action).OfType<Action<T>>())
                handler(Data);

        }
    }
}