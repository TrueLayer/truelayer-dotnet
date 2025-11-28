using System;
using TrueLayer.Common;

namespace TrueLayer.Mandates;

internal sealed class HppLinkBuilder
{
    private readonly Uri _baseUri;

    public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
    {
        _baseUri = baseUri ?? (useSandbox ? TrueLayerBaseUris.SandboxHppBaseUri : TrueLayerBaseUris.ProdHppBaseUri);
    }

    public string Build(string id, string token, Uri returnUri)
    {
        id.NotNullOrWhiteSpace(nameof(id));
        id.NotAUrl(nameof(id));
        token.NotNullOrWhiteSpace(nameof(token));
        returnUri.NotNull(nameof(returnUri));

        var builder = new UriBuilder(_baseUri)
        {
            Path = "mandates",
            Fragment = $"mandate_id={id}&resource_token={token}&return_uri={returnUri.AbsoluteUri}"
        };

        return builder.Uri.AbsoluteUri;
    }
}
