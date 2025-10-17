using System;
using System.Web;

namespace TrueLayer.Payouts.Model;

/// <summary>
/// Helper class for building verification Hosted Payment Page (HPP) links for verified payouts
/// </summary>
public static class PayoutHppLinkBuilder
{
    /// <summary>
    /// Creates a verification HPP link for a verified payout
    /// </summary>
    /// <param name="payoutId">The unique payout identifier</param>
    /// <param name="resourceToken">The resource token from the payout creation response</param>
    /// <param name="returnUri">The URI to redirect to after verification</param>
    /// <param name="useSandbox">Whether to use sandbox environment (default: false)</param>
    /// <returns>The complete HPP link for the payout verification</returns>
    /// <exception cref="ArgumentException">If any of the required parameters are null or whitespace</exception>
    public static string CreateVerificationLink(
        string payoutId,
        string resourceToken,
        string returnUri,
        bool useSandbox = false)
    {
        if (string.IsNullOrWhiteSpace(payoutId))
            throw new ArgumentException("Payout ID cannot be null or empty", nameof(payoutId));

        if (string.IsNullOrWhiteSpace(resourceToken))
            throw new ArgumentException("Resource token cannot be null or empty", nameof(resourceToken));

        if (string.IsNullOrWhiteSpace(returnUri))
            throw new ArgumentException("Return URI cannot be null or empty", nameof(returnUri));

        var baseUrl = useSandbox
            ? "https://app.truelayer-sandbox.com/payouts"
            : "https://app.truelayer.com/payouts";

        var encodedReturnUri = HttpUtility.UrlEncode(returnUri);

        return $"{baseUrl}#payout_id={payoutId}&resource_token={resourceToken}&return_uri={encodedReturnUri}";
    }

    /// <summary>
    /// Creates a verification HPP link from a verified payout creation response
    /// </summary>
    /// <param name="response">The authorization required response from payout creation</param>
    /// <param name="returnUri">The URI to redirect to after verification</param>
    /// <param name="useSandbox">Whether to use sandbox environment (default: false)</param>
    /// <returns>The complete HPP link for the payout verification</returns>
    public static string CreateVerificationLink(
        CreatePayoutResponse.AuthorizationRequired response,
        string returnUri,
        bool useSandbox = false)
    {
        response.NotNull(nameof(response));
        return CreateVerificationLink(response.Id, response.ResourceToken, returnUri, useSandbox);
    }
}