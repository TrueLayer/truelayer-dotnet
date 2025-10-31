using System;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Hosted page parameters for auto-constructed hosted page URI.
/// Cannot be provided if starting the authorization_flow explicitly.
/// </summary>
public class HostedPageRequest
{
    /// <summary>
    /// Creates a new <see cref="HostedPageRequest"/>
    /// </summary>
    /// <param name="returnUri">The URI where the user will be redirected to after the authorization flow has completed on the hosted page.
    /// You must register the return_uri in your settings in Console.</param>
    /// <param name="countryCode">The country code of the user. This is used to determine which banks to show on the hosted page initially.
    /// The country code must be in ISO 3166-1 alpha-2 format.</param>
    /// <param name="languageCode">The language code of the user. This is used to determine which language to show on the hosted page, overriding the browser's locale.
    /// The language code must be in ISO 639-1 format.</param>
    /// <param name="maxWaitForResult">The maximum time to wait for a result from the hosted page after the user has completed the authorization flow, in seconds (0-60).</param>
    public HostedPageRequest(
        Uri returnUri,
        string? countryCode = null,
        string? languageCode = null,
        int? maxWaitForResult = null)
    {
        ReturnUri = returnUri.NotNull(nameof(returnUri));
        CountryCode = countryCode;
        LanguageCode = languageCode;
        MaxWaitForResult = maxWaitForResult;
    }

    /// <summary>
    /// Gets the URI where the user will be redirected to after the authorization flow has completed on the hosted page.
    /// You must register the return_uri in your settings in Console.
    /// </summary>
    public Uri ReturnUri { get; }

    /// <summary>
    /// Gets the country code of the user. This is used to determine which banks to show on the hosted page initially.
    /// The country code must be in ISO 3166-1 alpha-2 format.
    /// </summary>
    public string? CountryCode { get; }

    /// <summary>
    /// Gets the language code of the user. This is used to determine which language to show on the hosted page, overriding the browser's locale.
    /// The language code must be in ISO 639-1 format.
    /// </summary>
    public string? LanguageCode { get; }

    /// <summary>
    /// Gets the maximum time to wait for a result from the hosted page after the user has completed the authorization flow, in seconds (0-60).
    /// </summary>
    public int? MaxWaitForResult { get; }
}
