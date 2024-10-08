using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer
{
    /// <summary>
    /// Defines the operations used for communicating with TrueLayer.com APIs.
    /// </summary>
    internal interface IApiClient
    {
        /// <summary>
        /// Executes a GET request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="customHeaders">Custom headers that should be included in the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TData">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<ApiResponse<TData>> GetAsync<TData>(
            Uri uri,
            string? accessToken = null,
            IDictionary<string, string>? customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="httpContent">Optional data that should be sent in the request body.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TData">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, HttpContent? httpContent = null, string? accessToken = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="idempotencyKey">Unique identifier for the request that allows it to be safely retried.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="signingKey">ES512 signing key used to sign the request with a JSON Web Signature</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <typeparam name="TData">The expected response type to be deserialized.</typeparam>
        /// <returns>A task that upon completion contains the specified API response data.</returns>
        Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null, SigningKey? signingKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="httpContent">Optional data that should be sent in the request body.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the API content-less response.</returns>
        Task<ApiResponse> PostAsync(Uri uri, HttpContent? httpContent = null, string? accessToken = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The API resource path.</param>
        /// <param name="request">Optional data that should be sent in the request body.</param>
        /// <param name="idempotencyKey">Unique identifier for the request that allows it to be safely retried.</param>
        /// <param name="accessToken">The access token used to authenticate the request.</param>
        /// <param name="signingKey">ES512 signing key used to sign the request with a JSON Web Signature</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the underlying HTTP request.</param>
        /// <returns>A task that upon completion contains the API content-less response.</returns>
        Task<ApiResponse> PostAsync(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null, SigningKey? signingKey = null, CancellationToken cancellationToken = default);
    }
}
