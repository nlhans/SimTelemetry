using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Logger
{
    internal class TelemetryLoggerSubscribedInstance
    {
        public Type type { get; protected set; }
        public object instance { get; protected set; }

        // Property <> Event Sensitivity
        public Dictionary<string, List<string>> Events { get; set; }

        // Property <> ID
        public Dictionary<string, int> Mapping { get; set; }

        // Analysis of class
        public PropertyDescriptorCollection PropertyDescriptors { get; set; }

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

            Events = new Dictionary<string, List<string>>();
            Mapping = new Dictionary<string, int>();
            Frequencies = new Dictionary<string, double>();
            FrequencyCounter = new Dictionary<string, int>();

            // Fill things.
            int indexer = 0;
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
                double Frequency = 100.0; // 100Hz normal
                attrs = pi.GetCustomAttributes(typeof(Loggable), false);
                if (attrs.Length == 1)
                    Frequency = ((Loggable)attrs[0]).Freqency;

                Frequencies.Add(fi.Name, Frequency);
                FrequencyCounter.Add(fi.Name, 0);

                // ---------- Log on change ----------
                attrs = pi.GetCustomAttributes(typeof(LogOnChange), false);
                // TODO: Implement this.
                
                // ---------- Events ----------
                attrs = pi.GetCustomAttributes(typeof(LogOnEvent), false);

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

            foreach (PropertyDescriptor fi in PropertyDescriptors)
            {
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


                // ---------- Event ----------
                // TODO: Implement this.

                // ---------- Log on change ----------
                // TODO: Implement this.

                // ---------- Dumping ----------
                if (dump)
                {
                    byte[] b = new byte[0];
                    object data = fi.GetValue(instance);
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