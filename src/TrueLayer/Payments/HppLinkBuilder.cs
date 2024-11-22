using System;
using TrueLayer.Common;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments
{
    internal sealed class HppLinkBuilder
    {
        private readonly Uri _baseUri;

        public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
        {
            _baseUri = baseUri ?? (useSandbox ? TrueLayerBaseUris.SandboxApiBaseUri : TrueLayerBaseUris.ProdApiBaseUri);
        }

        public string Build(string id, string token, Uri returnUri, ResourceType resourceType = ResourceType.Payment)
        {
            id.NotNullOrWhiteSpace(nameof(id));
            token.NotNullOrWhiteSpace(nameof(token));
            returnUri.NotNull(nameof(returnUri));

            var builder = new UriBuilder(_baseUri)
            {
                Path = ToResourcePath(resourceType),
                Fragment = ToFragment(id, token, returnUri, resourceType)
            };

            return builder.Uri.AbsoluteUri;
        }

        private static string ToFragment(string id, string token, Uri returnUri, ResourceType resourceType) =>
            $"{ToResourceFieldId(resourceType)}={id}&resource_token={token}&return_uri={returnUri.AbsoluteUri}";

        private static string ToResourceFieldId(ResourceType resourceType) =>
            resourceType switch
            {
                ResourceType.Payment => "payment_id",
                ResourceType.Mandate => "mandate_id",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };

        private static string ToResourcePath(ResourceType resourceType) =>
            resourceType switch
            {
                ResourceType.Payment => "payments",
                ResourceType.Mandate => "mandates",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
    }
}
