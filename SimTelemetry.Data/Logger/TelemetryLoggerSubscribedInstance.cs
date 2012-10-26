using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using SimTelemetry.Objects;
using SimTelemetry.Objects.HyperType;

namespace SimTelemetry.Data.Logger
{
    internal class TelemetryLoggerSubscribedInstance
    {
        public Type type { get; protected set; }
        public object instance { get; protected set; }

        // Analysis of class
        public PropertyDescriptorCollection PropertyDescriptors { get; set; }

        // TODO: Place all these dictionaries to 1 struct.

        // Property <> Event Sensitivity
        public Dictionary<string, List<List<string>>> Events { get; set; }

        // Property <> Log on change
        public Dictionary<string, bool> LogOnChange { get; set; }
        public Dictionary<string, object> LogOnChangePreviousValue { get; set; }

        // Property <> ID
        public Dictionary<string, int> Mapping { get; set; }

        // Property <> Log Frequency
        public Dictionary<string, double> Frequencies { get; set; }

        // Property <> Log Frequency Counter
        public Dictionary<string, int> FrequencyCounter { get; set; }

        // Unique ID for dump file
        public ushort ID { get; set; }

        public TelemetryLoggerSubscribedInstance(Type type, object instance)
        {
            this.type = type;
            this.instance = instance;

           // HyperTypeDescriptionProvider.Add(type);

            Events = new Dictionary<string, List<List<string>>>();
            LogOnChange = new Dictionary<string, bool>();
            LogOnChangePreviousValue = new Dictionary<string, object>();
            Mapping = new Dictionary<string, int>();
            Frequencies = new Dictionary<string, double>();
            FrequencyCounter = new Dictionary<string, int>();

            // Fill things.
            int indexer = 0;
            double Frequency; 

            PropertyDescriptors = TypeDescriptor.GetProperties(type);
            PropertyInfo[] pic = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in pic)
            {
                PropertyDescriptor fi = PropertyDescriptors[pi.Name];
                if (fi == null || fi.Attributes.Contains(new Unloggable()))
                    continue;

                // ---------- Unloggable ----------
                object[] attrs = pi.GetCustomAttributes(typeof(Unloggable), false);
                if (attrs.Length == 1)// is unloggable?
                    continue;

                // ---------- Frequency ----------
                attrs = pi.GetCustomAttributes(typeof(Loggable), false);

                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;
                else
                    Frequency = 100.0; // 100Hz normal

                Frequencies.Add(fi.Name, Frequency);
                FrequencyCounter.Add(fi.Name, 0);

                // ---------- Log on change ----------
                attrs = pi.GetCustomAttributes(typeof(LogOnChange), false);
                LogOnChange.Add(fi.Name, ((attrs.Length == 1) ? true : false));
                LogOnChangePreviousValue.Add(fi.Name, null);
                
                // ---------- Events ----------
                attrs = pi.GetCustomAttributes(typeof(LogOnEvent), false);
                Events.Add(fi.Name, new List<List<string>>());
                if (attrs.Length > 0)
                {
                    foreach(LogOnEvent logEvent in attrs)
                        Events[fi.Name].Add(logEvent.Events);
                }


                // ---------- Mapping ----------
                indexer++;
                Mapping.Add(fi.Name, indexer);
            }
        }

        public byte[] Dump(List<string> EventsFired)
        {
            // Do a live data dump.
            List<byte> output = new List<byte>();

            output.AddRange(BitConverter.GetBytes(ID));

            object data = -10293812;
            bool data_read = false;
            foreach (PropertyDescriptor fi in PropertyDescriptors)
            {
                data_read = false;

                // ---------- ID ----------
                if (Mapping.ContainsKey(fi.Name) == false) continue;
                int id = Mapping[fi.Name];

                bool dump = false;
                // ---------- Frequency ----------
                double Frequency = Frequencies[fi.Name]; // 1Hz normal

                // 10ms ticks.
                FrequencyCounter[fi.Name]++;
                double val = FrequencyCounter[fi.Name]/100.0;
                if (val >=1.0/Frequency)
                {
                    FrequencyCounter[fi.Name] = 0;
                    dump = true;
                }

                // ---------- Log on change ----------
                if (LogOnChange[fi.Name])
                {
                    data = fi.GetValue(instance);
                    data_read = true;

                    // The on-change event overrules frequency events.
                    dump = !data.Equals(LogOnChangePreviousValue[fi.Name]);

                    LogOnChangePreviousValue[fi.Name] = data;
                }


                // ---------- Event ----------

                // It is possible to enter multiple LogOnEvents lists.
                // The multiple lists are OR'ed.
                // All elements in 1 list are AND'ed.
                if (Events.ContainsKey(fi.Name))
                {
                    foreach (List<string> evts in Events[fi.Name])
                    {
                        bool log = true;

                        // AND all elements in this list.
                        foreach (string evt in evts)
                        {
                            if (EventsFired.Contains(evt) == false)
                            {
                                log = false;
                                break;
                            }
                        }

                        // OR elements from the lists (one event must be fired)
                        if (log)
                        {
                            dump = true;
                            break;
                        }
                    }
                }

                // ---------- Dumping ----------
                if (dump)
                 {
                    byte[] b = new byte[0];
                    if(!data_read)
                        data = fi.GetValue(instance);

                    Type t = fi.PropertyType;

                    if (t == typeof(double))
                        b = BitConverter.GetBytes((double)data);

                    if (t == typeof(float))
                        b = BitConverter.GetBytes((float)data);

                    if (t == typeof(Int32))
                        b = BitConverter.GetBytes((Int32)data);

                    if (t == typeof(byte))
                        b = BitConverter.GetBytes((byte)data);

                    if (t == typeof(bool))
                        b = BitConverter.GetBytes((bool)data);

                    if (t == typeof(string))
                        b = ASCIIEncoding.ASCII.GetBytes((string)data);

                    output.AddRange(BitConverter.GetBytes((Int32)id));
                    output.AddRange(b);
                    if (t == typeof(string))
                        output.Add(0);


                }
            }
            return output.ToArray();

        }

    }
}