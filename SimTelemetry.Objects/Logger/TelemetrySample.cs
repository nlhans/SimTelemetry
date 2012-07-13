using System;
using System.Collections.Generic;

namespace SimTelemetry.Objects
{
    public class TelemetrySample
    {
        public double Time { get; set; }
        public Dictionary<int, Dictionary<int, object>> Data { get; set; }

        //TODO: Automatic searching!!!
        public double this[string key]
        {
            get
            {
                string[] d = key.Split(".".ToCharArray());
                if (d.Length != 2)
                {
                    return 0;
                }
                else
                {
                    string obj = d[0];
                    string val = d[1];
                    if (val == "Coordinate_Z") return (double)Data[3][7];
                    if (val == "Coordinate_X") return (double)Data[3][9];
                    if (val == "Pedals_Brake") return (double)Data[3][11];
                    if (val == "Pedals_Throttle") return (double)Data[3][10];

                    return 0;
                }
            }
        }

        public TelemetrySample()
        {
            Data = new Dictionary<int, Dictionary<int, object>>();
        }

        public TelemetrySample Clone()
        {
            TelemetrySample sample = new TelemetrySample
                                         {
                                             Time = Time,
                                             Data = new Dictionary<int, Dictionary<int, object>>()
                                         };

            foreach(KeyValuePair<int, Dictionary<int, object>> kvp in this.Data)
            {
                sample.Data.Add(kvp.Key, new Dictionary<int, object>(kvp.Value));
            }
            return sample;
        }

        public string GetString(string key)
        {
                string[] d = key.Split(".".ToCharArray());
                if (d.Length != 2)
                {
                    return "NULL";
                }
                else
                {
                    string obj = d[0];
                    string val = d[1];

                    if (val == "GameDirectory") return Data[1][1].ToString();
                    if (val == "Track") return Data[1][4].ToString();
                    return "";
                }
        }
    }
}