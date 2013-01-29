using System;

namespace SimTelemetry.Domain.Memory
{
    public class MemoryLazyField<T> : MemoryField<T>
    {
        #region Lazyness
        protected Lazy<T> _LazyValue;
        public override T Value { get { return _LazyValue.Value; } }

        public override void Refresh()
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

        public MemoryLazyField(string name, MemoryAddress type, int address, int size)
            : base(name, type, address, size)
        {
        }

        public MemoryLazyField(string name, MemoryAddress type, int address, int offset, int size)
            : base(name, type, address, offset, size)
        {
        }

        public MemoryLazyField(string name, MemoryAddress type, MemoryPool pool, int offset, int size)
            : base(name, type, pool, offset, size)
        {
        }

        public MemoryLazyField(string name, MemoryAddress type, int address, int size, Func<T, T> conversion)
            : base(name, type, address, size, conversion)
        {
        }

        public MemoryLazyField(string name, MemoryAddress type, int address, int offset, int size, Func<T, T> conversion)
            : base(name, type, address, offset, size, conversion)
        {
        }

        public MemoryLazyField(string name, MemoryAddress type, MemoryPool pool, int offset, int size, Func<T, T> conversion)
            : base(name,  type, pool, offset, size, conversion)
        {
        }

        #endregion
    }
}