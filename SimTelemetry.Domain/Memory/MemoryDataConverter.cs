using System;
using System.Collections.Generic;
using System.Text;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryDataConverter
    {
        protected static readonly Dictionary<Type, object> Providers = new Dictionary<Type, object>();

        static MemoryDataConverter()
        {
            Providers.Add(typeof(bool), new MemoryDataConverterProvider<bool>(BitConverter.ToBoolean, Convert.ToBoolean));

            Providers.Add(typeof(byte), new MemoryDataConverterProvider<byte>(ToByte, Convert.ToByte));

            Providers.Add(typeof(char), new MemoryDataConverterProvider<char>(BitConverter.ToChar, Convert.ToChar));

            Providers.Add(typeof(short), new MemoryDataConverterProvider<short>(BitConverter.ToInt16, Convert.ToInt16));
            Providers.Add(typeof(ushort), new MemoryDataConverterProvider<ushort>(BitConverter.ToUInt16, Convert.ToUInt16));

            Providers.Add(typeof(int), new MemoryDataConverterProvider<int>(BitConverter.ToInt32, Convert.ToInt32));
            Providers.Add(typeof(int[]), new MemoryDataConverterProvider<int[]>(ByteToIntArray, ObjToIntArray));
            Providers.Add(typeof(uint), new MemoryDataConverterProvider<uint>(BitConverter.ToUInt32, Convert.ToUInt32));

            Providers.Add(typeof(long), new MemoryDataConverterProvider<long>(BitConverter.ToInt64, Convert.ToInt64));
            Providers.Add(typeof(ulong), new MemoryDataConverterProvider<ulong>(BitConverter.ToUInt64, Convert.ToUInt64));

            Providers.Add(typeof(double), new MemoryDataConverterProvider<double>(BitConverter.ToDouble, Convert.ToDouble));
            Providers.Add(typeof(float), new MemoryDataConverterProvider<float>(BitConverter.ToSingle, Convert.ToSingle));

            Providers.Add(typeof(string), new MemoryDataConverterProvider<string>(BytesToString, Convert.ToString));

        }

        private static int[] ObjToIntArray(object arg)
        {
            if (arg is int[]) return ((int[]) arg);
            else return new int[0];
        }

        private static int[] ByteToIntArray(byte[] arg1, int arg2)
        {
            int inputLength = arg1.Length - arg2;
            var intArray = new int[inputLength/4];

            for (int i = 0; i < intArray.Length; i ++)
                intArray[i] = BitConverter.ToInt32(arg1, arg2+ i*4);

            return intArray;
        }

        public static void AddProvider<T>(MemoryDataConverterProvider<T> provider)
        {
            Type t = typeof (T);
            if (!Providers.ContainsKey(t))
                Providers.Add(t, provider);
        }

        public static void RemoveProvider<T>()
        {
            Type t = typeof (T);
            if (Providers.ContainsKey(t))
                Providers.Remove(t);
        }

        protected static string BytesToString(byte[] datainput, int index)
        {
            int end_index = index;
            while (end_index < datainput.Length)
            {
                if (datainput[end_index] == 0) break;
                end_index++;
            }
            if (end_index == index) return "";
            return Encoding.ASCII.GetString(datainput, index, end_index - index);
        }

        protected static byte ToByte(byte[] datainput, int index)
        {
            return datainput[index];
        }

        public static T Read<T>(byte[] dataInput, int index)
        {
            if (dataInput.Length <= index)
            {
                index = 0;
                dataInput = new byte[128];
            }
            Type inputType = typeof(T);
            return ((MemoryDataConverterProvider<T>)Providers[inputType]).Byte2Obj(dataInput, index);
        }

        public static TOutput Cast<TSource, TOutput>(TSource value)
        {
            Type outputType = typeof(TOutput);
            return ((MemoryDataConverterProvider<TOutput>)Providers[outputType]).Obj2Obj(value);

        }

        public static TOutput Read<TSource, TOutput>(byte[] dataInput, int index)
        {
            Type inputType = typeof (TSource);
            Type outputType = typeof (TOutput);
            if (inputType.Equals(outputType))
                return Read<TOutput>(dataInput, index);

            var intermediate = Read<TSource>(dataInput, index);
            try
            {
                return ((MemoryDataConverterProvider<TOutput>) Providers[outputType]).Obj2Obj(intermediate);
            }
            catch(Exception)
            {
                return (TOutput) Convert.ChangeType(0, typeof(TOutput));
            }
        }

        public static byte[] Rawify(Func<object> reader)
        {
            object data = reader();

            if (data is double) return BitConverter.GetBytes((double)data);
            if (data is float) return BitConverter.GetBytes((float)data);
            if (data is bool) return BitConverter.GetBytes((bool)data);
            if (data is int) return BitConverter.GetBytes((int)data);
            if (data is short) return BitConverter.GetBytes((short)data);
            if (data is long) return BitConverter.GetBytes((long) data);
            if (data is string)
            {
                byte[] rawData = Encoding.ASCII.GetBytes((string)data);
                byte[] outData = new byte[rawData.Length + 4];
                Array.Copy(BitConverter.GetBytes(rawData.Length), 0, outData, 0, 4);
                Array.Copy(rawData, 0, outData, 4, rawData.Length);
                return outData;

            }
            if (data is byte) return BitConverter.GetBytes((byte)data);
            if (data is ushort) return BitConverter.GetBytes((ushort)data);
            if (data is ulong) return BitConverter.GetBytes((ulong)data);
            if (data is uint) return BitConverter.GetBytes((uint)data);

            return new byte[0];
        }
    }
}