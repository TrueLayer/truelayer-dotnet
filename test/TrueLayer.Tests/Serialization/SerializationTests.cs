using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
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

        [Fact]
        public void Can_deserialize_oneof()
        {
            // TODO test nested discriminators
            
            string json = @"{ 
                ""__Type"": ""bike"",
                ""Gears"": 10
            }";

            object? obj = new Bike();
            Vehicle vehicle = (Vehicle)(obj);
            _ = vehicle.Match(car => 1, bike => 2);

            var options = new JsonSerializerOptions
            {
                Converters = { new DiscriminatedUnionConverterFactory() }
            };

            var sut = JsonSerializer.Deserialize<Vehicle>(json, options);
            sut.ShouldNotBeNull();
            sut.IsT1.ShouldBeTrue();
            // sut.Vehicle.ShouldNotBeNull();
            // sut.Vehicle.Type.ShouldBe("car");
            // sut.Vehicle.ShouldBeOfType<Car>()
            //     .Doors.ShouldBe(3);
        }        

        class Sut
        {
            public Vehicle? Vehicle { get; set; }
        }

        [JsonKnownType(typeof(Car), "car")]
        [JsonKnownType(typeof(Bike), "bike")]
        [JsonDiscriminator("__Type")]
        class Vehicle : OneOfBase<Car, Bike>
        {
            protected Vehicle(OneOf<Car, Bike> _) : base(_)
            {
            }

            [JsonPropertyName("__Type")]
            public string Type { get; set; } = null!;

            public static implicit operator Vehicle(Car car) => new Vehicle(car);
            public static explicit operator Vehicle(Bike bike) => new Vehicle(bike);
        }

        class Car
        {
            public int Doors { get; set; }
        }

        class Bike
        {
            public int Gears { get; set; }
        }

        record TestRecord(string RequiredField, string? OptionalField);
    }
}
