using System.Net;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerApiExceptionTests
    {
        [Fact]
        public void Generates_exception_message_with_status_code()
        {
            var ex = new TrueLayerApiException(HttpStatusCode.InternalServerError, "trace");
            Assert.Equal("The API response status code 500 does not indicate success.", ex.Message);
        }

        [Fact]
        public void Generates_exception_message_with_status_code_and_additional_info()
        {
            var ex = new TrueLayerApiException(HttpStatusCode.InternalServerError, "trace", "Invalid Parameters.");
            Assert.Equal("The API response status code 500 does not indicate success. Invalid Parameters.", ex.Message);
        }
    }
}
