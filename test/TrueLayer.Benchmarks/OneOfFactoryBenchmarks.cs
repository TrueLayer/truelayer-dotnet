using System;
using BenchmarkDotNet.Attributes;
using OneOf;

namespace TrueLayer.Benchmarks;

[MemoryDiagnoser]
public class OneOfFactoryBenchmarks
{
    private static Delegate DelegateFactory;

    static OneOfFactoryBenchmarks()
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
    public OneOf<string, int> Direct() => OneOf<string, int>.FromT1(10);

    [Benchmark]
    public OneOf<string, int> ExpressionFactory() => ((Func<object, OneOf<string, int>>)DelegateFactory).Invoke(10);
}