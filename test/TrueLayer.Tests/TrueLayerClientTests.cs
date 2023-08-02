using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Shouldly;
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

            var client = new TrueLayerClient(new ApiClient(new HttpClient()), Options.Create(options));
            client.Auth.ShouldNotBeNull();
            client.Payments.ShouldNotBeNull();
            client.MerchantAccounts.ShouldNotBeNull();
            client.Mandates.ShouldNotBeNull();
        }
    }
}
