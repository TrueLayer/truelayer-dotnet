using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Auth
{
    public interface IAuthApi
    {
        ValueTask<ApiResponse<GetAuthTokenResponse>> GetAuthToken(GetAuthTokenRequest authTokenRequest, CancellationToken cancellationToken = default);
    }
}
