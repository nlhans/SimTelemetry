using System;
using System.Collections.Generic;
using System.Linq;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryPool : IMemoryObject
    {
        public byte[] Value;

        public string Name { get; protected set; }
        public MemoryProvider Memory { get; set; }
        public MemoryAddress AddressType { get; protected set; }

        public bool IsDynamic { get { return (AddressType == MemoryAddress.DYNAMIC); } }
        public bool IsStatic { get { return (AddressType == MemoryAddress.STATIC || AddressType == MemoryAddress.STATIC_ABSOLUTE); } }

        public MemoryPool Pool { get; protected set; }
        public int Offset { get; protected set; }
        public int Address { get; protected set; }
        public int Size { get; protected set; }


        public IEnumerable<IMemoryObject> Fields { get { return _fields; } }
        public IEnumerable<MemoryPool> Pools { get { return _pools; } }

        private readonly IList<IMemoryObject> _fields = new List<IMemoryObject>();
        private readonly IList<MemoryPool> _pools = new List<MemoryPool>();


        public TOut ReadAs<TOut>()
        {
            throw new NotImplementedException();
        }

        public TOut ReadAs<TOut>(int offset)
        {
            return MemoryDataConverter.Read<TOut>(Value, offset);
        }

        public TOut ReadAs<TSource, TOut>(int offset)
        {
            return MemoryDataConverter.Read<TSource, TOut>(Value, offset);
        }

        public TOut ReadAs<TOut>(string field)
        {
            if (Fields.Any(x => x.Name == field))
                return Fields.Where(x => x.Name == field).FirstOrDefault().ReadAs<TOut>();
            else
                throw new NotImplementedException();
        }
        public void Refresh()
        {
            var computedAddress = 0;
            // Refresh this memory block.
            if (IsStatic)
            {
                if (Address != 0 && Offset != 0)
                {
                    computedAddress = Memory.Reader.ReadInt32(Memory.BaseAddress + Address) + Offset;
                }
                else
                {
                    computedAddress = AddressType == MemoryAddress.STATIC
                                          ? Memory.BaseAddress + Address
                                          : Address;
                }
            }
            else
            {
                computedAddress = MemoryDataConverter.Read<int>(Pool.Value, Offset);
            }

            Value = Memory.Reader.ReadBytes(computedAddress, (uint) Size);

            // Refresh underlying fields.
            foreach (var field in Fields)
                field.Refresh();
            foreach (var pool in Pools)
                pool.Refresh();

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
        }

        public void Add<T>(T obj) where T : IMemoryObject
        {
            if (typeof(T).Name == "MemoryPool") throw new Exception();
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

        public MemoryPool(string name,  MemoryAddress type, int address, int size)
        {
            Name = name;
            Address = address;
            Offset = 0;
            Size = size;
            AddressType = type;
        }

        public MemoryPool(string name,  MemoryAddress type, int address, int offset, int size)
        {
            Name = name;
            Address = address;
            Offset = offset;
            Size = size;
            AddressType = type;
        }

        public MemoryPool(string name,  MemoryAddress type, MemoryPool pool, int offset, int size)
        {
            Name = name;
            Pool = pool;
            Offset = offset;
            Size = size;
            AddressType = type;
        }

        protected byte[] ReadObject(byte[] dataIn, int index)
        {
            var dataOut = new byte[Size];
            Array.Copy(dataIn, index, dataOut, 0, dataOut.Length);
            return dataOut;
        }

    }
}