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
using TrueLayer.Signing;
using System.Xml.Linq;
#if NET6_0 || NET6_0_OR_GREATER
using System.Net.Http.Json;
#endif

namespace TrueLayer
{
    using System.Net;

    /// <summary>
    /// Handles the authentication, serialization and sending of HTTP requests to TrueLayer APIs.
    /// </summary>
    internal class ApiClient : IApiClient
    {
        private static readonly ProductInfoHeaderValue UserAgentHeader
            = new("truelayer-dotnet", ReflectionUtils.GetAssemblyVersion<ITrueLayerClient>());

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
        /// </summary>
        /// <param name="httpClient">The client used to make HTTP requests.</param>
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

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken);
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

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken);
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

            return await CreateResponseAsync<TData>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> PostAsync(Uri uri, HttpContent? httpContent = null, string? accessToken = null, CancellationToken cancellationToken = default)
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

            return await CreateResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> PostAsync(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null, SigningKey? signingKey = null, CancellationToken cancellationToken = default)
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

            return await CreateResponseAsync(httpResponse, cancellationToken);
        }

        private async Task<ApiResponse<TData>> CreateResponseAsync<TData>(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            httpResponse.Headers.TryGetValues(CustomHeaders.TraceId, out var traceIdHeader);
            string? traceId = traceIdHeader?.FirstOrDefault();

            if (httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != HttpStatusCode.NoContent)
            {
                var data = await DeserializeJsonAsync<TData>(httpResponse, traceId, cancellationToken);
                return new ApiResponse<TData>(data, httpResponse.StatusCode, traceId);
            }

            // In .NET Standard 2.1 HttpResponse.Content can be null
            if (httpResponse.Content?.Headers.ContentType?.MediaType == "application/problem+json")
            {
                var problemDetails = await DeserializeJsonAsync<ProblemDetails>(httpResponse, traceId, cancellationToken);
                return new ApiResponse<TData>(problemDetails, httpResponse.StatusCode, traceId);
            }

            return new ApiResponse<TData>(httpResponse.StatusCode, traceId);
        }

        private async Task<ApiResponse> CreateResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            httpResponse.Headers.TryGetValues(CustomHeaders.TraceId, out var traceIdHeader);
            string? traceId = traceIdHeader?.FirstOrDefault();

            // In .NET Standard 2.1 HttpResponse.Content can be null
            if (httpResponse.Content?.Headers.ContentType?.MediaType == "application/problem+json")
            {
                var problemDetails = await DeserializeJsonAsync<ProblemDetails>(httpResponse, traceId, cancellationToken);
                return new ApiResponse(problemDetails, httpResponse.StatusCode, traceId);
            }

            return new ApiResponse(httpResponse.StatusCode, traceId);
        }

        private async Task<TData> DeserializeJsonAsync<TData>(HttpResponseMessage httpResponse, string? traceId, CancellationToken cancellationToken)
        {
            TData? data = default;

            if (httpResponse.Content != null)
            {
                try
                {
#if NET6_0 || NET6_0_OR_GREATER
                    data = await httpResponse.Content.ReadFromJsonAsync<TData>(SerializerOptions.Default, cancellationToken);
#else
                    using var contentStream = await httpResponse.Content.ReadAsStreamAsync();
                    data = await JsonSerializer.DeserializeAsync<TData>(contentStream, SerializerOptions.Default, cancellationToken);
#endif
                }
                catch (NotSupportedException) // Unsupported media type or invalid JSON
                {
                    throw new TrueLayerApiException(httpResponse.StatusCode, traceId, additionalInformation: "Invalid JSON");
                }
            }

            return data ?? throw new JsonException($"Failed to deserialize to type {typeof(TData).Name}");
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

            if (signingKey != null)
            {
                var signer = Signer.SignWith(signingKey.KeyId, signingKey.Value)
                    .Method(httpMethod.Method)
                    .Path(uri.AbsolutePath.TrimEnd('/'));

                if (request is { })
                {
                    // Only serialize to string if signing is required,
                    string json = JsonSerializer.Serialize(request, request.GetType(), SerializerOptions.Default);

                    signer.Body(json);

                    httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
                }

                if (!string.IsNullOrWhiteSpace(idempotencyKey))
                {
                    signer.Header(CustomHeaders.IdempotencyKey, idempotencyKey);
                }

                signature = signer.Sign();
            }
            else if (request is { }) // Otherwise we can serialize directly to stream for .NET 5.0 onwards
            {
#if (NET6_0 || NET6_0_OR_GREATER)
                httpContent = JsonContent.Create(request, request.GetType(), options: SerializerOptions.Default);
#else
                // for older versions of .NET we'll have to fall back to using StringContent
                string json = JsonSerializer.Serialize(request, request.GetType(), SerializerOptions.Default);
                httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
#endif
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

            httpRequest.Headers.UserAgent.Add(UserAgentHeader);

            // HttpCompletionOption.ResponseHeadersRead reduces allocations by by avoiding the pre-buffering of the response content
            // and allows us to access the content stream faster.
            // Doing so requires that always dispose of HttpResponseMessage to free up the connection
            // Ref: https://www.stevejgordon.co.uk/using-httpcompletionoption-responseheadersread-to-improve-httpclient-performance-dotnet
            return _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
    }
}
