using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    public static class PaymentMethod
    {
        /// <summary>
        /// Used to request payments via bank transfer
        /// </summary>
        [JsonDiscriminator("bank_transfer")]
        public class BankTransfer : IDiscriminated
        {
            public string Type => "bank_transfer";
            public string? StatementReference { get; set; }
            public ProviderFilter? ProviderFilter { get; set; }
        }
    }
}
