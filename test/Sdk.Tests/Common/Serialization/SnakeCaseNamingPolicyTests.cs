using System;
using System.Text.Json;
using TrueLayerSdk.Common.Serialization;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TrueLayer.Sdk.Tests.Common.Serialization
{
    public class SnakeCaseNamingPolicyTests
    {
        [Fact]
        public void SnakeCaseSettings_ShouldBehaveAsIntended()
        {
            var student = new Student
            {
                RegistrationDate = new DateTime(2001, 12, 22, 10, 40, 55, DateTimeKind.Utc),
                StudentName = "StudentName?",
                StudentId = 12345
            };

            var snakeCaseOptions = new JsonSerializerOptions {PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance};
            
            var json = JsonSerializer.Serialize(student, snakeCaseOptions);

            Assert.Equal("{\"student_id\":12345,\"student_name\":\"StudentName?\",\"registration_date\":\"2001-12-22T10:40:55Z\"}", json);

            var deserializedStudent = JsonSerializer.Deserialize<Student>(json, snakeCaseOptions);
            if (deserializedStudent is null) throw new ArgumentNullException(nameof(deserializedStudent));
            
            Assert.Equal(student.RegistrationDate, deserializedStudent.RegistrationDate);
            Assert.Equal(student.StudentName, deserializedStudent.StudentName);
            Assert.Equal(student.StudentId, deserializedStudent.StudentId);

            json = JsonSerializer.Serialize(student);
            Assert.Equal("{\"StudentId\":12345,\"StudentName\":\"StudentName?\",\"RegistrationDate\":\"2001-12-22T10:40:55Z\"}", json);
        }

        private class Student
        {
            public int StudentId { get; init; }
            public string StudentName { get; init; }
            public DateTime RegistrationDate { get; init; }
        }
    }
}
