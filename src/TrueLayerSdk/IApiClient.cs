using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Common;

namespace TrueLayerSdk
{
    /// <summary>
    /// Defines the operations used for communicating with Truelayer.com APIs.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Executes a POST request to the specified <paramref name="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="functionality">Target api platform</param>
        /// <param name="httpContent">Optional data that should be sent in the request body.</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> PostAsync<TResult>(string path, CancellationToken cancellationToken, Functionality functionality, HttpContent httpContent = null);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="path"/>. 
        /// </summary>
        /// <param name="path">The API resource path.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="functionality">Target api platform</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> PostAsync<TResult>(string path, Functionality functionality, CancellationToken cancellationToken,
            string accessToken, object request = null);
    }
}
