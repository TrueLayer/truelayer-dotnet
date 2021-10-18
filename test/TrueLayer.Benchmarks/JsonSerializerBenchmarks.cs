using System.Text.Json;
using BenchmarkDotNet.Attributes;
using TrueLayer.Serialization;

namespace TrueLayer.Benchmarks
{
    [MemoryDiagnoser]
    public class JsonSerializerBenchmarks
    {
        static string json = @"{
            ""type"": ""Bar"",
            ""BarProp"": 10
        }";

        [Benchmark]
        public Bar DeserializeUnion()
            => JsonSerializer.Deserialize<Union<Foo, Bar>>(json, SerializerOptions.Default).AsT1;
        
        public class Foo
        {
            public string? FooProp { get; set; }
        }

        public class Bar
        {
            public int BarProp { get; set; }
        }
    }


}
