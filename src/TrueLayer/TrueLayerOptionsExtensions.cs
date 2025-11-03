using System;
using TrueLayer.Common;

namespace TrueLayer;

internal static class TrueLayerOptionsExtensions
{
    internal static Uri GetApiBaseUri(this TrueLayerOptions options)
    {
        var baseUri = options.UseSandbox ?? true
            ? TrueLayerBaseUris.SandboxApiBaseUri
            : TrueLayerBaseUris.ProdApiBaseUri;

        return options.Payments?.Uri ?? baseUri;
    }

    internal static Uri GetAuthBaseUri(this TrueLayerOptions options)
    {
        var baseUri = options.UseSandbox ?? true
            ? TrueLayerBaseUris.SandboxAuthBaseUri
            : TrueLayerBaseUris.ProdAuthBaseUri;

        return options.Auth?.Uri ?? baseUri;
    }
}