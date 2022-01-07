using System;
using TrueLayer.Serialization;
using TrueLayer.Users.Model;

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
        /// <param name="PaymentToken">The token used to complete the payment via a front-end channel</param>
        /// <param name="User">The end user details</param>
        [JsonDiscriminator("authorization_required")]
        public record AuthorizationRequired(string Id, string PaymentToken, User User);
    }
}
