using System.Collections.Generic;

namespace TrueLayer.Payments.Model;

public record CreatePaymentRefundRequest(
    string Reference,
    uint? AmountInMinor = null,
    Dictionary<string, string>? Metadata = null);
