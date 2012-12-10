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
            if (Directory.Exists(directory))
            foreach (string file in Directory.GetFiles(directory, Path.GetFileName(pattern), SearchOption.AllDirectories))
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
            else if (files.Count == 0) throw new Exception("Could not find 1 file");
            else return files[0]; // throw new Exception("Found multiple files");
        }
    }
}
