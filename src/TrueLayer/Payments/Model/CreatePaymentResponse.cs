using System;

namespace TrueLayer.Payments.Model
{
    public record CreatePaymentResponse(string Status)
    {
        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, string ResourceToken)
            : CreatePaymentResponse(Status);
    }
}
