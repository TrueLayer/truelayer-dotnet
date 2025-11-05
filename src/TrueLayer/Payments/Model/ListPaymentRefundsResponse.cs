using System.Collections.Generic;
using OneOf;

namespace TrueLayer.Payments.Model;

using RefundUnion = OneOf<RefundPending, RefundAuthorized, RefundExecuted, RefundFailed>;

/// <summary>
/// Represents the response from listing payment refunds.
/// </summary>
/// <param name="Items">The list of refunds for the payment.</param>
public record ListPaymentRefundsResponse(List<RefundUnion> Items);
