namespace TrueLayer.Payments.Model;

/// <summary>
/// Represents the response from creating a payment refund.
/// </summary>
/// <param name="Id">The unique identifier of the created refund.</param>
public record CreatePaymentRefundResponse(string Id);
