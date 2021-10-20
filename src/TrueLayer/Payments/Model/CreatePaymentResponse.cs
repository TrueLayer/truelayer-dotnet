using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class CreatePaymentResponse
    {
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, string ResourceToken);
    }
}
