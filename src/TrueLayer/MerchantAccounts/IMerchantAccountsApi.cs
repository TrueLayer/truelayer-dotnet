using System.Threading;
using System.Threading.Tasks;
using TrueLayer.MerchantAccounts.Model;

namespace TrueLayer.MerchantAccounts
{
    /// <summary>
    /// Provides access to the TrueLayer Merchant Accounts API
    /// </summary>
    public interface IMerchantAccountsApi
    {
        /// <summary>
        /// List all your TrueLayer merchant accounts. There might be more than one account per currency.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes a list of merchant accounts if successful, otherwise problem details.</returns>
        Task<ApiResponse<ResourceCollection<MerchantAccount>>> ListMerchantAccounts(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the details of a single merchant account.
        /// </summary>
        /// <param name="merchantAccountId">The merchant account identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes the merchant account details if successful, otherwise problem details.</returns>
        Task<ApiResponse<MerchantAccount>> GetMerchantAccount(string merchantAccountId, CancellationToken cancellationToken = default);
    }
}
