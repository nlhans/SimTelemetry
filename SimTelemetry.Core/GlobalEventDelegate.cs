using System;

namespace SimTelemetry.Core
{
    public class GlobalEventDelegate
    {
        public Delegate Action { get; set; }
        public bool Network { get; set; }
    }

    public interface IEvent
    {
        
    }

    public interface ILoggableEvent : IEvent
    {
        string GetHashCode();
        void Deserialize(byte[] data);
        byte[] Serialize();

        object Value { get; set; }
    }

    public class GlobalEventLog
    {
        public ILoggableEvent Event { get; protected set; }
        public DateTime Date { get; protected set;  }

        public GlobalEventLog(ILoggableEvent e, DateTime d)
        {
            Date = d;
            Event = e;
        }
    }

    public class EngineRPM : ILoggableEvent
    {
        public byte[] Serialize()
        {
            return BitConverter.GetBytes((double) Value);
        }

        public void Deserialize(byte[] data)
        {
            Value = BitConverter.ToDouble(data,0);
        }

        public object Value { get; set; }
        public new string GetHashCode()  { return GetType().FullName; }
    }
}
