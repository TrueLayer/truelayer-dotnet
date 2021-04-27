using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using TrueLayerSdk;
using Xunit;

namespace TrueLayer.Sdk.Tests
{
    public class UserAgentHandlerTests
    {
        [Fact]
        public async Task Adds_user_agent_to_outgoing_requests()
        {
            var handler = new UserAgentHandler
            {
                InnerHandler = new EchoHandler()
            };

            var client = new HttpClient(handler);
            HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost"));
            response.Headers.TryGetValues("User-Agent", out var value).ShouldBeTrue();
            ProductHeaderValue.TryParse(value.FirstOrDefault(), out var product).ShouldBeTrue();
            product.Name.ShouldBe("truelayer-sdk-net");
            product.Version.ShouldBe(ReflectionUtils.GetAssemblyVersion<ITruelayerApi>());
        }

        class EchoHandler : HttpMessageHandler
        {
            public HttpRequestHeaders Headers { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Headers = request.Headers;
                var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

                foreach (var header in request.Headers)
                {
                    httpResponse.Headers.Add(header.Key, header.Value);
                }

                return Task.FromResult(httpResponse);
            }
        }
    }
}
