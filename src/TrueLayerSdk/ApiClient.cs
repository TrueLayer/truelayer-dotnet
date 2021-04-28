using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Common.CustomHttpResponses;
using TrueLayerSdk.Common.Exceptions;
using TrueLayerSdk.Common.Serialization;

namespace TrueLayerSdk
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
        
        public async Task<TResult> GetAsync<TResult>(Uri uri, string accessToken, CancellationToken cancellationToken)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Get,
                uri: uri,
                accessToken: accessToken,
                httpContent: null,
                cancellationToken: cancellationToken
            );
            return await DeserializeJsonAsync<TResult>(httpResponse);
        }
        
        public async Task<TResult> PostAsync<TResult>(Uri uri, CancellationToken cancellationToken,
            HttpContent httpContent = null)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                accessToken: null,
                httpContent: httpContent,
                cancellationToken: cancellationToken
            );
            return await DeserializeJsonAsync<TResult>(httpResponse);
        }
        public async Task<TResult> PostAsync<TResult>(Uri uri, CancellationToken cancellationToken,
            string accessToken, object request = null)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));

            using var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                accessToken: accessToken,
                request: request,
                cancellationToken: cancellationToken
            );
            return await DeserializeJsonAsync<TResult>(httpResponse);
        }

        private async Task<TResult> DeserializeJsonAsync<TResult>(HttpResponseMessage httpResponse)
        {
            var result = await DeserializeJsonAsync(httpResponse, typeof(TResult));
            return (TResult)result;
        }
        
        private async Task<dynamic> DeserializeJsonAsync(HttpResponseMessage httpResponse, Type resultType)
        {
            // TODO: chech why expression is always false
            // if (httpResponse.Content == null)
            //     return null;

            var json = await httpResponse.Content.ReadAsStringAsync();
            if(!string.IsNullOrWhiteSpace(json))
                return _serializer.Deserialize(json, resultType);
            
            return httpResponse.StatusCode switch
            {
                HttpStatusCode.OK => new TruelayerOkApiResponse(httpResponse.Headers),
                HttpStatusCode.Accepted => new TruelayerAcceptedApiResponse(httpResponse.Headers),
                HttpStatusCode.NoContent => new TruelayerNoContentApiResponse(httpResponse.Headers),
                HttpStatusCode.BadRequest => new TruelayerBadRequestApiResponse(httpResponse.Headers),
                HttpStatusCode.Unauthorized => new TruelayerUnauthorizedApiResponse(httpResponse.Headers),
                HttpStatusCode.Forbidden => new TruelayerForbiddenApiResponse(httpResponse.Headers),
                HttpStatusCode.NotFound => new TruelayerNotFoundApiResponse(httpResponse.Headers),
                HttpStatusCode.Conflict => new TruelayerConflictApiResponse(httpResponse.Headers),
                (HttpStatusCode) 422 => new TruelayerUnprocessableEntityApiResponse(httpResponse.Headers),
                (HttpStatusCode) 429 => new TruelayerTooManyRequestsApiResponse(httpResponse.Headers),
                HttpStatusCode.InternalServerError => new TruelayerInternalServerErrorApiResponse(httpResponse.Headers),
                HttpStatusCode.BadGateway => new TruelayerBadGatewayApiResponse(httpResponse.Headers),
                _ => throw new NotImplementedException(
                    $"Handling a contentless API response with status code {httpResponse.StatusCode} is not implemented.")
            };
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod httpMethod, Uri uri, string accessToken,
            object request, CancellationToken cancellationToken)
        {
            HttpContent httpContent = null;
            if (request != null)
            {
                httpContent = new StringContent(_serializer.Serialize(request), Encoding.UTF8, "application/json");
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

            if (!string.IsNullOrEmpty(accessToken))
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Logger.Info("{HttpMethod} {Uri}", httpMethod, httpRequest.RequestUri.AbsoluteUri);
            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            ValidateResponseAsync(httpResponse);

            return httpResponse;
        }
        
        private void ValidateResponseAsync(HttpResponseMessage httpResponse)
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
