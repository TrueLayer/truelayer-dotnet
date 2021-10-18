namespace TrueLayer.Payments.Model
{
    using PaymentMethodUnion = Union<BankTransferPaymentMethod>;
    using BeneficiaryUnion = Union<MerchantAccountBeneficiary, ExternalAccountBeneficiary>;

    /// <summary>
    /// Represents a request for payment
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>
        /// Creates a new <see cref="CreatePaymentRequest"/>
        /// </summary>
        /// <param name="amountInMinor">The payment amount in the minor currency unit</param>
        /// <param name="currency">The three-letter ISO currency code</param>
        /// <param name="paymentMethod">The method of payment</param>
        /// <param name="beneficiary">The payment beneficiary details</param>
        public CreatePaymentRequest(
            long amountInMinor,
            string currency,
            PaymentMethodUnion paymentMethod,
            BeneficiaryUnion beneficiary)
        {
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            PaymentMethod = paymentMethod.NotNull(nameof(paymentMethod));
            Beneficiary = beneficiary;
        }

        /// <summary>
        /// Gets the payment amount in the minor currency unit e.g. cents
        /// </summary>
        public long AmountInMinor { get; }

        /// <summary>
        /// Gets the three-letter ISO currency code
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Gets the method of payment 
        /// </summary>
        public PaymentMethodUnion PaymentMethod { get; }

        /// <summary>
        /// Gets the beneficiary details
        /// </summary>
        public BeneficiaryUnion Beneficiary { get; }
    }
}
