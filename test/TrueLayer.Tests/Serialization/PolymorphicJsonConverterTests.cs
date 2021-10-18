using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
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
            sut.ShouldNotBeNull();
            sut.Vehicle.ShouldNotBeNull();
            sut.Vehicle.Type.ShouldBe("car");
            sut.Vehicle.ShouldBeOfType<Car>()
                .Doors.ShouldBe(3);
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
}
