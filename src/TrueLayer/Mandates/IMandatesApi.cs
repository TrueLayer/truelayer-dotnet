using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Mandates
{
    using TrueLayer.Mandates.Model;

    /// <summary>
    /// Provides access to the TrueLayer Payments API
    /// </summary>
    public interface IMandatesApi
    {
        /// <summary>
        /// Creates a new mandate
        /// </summary>
        /// <param name="mandateRequest">The payment request details</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the created payment if successful, otherwise problem details</returns>
        Task<ApiResponse<CreateMandateResponse>> CreateMandate(
            CreateMandateRequest mandateRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revoke mandate
        /// </summary>
        /// <param name="id">The id of the mandate</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the payment details if successful, otherwise problem details</returns>
        Task<ApiResponse<Task>> RevokeMandate(string id, string idempotencyKey, CancellationToken cancellationToken = default);
    }
}
