using System;

namespace TrueLayer.Payments.Model
{
    public record InitiatePaymentResponse
    {
        public InitiatePaymentResponseResult Result { get; init; } = null!;
    }

    public record InitiatePaymentResponseResult
    {
        public SingleImmediatePaymentResponse SingleImmediatePayment { get; init; } = null!;
        public AuthFlowResponse AuthFlow { get; init; } = null!;
    }

    public record SingleImmediatePaymentResponse
    {
        public Guid SingleImmediatePaymentId { get; init; }
        public DateTimeOffset InitiatedAt { get; init; }

        /// <summary>
        /// Payment status.
        /// </summary>
        public string Status { get; init; } = null!;

        /// <summary>
        /// The id of the provider, as given on our /providers endpoint.
        /// </summary>
        /// <value></value>
        public string ProviderId { get; init; } = null!;

        /// <summary>
        /// The id of the scheme to make the payment over. This must be one of the schemes advertised by the provider from the provider results.
        /// </summary>
        /// <value></value>
        public string SchemeId { get; init; } = null!;

        /// <summary>
        /// The amount, specified in terms of the fractional monetary unit of the payment currency, to be paid.
        /// </summary>
        /// <value></value>
        public long AmountInMinor { get; init; }

        /// <summary>
        /// The ISO 4217 code of the currency for the payment.
        /// </summary>
        /// <value></value>
        public string Currency { get; init; } = null!;

        /// <summary>
        /// Beneficiary data.
        /// </summary>
        /// <value></value>
        public Beneficiary Beneficiary { get; set; } = null!;

        /// <summary>
        /// Remitter data.
        /// </summary>
        /// <value></value>
        public Remitter? Remitter { get; set; }

        /// <summary>
        /// Payment references.
        /// </summary>
        /// <value></value>
        public References? References { get; set; }
    }

    public record AuthFlowResponse
    {
        /// <summary>
        /// Value indicating the type of authorisation flow, i.e. redirect
        /// </summary>
        public string Type { get; init; } = null!;

        /// <summary>
        /// The URI for the end user to authorise the payment in the bank’s UI.
        /// </summary>
        public string Uri { get; set; } = null!;

        /// <summary>
        /// An expiry time, if one is known, after which the URI to authorise the payment will become invalid. In UTC, 
        /// formatted as an ISO 8601 string (YYYY-MM-DDThh:mm:ss.sssZ).
        /// </summary>
        public DateTime Expiry { get; set; }
    }
}
