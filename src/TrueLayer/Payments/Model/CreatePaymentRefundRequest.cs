using System.Collections.Generic;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Represents a request to create a refund for a payment.
/// </summary>
/// <param name="Reference">The reference for the refund.</param>
/// <param name="AmountInMinor">Optional refund amount in minor currency units. If not provided, refunds the full payment amount.</param>
/// <param name="Metadata">Optional custom metadata for the refund.</param>
public record CreatePaymentRefundRequest(
    string Reference,
    uint? AmountInMinor = null,
    Dictionary<string, string>? Metadata = null);
