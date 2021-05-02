using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.Auth.Model;
using Xunit;
using System;
using Microsoft.Extensions.Configuration;

namespace TrueLayer.Sdk.Tests
{
    public class TrueLayerServiceCollectionExtensionsTests
    {
        private readonly TruelayerConfiguration _configuration;

        public TrueLayerServiceCollectionExtensionsTests()
        {
            _configuration = new TruelayerConfiguration("client_id", "secret", true);
        }

        [Fact]
        public async Task Can_customise_http_client_builder()
        {
            var services = new ServiceCollection()       
                .AddTruelayerSdk(_configuration, builder 
                    => builder.ConfigurePrimaryHttpMessageHandler(() => new FakeHandler()))
                .BuildServiceProvider();

            var api = services.GetRequiredService<ITrueLayerApi>();
            await Assert.ThrowsAsync<TrueLayerResourceNotFoundException>(async () => await api.Auth.GetPaymentToken(new GetPaymentTokenRequest()));
        }

        [Theory]
        [InlineData("https://auth.uri")]
        public void Can_customise_uris(string overrideUri)
        {
            // ARRANGE
            var options = new TruelayerOptions
            {
                ClientId = "client-id",
                ClientSecret = "client-secret",
                UseSandbox = true,
                AuthUri = overrideUri,
            };

            // ACT
            var config = options.CreateConfiguration();

            // ASSERT
            Assert.NotNull(config);
            Assert.Equal(new Uri(overrideUri).AbsoluteUri, config.AuthUri.AbsoluteUri);
            Assert.Equal(TruelayerConfiguration.PaymentsSandboxUri.AbsoluteUri, config.PaymentsUri.AbsoluteUri);
        }
        
        class FakeHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);  
                return Task.FromResult(httpResponse);
            }
        }
    }
}
