using System;
using TrueLayer.Payments;
using Xunit;

namespace TrueLayer.Tests
{
    public class HppLinkBuilderTests
    {
        [Fact]
        public void Can_generate_hpp_link()
        {
            var baseUri = new Uri("https://payment.truelayer-sandbox.com");
            var builder = new HppLinkBuilder(baseUri);

            var link = builder.Build("payment-id", "payment-token", new Uri("https://localhost.com"));
            Assert.Equal("https://payment.truelayer-sandbox.com/payments#payment_id=payment-id&resource_token=payment-token&return_uri=https://localhost.com/", link);
        }
    }
}
