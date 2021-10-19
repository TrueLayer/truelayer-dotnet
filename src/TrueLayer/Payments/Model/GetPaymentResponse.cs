using System;
using OneOf;
using TrueLayer.Serialization;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.PaymentMethod;

namespace TrueLayer.Payments.Model
{
    using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;
    using PaymentMethodUnion = OneOf<BankTransfer>;

    public static class GetPaymentResponse
    {
        // TODO beneficiary/payment method types
        public record PaymentDetails
        {
            public string Id { get; init; } = null!;
            public long AmountInMinor { get; init; }
            public string Currency { get; init; } = null!;
            public string Status { get; init; } = null!;
            public DateTime CreatedAt { get; init; }
            public BeneficiaryUnion Beneficiary { get; init; }
            public PaymentMethodUnion PaymentMethod { get; init; }
        }

        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired : PaymentDetails;

        [JsonDiscriminator("authorizing")]
        public record Authorizing : PaymentDetails;

        [JsonDiscriminator("authorized")]
        public record Authorized(DateTime AuthorizedAt) : PaymentDetails;

        [JsonDiscriminator("authorization_failed")]
        public record AuthorizationFailed(DateTime FailedAt, string FailureReason) : PaymentDetails;

        // TODO source_of_funds
        [JsonDiscriminator("successful")]
        public record Successful(DateTime AuthorizedAt, DateTime SucceededAt) : PaymentDetails;

        // TODO source_of_funds
        [JsonDiscriminator("settled")]
        public record Settled(DateTime AuthorizedAt, DateTime SucceededAt, DateTime SettledAt) : PaymentDetails;

        [JsonDiscriminator("failed")]
        public record Failed(DateTime AuthorizedAt, DateTime FailedAt, string FailureReason) : PaymentDetails;
    }
}
