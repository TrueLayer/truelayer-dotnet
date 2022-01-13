using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
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
            /// Gets or sets the filter used to determine the banks that should be displayed on the bank selection screen
            /// </summary>
            public ProviderFilter? ProviderFilter { get; init; }
        }
    }
}
