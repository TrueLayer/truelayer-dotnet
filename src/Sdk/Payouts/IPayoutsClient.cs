using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Payouts.Model;

namespace TrueLayer.Payouts
{
    /// <summary>
    /// Client for the TrueLayer Payouts API
    /// </summary>
    public interface IPayoutsClient
    {
        /// <summary>
        /// Retrieves the details and balances of accounts available to perform payouts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<QueryResponse<AccountBalance>> GetAccountBalances(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Initiates a payout
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task InitiatePayout(InitiatePayoutRequest request, CancellationToken cancellation = default);

        /// <summary>
        /// Validates the configured signing key
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ValidateSigningKey(CancellationToken cancellationToken = default);
    }
}
