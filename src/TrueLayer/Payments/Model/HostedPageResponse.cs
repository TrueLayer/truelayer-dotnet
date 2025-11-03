using System;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Hosted page response containing the URI to redirect the user to authorize the payment.
/// Returned if the hosted_page object in the request body is populated.
/// </summary>
public record HostedPageResponse
{
    /// <summary>
    /// Gets the URI to redirect the user to authorize the payment using the hosted page.
    /// </summary>
    public Uri Uri { get; init; } = null!;
}
