using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer
{
    internal class UserAgentHandler : DelegatingHandler
    {
        private static readonly ProductInfoHeaderValue UserAgentHeader
            = new ProductInfoHeaderValue("truelayer-sdk-net", ReflectionUtils.GetAssemblyVersion<ITrueLayerApi>());
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            request.Headers.UserAgent.Add(UserAgentHeader);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
