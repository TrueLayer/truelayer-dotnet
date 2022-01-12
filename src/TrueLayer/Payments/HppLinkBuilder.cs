using System;

namespace TrueLayer.Payments
{
    internal sealed class HppLinkBuilder
    {
        internal const string SandboxUrl = "https://checkout.truelayer-sandbox.com/";
        internal const string ProdUrl = "https://checkout.truelayer.com/";

        private readonly Uri _baseUri;

        public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
        {
            _baseUri = baseUri ??
                new Uri((useSandbox) ? SandboxUrl : ProdUrl);
        }

        public string Build(string paymentId, string paymentToken, Uri returnUri)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));
            paymentToken.NotNullOrWhiteSpace(nameof(paymentToken));
            returnUri.NotNull(nameof(returnUri));

            var fragment = $"payment_id={paymentId}&payment_token={paymentToken}&return_uri={returnUri.AbsoluteUri}";

            var builder = new UriBuilder(_baseUri);
            builder.Path = "payments";
            builder.Fragment = fragment;

            return builder.Uri.AbsoluteUri;
        }
    }
}
