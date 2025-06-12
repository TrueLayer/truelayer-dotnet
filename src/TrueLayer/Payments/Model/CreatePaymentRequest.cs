using System.Collections.Generic;
using OneOf;
using TrueLayer.Payments.Model.AuthorizationFlow;
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
        /// <param name="authorizationFlow">The authorization flow parameter.
        /// If provided, the start authorization flow endpoint does not need to be called</param>
        /// <param name="metadata">Add to the payment a list of custom key-value pairs as metadata</param>
        /// <param name="riskAssessment">The risk assessment and the payment_creditable webhook configuration.</param>
        /// <param name="subMerchants">The details related to any applicable sub-merchants</param>
        public CreatePaymentRequest(
            long amountInMinor,
            string currency,
            PaymentMethodUnion paymentMethod,
            PaymentUserRequest? user = null,
            RelatedProducts? relatedProducts = null,
            StartAuthorizationFlowRequest? authorizationFlow = null,
            Dictionary<string, string>? metadata = null,
            RiskAssessment? riskAssessment = null,
            PaymentSubMerchants? subMerchants = null)
        {
            AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
            Currency = currency.NotNullOrWhiteSpace(nameof(currency));
            PaymentMethod = paymentMethod;
            User = user;
            RelatedProducts = relatedProducts;
            AuthorizationFlow = authorizationFlow;
            Metadata = metadata;
            RiskAssessment = riskAssessment;
            SubMerchants = subMerchants;
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

        /// <summary>
        /// Gets the payments authorization flow request
        /// </summary>
        public StartAuthorizationFlowRequest? AuthorizationFlow { get; }

        /// <summary>
        /// Gets the metadata for the payment
        /// </summary>
        public Dictionary<string, string>? Metadata { get; }

        /// <summary>
        /// Gets the risk assessment configuration
        /// </summary>
        public RiskAssessment? RiskAssessment { get; }

        /// <summary>
        /// Gets the sub-merchants details
        /// </summary>
        public PaymentSubMerchants? SubMerchants { get; }
    }
}
