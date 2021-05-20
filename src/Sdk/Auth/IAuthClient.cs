using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Auth.Model;

namespace TrueLayer.Auth
{
    public interface IAuthClient
    {        
        Task<GetAuthUriResponse> GetAuthUri(GetAuthUriRequest request);
        Task<ExchangeCodeResponse> ExchangeCode(ExchangeCodeRequest request, CancellationToken cancellationToken = default);
        Task<AuthTokenResponse> GetPaymentToken(CancellationToken cancellationToken = default);
        Task <AuthTokenResponse> GetOAuthToken(string[] scopes, CancellationToken cancellationToken = default);
    }
}
