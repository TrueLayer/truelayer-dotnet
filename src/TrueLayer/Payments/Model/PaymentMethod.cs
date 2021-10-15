namespace TrueLayer.Payments.Model
{
    public interface IPaymentMethod
    {
        string Type { get; }
    }

    public static class PaymentMethod
    {
        public static BankTransferPaymentMethod BankTransfer(string? statementReference = null, ProviderFilter? providerFilter = null)
            => new BankTransferPaymentMethod
            {
                StatementReference = statementReference,
                ProviderFilter = providerFilter
            };
    }

    /// <summary>
    /// Used to request payments via bank transfer
    /// </summary>
    public class BankTransferPaymentMethod : IPaymentMethod
    {
        public string Type => "bank_transfer";
        public string? StatementReference { get; set; }
        public ProviderFilter? ProviderFilter { get; set; }
    }
}
