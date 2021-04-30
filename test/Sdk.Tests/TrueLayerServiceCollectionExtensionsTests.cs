using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TrueLayerSdk;
using TrueLayer.Auth.Model;
using TrueLayerSdk.Common.Exceptions;
using Xunit;

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

            var api = services.GetRequiredService<ITruelayerApi>();
            await Assert.ThrowsAsync<TrueLayerResourceNotFoundException>(async () => await api.Auth.GetPaymentToken(new GetPaymentTokenRequest()));
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
