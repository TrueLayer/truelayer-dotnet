using System;
using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Create Payment Response types
    /// </summary>
    public static class CreatePaymentResponse
    {
        /// <summary>
        /// Returned when a payment requires further authorization
        /// </summary>
        /// <param name="Id">The unique identifier of the payment</param>
        /// <param name="AmountInMinor">The payment amount in the minor currency unit e.g. cents</param>
        /// <param name="Currency">The three-letter ISO alpha currency code</param>
        /// <param name="Status">The status of the payment</param>
        /// <param name="CreatedAt">The data and time the payment was created</param>
        /// <param name="ResourceToken">The resource token used to complete the payment via a front-end channel</param>
        /// <returns></returns>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired(string Id, long AmountInMinor, string Currency, string Status, DateTime CreatedAt, string ResourceToken);
    }
}
