using System.Collections.Generic;
using System.Threading.Tasks;
using TrueLayer.PaymentsProviders.Model;

namespace TrueLayer.PaymentsProviders
{
    /// <summary>
    /// Provides access to the TrueLayer Payments Providers API
    /// </summary>
    public interface IPaymentsProvidersApi
    {
        /// <summary>
        /// Gets the details of a payments provider
        /// </summary>
        /// <param name="id">The provider identifier</param>
        /// <returns>An API response that includes the payments provider details if successful, otherwise problem details</returns>
        Task<ApiResponse<PaymentsProvider>> GetPaymentsProvider(string id);

        /// <summary>
        /// Search for payments providers matching the given criteria
        /// </summary>
        /// <param name="searchPaymentsProvidersRequest">The provider search request</param>
        /// <returns>An API response that includes all the providers that match the criteria specified on the request</returns>
        Task<ApiResponse<SearchPaymentsProvidersResponse>> SearchPaymentsProviders(SearchPaymentsProvidersRequest searchPaymentsProvidersRequest);
    }
}
