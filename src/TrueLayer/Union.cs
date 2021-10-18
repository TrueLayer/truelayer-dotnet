using System;
using System.Diagnostics.CodeAnalysis;

namespace TrueLayer
{
    internal interface IUnion
    {
        object Value { get; }
    }

    public struct Union<T0, T1> : IUnion
    {
        private T0? _value0;
        private T1? _value1;
        private int _index;

        public Union(T0 t0)
        {
            _value0 = t0.NotNull(nameof(t0));
            _value1 = default;
            _index = 0;
        }

        public Union(T1 t1)
        {
            _value1 = t1.NotNull(nameof(t1));
            _value0 = default;
            _index = 1;
        }

        public object Value => _index switch
        {
            0 => _value0!,
            1 => _value1!,
            _ => throw new InvalidOperationException()
        };

        public bool IsT0 => _index == 0;
        public bool IsT1 => _index == 1;

        public T0 AsT0 => _index == 0 
                ? _value0!
                : throw new InvalidOperationException($"Cannot return as T0 as result is T{_index}");

        public T1 AsT1 => _index == 1 
                ? _value1!
                : throw new InvalidOperationException($"Cannot return as T1 as result is T{_index}");

        public void Switch(Action<T0> f0, Action<T1> f1)
        {
            if (_index == 0 && f0 != null)
            {
                f0(_value0!);
                return;
            }
            if (_index == 1 && f1 != null)
            {
                f1(_value1!);
                return;
            }
            throw new InvalidOperationException();
        }

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1)
        {
            if (_index == 0 && f0 != null)
            {
                return f0(_value0!);
            }
            if (_index == 1 && f1 != null)
            {
                return f1(_value1!);
            }
            throw new InvalidOperationException();
        }


        public static implicit operator Union<T0, T1>(T0 t0) => new (t0);
        public static implicit operator Union<T0, T1>(T1 t1) => new (t1);
    }
}
