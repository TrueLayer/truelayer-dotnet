using System.Text.Json;
using BenchmarkDotNet.Attributes;
using OneOf;
using TrueLayer.Serialization;

namespace TrueLayer.Benchmarks;

/// <summary>
/// Benchmarks the full OneOf deserialization path including:
/// - Converter lookup/caching
/// - Discriminator extraction (streaming vs JsonDocument)
/// - Type factory invocation
/// </summary>
[MemoryDiagnoser]
public class OneOfDeserializationBenchmarks
{
    private static readonly byte[] SimpleJson = """{"type": "Bar", "BarProp": 42}"""u8.ToArray();

    private static readonly byte[] ComplexJson = """
                                                 {
                                                         "type": "sort_code_account_number",
                                                         "sort_code": "12-34-56",
                                                         "account_number": "12345678"
                                                     }
                                                 """u8.ToArray();

    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new OneOfJsonConverterFactory() }
    };

    [Benchmark(Baseline = true)]
    public OneOf<Foo, Bar>? DeserializeSimple()
    {
        return JsonSerializer.Deserialize<OneOf<Foo, Bar>>(SimpleJson, Options);
    }

    [Benchmark]
    public OneOf<Foo, Bar>? DeserializeSimpleWithStatus()
    {
        var json = """{"status": "Bar", "BarProp": 99}"""u8.ToArray();
        return JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, Options);
    }

    [Benchmark]
    public OneOf<AccountIdScan, AccountIdIban>? DeserializeComplex()
    {
        return JsonSerializer.Deserialize<OneOf<AccountIdScan, AccountIdIban>>(ComplexJson, Options);
    }

    [Benchmark]
    public OneOf<Foo, Bar>? DeserializeWithManyFields()
    {
        var json = """
                   {
                               "field1": "value1",
                               "field2": "value2",
                               "field3": 123,
                               "field4": true,
                               "type": "Bar",
                               "BarProp": 42,
                               "field5": "value5",
                               "field6": 456
                           }
                   """u8.ToArray();
        return JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, Options);
    }

    public class Foo
    {
        public string? FooProp { get; set; }
    }

    public class Bar
    {
        public int BarProp { get; set; }
    }

    [JsonDiscriminator("sort_code_account_number")]
    public class AccountIdScan
    {
        public string Type => "sort_code_account_number";
        public string SortCode { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
    }

    [JsonDiscriminator("iban")]
    public class AccountIdIban
    {
        public string Type => "iban";
        public string Iban { get; set; } = null!;
    }
}
