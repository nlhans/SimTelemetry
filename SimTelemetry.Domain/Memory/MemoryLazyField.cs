using System;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryLazyField<T> : MemoryField<T>
    {
        #region Lazyness
        protected Lazy<T> _LazyValue;
        public override T Value { get { return _LazyValue.Value; } }

        public override void Refresh(MemoryRefreshLevel level)
        {
            if (_LazyValue == null || _LazyValue.IsValueCreated)
            {
                _LazyValue = new Lazy<T>(() =>
                                             {
                                                 if (IsStatic)
                                                     RefreshStatic();
                                                 else
                                                     RefreshDynamic();

                                                 if (_Value != null && Conversion != null)
                                                     _Value = Conversion(_Value);
                                                 return _Value;
                                             });
            }
        }
        #endregion
        #region Constructors

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, int address, int size)
            : base(name, level, type, address, size)
        {
        }

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, int address, int offset, int size)
            : base(name, level, type, address, offset, size)
        {
        }

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, MemoryPool pool, int offset, int size)
            : base(name, level, type, pool, offset, size)
        {
        }

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, int address, int size, Func<T, T> conversion)
            : base(name, level, type, address, size, conversion)
        {
        }

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, int address, int offset, int size, Func<T, T> conversion)
            : base(name, level, type, address, offset, size, conversion)
        {
        }

        public MemoryLazyField(string name, MemoryRefreshLevel level, MemoryAddressType type, MemoryPool pool, int offset, int size, Func<T, T> conversion)
            : base(name, level, type, pool, offset, size, conversion)
        {
        }

        #endregion
    }
}