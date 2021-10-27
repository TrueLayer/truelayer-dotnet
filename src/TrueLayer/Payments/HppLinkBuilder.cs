using System;

namespace TrueLayer.Payments
{
    internal class HppLinkBuilder
    {
        internal const string SandboxUrl = "https://checkout.truelayer-sandbox.com/";
        internal const string ProdUrl = "https://checkout.truelayer.com/";
        
        private readonly Uri _baseUri;

        public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
        {
            _baseUri = baseUri ??
                new Uri((useSandbox) ? SandboxUrl : ProdUrl);
        }

        public string Build(string paymentId, string resourceToken, Uri returnUri)
        {
            paymentId.NotNullOrWhiteSpace(nameof(paymentId));
            resourceToken.NotNullOrWhiteSpace(nameof(resourceToken));
            returnUri.NotNull(nameof(returnUri));
            
            var fragment = $"payment_id={paymentId}&resource_token={resourceToken}&return_uri={returnUri.AbsoluteUri}";

            var builder = new UriBuilder(_baseUri);
            builder.Path = "payments";
            builder.Fragment = fragment;
                
            return builder.Uri.AbsoluteUri;
        }        
    }
}
