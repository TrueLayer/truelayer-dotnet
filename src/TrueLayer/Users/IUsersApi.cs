using System.Threading;
using System.Threading.Tasks;
using TrueLayer.Users.Model;

namespace TrueLayer.Users
{
    public interface IUsersApi
    {
        /// <summary>
        /// Get the details of a user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An API response that includes details of a user.</returns>
        Task<ApiResponse<GetUserResponse>> GetUser(string id, CancellationToken cancellationToken = default);
    }
}
