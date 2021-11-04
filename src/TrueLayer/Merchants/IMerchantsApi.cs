using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Merchants.Model;

namespace TrueLayer.Merchants
{
    public interface IMerchantsApi
    {
        Task<ApiResponse<ListMerchantsResponse>> ListMerchants(CancellationToken cancellationToken = default);
        Task<ApiResponse<MerchantAccount>> GetMerchant(string merchantId, CancellationToken cancellationToken = default);
    }
}
