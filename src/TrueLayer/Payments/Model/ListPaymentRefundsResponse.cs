using System.Collections.Generic;
using OneOf;

namespace TrueLayer.Payments.Model;

using RefundUnion = OneOf<Pending, Authorized>;

public record ListPaymentRefundsResponse(List<RefundUnion> Items);
