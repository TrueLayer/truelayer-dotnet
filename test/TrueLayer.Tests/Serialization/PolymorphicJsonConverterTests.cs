using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization;

public class PolymorphicJsonConverterTests
{
    [Fact]
    public void Can_deserialize_derived_types()
    {
        string json = @"{
               ""Vehicle"": {
                   ""__Type"": ""car"",
                   ""Doors"": 3
               }
            }";

        var options = new JsonSerializerOptions
        {
            Converters = { new PolymorphicJsonConverterFactory() }
        };

        var sut = JsonSerializer.Deserialize<Sut>(json, options);
        sut.Should().NotBeNull();
        sut!.Vehicle.Should().NotBeNull();
        sut.Vehicle!.Type.Should().Be("car");
        sut.Vehicle.Should().BeOfType<Car>().Which
            .Doors.Should().Be(3);
    }

    class Sut
    {
        public Vehicle? Vehicle { get; set; }
    }

    [JsonKnownType(typeof(Car), "car")]
    [JsonKnownType(typeof(Bike), "bike")]
    [JsonDiscriminator("__Type")]
    abstract class Vehicle
    {
        [JsonPropertyName("__Type")]
        public string Type { get; set; } = null!;
    }

    class Car : Vehicle
    {
        public int Doors { get; set; }
    }

    class Bike : Vehicle
    {
        public int Gears { get; set; }
    }
}