using System.Text.Json;
using Shouldly;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Serialization
{
    public class DiscriminatedJsonConverterTests
    {
        [Fact]
        public void Derived_properties_omitted_by_default()
        {
            IVehicle car = new Car { Doors = 3 };
            string json = JsonSerializer.Serialize(car);

            json.ShouldNotContain(nameof(Car.Doors));
        }

        [Fact]
        public void Can_serialize_derived_properties()
        {
            IVehicle car = new Car { Doors = 3 };
            string json = JsonSerializer.Serialize(car, new JsonSerializerOptions {
                Converters = { new DiscriminatedJsonConverter() }
            });

            json.ShouldNotContain(nameof(Car.Doors));
        }

        public interface IVehicle
        {
            string Type { get; }
        }

        public class Car : IVehicle
        {
            public string Type => "cat";
            public int Doors { get; set; }
        }
    }
}
