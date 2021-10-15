using Shouldly;
using TrueLayer.Serialization;
using Xunit;


namespace TrueLayer.Sdk.Tests.Serialization
{
    public class SerializationTests
    {
        private readonly JsonSerializer _serializer = new();

        [Fact]
        public void Can_handle_nullable_fields_with_record_constructors()
        {
            TestRecord obj = new("Required", null);
            string json = _serializer.Serialize(obj);

            TestRecord deserialized = _serializer.Deserialize(json, typeof(TestRecord)) as TestRecord;
            deserialized.ShouldNotBeNull();
            deserialized.RequiredField.ShouldBe(obj.RequiredField);
            deserialized.OptionalField.ShouldBe(obj.OptionalField);
        }
        
        record TestRecord(string RequiredField, string OptionalField);
    }
}
