using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mime;
using TrueLayer.Serialization;

namespace TrueLayer
{
    /// <summary>
    /// Handles the authentication, serialization and sending of HTTP requests to Truelayer APIs.
    /// </summary>
    internal class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        
        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="httpClient">The client used to make HTTP requests.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>
        public ApiClient(HttpClient httpClient, ISerializer serializer)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        /// <inheritdoc />
        public async Task<TResult> GetAsync<TResult>(Uri uri, string accessToken = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Get,
                uri: uri,
                accessToken: accessToken,
                httpContent: null,
                cancellationToken: cancellationToken
            );

            return await DeserializeJsonAsync<TResult>(httpResponse, cancellationToken);
        }
        
        /// <inheritdoc />
        public async Task<TResult> PostAsync<TResult>(Uri uri, HttpContent httpContent = null, string accessToken = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                accessToken: accessToken,
                httpContent: httpContent,
                cancellationToken: cancellationToken
            );

            return await DeserializeJsonAsync<TResult>(httpResponse, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResult> PostAsync<TResult>(Uri uri, object request = null, string accessToken = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                accessToken: accessToken,
                request: request,
                cancellationToken: cancellationToken
            );

            return await DeserializeJsonAsync<TResult>(httpResponse, cancellationToken);
        }

        private async Task<TResult> DeserializeJsonAsync<TResult>(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            string json = await httpResponse.Content?.ReadAsStringAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return (TResult)_serializer.Deserialize(json, typeof(TResult));
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod httpMethod, Uri uri, string accessToken, 
            object request, CancellationToken cancellationToken)
        {
            HttpContent httpContent = null;
            
            if (request is {})
            {
                httpContent = new StringContent(_serializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json);
            }
            
            return SendRequestAsync(httpMethod, uri, accessToken, httpContent, cancellationToken);
        }
        
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, Uri uri, string accessToken,
            HttpContent httpContent, CancellationToken cancellationToken)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            var httpRequest = new HttpRequestMessage(httpMethod, uri)
            {
                Content = httpContent
            };

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await ValidateResponseAsync(httpResponse, cancellationToken);

            return httpResponse;
        }
        
        private async Task ValidateResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                httpResponse.Headers.TryGetValues(CustomHeaders.RequestId, out var requestIdHeader);
                string requestId = requestIdHeader?.FirstOrDefault();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        throw new TrueLayerResourceNotFoundException(requestId);
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.UnprocessableEntity:
                        var errorResponse = await DeserializeJsonAsync<ErrorResponse>(httpResponse, cancellationToken);
                        throw new TrueLayerValidationException(errorResponse, httpResponse.StatusCode, requestId);
                    default:
                        throw new TrueLayerApiException(httpResponse.StatusCode, requestId);
                }
            }
        }
    }
}
