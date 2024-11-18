using System.Net;
using FluentAssertions;
using Xunit;

namespace TrueLayer.Tests
{
    public class ApiResponseTests
    {
        [Theory]
        [InlineData(HttpStatusCode.OK, true)]
        [InlineData(HttpStatusCode.Created, true)]
        [InlineData(HttpStatusCode.BadRequest, false)]
        [InlineData(HttpStatusCode.Unauthorized, false)]
        [InlineData(HttpStatusCode.Conflict, false)]
        [InlineData(HttpStatusCode.BadGateway, false)]
        public void Is_successful_when_status_code_indicates_success(HttpStatusCode statusCode, bool expected)
        {
            var response = new ApiResponse<Stub>(statusCode, "trace-id");
            response.IsSuccessful.Should().Be(expected);
        }

        class Stub
        {

        }
    }
}
