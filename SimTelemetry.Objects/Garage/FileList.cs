using System;
using System.Collections.Generic;
using System.IO;

namespace SimTelemetry.Objects.Garage
{
    public class FileList
    {
        private List<string> Files;

        public FileList(string root)
        {
            if (root != null)
            {
                Files = new List<string>();
                foreach (string f in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
                    Files.Add(f.ToLower());
            }
        }

        /// <summary>
        /// Searches for all vehicles with specific pattern in a directory.
        /// Returns all files (in relative path to directory) in list.
        /// All files are locally cached.
        /// </summary>
        /// <param name="directory">Absolute path to directory to search.</param>
        /// <param name="pattern">File pattern. *.txt returns all txt-extension files.</param>
        /// <returns>List of found files (relative path)</returns>
        public List<string> SearchFiles(string directory, string pattern)
        {
            if(pattern.StartsWith("*."))
                pattern = pattern.Substring(1);
            pattern = pattern.ToLower();
            directory = directory.ToLower();
            List<string> files = Files.FindAll(delegate(string f) { return f.StartsWith(directory) && f.EndsWith(pattern); });
            return files;
        }
        /// <summary>
        /// Searches for 1 file of a pattern in a directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string SearchFile(string directory, string pattern)
        {
            List<string> files = SearchFiles(directory, pattern);
            if (files.Count == 1) return files[0];
            else if (files.Count == 0) throw new Exception("Could not find 1 file");
            else return files[0]; // throw new Exception("Found multiple files");
        }
    }
}