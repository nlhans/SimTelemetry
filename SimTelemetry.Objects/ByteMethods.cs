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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimTelemetry.Objects
{
    public class ByteMethods
    {

        public static object DeserializeFromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            var mstream = new MemoryStream();
            mstream.Write(bytes, 0, bytes.Length);
            mstream.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(mstream);
        }

        public static byte[] SerializeToBytes(object obj)
        {
            var mstream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(mstream, obj);
            byte[] d = new byte[mstream.Position];
            mstream.Seek(0, SeekOrigin.Begin);
            mstream.Read(d, 0, d.Length);
            return d;
        }

        /// <summary>
        /// Deprecate this.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <param name="dst_offset"></param>
        /// <param name="src_offset"></param>
        public static void memcpy(byte[] destination, byte[] source, int length, int dst_offset, int src_offset)
        {
            for (int i = 0; i < length; i++)
            {
                if (destination.Length < dst_offset + i + 1) break;
                if (source.Length < src_offset + i + 1) break;
                destination[dst_offset + i] = source[src_offset + i];
            }
        }

        private static void ToObject(byte[] bytearray, ref object obj)
        {

            int len = Marshal.SizeOf(obj);

            IntPtr i = Marshal.AllocHGlobal(len);

            Marshal.Copy(bytearray, 0, i, len);

            obj = Marshal.PtrToStructure(i, obj.GetType());

            Marshal.FreeHGlobal(i);

        }



        public static byte[] ToBytes(object obj)
        {

            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;

        }

        public static T ToObject<T>(byte[] data)
        {
            T item = (T)Activator.CreateInstance(typeof(T));
            object o = (object)item;

            ByteMethods.ToObject(data, ref o);
            return (T)o;
        }
    }

}
