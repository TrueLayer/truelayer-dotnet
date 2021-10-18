using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class UnionFactoryBenchmarks
    {
        private static Func<int, Union<string, int>> TypedFactory = CreateTypedFactory<int, Union<string, int>>();
        private static Delegate DelegateFactory;

        static UnionFactoryBenchmarks()
        {
            if (UnionTypeDescriptor.TryCreate(typeof(Union<string, int>), out var descriptor))
            {
                DelegateFactory = descriptor.TypeFactories[nameof(Int32)].Factory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        [Benchmark(Baseline = true)]
        public Union<string, int> DirectConstructor() => new Union<string, int>(10);

        //[Benchmark]
        public Union<string, int> Reflection()
        {
            return (Union<string, int>)Activator.CreateInstance(typeof(Union<string, int>), 10)!;
        }

        [Benchmark]
        public Union<string, int> TypedExpression() => TypedFactory.Invoke(10);

        [Benchmark]
        public Union<string, int> DynamicInvokeExpression() => (Union<string, int>)DelegateFactory.DynamicInvoke(10)!;

        [Benchmark()]
        public Union<string, int> Current() => ((Func<object, Union<string, int>>)DelegateFactory).Invoke(10);


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
