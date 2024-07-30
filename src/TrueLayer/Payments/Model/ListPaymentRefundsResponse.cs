using System.Collections.Generic;
using OneOf;

namespace TrueLayer.Payments.Model;

using RefundUnion = OneOf<RefundPending, RefundAuthorized>;

public record ListPaymentRefundsResponse(List<RefundUnion> Items);
