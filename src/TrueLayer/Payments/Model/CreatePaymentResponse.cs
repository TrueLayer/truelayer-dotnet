using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    [JsonKnownType(typeof(CreatePaymentResponse.AuthorizationRequired), "authorization_required")]
    [JsonDiscriminator("status")]
    public abstract record CreatePaymentResponse(string Status)
    {
        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, string ResourceToken)
            : CreatePaymentResponse(Status);
    }
}
