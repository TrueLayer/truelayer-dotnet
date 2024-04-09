using System;

namespace TrueLayer.Common;

internal static class TrueLayerBaseUris
{
    internal static readonly Uri ProdApiBaseUri = new("https://api.truelayer.com/");
    internal static readonly Uri SandboxApiBaseUri = new("https://api.truelayer-sandbox.com/");
    internal static readonly Uri ProdAuthBaseUri = new("https://auth.truelayer.com/");
    internal static readonly Uri SandboxAuthBaseUri = new("https://auth.truelayer-sandbox.com/");
    internal static readonly Uri ProdHppBaseUri = new("https://payment.truelayer.com/");
    internal static readonly Uri SandboxHppBaseUri = new("https://payment.truelayer-sandbox.com/");
}
