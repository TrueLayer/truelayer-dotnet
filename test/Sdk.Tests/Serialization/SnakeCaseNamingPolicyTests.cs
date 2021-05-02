using System;
using System.Text.Json;
using TrueLayer.Serialization;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TrueLayer.Sdk.Tests.Serialization
{
    public class SnakeCaseNamingPolicyTests
    {
        private readonly JsonSerializerOptions _snakeCaseOptions = new JsonSerializerOptions
            {PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance};

        [Fact]
        public void Serialization_WithSnakeCaseSettings_ShouldProduceSnakeCaseJsonProperties()
        {
            // ARRANGE
            var student = new Student
            {
                RegistrationDate = new DateTime(2001, 12, 22, 10, 40, 55, DateTimeKind.Utc),
                StudentName = "StudentName?",
                StudentId = 12345
            };

            // ACT
            var json = JsonSerializer.Serialize(student, _snakeCaseOptions);

            // ASSERT
            Assert.Equal("{\"student_id\":12345,\"student_name\":\"StudentName?\",\"registration_date\":\"2001-12-22T10:40:55Z\"}", json);
        }

        [Fact]
        public void Deserialization_WithSnakeCaseSettings_ShouldRecognizeSnakeCaseJsonProperties()
        {
            // ARRANGE
            var registrationDate = new DateTime(2001, 12, 22, 10, 40, 55, DateTimeKind.Utc);
            const string studentName = "StudentName?";
            const int studentId = 12345;
            var json =
                $"{{\"student_id\":{studentId}," +
                $"\"student_name\":\"{studentName}\"," +
                $"\"registration_date\":\"{registrationDate:yyyy-MM-ddTHH:mm:ssZ}\"}}";

            // ACT
            var deserializedStudent = JsonSerializer.Deserialize<Student>(json, _snakeCaseOptions);
            
            // ASSERT
            Assert.NotNull(deserializedStudent);
            Assert.Equal(registrationDate, deserializedStudent.RegistrationDate);
            Assert.Equal(studentName, deserializedStudent.StudentName);
            Assert.Equal(studentId, deserializedStudent.StudentId);
        }

        [Fact]
        public void DefaultSerialization_ShouldProducePascalCaseProperties()
        {
            // ARRANGE
            var student = new Student
            {
                RegistrationDate = new DateTime(2001, 12, 22, 10, 40, 55, DateTimeKind.Utc),
                StudentName = "StudentName?",
                StudentId = 12345
            };
            
            // ACT
            var json = JsonSerializer.Serialize(student);
            
            // ASSERT
            Assert.Equal(
                "{\"StudentId\":12345,\"StudentName\":\"StudentName?\",\"RegistrationDate\":\"2001-12-22T10:40:55Z\"}",
                json);
        }
        
        private class Student
        {
            public int StudentId { get; init; }
            public string StudentName { get; init; }
            public DateTime RegistrationDate { get; init; }
        }
    }
}
