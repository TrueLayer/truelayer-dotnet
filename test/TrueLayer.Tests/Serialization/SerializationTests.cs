using System;
using System.Linq.Expressions;
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

            // object obj = new Bike();
            // OneOf<Car, Bike> foo = obj;
            // _ = vehicle.Match(car => 1, bike => 2);

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

        [Fact]
        public void Union_test()
        {
            var unionType = typeof(Union<string, int>);
            var union = (Union<string, int>)Activator.CreateInstance(unionType, "test")!;

            union.Value.ShouldBe("test");

            var union2 = (Union<string, int>)Activator.CreateInstance(unionType, 10)!;
            union2.Value.ShouldBe(10);

            // Ref https://gist.github.com/adjames/1736161

            Type paramType = typeof(string);
            var param = Expression.Parameter(paramType);
            var constructor = unionType.GetConstructor(new Type[] { paramType });
            //var body = Expression.New(constructor!, param);
            //var constructorExpression = Expression.Lambda<Func<string, object>>(body, param);


            NewExpression constructorExpression = Expression.New(constructor!, param);


            //NewExpression constructorExpression = Expression.New(unionType);
            // Expression<Func<string, Union<string, int>>> lambdaExpression
            //     = Expression.Lambda<Func<string, Union<string, int>>>(constructorExpression, new[] { param });
            // Func<string, Union<string, int>> createUnionFunc = lambdaExpression.Compile();
            // object union3 = createUnionFunc("hello");

            // var factory = CreateFactory<int, Union<string, int>>();
            // var union4 = factory.Invoke(100);
            // union4.Value.ShouldBe(100);

            var factory2 = CreateFactory(typeof(string), typeof(Union<string, int>)) as Func<object, Union<string, int>>;
            
            object input = "Foobar";
            var union5 = factory2!.Invoke(input);
            union5.ShouldBeOfType<Union<string, int>>().Value.ShouldBe("Foobar");
        }

        static Func<TArg, T> CreateFactory<TArg, T>()
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(TArg) });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var parameter = Expression.Parameter(typeof(TArg));
            var factoryExpression = Expression.Lambda<Func<TArg, T>>(
                Expression.New(constructor, new[] { parameter}), parameter);

            return factoryExpression.Compile();
        }

        static Delegate CreateFactory(Type argType, Type unionType)
        {
            var constructor = unionType.GetConstructor(new[] { argType });

            if (constructor is null)
                throw new ArgumentException(nameof(constructor));

            var parameter = Expression.Parameter(argType);
            var factoryType = typeof(Func<,>).MakeGenericType(typeof(object), unionType);     

            var input = Expression.Parameter(typeof(object));
            var convertedParameter = Expression.Convert(input, argType);  

            // var factoryExpression = Expression.Lambda<Func<object, object>>(
            //     Expression.New(constructor, new[] { convertedParameter }), Expression.Parameter(typeof(object)));

            var factoryExpression = Expression.Lambda(
                factoryType,
                Expression.New(constructor, new[] { convertedParameter }), input);

            return factoryExpression.Compile();
        }

//         MethodInfo mi = genericType.GetMethod("DoSomething",
//                                 BindingFlags.Instance | BindingFlags.Public);

// var p1 = Expression.Parameter(genericType, "generic");
// var p2 = Expression.Parameter(fieldType, "instance");
// var func = typeof (Func<,,>);
// var genericFunc = func.MakeGenericType(genericType, fieldType, typeof(int));
// var x = Expression.Lambda(genericFunc, Expression.Call(p1, mi, p2),
//                 new[] { p1, p2 }).Compile();



        class Sut
        {
            public Vehicle? Vehicle { get; set; }
        }

        [JsonKnownType(typeof(Car), "car")]
        [JsonKnownType(typeof(Bike), "bike")]
        [JsonDiscriminator("__Type")]
        class Vehicle : OneOfBase<Car, Bike>
        {
            protected Vehicle(OneOf.OneOf<Car, Bike> _) : base(_)
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
