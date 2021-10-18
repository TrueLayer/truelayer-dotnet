using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class DynamicInvokeBenchmarks
    {
        private static Func<int, Union<string, int>> TypedFactory = UnionFactory.CreateTypedFactory<int, Union<string, int>>();
        private static Delegate DelegateFactory = UnionFactory.CreateDelegateFactory(typeof(int), typeof(Union<string, int>));

        private static Delegate DelegateObjectFactory = UnionFactory.CreateDelegateFactoryObject(typeof(int), typeof(Union<string, int>));

        
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

        [Benchmark]
        public Union<string, int> ObjectHandlingExpression() => ((Func<object, Union<string, int>>)DelegateObjectFactory).Invoke(10);

    }

    public static class UnionFactory
    {
        public static Func<TArg, T> CreateTypedFactory<TArg, T>()
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(TArg) });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var parameter = Expression.Parameter(typeof(TArg));
            var factoryExpression = Expression.Lambda<Func<TArg, T>>(
                Expression.New(constructor, new[] { parameter}), parameter);

            return factoryExpression.Compile();
        }


        public static Delegate CreateDelegateFactory(Type argType, Type unionType)
        {
            var constructor = unionType.GetConstructor(new[] { argType });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var parameter = Expression.Parameter(argType);
            var factoryType = typeof(Func<,>).MakeGenericType(argType, unionType);       

            var factoryExpression = Expression.Lambda(
                factoryType,
                Expression.New(constructor, new[] { parameter}), parameter);

            return factoryExpression.Compile();
        }

        public static Delegate CreateDelegateFactoryObject(Type argType, Type unionType)
        {
            var constructor = unionType.GetConstructor(new[] { argType });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var input = Expression.Parameter(typeof(object));
            
            // Parameters will be passed as object and need to be converted to the appropriate type  
            var convertedParameter = Expression.Convert(input, argType);

            var factoryType = typeof(Func<,>).MakeGenericType(typeof(object), unionType);

            var factoryExpression = Expression.Lambda(
                factoryType,
                Expression.New(constructor, new[] { convertedParameter }), input);

            return factoryExpression.Compile();
        }
    }

    public struct Union<T0, T1>
    {
        private T0? _value0;
        private T1? _value1;
        private int _index;

        public Union(T0 t0)
        {
            _value0 = t0;
            _value1 = default;
            _index = 0;
        }

        public Union(T1 t1)
        {
            _value1 = t1;
            _value0 = default;
            _index = 1;
        }

        public object Value => _index switch
        {
            0 => _value0!,
            1 => _value1!,
            _ => throw new InvalidOperationException()
        };
    }
}
