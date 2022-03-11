using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payouts.Model;
using OneOf;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

namespace TrueLayer.Payouts
{
    using GetPayoutUnion = OneOf<
        Pending,
        Authorized,
        Successful,
        Failed
    >;

    /// <summary>
    /// Provides access to the TrueLayer Payouts API
    /// </summary>
    public interface IPayoutsApi
    {
        /// <summary>
        /// Creates a new payout
        /// </summary>
        /// <param name="payoutRequest">The payout request details</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the created payout if successful, otherwise problem details</returns>
        Task<ApiResponse<CreatePayoutResponse>> CreatePayout(
            CreatePayoutRequest payoutRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the details of an existing payment
        /// </summary>
        /// <param name="id">The payout identifier</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the payout details if successful, otherwise problem details</returns>
        Task<ApiResponse<GetPayoutUnion>> GetPayout(string id, CancellationToken cancellationToken = default);
    }
}
