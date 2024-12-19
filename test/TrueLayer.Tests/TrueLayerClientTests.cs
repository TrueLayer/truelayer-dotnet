using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.Payments;
using TrueLayer.Tests.Mocks;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerClientTests
    {
        [Fact]
        public void Can_Create_TrueLayer_Client_From_Options()
        {
            var configName = "TrueLayer";
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
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();

            var optionsMock = new OptionSnapshotMock(options);
            var factory = new TrueLayerClientFactory(
                new ApiClient(new HttpClient()),
                optionsMock,
                provider);
            var client = factory.Create(configName);


            client.Auth.Should().NotBeNull();
            client.Payments.Should().NotBeNull();
            client.MerchantAccounts.Should().NotBeNull();
            client.Mandates.Should().NotBeNull();
        }
    }
}
