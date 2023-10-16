using OneOf;
using static TrueLayer.Payments.Model.PaymentMethod;

namespace TrueLayer.Payments.Model
{
    using PaymentMethodUnion = OneOf<BankTransfer, Mandate>;

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
        /// <param name="user">The end user details</param>
        /// <param name="relatedProducts">Related products</param>
        public CreatePaymentRequest(
            long amountInMinor,
            string currency,
            PaymentMethodUnion paymentMethod,
            PaymentUserRequest? user,
            RelatedProducts? relatedProducts)
        {
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            PaymentMethod = paymentMethod;
            User = user;
            RelatedProducts = relatedProducts;
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
        /// Gets the end user details
        /// </summary>
        public PaymentUserRequest? User { get; }

        /// <summary>
        /// Gets the related products
        /// </summary>
        public RelatedProducts? RelatedProducts { get; }
    }
}
