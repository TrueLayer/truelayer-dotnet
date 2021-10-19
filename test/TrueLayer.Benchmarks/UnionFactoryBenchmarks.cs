using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using OneOf;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class UnionFactoryBenchmarks
    {
        private static Func<int, OneOf<string, int>> TypedFactory = CreateTypedFactory<int, OneOf<string, int>>();
        private static Delegate DelegateFactory;

        static UnionFactoryBenchmarks()
        {
            if (OneOfTypeDescriptor.TryCreate(typeof(OneOf<string, int>), out var descriptor))
            {
                DelegateFactory = descriptor.TypeFactories[nameof(Int32)].Factory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        [Benchmark(Baseline = true)]
        public OneOf<string, int> DirectConstructor() => OneOf<string, int>.FromT1(10);

        //[Benchmark]
        public OneOf<string, int> Reflection()
        {
            return (OneOf<string, int>)Activator.CreateInstance(typeof(OneOf<string, int>), 10)!;
        }

        [Benchmark]
        public OneOf<string, int> TypedExpression() => TypedFactory.Invoke(10);

        [Benchmark]
        public OneOf<string, int> DynamicInvokeExpression() => (OneOf<string, int>)DelegateFactory.DynamicInvoke(10)!;

        [Benchmark()]
        public OneOf<string, int> Current() => ((Func<object, OneOf<string, int>>)DelegateFactory).Invoke(10);


        public static Func<TArg, T> CreateTypedFactory<TArg, T>()
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(TArg) });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var parameter = Expression.Parameter(typeof(TArg));
            var factoryExpression = Expression.Lambda<Func<TArg, T>>(
                Expression.New(constructor, new[] { parameter }), parameter);

            return factoryExpression.Compile();
        }
    }
}
