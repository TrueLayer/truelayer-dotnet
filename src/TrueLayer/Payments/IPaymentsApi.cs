using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using TrueLayer.Payments.Model.AuthorizationFlow;

namespace TrueLayer.Payments
{
    using AuthorizationResponseUnion = OneOf<
        AuthorizationFlowResponse.AuthorizationFlowAuthorizing,
        AuthorizationFlowResponse.AuthorizationFlowAuthorizationFailed
    >;
    using CreatePaymentUnion = OneOf<
        CreatePaymentResponse.AuthorizationRequired,
        CreatePaymentResponse.Authorized,
        CreatePaymentResponse.Failed,
        CreatePaymentResponse.Authorizing
    >;
    using GetPaymentUnion = OneOf<
        GetPaymentResponse.AuthorizationRequired,
        GetPaymentResponse.Authorizing,
        GetPaymentResponse.Authorized,
        GetPaymentResponse.Executed,
        GetPaymentResponse.Settled,
        GetPaymentResponse.Failed
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
        Task<ApiResponse<CreatePaymentUnion>> CreatePayment(
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

        /// <summary>
        /// Start the authorization flow for a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="request">The start authorization request details</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns></returns>
        Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(
            string paymentId,
            string idempotencyKey,
            StartAuthorizationFlowRequest request,
            CancellationToken cancellationToken = default);

        //TODO: doc
        //TODO: empty response?
        Task<ApiResponse> CreatePaymentRefund(string paymentId, string idempotencyKey, CreatePaymentRefundRequest createPaymentRefundRequest);

        //TODO: doc
        Task<ApiResponse<ListPaymentRefundsResponse>> ListPaymentRefunds(string paymentId);

        //TODO: doc
        Task<ApiResponse<Refund>> GetPaymentRefund(string paymentId, string refundId);
    }
}
