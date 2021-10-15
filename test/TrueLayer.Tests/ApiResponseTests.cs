using System.Net;
using Shouldly;
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
            response.Success.ShouldBe(expected);
        }

        class Stub
        {

        }
    }
}
