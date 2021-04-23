using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrueLayerSdk.Common;
using TrueLayerSdk.Common.CustomHttpResponses;
using TrueLayerSdk.Common.Exceptions;

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
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration.
        /// </summary>
        /// <param name="configuration">The Truelayer configuration required to configure the client.</param>
        public ApiClient(TruelayerConfiguration configuration)
            : this(configuration, new DefaultHttpClientFactory(), new JsonSerializer())
        {
        }
        
        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="configuration">The Truelayer configuration required to configure the client.</param>
        /// <param name="httpClientFactory">A factory for creating HTTP client instances.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>
        public ApiClient(
            TruelayerConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ISerializer serializer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _httpClient = httpClientFactory.CreateClient();
        }
        
        public async Task<TResult> PostAsync<TResult>(string path, CancellationToken cancellationToken,
            Functionality functionality, HttpContent httpContent = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                path: path,
                accessToken: null,
                httpContent: httpContent,
                cancellationToken: cancellationToken,
                functionality: functionality
            );
            return await DeserializeJsonAsync<TResult>(httpResponse);
        }
        public async Task<TResult> PostAsync<TResult>(string path, Functionality functionality, CancellationToken cancellationToken,
            string accessToken, object request = null)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(accessToken)) throw new ArgumentNullException(nameof(accessToken));

            using var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                path: path,
                accessToken: accessToken,
                request: request,
                cancellationToken: cancellationToken,
                functionality: functionality
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
            switch(httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new TruelayerOkApiResponse(httpResponse.Headers);
                case HttpStatusCode.Accepted:
                    return new TruelayerAcceptedApiResponse(httpResponse.Headers);
                // case HttpStatusCode.NoContent:
                //     return new TruelayerNoContentApiResponse(httpResponse.Headers);
                case HttpStatusCode.BadRequest:
                    return new TruelayerBadRequestApiResponse(httpResponse.Headers);
                case HttpStatusCode.Unauthorized:
                    return new TruelayerUnauthorizedApiResponse(httpResponse.Headers);
                // case HttpStatusCode.Forbidden:
                //     return new TruelayerForbiddenApiResponse(httpResponse.Headers);
                // case HttpStatusCode.NotFound:
                //     return new TruelayerNotFoundApiResponse(httpResponse.Headers);                
                // case HttpStatusCode.Conflict:
                //     return new TruelayerConflictApiResponse(httpResponse.Headers);
                // case (HttpStatusCode)422:
                //     return new TruelayerUnprocessableEntityApiResponse(httpResponse.Headers);
                // case (HttpStatusCode)429:
                //     return new TruelayerTooManyRequestsApiResponse(httpResponse.Headers);
                case HttpStatusCode.InternalServerError:
                    return new TruelayerInternalServerErrorApiResponse(httpResponse.Headers);
                // case HttpStatusCode.BadGateway:
                //     return new TruelayerBadGatewayApiResponse(httpResponse.Headers);
                default:
                    throw new NotImplementedException($"Handling a contentless API response with status code {httpResponse.StatusCode} is not implemented.");
            }
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(HttpMethod httpMethod, string path, string accessToken,
            object request, CancellationToken cancellationToken, Functionality functionality)
        {
            HttpContent httpContent = null;
            if (request != null)
            {
                httpContent = new StringContent(_serializer.Serialize(request), Encoding.UTF8, "application/json");
            }
            return SendRequestAsync(httpMethod, path, accessToken, httpContent, cancellationToken, functionality);
        }
        
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, string accessToken,
            HttpContent httpContent, CancellationToken cancellationToken, Functionality functionality)
        {
            const string product = "truelayer-sdk-net";
            var productVersion = "v1";

            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var httpRequest = new HttpRequestMessage(httpMethod, GetRequestUri(path, functionality))
            {
                Content = httpContent
            };

            httpRequest.Headers.UserAgent.ParseAdd($"{product}/{productVersion}");
            if (!string.IsNullOrEmpty(accessToken))
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Logger.Info("{HttpMethod} {Uri}", httpMethod, httpRequest.RequestUri.AbsoluteUri);

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
            await ValidateResponseAsync(httpResponse);

            return httpResponse;
        }
        
        private async Task ValidateResponseAsync(HttpResponseMessage httpResponse)
        {
            await Task.CompletedTask;
            if (!httpResponse.IsSuccessStatusCode)
            {
                httpResponse.Headers.TryGetValues("Tl-Request-Id", out var requestIdHeader);
                var requestId = requestIdHeader?.FirstOrDefault();
                // var content = await httpResponse.Content.ReadAsStringAsync();
                // if (httpResponse.StatusCode == Unprocessable)
                // {
                //     var error = await DeserializeJsonAsync<ErrorResponse>(httpResponse);
                //     throw new CheckoutValidationException(error, httpResponse.StatusCode, requestId);
                // }

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new TruelayerResourceNotFoundException(requestId);

                throw new TruelayerApiException(httpResponse.StatusCode, requestId);
            }
        }
        
        private Uri GetRequestUri(string path, Functionality functionality)
        {
            var baseUri = functionality switch
            {
                Functionality.Auth => new Uri(_configuration.AuthUri),
                Functionality.Data => new Uri(_configuration.DataUri),
                Functionality.Payments => new Uri(_configuration.PaymentsUri),
                _ => throw new ArgumentOutOfRangeException(nameof(functionality), functionality, null)
            };
            Uri.TryCreate(baseUri, path, out var uri);

            return uri;
        }
    }
}
