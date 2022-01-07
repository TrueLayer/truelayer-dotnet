using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.GetPaymentResponse;

namespace TrueLayer.Payments
{
    using GetPaymentUnion = OneOf<
        AuthorizationRequired,
        Authorizing,
        Authorized,
        Executed,
        Settled,
        Failed
    >;

    /// <summary>
    /// Provides access to the TrueLayer Payments API
    /// </summary>
    public interface IPaymentsApi
    {
        /// <summary>
        /// Creates a new payment
        /// </summary>
        /// <param name="paymentRequest">The payment request details</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the created payment if successful, otherwise problem details</returns>
        Task<ApiResponse<OneOf<CreatePaymentResponse.AuthorizationRequired>>> CreatePayment(
            CreatePaymentRequest paymentRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the details of an existing payment
        /// </summary>
        /// <param name="id">The payment identifier</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the payment details if successful, otherwise problem details</returns>
        Task<ApiResponse<GetPaymentUnion>> GetPayment(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a link to the TrueLayer hosted payment page
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="paymentToken">The payment token, returned from <see cref="CreatePayment"/></param>
        /// <param name="returnUri">
        /// Your return URI to which the end user will be redirected after the payment is completed.
        /// Note this should be configured in the TrueLayer console under your application settings.
        /// </param>
        /// <returns>The HPP link you can redirect the end user to</returns>
        string CreateHostedPaymentPageLink(string paymentId, string paymentToken, Uri returnUri);
    }
}
