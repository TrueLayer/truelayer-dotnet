using System;
using TrueLayer.Payments.Enums;

namespace TrueLayer.Payments
{
    internal sealed class HppLinkBuilder
    {
        internal const string SandboxUrl = "https://payment.truelayer-sandbox.com/";
        internal const string ProdUrl = "https://payment.truelayer.com/";

        private readonly Uri _baseUri;

        public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
        {
            _baseUri = baseUri ??
                       new Uri((useSandbox) ? SandboxUrl : ProdUrl);
        }

        public string Build(string id, string token, Uri returnUri, HppType hppType = HppType.Payment)
        {
            id.NotNullOrWhiteSpace(nameof(id));
            token.NotNullOrWhiteSpace(nameof(token));
            returnUri.NotNull(nameof(returnUri));

            var idName = hppType == HppType.Payment ? "payment_id" : "mandate_id";
            var fragment = $"{idName}={id}&resource_token={token}&return_uri={returnUri.AbsoluteUri}";

            var builder = new UriBuilder(_baseUri)
            {
                Path = hppType == HppType.Payment ? "payments" : "mandates",
                Fragment = fragment
            };

            return builder.Uri.AbsoluteUri;
        }
    }
}
