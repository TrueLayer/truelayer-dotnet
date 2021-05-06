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
    }
}
