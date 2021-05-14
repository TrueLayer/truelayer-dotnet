using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.Auth;
using TrueLayer.Payments;
using Xunit;
using Moq;

namespace TrueLayer.Sdk.Tests
{
    public class TrueLayerServiceCollectionExtensionsTests
    {
        private readonly TrueLayerOptions _options;

        public TrueLayerServiceCollectionExtensionsTests()
        {
            _options = new TrueLayerOptions
            {
                ClientId = "client_id",
                ClientSecret = "secret",
                UseSandbox = true,
            };
        }

        [Fact]
        public async Task Can_customise_http_client_builder()
        {
            var services = new ServiceCollection()       
                .AddTrueLayerSdk(_options, builder 
                    => builder.ConfigurePrimaryHttpMessageHandler(() => new FakeHandler()))
                .BuildServiceProvider();

            var api = services.GetRequiredService<ITrueLayerApi>();
            await Assert.ThrowsAsync<TrueLayerResourceNotFoundException>(async () => await api.Auth.GetPaymentToken());
        }

        [Theory]
        [InlineData("https://auth.uri")]
        public void Can_customise_uris(string overrideUri)
        {
            // ARRANGE
            var options = new TrueLayerOptions
            {
                ClientId = "client-id",
                ClientSecret = "client-secret",
                UseSandbox = true,
                Auth = new ApiOptions
                {
                    Uri = new Uri(overrideUri),
                },
            };
            var apiClient = Mock.Of<IApiClient>();
            
            // ACT
            var authClient = new AuthClient(apiClient,new LocalTokenCache(), options);
            var payClients = new PaymentsClient(apiClient, options, authClient);

            // ASSERT
            Assert.Equal(new Uri(overrideUri).AbsoluteUri, authClient.BaseUri.AbsoluteUri);
            Assert.Equal(PaymentsClient.SandboxUrl, payClients.BaseUri.AbsoluteUri);
        }

        [Fact]
        public void Options_should_throw_when_uri_invalid()
        {
            // ARRANGE
            const string json = "{\"ClientId\":\"client-id\",\"ClientSecret\":\"client-secret\",\"UseSandbox\":true," +
                                "\"Auth\":{\"Uri\":\"not-a-valid-uri\"}}";
            var options = JsonSerializer.Deserialize<TrueLayerOptions>(json);
            
            // ACT
            var ex = Record.Exception(() => options?.Validate());
            
            // ASSERT
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Uri must be a valid and absolute uri.", ex.Message);
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
