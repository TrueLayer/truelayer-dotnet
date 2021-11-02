using OneOf;
using static TrueLayer.Payments.Model.Beneficiary;
using static TrueLayer.Payments.Model.PaymentMethod;
using static TrueLayer.Payments.Model.PaymentUser;

namespace TrueLayer.Payments.Model
{
    using BeneficiaryUnion = OneOf<MerchantAccount, ExternalAccount>;
    using PaymentMethodUnion = OneOf<BankTransfer>;
    using UserUnion = OneOf<NewUser, ExistingUser>;

    /// <summary>
    /// Represents a request for payment
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>
        /// Creates a new <see cref="CreatePaymentRequest"/>
        /// </summary>
        /// <param name="amountInMinor">The payment amount in the minor currency unit e.g. cents</param>
        /// <param name="currency">The three-letter ISO alpha currency code</param>
        /// <param name="paymentMethod">The method of payment</param>
        /// <param name="beneficiary">The payment beneficiary details</param>
        /// <param name="user">The end user details</param>
        public CreatePaymentRequest(
            long amountInMinor,
            string currency,
            PaymentMethodUnion paymentMethod,
            BeneficiaryUnion beneficiary,
            UserUnion user)
        {
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            PaymentMethod = paymentMethod;
            Beneficiary = beneficiary;
            User = user;
        }

        /// <summary>
        /// Gets the payment amount in the minor currency unit e.g. cents
        /// </summary>
        public long AmountInMinor { get; }

        /// <summary>
        /// Gets the three-letter ISO currency code
        /// </summary>
        /// <example>EUR</example>
        public string Currency { get; }

        /// <summary>
        /// Gets the method of payment 
        /// </summary>
        public PaymentMethodUnion PaymentMethod { get; }

        /// <summary>
        /// Gets the beneficiary details
        /// </summary>
        public BeneficiaryUnion Beneficiary { get; }

        /// <summary>
        /// Gets the end user details
        /// </summary>
        public UserUnion User { get; }
    }
}
