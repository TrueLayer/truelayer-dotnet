using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Merchants.Model;

namespace TrueLayer.Merchants
{
    /// <summary>
    /// Provides access to the TrueLayer Merchant Accounts API
    /// </summary>
    public interface IMerchantsApi
    {
        /// <summary>
        /// List all your TrueLayer's merchant accounts. There might be more than one account per currency.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes a list of merchant accounts if successful, otherwise problem details.</returns>
        Task<ApiResponse<ListMerchantsResponse>> ListMerchants(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the details of a single merchant account.
        /// </summary>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes the merchant account details if successful, otherwise problem details.</returns>
        Task<ApiResponse<MerchantAccount>> GetMerchant(string merchantId, CancellationToken cancellationToken = default);
    }
}
