using System;
using System.Diagnostics;

namespace TrueLayer
{
    internal interface IUnion
    {
        object Value { get; }
    }

    public struct Union<T0> : IUnion
    {
        private readonly T0 _value0;
        private int _index;

        internal Union(T0 t0)
        {
            _value0 = t0.NotNull(nameof(t0));
            _index = 0;
        }

        public object Value => _value0!;

        public bool IsT0 => _index == 0;

        public T0 AsT0 => _index == 0
                ? _value0
                : throw new InvalidOperationException($"Cannot return as T0 as result is T{_index}");

        [DebuggerStepThrough]
        public void Switch(Action<T0> f0)
        {
            if (IsT0 && f0 != null)
            {
                f0(_value0);
                return;
            }

            throw new InvalidOperationException();
        }

        [DebuggerStepThrough]
        public TResult Match<TResult>(Func<T0, TResult> f0)
        {
            if (IsT0 && f0 != null)
            {
                return f0(_value0);
            }

            throw new InvalidOperationException();
        }

        [DebuggerStepThrough]
        public static implicit operator Union<T0>(T0 t0) => new(t0);
    }

    public struct Union<T0, T1> : IUnion
    {
        private readonly T0? _value0;
        private readonly T1? _value1;

        private int _index;

        internal Union(T0 t0)
        {
            _value0 = t0.NotNull(nameof(t0));
            _value1 = default;
            _index = 0;
        }

        internal Union(T1 t1)
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


        [DebuggerStepThrough]
        public void Switch(Action<T0> f0, Action<T1> f1)
        {
            if (IsT0 && f0 != null)
            {
                f0(_value0!);
                return;
            }
            if (IsT1 && f1 != null)
            {
                f1(_value1!);
                return;
            }
            throw new InvalidOperationException();
        }

        [DebuggerStepThrough]
        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1)
        {
            if (IsT0 && f0 != null)
            {
                return f0(_value0!);
            }
            if (IsT1 && f1 != null)
            {
                return f1(_value1!);
            }
            throw new InvalidOperationException();
        }


        [DebuggerStepThrough]
        public static implicit operator Union<T0, T1>(T0 t0) => new(t0);

        [DebuggerStepThrough]
        public static implicit operator Union<T0, T1>(T1 t1) => new(t1);
    }
}
