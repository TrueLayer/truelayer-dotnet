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
        GetPaymentResponse.Failed,
        GetPaymentResponse.AttemptFailed
    >;

    using RefundUnion = OneOf<RefundPending, RefundAuthorized, RefundExecuted, RefundFailed>;


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
        /// If not provided an idempotency key is automatically generated.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the created payment if successful, otherwise problem details</returns>
        Task<ApiResponse<CreatePaymentUnion>> CreatePayment(
            CreatePaymentRequest paymentRequest,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the details of an existing payment
        /// </summary>
        /// <param name="id">The payment identifier</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the payment details if successful, otherwise problem details</returns>
        Task<ApiResponse<GetPaymentUnion>> GetPayment(
            string id,
            CancellationToken cancellationToken = default);

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
        string CreateHostedPaymentPageLink(
            string paymentId,
            string paymentToken,
            Uri returnUri);

        /// <summary>
        /// Start the authorization flow for a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// If not provided an idempotency key is automatically generated.
        /// </param>
        /// <param name="request">The start authorization request details</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns></returns>
        Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(
            string paymentId,
            string? idempotencyKey,
            StartAuthorizationFlowRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a partial or full refund for a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// If not provided an idempotency key is automatically generated.
        /// </param>
        /// <param name="request">The create payment refund request</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>The id of the created refund</returns>
        Task<ApiResponse<CreatePaymentRefundResponse>> CreatePaymentRefund(
            string paymentId,
            string? idempotencyKey,
            CreatePaymentRefundRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the list of all refunds for a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>The list of refunds for a payment.</returns>
        Task<ApiResponse<ListPaymentRefundsResponse>> ListPaymentRefunds(
            string paymentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific refund for a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="refundId">The refund identifier</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>The details of the selected refund</returns>
        Task<ApiResponse<RefundUnion>> GetPaymentRefund(
            string paymentId,
            string refundId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel a payment.
        /// </summary>
        /// <param name="paymentId">The payment identifier</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// If not provided an idempotency key is automatically generated.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>HTTP 202 Accepted if successful, otherwise problem details.</returns>
        Task<ApiResponse> CancelPayment(
            string paymentId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);
    }
}
