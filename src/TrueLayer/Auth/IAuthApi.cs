using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Auth;

/// <summary>
/// Provides access to the TrueLayer Authorization API
/// </summary>
public interface IAuthApi
{
    /// <summary>
    /// Request an OAuth Access Token using the Client Credentials grant type
    /// </summary>
    /// <param name="authTokenRequest">The authorization token request</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>An API response that includes the authorization token if successful, otherwise problem details</returns>
    ValueTask<ApiResponse<GetAuthTokenResponse>> GetAuthToken(GetAuthTokenRequest authTokenRequest, CancellationToken cancellationToken = default);
}