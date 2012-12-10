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
using System.IO;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.rFactor2.Garage
{
    public class rFactor2FileList
    {
        private List<MAS2Reader> MAS = new List<MAS2Reader>();
        private List<MAS2File> MASFiles = new List<MAS2File>();

        public rFactor2FileList(string dir, string[] extensions)
        {
            List<string> mas_Files = GarageTools.SearchFiles(dir, "*.mas");
            foreach(string mas_file in mas_Files)
            {
                MAS2Reader mas2r = new MAS2Reader(mas_file, extensions);
                MASFiles.AddRange(mas2r.Files);
                MAS.Add(mas2r);
            }
        }

        /// <summary>
        /// Searches for all vehicles with specific pattern in a MAS archive.
        /// Returns all files (in relative path to directory) in list.
        /// All files are locally cached.
        /// </summary>
        /// <param name="directory">Absolute path to directory to search.</param>
        /// <param name="pattern">File pattern. *.txt returns all txt-extension files.</param>
        /// <returns>List of found files (relative path)</returns>
        public List<MAS2File> SearchFiles(string directory, string pattern)
        {
            if (pattern.StartsWith("*."))
                pattern = pattern.Substring(1);
            pattern = pattern.ToLower();
            directory = directory.ToLower();
            List<MAS2File> files = MASFiles.FindAll(delegate(MAS2File f) { return f.Master.File.ToLower().Contains(directory) && f.Filename.EndsWith(pattern); });
            return files;
        }

        /// <summary>
        /// Searches for all vehicles with specific pattern.
        /// Returns all files matching.
        /// All files are locally cached.
        /// </summary>
        /// <param name="pattern">File pattern. *.txt returns all txt-extension files.</param>
        /// <returns>List of found files (relative path)</returns>
        public List<MAS2File> SearchFiles(string pattern)
        {
            if (pattern.StartsWith("*."))
                pattern = pattern.Substring(1);
            pattern = pattern.ToLower();
            int k = pattern.Length;
            List<MAS2File> files = MASFiles.FindAll(delegate(MAS2File f)
                                                        {
                                                            int l = f.Filename.Length;
                                                            if (l == k)
                                                                return f.Filename.Equals(pattern);
                                                            else if (l >= k)
                                                            {
                                                                string end = f.Filename.Substring(l - k, k);
                                                                return pattern.Equals(end);
                                                            }
                                                            return false;
                                                        });
            return files;
        }
        /// <summary>
        /// Searches for 1 file of a pattern in a MAS archive.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public MAS2File SearchFile(string directory, string pattern)
        {
            List<MAS2File> files = SearchFiles(directory, pattern);
            if (files.Count == 0)
                files = SearchFiles(Path.GetFileName(pattern));
            if (files.Count == 1) return files[0];
            else if (files.Count == 0) throw new Exception("Could not find 1 file");
            else return files[0]; // throw new Exception("Found multiple files");
        }
        /// <summary>
        /// Searches for 1 file of a pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public MAS2File SearchFile(string pattern)
        {
            List<MAS2File> files = SearchFiles(pattern);
            if (files.Count == 0)
                files = SearchFiles(Path.GetFileName(pattern));
            if (files.Count == 1) return files[0];
            else if (files.Count == 0)
                throw new Exception("Could not find 1 file");
            else
                return files[0];
            throw new Exception("Found multiple files");
        }
    }
}