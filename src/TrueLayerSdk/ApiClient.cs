using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Common.Exceptions;
using TrueLayerSdk.Common.Serialization;
using System.Net.Mime;

namespace TrueLayerSdk
{
    /// <summary>
    /// Handles the authentication, serialization and sending of HTTP requests to Truelayer APIs.
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly TruelayerConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        
        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="configuration">The Truelayer configuration required to configure the client.</param>
        /// <param name="httpClient">The client used to make HTTP requests.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>
        public ApiClient(
            TruelayerConfiguration configuration,
            HttpClient httpClient,
            ISerializer serializer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        /// <inheritdoc />
        public async Task<TResult> GetAsync<TResult>(Uri uri, string accessToken, CancellationToken cancellationToken = default)
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
        public async Task<TResult> PostAsync<TResult>(Uri uri, HttpContent httpContent = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                accessToken: null,
                httpContent: httpContent,
                cancellationToken: cancellationToken
            );

            return await DeserializeJsonAsync<TResult>(httpResponse, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResult> PostAsync<TResult>(Uri uri, string accessToken, object request = null, CancellationToken cancellationToken = default)
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
            
            if (json is null)
            {
                return default;
            }

            return (TResult)_serializer.Deserialize(json, typeof(TResult));
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod httpMethod, Uri uri, string accessToken, 
            object request, CancellationToken cancellationToken)
        {
            HttpContent httpContent = null;
            
            if (request is null)
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

            // Logger.Info("{HttpMethod} {Uri}", httpMethod, httpRequest.RequestUri.AbsoluteUri);
            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await ValidateResponseAsync(httpResponse);

            return httpResponse;
        }
        
        private async Task ValidateResponseAsync(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                httpResponse.Headers.TryGetValues("Tl-Request-Id", out var requestIdHeader);
                var requestId = requestIdHeader?.FirstOrDefault();
                // var content = await httpResponse.Content.ReadAsStringAsync();
                // if (httpResponse.StatusCode == Unprocessable)
                // {
                //     var error = await DeserializeJsonAsync<ErrorResponse>(httpResponse);
                //     throw new TruelayerValidationException(error, httpResponse.StatusCode, requestId);
                // }

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new TruelayerResourceNotFoundException(requestId);

                throw new TruelayerApiException(httpResponse.StatusCode, requestId);
            }
        }
    }
}
