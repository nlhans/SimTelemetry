using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using SimTelemetry.Domain.Events;

namespace SimTelemetry.Domain
{
    public class GlobalEvents
    {
        private static ConcurrentDictionary<object, GlobalEventDelegate> _handlers = new ConcurrentDictionary<object, GlobalEventDelegate>();
        private static ConcurrentDictionary<Type, DateTime> _lastFires = new ConcurrentDictionary<Type, DateTime>();

#if DEBUG
        public static int Count { get { return _handlers.Count; } }
        public static IEnumerable<GlobalEventDelegate> List { get { return _handlers.Values; } }
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
            //if (_handlers.Any(x => x.Value.Action.Equals(handler)) == false)
            if (!_handlers.ContainsKey(handler))
                if (!_handlers.TryAdd(handler, new GlobalEventDelegate { Action = handler, Network = includeNetwork }))
                    throw new Exception("Handler dictionary is currently locked");
        }


        public static void Unhook<T>(Action<T> handler)
        {
            var handlerHash = handler.GetHashCode();

            GlobalEventDelegate n;

            while (_handlers.Count(x => x.Value.Action.GetHashCode() == handlerHash) > 0)
                _handlers.TryRemove(_handlers.Where(x => x.Value.Action.GetHashCode() == handlerHash).First().Key, out n);
        }

        public static void Fire<T>(T Data, bool includeNetwork)
        {
            if (!(typeof(T).Name == "TelemetryRefresh"))
            {
                Debug.WriteLine(typeof (T).Name + " fired");
            }
            foreach (var handler in _handlers
                .Where(x => includeNetwork || (includeNetwork == false && x.Value.Network == false))
                .Select(x => x.Value.Action).OfType<Action<T>>())
                handler(Data);

        }

        protected static bool ViolatesPeriod(Type what, double period)
        {
            if (_lastFires.ContainsKey(what) == false)
            {
                _lastFires.TryAdd(what, DateTime.Now);
                return false;
            }
            else
            {
                var difference = DateTime.Now.Subtract(_lastFires[what]);
                return (difference.TotalMilliseconds >= period) ? false : true;
            }
        }

        public static void Fire<T>(T Data, bool includeNetwork, double MinPeriod)
        {
            if (ViolatesPeriod(typeof(T), MinPeriod))
                return;

            foreach (var handler in _handlers
                .Where(x => includeNetwork || (includeNetwork == false && x.Value.Network == false))
                .Select(x => x.Value.Action).OfType<Action<T>>())
                handler(Data);

        }
    }
}