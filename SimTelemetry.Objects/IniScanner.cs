using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Triton;

namespace SimTelemetry.Objects
{

    public class IniScanner
    {
        /// <summary>
        /// The data dictionary with groups, keys and then data (arrays).
        /// </summary>
        public Dictionary<string, Dictionary<string, string[]>> Data =
            new Dictionary<string, Dictionary<string, string[]>>();

        /// <summary>
        /// This event is fired whenever the INI parser comes across a line that isn't a key or group indicator.
        /// </summary>
        public Signal HandleUnknownLine;
        /// <summary>
        /// This event is fired when a duplicate is tried to be inserted.
        /// </summary>
        public Signal HandleDuplicateKey;
        
        /// <summary>
        /// This event is fired when the current line tries to insert a key that is present in the FireEventsForKeys property.
        /// The key value is NOT inserted into the final data dictionary!
        /// </summary>
        public Signal HandleCustomKeys;

        /// <summary>
        /// This property contans all keys where the HandleCustomKeys event for will be fired.
        /// </summary>
        public List<string> FireEventsForKeys { get; set; }

        /// <summary>
        /// The INI file that must be read.
        /// When streams in memory must be parsed; pass data via IniData.
        /// </summary>
        public string IniFile { get; set; }

        /// <summary>
        /// Alternatively to IniFile property; raw string data that must be parsed.
        /// </summary>
        public string IniData { get; set; }

        /// <summary>
        /// This setting places all keys in one group.
        /// Default: true
        /// </summary>
        public bool IgnoreGroups = true;

        private List<string> Groups = new List<string>();
        private string Group
        {
            get
            {
                if (IgnoreGroups)
                    return "Main";
                string g = "";
                for (int i = 0; i < Groups.Count; i++)
                {
                    if (i == 0)
                        g += Groups[i].Trim();
                    else
                        g += "." + Groups[i].Trim();
                }
                return g;
            }
            set
            {
                // Set to single-group
                Groups = new List<string>();
                Groups.Add(value);
            }
        }

        public string TryGetString(string key)
        {
            string[] d = TryGetData("Main", key);
            return d[0].Trim();
        }

        public string TryGetString(string group, string key)
        {
            string[] d = TryGetData(group, key);
            return d[0].Trim();
        }

        public string[] TryGetData(string group, string key)
        {
            key = key.ToLower();
            if(Data.ContainsKey(group) && Data[group].ContainsKey(key))
            {
                if(Data[group][key].Length >= 1)
                    return Data[group][key].ToArray();
            }
            return new string[1] { "" };
        }

        public int TryGetInt32(string key)
        {
            int value = 0;
            Int32.TryParse(TryGetString(key), out value);
            return value;
        }

        public int TryGetInt32(string group, string key)
        {
            int value = 0;
            Int32.TryParse(TryGetString(group, key), out value);
            return value;
        }

        public double TryGetDouble(string key)
        {
            double value = 0;
            double.TryParse(TryGetString(key), out value);
            return value;
        }

        public double TryGetDouble(string group, string key)
        {
            double value = 0;
            Double.TryParse(TryGetString(group, key), out value);
            return value;
        }
        
        public IniScanner()
        {
            FireEventsForKeys = new List<string>();
            Group = "Main";
            
        }

        private void PushGroup(string group)
        {
            Groups.Add(group);
        }
        private void PopGroup()
        {
            if (Groups.Count >= 1)
            {
                Groups.RemoveAt(Groups.Count - 1);
            }
        }

        private static string[] ParseParameter(string d)
        {
            d = d.Trim();
            // Determine if the line contains a list of values.)
            if (d.Length > 2 && d.Substring(0,1) == "(" && d.Substring(d.Length-1,1) == ")")
            {
                // Remove () from line, then split on comma.
                d = d.Substring(1, d.Length-2);

                string[] d_a= d.Split(",".ToCharArray());

                for (int i = 0; i < d_a.Length; i++)
                {
                    d_a[i] = d_a[i].Trim();
                    int l = d_a[i].Length;
                    if (l>2 && d_a[i].Substring(0,1) == "\"" && d_a[i].Substring(l-1,1) == "\"")
                        d_a[i] = d_a[i].Substring(1, d_a.Length - 2);
                }
                return d_a;
            }
            else
            {
                return new string[1] {d};
            }

        }

        public void Read()
        {
            if (IniData == null && IniFile == null) return;

            string[] lines = new string[0];
            if(IniFile == null)
            {
                lines = IniData.Split("\n".ToCharArray());
            }
            else
            {
             lines =File.ReadAllLines(IniFile);
                
            }
            
            // Set property to 'main'
            Group = "Main";

            for (int i = 0; i < lines.Length;i ++)
            {
                string line = lines[i];
                // Is there a comment at the end of this line? Remove.
                if (line.Contains("//"))
                    line = line.Substring(0, line.LastIndexOf("//"));
                line = line.TrimEnd();

                if (line.Length == 0) continue;

                string first_char = line.Substring(0, 1);
                if (first_char == "[" && line.EndsWith("]"))
                {
                    Group = line.Substring(1, line.Length - 2);
                }
                else if(first_char == "{")
                {
                    // Ignore
                }
                else if(i< lines.Length-1 && lines[i+1].Trim() == "{")
                {
                    // This starts a new group at this line.
                    PushGroup(line);
                }
                else if (first_char == "}")
                {
                    // This ends the group
                    PopGroup();
                }
                else if (line.Contains("="))
                {
                    string[] key_data = line.Split("=".ToCharArray(), 2);
                    key_data[0] = key_data[0].ToLower().Trim();
                    string[] data = ParseParameter(key_data[1]);

                    // Is this line in the cusotm keys list?
                    if(FireEventsForKeys.Contains(Group+"."+key_data[0]))
                    {
                        if (HandleCustomKeys != null)
                            HandleCustomKeys(new object[2] { Group + "." + key_data[0], data });
                    }
                    else
                    {
                        // If group doesn't exist; create it in the data dict.
                        if (Data.ContainsKey(Group) == false)
                            Data.Add(Group, new Dictionary<string, string[]>());
                        
                        // Check if data exists. Otherwise throw event DuplicateKey
                        if (Data[Group].ContainsKey(key_data[0]) == false)
                            Data[Group].Add(key_data[0], data);
                        else if (HandleDuplicateKey != null)
                            HandleDuplicateKey(key_data);
                    }
                }
                else if (line.Trim() != "" && first_char != "#") 
                {
                    // No comment, not empty.. Dunno what to do with this!
                    if (HandleUnknownLine != null)
                        HandleUnknownLine(line);
                }

            }

        }

    }
}