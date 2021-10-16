using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class SerializationTests
    {
        [Fact]
        public void Can_handle_nullable_fields_with_record_constructors()
        {
            TestRecord obj = new("Required", null);
            string json = JsonSerializer.Serialize(obj, SerializerOptions.Default);

            TestRecord? deserialized = JsonSerializer.Deserialize<TestRecord>(json, SerializerOptions.Default);
            deserialized.ShouldNotBeNull();
            deserialized.RequiredField.ShouldBe(obj.RequiredField);
            deserialized.OptionalField.ShouldBe(obj.OptionalField);
        }

        [Fact]
        public void Can_deserialize_discriminated_unions()
        {
            // TODO test nested discriminators
            
            string json = @"{ 
               ""Vehicle"": {
                   ""__Type"": ""car"",
                   ""Doors"": 3
               } 
            }";

            // var options = new JsonSerializerOptions
            // {
            //     Converters = { new UnionTypeConverter<Vehicle>(new Dictionary<string, Type> {
            //         { "car", typeof(Car)}
            //     }) }
            // };

            var options = new JsonSerializerOptions
            {
                Converters = { new DiscriminatedUnionConverterFactory() }
            };

            var sut = JsonSerializer.Deserialize<Sut>(json, options);
            sut.ShouldNotBeNull();
            sut.Vehicle.ShouldNotBeNull();
            sut.Vehicle.Type.ShouldBe("car");
            sut.Vehicle.ShouldBeOfType<Car>()
                .Doors.ShouldBe(3);
        }

        [Fact]
        public void Can_deserialize_resource_collection()
        {
            string json = @"{ 
               ""items"": [{
                   ""__Type"": ""car"",
                   ""Doors"": 3
               }]
            }";

            var cars = JsonSerializer.Deserialize<ResourceCollection<Car>>(json, SerializerOptions.Default);
            cars.ShouldNotBeNull();
            cars.Items.ShouldNotBeEmpty();
        }

        class Sut
        {
            public Vehicle? Vehicle { get; set; }
        }

        [JsonKnownType(typeof(Car), "car")]
        [JsonDiscriminator("__Type")]
        public abstract class Vehicle
        {
            [JsonPropertyName("__Type")]
            public string Type { get; set; } = null!;
            public bool IsCar() => this is Car;
        }

        class Car : Vehicle
        {
            public int Doors { get; set; }
        }

        record TestRecord(string RequiredField, string? OptionalField);
    }
}
