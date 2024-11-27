namespace TrueLayer.Mandates;

internal static class MandatesEndpoints
{
    internal static readonly string V3Mandates = new("/v3/mandates/");
    internal static readonly string AuthorizationFlow = new("/authorization-flow");
    internal static readonly string ProviderSelection = new("/authorization-flow/actions/provider-selection");
    internal static readonly string Consent = new("/authorization-flow/actions/consent");
    internal static readonly string Funds = new("/funds");
    internal static readonly string Constraints = new("/constraints");
    internal static readonly string Revoke = new("/revoke");
}
