using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrueLayerSdk
{
    /// <summary>
    /// Defines the operations used for communicating with Truelayer.com APIs.
    /// </summary>
    internal interface IApiClient
    {
        /// <summary>
        /// Executes a GET request to the specified <paramref name="uri"/>. 
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> GetAsync<TResult>(Uri uri, string accessToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>. 
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="httpContent">Optional data that should be sent in the request body.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> PostAsync<TResult>(Uri uri, HttpContent httpContent = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>. 
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TResult">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<TResult> PostAsync<TResult>(Uri uri, string accessToken, object request = null, CancellationToken cancellationToken = default);
    }
}
