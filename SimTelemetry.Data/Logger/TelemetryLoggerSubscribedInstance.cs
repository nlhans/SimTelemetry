/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
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
        protected string Name { get; set; }

        // Frequency of telemetrylogger
        public double RunFrequency { get; set; }

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

        public bool ResetOnchange { get; set; }

        public TelemetryLoggerSubscribedInstance(string name, Type type, object instance) : this(type, instance)
        {
            this.Name = name;
        }

        public TelemetryLoggerSubscribedInstance(Type type, object instance)
        {
            this.type = type;
            this.instance = instance;


            RunFrequency = 100.0;

            HyperTypeDescriptionProvider.Add(type);

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
                    foreach (LogOnEvent logEvent in attrs)
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

            // TODO: Put PropertyDescriptor, OnChange, Data, FrequencyCounter etc.. in a struct/class.
            for (int i = 0; i < PropertyDescriptors.Count; i++)
            {
                PropertyDescriptor fi = PropertyDescriptors[i];
                string name = fi.Name;
                data_read = false;

                // ---------- ID ----------
                if (Mapping.ContainsKey(name) == false) continue;
                int id = Mapping[name];

                bool dump = false;
                // ---------- Frequency ----------
                double Frequency = Frequencies[name]; // 1Hz normal

                // 10ms ticks.
                FrequencyCounter[name]++;
                double val = FrequencyCounter[name] / RunFrequency;
                if (val >= 1.0 / Frequency)
                {
                    FrequencyCounter[name] = 0;
                    dump = true;
                }

                // ---------- Log on change ----------
                if (LogOnChange[name])
                {
                    data = fi.GetValue(instance);
                    data_read = true;

                    // The on-change event overrules frequency events.
                    dump = !data.Equals(LogOnChangePreviousValue[name]);

                    LogOnChangePreviousValue[name] = data;

                    if (this.ResetOnchange)
                        dump = true;
                }


                // ---------- Event ----------

                // It is possible to enter multiple LogOnEvents lists.
                // The multiple lists are OR'ed.
                // All elements in 1 list are AND'ed.
                if (Events.ContainsKey(name))
                {
                    foreach (List<string> evts in Events[name])
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
                    if (!data_read)
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

        public byte[] ExportHeader()
        {

            List<byte> tmpheader = new List<byte>();


            byte[] data = new byte[64];
            ByteMethods.memcpy(data, ASCIIEncoding.ASCII.GetBytes(this.Name), 64, 0, 0);

            byte[] header = new byte[10];
            header[0] = (byte)'$';
            header[1] = (byte)'#';                                                                      // Sync
            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.InstanceName), 2, 4, 0); // Packet ID
            ByteMethods.memcpy(header, BitConverter.GetBytes(this.ID), 2, 6, 0);          // Instance ID
            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)data.Length), 2, 8, 0);            // Data length

            tmpheader.AddRange(header);
            tmpheader.AddRange(data);

            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)TelemetryLogPacket.InstanceMember), 2, 4, 0); // Packet ID
            ByteMethods.memcpy(header, BitConverter.GetBytes((ushort)68), 2, 8, 0);            // Data length
            foreach (KeyValuePair<string, int> properyMap in this.Mapping)
            {
                data = new byte[68];
                short type = 0xFF;
                Type t = this.PropertyDescriptors.Find(properyMap.Key, true).PropertyType;

                // TODO: Put in enumerator
                // Or find something nicer for this.
                if (t == typeof(double)) type = 0;
                if (t == typeof(float)) type = 1;
                if (t == typeof(Int32)) type = 2;
                if (t == typeof(Int16)) type = 3;
                if (t == typeof(byte)) type = 4;
                if (t == typeof(UInt32)) type = 5;
                if (t == typeof(UInt16)) type = 6;
                if (t == typeof(char)) type = 7;
                if (t == typeof(string)) type = 8;
                if (t == typeof(bool)) type = 9;
                if (type < 0xFF)
                {
                    ByteMethods.memcpy(data, BitConverter.GetBytes(type), 2, 0, 0);
                    ByteMethods.memcpy(data, BitConverter.GetBytes(properyMap.Value), 2, 2, 0);
                    ByteMethods.memcpy(data, ASCIIEncoding.ASCII.GetBytes(properyMap.Key), 64, 4, 0);

                    tmpheader.AddRange(header);
                    tmpheader.AddRange(data);
                }
            }


            return tmpheader.ToArray();
        }

    }
}