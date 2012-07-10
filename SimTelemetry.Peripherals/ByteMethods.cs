using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SimTelemetry.Peripherals
{
    public class ByteMethods
    {
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
