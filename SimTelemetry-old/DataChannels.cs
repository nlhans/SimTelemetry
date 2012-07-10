using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using SimTelemetry.Channels;
using SimTelemetry.Data;
using SimTelemetry.Objects;

namespace SimTelemetry
{
    public class DataChannels
    {
        private static Dictionary<string, List<double>> Afgeleides = new Dictionary<string, List<double>>();

        private static List<PropertyDescriptor> Descriptors_Player = new List<PropertyDescriptor>();
        private static List<PropertyDescriptor> Descriptors_Driver = new List<PropertyDescriptor>();
        private static List<PropertyDescriptor> Descriptors_Session = new List<PropertyDescriptor>();

        private static Dictionary<string, DataConversions> Conversions_Player = new Dictionary<string, DataConversions>();
        private static Dictionary<string, DataConversions> Conversions_Driver = new Dictionary<string, DataConversions>();
        private static Dictionary<string, DataConversions> Conversions_Session = new Dictionary<string, DataConversions>();

        public static void Reset()
        {
            Afgeleides = new Dictionary<string, List<double>>();
        }

        public static double Parse(string key, DataSample sample, double time)
        {
            int afgeleide = 0;

            if(Descriptors_Driver.Count == 0)
            {
                Descriptors_Driver = TypeDescriptor.GetProperties(typeof(IDriverGeneral)).OfType<PropertyDescriptor>().ToList();
                Descriptors_Player = TypeDescriptor.GetProperties(typeof(IDriverPlayer)).OfType<PropertyDescriptor>().ToList();
                Descriptors_Session = TypeDescriptor.GetProperties(typeof(ISession)).OfType<PropertyDescriptor>().ToList();

                foreach (PropertyInfo prop in typeof(IDriverGeneral).GetProperties())
                {
                    object[] attrs = prop.GetCustomAttributes(typeof(DisplayConversion), false);
                    if (attrs.Length == 1)
                        Conversions_Driver.Add(prop.Name, ((DisplayConversion)attrs[0]).Conversion);
                }
                foreach (PropertyInfo prop in typeof(IDriverPlayer).GetProperties())
                {
                    object[] attrs = prop.GetCustomAttributes(typeof(DisplayConversion), false);
                    if (attrs.Length == 1)
                        Conversions_Player.Add(prop.Name, ((DisplayConversion)attrs[0]).Conversion);
                }
                foreach (PropertyInfo prop in typeof(ISession).GetProperties())
                {
                    object[] attrs = prop.GetCustomAttributes(typeof(DisplayConversion), false);
                    if (attrs.Length == 1)
                        Conversions_Session.Add(prop.Name, ((DisplayConversion)attrs[0]).Conversion);
                }
            }

            while (key.Length > 0 && key.Substring(0, 1) == "_")
            {
                key = key.Substring(1);
                afgeleide++;
            }

            // Get the target object
            string target = key.Substring(0, 1);
            key = key.Substring(1);

            object source;
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            Dictionary<string, DataConversions> Conversions = new Dictionary<string, DataConversions>();
            switch (target)
            {
                case "P": // player
                    properties = Descriptors_Player;
                    Conversions = Conversions_Player;
                    source = sample.Player;
                    break;

                case "D": // driver data
                    properties =Descriptors_Driver;
                    Conversions = Conversions_Driver;
                    source = sample.Drivers[0];
                    break;

                case "S": // session
                    properties = Descriptors_Session;
                    Conversions = Conversions_Session;
                    source = sample.Session;
                    break;
                default: 
                    return 0.0;
                    break;
            }

            // Check if the thing contains
            foreach (PropertyDescriptor prop in properties)
            {
                object value = prop.GetValue(source);

                if (prop.Name == key)
                {
                    double v = 0;
                    if (Conversions.ContainsKey(prop.Name))
                    {
                        // Yes!
                        // Get the conversion
                        DataConversions conv = Conversions[prop.Name];

                        switch (conv)
                        {
                            case DataConversions.ROTATION_RADS_TO_RPM:
                                v= Rotations.Rads_RPM((double)value);
                                break;

                            case DataConversions.SPEED_MS_TO_KMH:
                                v= Speed.MS_KPH((double)value);
                                break;
                        }
                    }
                    else
                        v = Convert.ToDouble(value);
                    if (afgeleide == 1)
                    {
                        double OriginalValue = v;
                        if (Afgeleides.ContainsKey(key) == false)
                        {
                            Afgeleides.Add(key, new List<double>());
                            Afgeleides[key].Add(time);
                            Afgeleides[key].Add(v);
                            Afgeleides[key].Add(0);
                        }
                        if (Math.Abs(Afgeleides[key][0] - time) < 0.02) // less than 1 ms difference?
                        {
                            return Afgeleides[key][2];
                        }
                        /*for(int a = 0 ;a < afgeleide; a++)
                        {
                            if (Afgeleides[key].Count <= a+2)
                            {
                                Afgeleides[key].Add(0);
                            }
                            double afg = (Afgeleides[key][a + 1] - Afgeleides[key][a + 2]) / (0.000001 + time - Afgeleides[key][0]);
                            if (double.IsNaN(afg) || double.IsInfinity(afg))
                                afg = 1;
                            Afgeleides[key][0] = time;
                            Afgeleides[key][a+2] = afg;
                            v = afg;
                        }*/
                        double dt = time - Afgeleides[key][0];
                        double dv = v - Afgeleides[key][1];
                        Afgeleides[key][0] = time;
                        Afgeleides[key][1] = OriginalValue;
                        Afgeleides[key][2] = Afgeleides[key][2] * 0.8 + 0.2 * dv/dt/3.6/9.81;
                        return Afgeleides[key][2];
                    }

                    /*
                    List<double> vorigeafgeleides = Afgeleides[key];
                    List<double> mijnafgeleides = new List<double>();
                    mijnafgeleides.Add(v);
                    for (int a = 1; a < afgeleide; a++)
                    {
                        mijnafgeleides[a] = 
                    }*/
                    return v;
                }
                
            }
            switch(key)
            {
                case "RPM":
                    //return Rotations.Rads_RPM((double)sample.Player.Engine_RPM);
                    break;

                case "Speed":
                    //return Speed.Ms_Kmh(sample.Player.Speed);
                    break;

                case "SteeringAngle":
                    return sample.Player.SteeringAngle;
                    break;

                case "Pedals_Throttle":
                    return sample.Player.Pedals_Throttle;
                    break;

                case "Pedals_Brake":
                    return sample.Player.Pedals_Brake;
                    break;
            }

            return 0;

        }
    }
}
