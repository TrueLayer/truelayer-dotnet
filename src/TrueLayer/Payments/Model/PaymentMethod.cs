using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class PaymentMethod
    {
        /// <summary>
        /// Used to request payments via bank transfer
        /// </summary>
        [JsonDiscriminator("bank_transfer")]
        public record BankTransfer : IDiscriminated
        {
            public string Type => "bank_transfer";
            public string? StatementReference { get; init; }
            public ProviderFilter? ProviderFilter { get; init; }
        }
    }
}
