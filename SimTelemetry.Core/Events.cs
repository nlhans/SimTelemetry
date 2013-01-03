using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Core
{
    public class EventDelegate
    {
        public Delegate Action { get; set; }
        public bool Network { get; set; }
    }

    public class Events
    {
        private static List<EventDelegate> _handlers = new List<EventDelegate>();

#if DEBUG
        public static int Count { get { return _handlers.Count; } }
        public static IEnumerable<EventDelegate> List { get { return _handlers; } }
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
            if (_handlers.Select(x => x.Action).Contains(handler) == false)
                _handlers.Add(new EventDelegate { Action = handler, Network = includeNetwork });
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
                    .Where(x => includeNetwork || !x.Network)
                    .Select(x => x.Action)
                    .OfType<Action<T>>())
            {
                handler(Data);
            }

        }
    }
}
