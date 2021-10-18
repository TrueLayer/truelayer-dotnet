using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class GetPaymentResponse
    {
        // TODO beneficiary/payment method types
        
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt);

        [JsonDiscriminator("authorizing")]
        public record Authorizing(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt);

        [JsonDiscriminator("authorized")]
        public record Authorized(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, DateTime AuthorizedAt);

        [JsonDiscriminator("authorization_failed")]
        public record AuthorizationFailed(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, DateTime FailedAt, string FailureReason);

        // TODO source_of_funds
        [JsonDiscriminator("successful")]
        public record Successful(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, DateTime AuthorizedAt, DateTime SucceededAt);

        // TODO source_of_funds
        [JsonDiscriminator("settled")]
        public record Settled(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, DateTime AuthorizedAt, DateTime SucceededAt, DateTime SettledAt);

        [JsonDiscriminator("failed")]
        public record Failed(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, DateTime AuthorizedAt, DateTime FailedAt, string FailureReason);
    }
}
