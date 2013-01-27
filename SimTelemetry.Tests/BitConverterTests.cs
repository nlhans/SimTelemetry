using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using SimTelemetry.Objects;
using Triton.Memory;

namespace SimTelemetry.Tests
{
    [TestFixture]
    public class BitConverterTests
    {
        [Test]
        public void TestBitconverterSpeed()
        {
            int samples = 10000000;
            int sampleSize = 8;
            double[] d = new double[samples];
            byte[] data = new byte[samples * sampleSize];
            for (int i = 0; i < samples; i++)
            {
                d[i] = i * i * 1.0;
                var bytes = BitConverter.GetBytes(d[i]);
                Array.Copy(bytes, 0, data, i * sampleSize, sampleSize);
            }

            Stopwatch w = new Stopwatch();
            w.Start();

            int xd = 0;

            for (int i = 0; i < samples; i++)
            {
                double v = d[i];
                double a = d[i];
                if (a == v) xd++;
                //Assert.AreEqual(v, a);
            }
            Assert.AreEqual(xd, samples);
            w.Stop();
            Debug.WriteLine("Baseline took " + w.ElapsedMilliseconds + "ms -> " + (w.ElapsedMilliseconds / sampleSize) + "ns per 1 call");
            w.Reset();
            w.Start();

            xd = 0;

            // Bitconverter
            for (int i = 0; i < samples; i++)
            {
                double v = BitConverter.ToDouble(data, i * sampleSize);
                double a = d[i];
                if (a == v) xd++;
                //Assert.AreEqual(v, a);
            }
            Assert.AreEqual(xd, samples);
            w.Stop();
            Debug.WriteLine("10M double bitconverters took " + w.ElapsedMilliseconds + "ms -> " + (w.ElapsedMilliseconds / sampleSize) + "ns per 1 call");

        }

        [Test]
        public void MemReaderTest()
        {

            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("flux")[0];

            Stopwatch w = new Stopwatch();
            w.Start();
            // Read some data from player.
            for (int i = 0; i < 10000; i++)
            {
                int[] ints = new int[256];
                for (int j = 0; j < 256; j++)
                    ints[j] = r.ReadInt32(new IntPtr(0x7154C0 + j * 4));
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 256 reads took " + w.ElapsedMilliseconds);
            w.Reset();


            w.Start();
            // Read some data from player.
            for (int i = 0; i < 10000; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                float[] ints = new float[256];
                for (int j = 0; j < 256; j++)
                    ints[j] = BitConverter.ToSingle(data, j * 4);
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10KB reads took " + w.ElapsedMilliseconds);
            w.Reset();


        }

        [Test]
        public void DataConversionTest()
        {
            var dataarr = new byte[1024];

            Array.Copy(BitConverter.GetBytes(1337.0f), 0, dataarr, 16, 4);
            Array.Copy(BitConverter.GetBytes(1337 * 2.0), 0, dataarr, 32, 8);

            float f1 = MemoryDataConverter.Read<float>(dataarr, 16);
            float f2 = MemoryDataConverter.Read<double, float>(dataarr, 32);

            double d1 = MemoryDataConverter.Read<double>(dataarr, 32);
            double d2 = MemoryDataConverter.Read<float, double>(dataarr, 16);

            Assert.AreEqual(1337.0f, f1);
            Assert.AreEqual(1337.0f * 2.0f, f2);

            Assert.AreEqual(1337 * 2.0, d1);
            Assert.AreEqual(1337.0, d2);

            // Speed tests.
            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("rfactor")[0];

            int[] refints = new int[256];
            Stopwatch w = new Stopwatch();
            w.Start();
            // Read some data from player.
            for (int i = 0; i < 250000 / 256; i++)
            {
                for (int j = 0; j < 256; j++)
                    refints[j] = r.ReadInt32(new IntPtr(0x7154C0 + j * 4));
            }
            w.Stop();
            Debug.WriteLine("10K iterations of 256 reads took " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

            w.Start();
            for (int i = 0; i < 250000 / 256; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                long[] ints = new long[256];
                for (int j = 0; j < 256; j++)
                {
                    ints[j] = MemoryDataConverter.Read<int, long>(data, j * 4);
                    if (ints[j] != refints[j])
                        Assert.Fail();
                }

            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10kB reads +256 conversions with MemoryDataConverter took  " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

            w.Start();
            for (int i = 0; i < 250000 / 256; i++)
            {
                byte[] data = r.ReadBytes(new IntPtr(0x7154C0), 0x4000);
                int[] ints = new int[256];
                for (int j = 0; j < 256; j++)
                {
                    ints[j] = BitConverter.ToInt32(data, j * 4);
                    if (ints[j] != refints[j])
                        Assert.Fail();
                }

            }
            w.Stop();
            Debug.WriteLine("10K iterations of 10kB reads +256 conversions with BitConverter took " + w.ElapsedMilliseconds + "(" + Math.Round(1.0 / w.ElapsedMilliseconds * 10000 * 256, 1) + "k IOPS)");
            w.Reset();

        }

        [Test]
        public void TestRfactor()
        {
            MemoryReader r = new MemoryReader();
            r.ReadProcess = Process.GetProcessesByName("rfactor")[0];
            r.Diagnostic = true;
            r.Open();

            // The MemoryProvider object is filled/generated inside a plugin.
            MemoryProvider provider = new MemoryProvider(r);
            MemoryPool Driver = new MemoryPool("Driver", 0x3154C0, 0x5F48); // base, 0x5F48 size
            Driver.Add(new MemoryField<float>("Speed", 0, 0x57C0, (x) => x*3.6f));
            Driver.Add(new MemoryField<float>("RPM", 0, 0xA4, Rotations.Rads_RPM));
            Driver.Add(new MemoryField<byte>("Gear", 0, 0x321C));
            provider.Add(Driver);

            // From here 'application'.
            provider.Refresh();

            MemoryPool drv1 = provider.Pools.FirstOrDefault();

            Debug.WriteLine(drv1.ReadAs<double>("RPM"));
            Debug.WriteLine(drv1.ReadAs<double>("Speed"));
            Debug.WriteLine(drv1.ReadAs<int>("Gear"));
            Thread.Sleep(1000);
            Debug.WriteLine("ReadMemory calls: " + r.ReadCalls);

            // Speed comparisons.
            Stopwatch w = new Stopwatch();
            w.Reset();
            int[] data = new int[300000];
            w.Start();
            for (int i = 0; i < 300000; i++)
            {
                data[i] = r.ReadInt32(0x7154C0 + i * 4);
            }
            w.Stop();
            Thread.Sleep(1000);
            Debug.WriteLine("ReadMemory calls: " + r.ReadCalls + " / " + w.ElapsedMilliseconds + "ms");

            w.Reset();
            w.Start();
            for (int i = 0; i < 4000; i++)
            {
                r.ReadBytes(0x7154C0, 0x5F48);
            }
            w.Stop();
            Thread.Sleep(1000);
            Debug.WriteLine("ReadMemory calls: " + r.ReadCalls + " / " + w.ElapsedMilliseconds + "ms");

        }
    }

    public interface IMemoryObject
    {
        string Name { get; }
        MemoryProvider Memory { get; }

        bool IsDynamic { get; }
        bool IsStatic { get; }

        MemoryPool Pool { get; }
        int Offset { get; }
        int Address { get; }
        int Size { get; }

        TOut ReadAs<TOut>();

        void Refresh();
        void SetProvider(MemoryProvider provider);
        void SetPool(MemoryPool pool);
    }

    public class MemoryDataConverterProvider<T>
    {
        public Type DataType { get { return typeof(T); } }
        public Func<byte[], int, T> Byte2Obj { get; private set; }
        public Func<object, T> Obj2Obj { get; private set; }

        public MemoryDataConverterProvider(Func<byte[], int, T> byte2obj, Func<object, T> obj2obj)
        {
            Byte2Obj = byte2obj;
            Obj2Obj = obj2obj;
        }
    }

    public class MemoryDataConverter
    {
        protected static readonly Dictionary<Type, object> Providers = new Dictionary<Type, object>();

        static MemoryDataConverter()
        {
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

        protected static string BytesToString(byte[] datainput, int index)
        {
            return Encoding.ASCII.GetString(datainput, index, datainput.Length - index);
        }

        protected static byte ToByte(byte[] datainput, int index)
        {
            return datainput[index];
        }

        public static T Read<T>(byte[] dataInput, int index)
        {
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

    public class MemoryField<T> : IMemoryObject
    {
        public string Name { get; protected set; }
        public MemoryProvider Memory { get; set; }

        public bool IsDynamic { get { return (Offset != 0); } }
        public bool IsStatic { get { return (Offset == 0); } }

        public MemoryPool Pool { get; protected set; }
        public int Offset { get; protected set; }
        public int Address { get; protected set; }
        public int Size { get; protected set; }

        public Type FieldType { get; protected set; }

        public Func<T, T> Conversion { get; protected set; }
        public virtual T Value { get; protected set; }

        public virtual T ReadAs()
        {
            return Value;
        }

        public virtual TOut ReadAs<TOut>()
        {
            return MemoryDataConverter.Cast<T, TOut>(Value);
        }

        public virtual void Refresh()
        {
            Value = Pool != null ? RefreshStatic() : RefreshDynamic();
            if (Conversion != null)
                Value = Conversion(Value);
        }

        public void SetProvider(MemoryProvider provider)
        {
            if (Memory != null) throw new Exception("Can only set 1 memory provider");
            Memory = provider;
        }

        public void SetPool(MemoryPool pool)
        {
            if (Pool != null) throw new Exception("Can only set 1 pool");
            Pool = pool;
            Refresh();
        }

        private T RefreshStatic()
        {
            if (Pool.Value == null)
                return MemoryDataConverter.Read<T>(new byte[((Size == 0) ? 16 : Size)], 0);
            return MemoryDataConverter.Read<T>(Pool.Value, Offset);
        }

        private T RefreshDynamic()
        {
            if (Memory == null)
                return MemoryDataConverter.Read<T>(new byte[((Size == 0)?16:Size)], 0);

            var computedAddress = Address != 0 && Offset != 0
                                      ? Memory.Reader.ReadInt32(Memory.BaseAddress + Address) + Offset
                                      : Memory.BaseAddress + Address;
            var data = Memory.Reader.ReadBytes(computedAddress, (uint) Size);
            return MemoryDataConverter.Read<T>(data, 0);
        }

        public MemoryField(string name, int address)
        {
            Name = name;
            FieldType = typeof(T);
            Address = address;
            Offset = 0;
            Refresh();
        }

        public MemoryField(string name, int address, Func<T,T>conversion)
        {
            Name = name;
            FieldType = typeof(T);
            Address = address;
            Offset = 0;
            Conversion = conversion;
            Refresh();
        }

        public MemoryField(string name, int address, int offset)
        {
            Name = name;
            FieldType = typeof(T);
            Address = address;
            Offset = offset;
            Refresh();
        }

        public MemoryField(string name, int address, int offset, Func<T, T> conversion)
        {
            Name = name;
            FieldType = typeof(T);
            Address = address;
            Offset = offset;
            Conversion = conversion;
            Refresh();
        }

        public MemoryField(string name, MemoryPool pool, int offset)
        {
            Name = name;
            FieldType = typeof(T);
            Pool = pool;
            Offset = offset;
            Refresh();
        }

        public MemoryField(string name, MemoryPool pool, int offset, Func<T, T> conversion)
        {
            Name = name;
            FieldType = typeof(T);
            Pool = pool;
            Offset = offset;
            Conversion = conversion;
            Refresh();
        }
    }

    public class MemoryPool : IMemoryObject
    {
        public byte[] Value;

        public string Name { get; protected set; }
        public MemoryProvider Memory { get; set; }

        public bool IsDynamic { get { return (Offset != 0); } }
        public bool IsStatic { get { return (Offset == 0); } }

        public MemoryPool Pool { get; protected set; }
        public int Offset { get; protected set; }
        public int Address { get; protected set; }
        public int Size { get; protected set; }


        public IEnumerable<IMemoryObject> Fields { get { return _fields; } }
        public IEnumerable<MemoryPool> Pools { get { return _pools; } }

        private IList<IMemoryObject> _fields = new List<IMemoryObject>();
        private IList<MemoryPool> _pools = new List<MemoryPool>();


        public TOut ReadAs<TOut>()
        {
            throw new NotImplementedException();
        }

        public TOut ReadAs<TOut>(string field)
        {
            return Fields.Where(x => x.Name == field).FirstOrDefault().ReadAs<TOut>();
        }
        public void Refresh()
        {
            // Refresh this memory block.
            if (Pool != null)
            {
                Value = Memory.Reader.ReadBytes(Address, (uint) Size);
            }
            else
            {
                var computedAddress = Address != 0 && Offset != 0
                                          ? Memory.Reader.ReadInt32(Memory.BaseAddress + Address) + Offset
                                          : Memory.BaseAddress + Address;
                Value = Memory.Reader.ReadBytes(computedAddress, (uint)Size);
            }

            // Refresh underlying fields.
            foreach (var field in Fields) field.Refresh();
            foreach (var pool in Pools) pool.Refresh();
        }

        public void SetProvider(MemoryProvider provider)
        {
            if (Memory != null) throw new Exception("Can only set 1 memory provider");
            Memory = provider;
            foreach (var field in _fields) field.SetProvider(provider);
            foreach (var pool in _pools) pool.SetProvider(provider);
        }

        public void SetPool(MemoryPool pool)
        {
            if (Pool != null) throw new Exception("Can only set 1 pool");
            Pool = pool;
            Refresh();
        }

        public void Add<T>(T obj) where T : IMemoryObject
        {
            if(typeof(T).Name == "MemoryPool") throw new Exception();
            _fields.Add(obj);

            obj.SetPool(this);
            if (Memory != null) obj.SetProvider(Memory);
        }

        public void Add(MemoryPool obj) 
        {
            _pools.Add(obj);

            obj.SetPool(this);
            if (Memory != null) obj.SetProvider(Memory);
        }

        public MemoryPool(string name, int address, int size)
        {
            Name = name;
            Address = address;
            Offset = 0;
            Size = size;
        }

        public MemoryPool(string name, int address, int offset, int size) 
        {
            Name = name;
            Address = address;
            Offset = offset;
            Size = size;
        }

        public MemoryPool(string name, MemoryPool pool, int offset, int size)
        {
            Name = name;
            Pool = pool;
            Offset = offset;
            Size = size;
        }

        protected byte[] ReadObject(byte[] dataIn, int index)
        {
            var dataOut = new byte[Size];
            Array.Copy(dataIn, index, dataOut,0, dataOut.Length);
            return dataOut;
        }

    }

    public class MemoryProvider
    {
        public MemoryProvider(MemoryReader reader)
        {
            BaseAddress = reader.ReadProcess.MainModule.BaseAddress.ToInt32();
            Reader = reader;
        }

        public int BaseAddress { get; protected set; }
        public MemoryReader Reader { get; protected set; }

        public IEnumerable<MemoryPool> Pools{get { return _pools; }}
        private readonly IList<MemoryPool> _pools = new List<MemoryPool> ();

        public void Add(MemoryPool pool)
        {
            _pools.Add(pool);
            pool.SetProvider(this);
        }
        public void Remove(MemoryPool pool)
        {
            _pools.Remove(pool);
        }

        public void Refresh()
        {
            foreach(var pool in _pools) pool.Refresh();
        }
    }
}
