using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryPool : IMemoryObject
    {
        public string Name { get; protected set; }
        public MemoryProvider Memory { get; set; }
        public MemoryAddress AddressType { get; protected set; }

        public bool IsDynamic { get { return (AddressType == MemoryAddress.Dynamic); } }
        public bool IsStatic { get { return (AddressType == MemoryAddress.Static || AddressType == MemoryAddress.StaticAbsolute); } }
        public bool IsConstant { get { return false; } }

        public MemoryPool Pool { get; protected set; }
        public int Offset { get; protected set; }
        public int Address { get; protected set; }
        public int Size { get; protected set; }
        public string Signature { get; protected set; }

        public byte[] Value { get; protected set; }

        public Dictionary<string, IMemoryObject> Fields { get { return _fields; } }
        public Dictionary<string, MemoryPool> Pools { get { return _pools; } }

        private readonly Dictionary<string, IMemoryObject> _fields = new Dictionary<string, IMemoryObject>();
        private readonly Dictionary<string, MemoryPool> _pools = new Dictionary<string, MemoryPool>();

        public IEnumerable<MemoryFieldSignaturePointer> Pointers { get; protected set; }



        public TOut ReadAs<TOut>()
        {
            return MemoryDataConverter.Read<TOut>(new byte[32], 0);
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
            if (Fields.ContainsKey(field))
                return Fields[field].ReadAs<TOut>();
            else
                return MemoryDataConverter.Read<TOut>(new byte[32], 0);
        }
        public void Refresh()
        {
            var computedAddress = 0;

            if (Signature != string.Empty && Offset == 0 && Address == 0 && Memory.Scanner.Enabled)
            {
                var result = Memory.Scanner.Scan<uint>(MemoryRegionType.EXECUTE, Signature);

                // Search the address and offset.
                switch (AddressType)
                {
                    case MemoryAddress.StaticAbsolute:
                    case MemoryAddress.Static:
                        if (result == 0) return;

                        if (Pointers.Count() == 0)
                        {
                            // The result is directly our address
                            Address = (int)result;
                        }
                        else
                        {
                            // We must follow one pointer.
                            if (AddressType == MemoryAddress.Static)
                            {
                                computedAddress = Memory.BaseAddress + (int)result;
                            }
                            else
                            {
                                computedAddress = (int)result;
                            }

                            foreach (var ptr in Pointers)
                            {
                                computedAddress = Memory.Reader.ReadInt32(computedAddress + ptr.Offset);
                            }

                            Address = computedAddress;
                        }
                        break;
                    case MemoryAddress.Dynamic:
                        Offset = (int)result;
                        break;
                    default:
                        throw new Exception("AddressType for '" + Name + "' is not valid");
                        break;
                }

            }

            // Refresh this memory block.
            if (Size > 0)
            {
                if (IsStatic)
                {
                    if (Address != 0 && Offset != 0)
                    {
                        computedAddress = Memory.Reader.ReadInt32(Memory.BaseAddress + Address) + Offset;
                    }
                    else
                    {
                        computedAddress = AddressType == MemoryAddress.Static
                                              ? Memory.BaseAddress + Address
                                              : Address;
                    }
                }
                else
                {
                    computedAddress = MemoryDataConverter.Read<int>(Pool.Value, Offset);
                }

                // Read into this buffer.
                Memory.Reader.Read(computedAddress, Value);
            }

            // Refresh underlying fields.
            foreach (var field in Fields)
                field.Value.Refresh();

            foreach (var pool in Pools.Values)
                pool.Refresh();

        }

        public void SetProvider(MemoryProvider provider)
        {
            if (Memory != null) throw new Exception("Can only set 1 memory provider");
            Memory = provider;
            foreach (var field in _fields) field.Value.SetProvider(provider);
            foreach (var pool in _pools) pool.Value.SetProvider(provider);
        }

        public void SetPool(MemoryPool pool)
        {
            if (Pool == pool) return;
            if (Pool != null) throw new Exception("Can only set 1 pool");
            Pool = pool;
        }

        public void Add<T>(T obj) where T : IMemoryObject
        {
            if (typeof(T).Name.Contains("MemoryPool")) throw new Exception();
            if (!_fields.ContainsKey(obj.Name))
            {
                _fields.Add(obj.Name, obj);

                obj.SetPool(this);
                if (Memory != null) obj.SetProvider(Memory);
            }
        }

        public void Add(MemoryPool obj)
        {
            if (!_pools.ContainsKey(obj.Name))
            {
                _pools.Add(obj.Name, obj);

                obj.SetPool(this);
                if (Memory != null) obj.SetProvider(Memory);
            }
        }

        public void ClearPools()
        {
            _pools.Clear();
        }

        public MemoryPool(string name, MemoryAddress type, string signature, int size)
        {
            Name = name;
            Address = 0;
            Offset = 0;
            Size = size;
            AddressType = type;
            Signature = signature;
            Pointers = new List<MemoryFieldSignaturePointer>();

            Value = new byte[size];
        }


        public MemoryPool(string name, MemoryAddress type, string signature, IEnumerable<int> pointers, int size)
        {
            Name = name;
            Address = 0;
            Offset = 0;
            Size = size;
            AddressType = type;
            Signature = signature;
            Pointers = pointers.Select(pointer => new MemoryFieldSignaturePointer(pointer)).ToList();

            Value = new byte[size];
        }

        public MemoryPool(string name,  MemoryAddress type, int address, int size)
        {
            Name = name;
            Address = address;
            Offset = 0;
            Size = size;
            AddressType = type;
            Signature = string.Empty;

            Value = new byte[Size];
        }

        public MemoryPool(string name,  MemoryAddress type, int address, int offset, int size)
        {
            Name = name;
            Address = address;
            Offset = offset;
            Size = size;
            AddressType = type;
            Signature = string.Empty;

            Value = new byte[Size];
        }

        public MemoryPool(string name,  MemoryAddress type, MemoryPool pool, int offset, int size)
        {
            Name = name;
            Pool = pool;
            Offset = offset;
            Size = size;
            AddressType = type;
            Signature = string.Empty;

            Value = new byte[Size];
        }

    }
}