using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerApiExceptionTests
    {
        [Fact]
        public void Generates_exception_message_with_status_code()
        {
            var ex = new TrueLayerApiException(HttpStatusCode.InternalServerError, "trace");
            ex.Message.Should().Be("The API response status code 500 does not indicate success.");
        }

        [Fact]
        public void Generates_exception_message_with_status_code_and_additional_info()
        {
            var ex = new TrueLayerApiException(HttpStatusCode.InternalServerError, "trace", "Invalid Parameters.");
            ex.Message.Should().Be("The API response status code 500 does not indicate success. Invalid Parameters.");
        }
    }
}
