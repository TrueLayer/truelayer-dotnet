using System;
using TrueLayer.Common;
using TrueLayer.Payments.Model;

namespace TrueLayer.Payments;

internal sealed class HppLinkBuilder
{
    private readonly Uri _baseUri;

    public HppLinkBuilder(Uri? baseUri = null, bool useSandbox = true)
    {
        _baseUri = baseUri ?? (useSandbox ? TrueLayerBaseUris.SandboxHppBaseUri : TrueLayerBaseUris.ProdHppBaseUri);
    }

    public string Build(
        string id,
        string token,
        Uri returnUri,
        ResourceType resourceType = ResourceType.Payment,
        int? maxWaitForResultSeconds = null,
        bool? signup = null)
    {
        id.NotNullOrWhiteSpace(nameof(id));
        id.NotAUrl(nameof(id));
        token.NotNullOrWhiteSpace(nameof(token));
        returnUri.NotNull(nameof(returnUri));

        var builder = new UriBuilder(_baseUri)
        {
            Path = ToResourcePath(resourceType),
            Fragment = ToFragment(id, token, returnUri, resourceType, maxWaitForResultSeconds, signup)
        };

        return builder.Uri.AbsoluteUri;
    }

    private static string ToFragment(string id, string token, Uri returnUri, ResourceType resourceType, int? maxWaitForResultSeconds, bool? signup)
    {
        var fragment = $"{ToResourceFieldId(resourceType)}={id}&resource_token={token}&return_uri={returnUri.AbsoluteUri}";

        if (maxWaitForResultSeconds is not null)
        {
            fragment += $"&max_wait_seconds={maxWaitForResultSeconds}";
        }

        if (signup is not null)
        {
            fragment += $"&signup={signup.Value.ToString().ToLowerInvariant()}";
        }

        return fragment;
    }

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