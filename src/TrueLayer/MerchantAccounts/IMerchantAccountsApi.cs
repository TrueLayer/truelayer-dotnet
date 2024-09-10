using System;
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
        /// <param name="id">The merchant account identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes the merchant account details if successful, otherwise problem details.</returns>
        Task<ApiResponse<MerchantAccount>> GetMerchantAccount(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the details of a user.
        /// </summary>
        /// <param name="merchantAccountId">The merchant account identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes details of a user.</returns>
        Task<ApiResponse<GetPaymentSourcesResponse>> GetPaymentSources(
            string merchantAccountId,
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the transactions of a single merchant account.
        /// </summary>
        /// <param name="merchantAccountId">The merchant account identifier.</param>
        /// <param name="from">The start date of the transaction.</param>
        /// <param name="to">The end date of the transaction.</param>
        /// <param name="getPaginatedResult">Indicated whether the result has to be paginated.</param>
        /// <param name="cursor">Cursor used for pagination purposes, returned as next_cursor in the response payload of the inital request. Not required to access the first page of items.</param>
        /// <param name="type">Filters transactions by payments or payouts (see <see cref="MerchantAccountTransactions.TransactionTypes"/>). If omitted, both payments and payouts are returned.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns></returns>
        Task<ApiResponse<GetTransactionsResponse>> GetTransactions(
            string merchantAccountId,
            DateTimeOffset from,
            DateTimeOffset to,
            bool getPaginatedResult = true,
            string? cursor = null,
            string? type = null,
            CancellationToken cancellationToken = default);
    }
}
