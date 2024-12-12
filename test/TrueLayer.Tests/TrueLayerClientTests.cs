using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TrueLayer.Payments;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerClientTests
    {
        [Fact]
        public void Can_create_truelayer_client_from_options()
        {
            var options = new TrueLayerOptions
            {
                ClientId = "clientid",
                ClientSecret = "secret",
                UseSandbox = true,
                Auth = new ApiOptions
                {
                    Uri = new Uri("https://auth")
                },
                Payments = new PaymentsOptions
                {
                    Uri = new Uri("https://payments"),
                    HppUri = new Uri("https://hpp"),
                    SigningKey = new SigningKey
                    {
                        KeyId = "key-id",
                        PrivateKey = "--private--"
                    }
                }
            };

            var factory = new TrueLayerClientFactory(new ApiClient(new HttpClient(), Options.Create(options)), Options.Create(options), new NullMemoryCache());
            var client = factory.Create();


            client.Auth.Should().NotBeNull();
            client.Payments.Should().NotBeNull();
            client.MerchantAccounts.Should().NotBeNull();
            client.Mandates.Should().NotBeNull();
        }
    }
}
