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

    [Benchmark]
    public OneOf<Foo, Bar>? DeserializeWithNestedObjects()
    {
        var json = """
                   {
                               "type": "Bar",
                               "status": "active",
                               "BarProp": 42,
                               "metadata": {
                                   "nested1": "value1",
                                   "nested2": {
                                       "deep1": "value2",
                                       "deep2": [1, 2, 3, 4, 5]
                                   }
                               },
                               "config": {
                                   "enabled": true,
                                   "settings": {
                                       "timeout": 30000,
                                       "retries": 3
                                   }
                               }
                           }
                   """u8.ToArray();
        return JsonSerializer.Deserialize<OneOf<Foo, Bar>>(json, Options);
    }

    [Benchmark]
    public OneOf<Foo, Bar>? DeserializeWithLargeNestedArrays()
    {
        var json = """
                   {
                               "type": "Bar",
                               "BarProp": 42,
                               "tags": ["tag1", "tag2", "tag3", "tag4", "tag5", "tag6", "tag7", "tag8"],
                               "items": [
                                   {"id": 1, "name": "item1", "active": true},
                                   {"id": 2, "name": "item2", "active": false},
                                   {"id": 3, "name": "item3", "active": true},
                                   {"id": 4, "name": "item4", "active": false}
                               ],
                               "extraData": {
                                   "values": [100, 200, 300, 400, 500],
                                   "mapping": {
                                       "key1": "value1",
                                       "key2": "value2",
                                       "key3": "value3"
                                   }
                               }
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
