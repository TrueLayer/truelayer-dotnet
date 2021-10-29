using System;
using Shouldly;
using TrueLayer.Payments;
using Xunit;

namespace TrueLayer.Tests
{
    public class HppLinkBuilderTests
    {
        [Fact]
        public void Can_generate_hpp_link()
        {
            var baseUri = new Uri("https://checkout.truelayer-sandbox.com");
            var builder = new HppLinkBuilder(baseUri);

            var link = builder.Build("payment-id", "resource-token", new Uri("https://localhost.com"));
            link.ShouldBe("https://checkout.truelayer-sandbox.com/payments#payment_id=payment-id&resource_token=resource-token&return_uri=https://localhost.com/");
        }
    }
}
