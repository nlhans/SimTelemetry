using System;
using System.Collections.Generic;
using System.IO;

namespace SimTelemetry.Objects.Garage
{
    public class GarageTools
    {
        /// <summary>
        /// Searches for all vehicles with specific pattern in a directory.
        /// Returns all files (in relative path to directory) in list.
        /// </summary>
        /// <param name="directory">Absolute path to directory to search.</param>
        /// <param name="pattern">File pattern. *.txt returns all txt-extension files.</param>
        /// <returns>List of found files (relative path)</returns>
        public static List<string> SearchFiles(string directory, string pattern)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(directory, pattern, SearchOption.AllDirectories))
                files.Add(file);
            return files;
        }
        /// <summary>
        /// Searches for 1 file of a pattern in a directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string SearchFile(string directory, string pattern)
        {
            List<string> files = SearchFiles(directory, pattern);
            if (files.Count == 1) return files[0];
            else if(files.Count == 0) throw new Exception("Could not find 1 file");
            else throw new Exception("Found multiple files");
        }
    }
}
