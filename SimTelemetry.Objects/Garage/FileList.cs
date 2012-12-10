﻿/*************************************************************************
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
using System.IO;

namespace SimTelemetry.Objects.Garage
{
    public class FileList
    {
        private List<string> Files;

        public FileList(string root, string[] extensions_array)
        {
            if (root != null)
            {
                Files = new List<string>();
                // All files of interest are:
                List<string> extensions = new List<string>(extensions_array);
                foreach (string f in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(f).ToLower();
                    if (extensions.Contains(ext))
                        Files.Add(f.ToLower());
                }
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
            int dl = directory.Length;
            int pl = pattern.Length;
            int tl = dl + pl;
            List<string> files = Files.FindAll(delegate(string f)
                                                   {
                                                       int l = f.Length;
                                                       if (l < tl)
                                                           return false;
                                                       string start = f.Substring(0,dl);
                                                       string end = f.Substring(l - pl, pl);
                                                       return (pattern.Equals(end) && start.Equals(directory));
                                                   });
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