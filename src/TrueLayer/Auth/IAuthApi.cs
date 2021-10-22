using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Auth
{
    public interface IAuthApi
    {
        /// <summary>
        /// Request an OAuth Access Token using the Client Credentials grant type
        /// </summary>
        /// <param name="authTokenRequest">The authorization token request</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns></returns>
        Task<ApiResponse<GetAuthTokenResponse>> GetAuthToken(GetAuthTokenRequest authTokenRequest, CancellationToken cancellationToken = default);
    }
}
