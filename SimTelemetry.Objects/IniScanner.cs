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
        public Dictionary<string, Dictionary<string, List<string>>> Data =
            new Dictionary<string, Dictionary<string, List<string>>>();

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
        ///  The INI file that must be read.
        /// </summary>
        public string IniFile { get; set; }

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
            return d[0];
        }

        public string TryGetString(string group, string key)
        {
            string[] d = TryGetData(group, key);
            return d[0];
        }

        public string[] TryGetData(string group, string key)
        {
            if(Data.ContainsKey(group) && Data[group].ContainsKey(key))
            {
                if(Data[group][key].Count >= 1)
                    return Data[group][key].ToArray();
            }

            return new string[1]{""};
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

        public void Read()
        {
            string[] lines = File.ReadAllLines(IniFile);
            
            // Set property to 'main'
            Group = "Main";

            for (int i = 0; i < lines.Length;i ++)
            {
                string line = lines[i].TrimEnd();
                // Is there a comment at the end of this line? Remove.
                if (line.Contains("//"))
                    line = line.Substring(0, line.LastIndexOf("//"));

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    Group = line.Substring(1, line.Length - 2);
                }
                else if(line == "{")
                {
                    // Ignore
                }
                else if(i>lines.Length-1 && lines[i+1] == "{")
                {
                    // This starts a new group at this line.
                    PushGroup(line);
                }
                else if(line == "}")
                {
                    // This ends the group
                    PopGroup();
                }
                else if (line.Contains("="))
                {
                    string[] key_data = line.Split("=".ToCharArray(), 2);
                        List<string> data = new List<string>();

                    // Is this line in the cusotm keys list?
                    if(FireEventsForKeys.Contains(Group+"."+key_data))
                    {
                        if (HandleCustomKeys != null)
                            HandleCustomKeys(key_data);
                    }
                    else
                    {
                        // Determine if the line contains a list of values.
                        if (line.Contains("(") && line.Trim().EndsWith(")"))
                        {
                            // Remove () from line, then split on comma.
                            string d = key_data[1];
                            d = d.Substring(d.IndexOf("(") + 1);
                            d = d.Substring(0, d.IndexOf(")"));

                            string[] data_parameters = d.Split(",".ToCharArray());

                            data = data_parameters.OfType<string>().ToList();
                        }
                        else
                        {
                            //  Add single value.
                            data.Add(key_data[1]);
                        }

                        // If group doesn't exist; create it in the data dict.
                        if (Data.ContainsKey(Group) == false)
                            Data.Add(Group, new Dictionary<string, List<string>>());
                        
                        // Check if data exists. Otherwise throw event DuplicateKey
                        if (Data[Group].ContainsKey(key_data[0].Trim()) == false)
                            Data[Group].Add(key_data[0].Trim(), data);
                        else if (HandleDuplicateKey != null)
                            HandleDuplicateKey(key_data);
                    }
                }
                else if (line.Trim() != "" && line.StartsWith("#") == false)
                {
                    // No comment, not empty.. Dunno what to do with this!
                    if (HandleUnknownLine != null)
                        HandleUnknownLine(line);
                }

            }

        }
    }
}