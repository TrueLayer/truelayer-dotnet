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
    }
}
