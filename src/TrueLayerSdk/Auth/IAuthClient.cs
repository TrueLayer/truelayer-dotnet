using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth.Model;

namespace TrueLayerSdk.Auth
{
    public interface IAuthClient
    {
        Task<GetAuthUriResponse> GetAuthUri(GetAuthUriRequest request);
        Task<ExchangeCodeResponse> ExchangeCode(ExchangeCodeRequest request, CancellationToken cancellationToken = default);
        Task<GetPaymentTokenResponse> GetPaymentToken(GetPaymentTokenRequest request, CancellationToken cancellationToken = default);
    }
}
