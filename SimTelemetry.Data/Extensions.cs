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
using System.Text;

namespace SimTelemetry.Data
{
    /// <summary>
    /// Multiple extensions used around the program.
    /// </summary>
    public static partial class Extensions
    {

        public static void WriteString(this FileStream fs, string f)
        {
            byte[] b = ASCIIEncoding.ASCII.GetBytes(f);

            fs.Write(b, 0, b.Length);

        }
        public static void WriteLine(this FileStream fs, string f)
        {
            try
            {
                byte[] b = ASCIIEncoding.ASCII.GetBytes(f + "\r\n");

                fs.Write(b, 0, b.Length);
            }catch(Exception e)
            {
                //throw e;
            }
        }

        public static string ReadLine(this FileStream fs)
        {
            List<byte> b = new List<byte>();

            while(b.Count == 0 ||  b[b.Count-1] != 10)
            {
                if (fs.Position == fs.Length) break;
                b.Add((byte)fs.ReadByte());
            }

            return ASCIIEncoding.ASCII.GetString(b.ToArray());
        }
    }
}