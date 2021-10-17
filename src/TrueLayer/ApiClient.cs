using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mime;
using TrueLayer.Serialization;
using System.Text.Json;

namespace TrueLayer
{
    /// <summary>
    /// Handles the authentication, serialization and sending of HTTP requests to TrueLayer APIs.
    /// </summary>
    internal class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="httpClient">The client used to make HTTP requests.</param>
        /// <param name="serializer">A serializer used to serialize and deserialize HTTP payloads.</param>
        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public async Task<ApiResponse<TData>> GetAsync<TData>(Uri uri, string? accessToken = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Get,
                uri: uri,
                idempotencyKey: null,
                accessToken: accessToken,
                httpContent: null,
                signature: null,
                cancellationToken: cancellationToken
            );

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken) ?? throw new ArgumentNullException();
        }

        /// <inheritdoc />
        public async Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, HttpContent? httpContent = null, string? accessToken = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                idempotencyKey: null,
                accessToken: accessToken,
                httpContent: httpContent,
                signature: null,
                cancellationToken: cancellationToken
            );

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken) ?? throw new ArgumentNullException();
        }

        /// <inheritdoc />
        public async Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null, SigningKey? signingKey = null, CancellationToken cancellationToken = default)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            using var httpResponse = await SendJsonRequestAsync(
                httpMethod: HttpMethod.Post,
                uri: uri,
                idempotencyKey: idempotencyKey,
                accessToken: accessToken,
                request: request,
                signingKey: signingKey,
                cancellationToken: cancellationToken
            );

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken) ?? throw new ArgumentNullException();
        }

        private async Task<ApiResponse<TData>> CreateResponseAsync<TData>(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            httpResponse.Headers.TryGetValues(CustomHeaders.TraceId, out var traceIdHeader);
            string? traceId = traceIdHeader?.FirstOrDefault();

            if (httpResponse.IsSuccessStatusCode)
            {
                var data = await DeserializeJsonAsync<TData>(httpResponse, traceId, cancellationToken);
                return new ApiResponse<TData>(data, httpResponse.StatusCode, traceId);
            }

            if (httpResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
            {
                var problemDetails = await DeserializeJsonAsync<ProblemDetails>(httpResponse, traceId, cancellationToken);
                return new ApiResponse<TData>(problemDetails, httpResponse.StatusCode, traceId);
            }

            return new ApiResponse<TData>(httpResponse.StatusCode, traceId);
        }

        private async Task<TData> DeserializeJsonAsync<TData>(HttpResponseMessage httpResponse, string? traceId, CancellationToken cancellationToken)
        {
            string? json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new TrueLayerApiException(httpResponse.StatusCode, traceId, additionalInformation: "The response body cannot be null");
            }

            return JsonSerializer.Deserialize<TData>(json, SerializerOptions.Default)
                ?? throw new JsonException($"Failed to deserialize to type {typeof(TData).Name}");
        }

        private Task<HttpResponseMessage> SendJsonRequestAsync(
            HttpMethod httpMethod,
            Uri uri,
            string? idempotencyKey,
            string? accessToken,
            object? request,
            SigningKey? signingKey,
            CancellationToken cancellationToken)
        {
            HttpContent? httpContent = null;
            string? signature = null;

            if (request is { })
            {
                string json = Serialize(request);

                if (signingKey != null)
                {
                    signature = RequestSignature.Create(
                        signingKey,
                        httpMethod,
                        uri,
                        json,
                        idempotencyKey
                    );
                }

                httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            }

            return SendRequestAsync(httpMethod, uri, idempotencyKey, accessToken, httpContent, signature, cancellationToken);
        }

        private Task<HttpResponseMessage> SendRequestAsync(
            HttpMethod httpMethod,
            Uri uri,
            string? idempotencyKey,
            string? accessToken,
            HttpContent? httpContent,
            string? signature,
            CancellationToken cancellationToken)
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

            if (!string.IsNullOrWhiteSpace(signature))
            {
                httpRequest.Headers.Add(CustomHeaders.Signature, signature);
            }

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                httpRequest.Headers.Add(CustomHeaders.IdempotencyKey, idempotencyKey);
            }

            return _httpClient.SendAsync(httpRequest, cancellationToken);
        }

        public static string Serialize(object value)
            => JsonSerializer.Serialize(value, value.GetType(), SerializerOptions.Default);
    }
}
