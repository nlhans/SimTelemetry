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
            Providers.Add(typeof(uint), new MemoryDataConverterProvider<uint>(BitConverter.ToUInt32, Convert.ToUInt32));

            Providers.Add(typeof(long), new MemoryDataConverterProvider<long>(BitConverter.ToInt64, Convert.ToInt64));
            Providers.Add(typeof(ulong), new MemoryDataConverterProvider<ulong>(BitConverter.ToUInt64, Convert.ToUInt64));

            Providers.Add(typeof(double), new MemoryDataConverterProvider<double>(BitConverter.ToDouble, Convert.ToDouble));
            Providers.Add(typeof(float), new MemoryDataConverterProvider<float>(BitConverter.ToSingle, Convert.ToSingle));

            Providers.Add(typeof(string), new MemoryDataConverterProvider<string>(BytesToString, Convert.ToString));

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
            Type inputType = typeof(TSource);
            Type outputType = typeof(TOutput);
            if (inputType.Equals(outputType))
                return Read<TOutput>(dataInput, index);

            var intermediate = Read<TSource>(dataInput, index);
            return ((MemoryDataConverterProvider<TOutput>)Providers[outputType]).Obj2Obj(intermediate);
        }
    }
}