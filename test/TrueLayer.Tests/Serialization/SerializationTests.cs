using System.Text.Json;
using FluentAssertions;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization;

public class SerializationTests
{
    [Fact]
    public void Can_handle_nullable_fields_with_record_constructors()
    {
        TestRecord obj = new("Required", null);
        string json = JsonSerializer.Serialize(obj, SerializerOptions.Default);

        TestRecord? deserialized = JsonSerializer.Deserialize<TestRecord>(json, SerializerOptions.Default);
        deserialized.Should().NotBeNull();
        deserialized!.RequiredField.Should().Be(obj.RequiredField);
        deserialized.OptionalField.Should().Be(obj.OptionalField);
    }

    [Fact]
    public void Can_deserialize_resource_collection()
    {
        string json = @"{
               ""items"": [{
                   ""required_field"": ""foo""
               }]
            }";

        var records = JsonSerializer.Deserialize<ResourceCollection<TestRecord>>(json, SerializerOptions.Default);
        records.Should().NotBeNull();
        records!.Items.Should().NotBeEmpty();
    }


    record TestRecord(string RequiredField, string? OptionalField);
}