using OneOf;
using static TrueLayer.Payments.Model.Provider;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    using ProviderUnion = OneOf<UserSelection>;

    /// <summary>
    /// Payment Method types
    /// </summary>
    public static class PaymentMethod
    {
        /// <summary>
        /// Defines a payment via Bank Transfer
        /// </summary>
        [JsonDiscriminator("bank_transfer")]
        public record BankTransfer : IDiscriminated
        {
            /// <summary>
            /// Gets the payment method type
            /// </summary>
            public string Type => "bank_transfer";

            /// <summary>
            /// Gets or sets the provider options
            /// </summary>
            public ProviderUnion Provider { get; init; }
        }
    }
}
