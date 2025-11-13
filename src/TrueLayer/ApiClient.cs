using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mime;
using TrueLayer.Serialization;
using System.Text.Json;
using TrueLayer.Caching;
using TrueLayer.Signing;
using System.Net.Http.Json;

namespace TrueLayer;

/// <summary>
/// Handles the authentication, serialization and sending of HTTP requests to TrueLayer APIs.
/// </summary>
internal class ApiClient : IApiClient
{
    private static readonly string TlAgentHeader
        = $"truelayer-dotnet/{ReflectionUtils.GetAssemblyVersion<ITrueLayerClient>()} ({System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription})";
    private readonly HttpClient _httpClient;
    private readonly IAuthTokenCache _authTokenCache;

    /// <summary>
    /// Creates a new <see cref="ApiClient"/> instance with the provided configuration, HTTP client factory and serializer.
    /// </summary>
    /// <param name="httpClient">The client used to make HTTP requests.</param>
    /// <param name="authTokenCache">The authentication token cache.</param>
    public ApiClient(HttpClient httpClient, IAuthTokenCache authTokenCache)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
        _authTokenCache = authTokenCache;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TData>> GetAsync<TData>(
        Uri uri,
        string? accessToken = null,
        IDictionary<string, string>? customHeaders = null,
        CancellationToken cancellationToken = default)
    {
        using var httpResponse = await SendRequestAsync(
            httpMethod: HttpMethod.Get,
            uri: uri,
            idempotencyKey: null,
            accessToken: accessToken,
            httpContent: null,
            signature: null,
            customHeaders: customHeaders,
            cancellationToken: cancellationToken
        );

        return await CreateResponseAsync<TData>(httpResponse, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, HttpContent? httpContent = null, string? accessToken = null, CancellationToken cancellationToken = default)
    {
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

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authTokenCache.Clear();
        }

        if (httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != HttpStatusCode.NoContent)
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

    private async Task<ApiResponse> CreateResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
    {
        httpResponse.Headers.TryGetValues(CustomHeaders.TraceId, out var traceIdHeader);
        string? traceId = traceIdHeader?.FirstOrDefault();

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authTokenCache.Clear();
        }

        if (httpResponse.Content.Headers.ContentType?.MediaType == "application/problem+json")
        {
            var problemDetails = await DeserializeJsonAsync<ProblemDetails>(httpResponse, traceId, cancellationToken);
            return new ApiResponse(problemDetails, httpResponse.StatusCode, traceId);
        }

        return new ApiResponse(httpResponse.StatusCode, traceId);
    }

    private async Task<TData> DeserializeJsonAsync<TData>(HttpResponseMessage httpResponse, string? traceId, CancellationToken cancellationToken)
    {
        try
        {
            var data = await httpResponse.Content.ReadFromJsonAsync<TData>(SerializerOptions.Default, cancellationToken);
            return data ?? throw new JsonException($"Failed to deserialize to type {typeof(TData).Name}");
        }
        catch (NotSupportedException) // Unsupported media type or invalid JSON
        {
            throw new TrueLayerApiException(httpResponse.StatusCode, traceId, additionalInformation: "Invalid JSON");
        }
    }

    private Task<HttpResponseMessage> SendJsonRequestAsync(
        HttpMethod httpMethod,
        Uri uri,
        string? idempotencyKey = null,
        string? accessToken = null,
        object? request = null,
        SigningKey? signingKey = null,
        IDictionary<string, string>? customHeaders = null,
        CancellationToken cancellationToken = default)
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
                byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(request, request.GetType(), SerializerOptions.Default);

                signer.Body(jsonBytes);

                httpContent = new ByteArrayContent(jsonBytes)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json) }
                };
            }

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                signer.Header(CustomHeaders.IdempotencyKey, idempotencyKey);
            }

            signature = signer.Sign();
        }
        else if (request is { })
        {
            httpContent = JsonContent.Create(request, request.GetType(), options: SerializerOptions.Default);
        }

        return SendRequestAsync(
            httpMethod,
            uri,
            idempotencyKey,
            accessToken,
            httpContent,
            signature,
            customHeaders,
            cancellationToken);
    }

    private Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod httpMethod,
        Uri uri,
        string? idempotencyKey = null,
        string? accessToken = null,
        HttpContent? httpContent = null,
        string? signature = null,
        IDictionary<string, string>? customHeaders = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);

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

        httpRequest.Headers.Add(CustomHeaders.Agent, TlAgentHeader);

        if (customHeaders is not null)
        {
            foreach (var (key, value) in customHeaders)
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value) && !httpRequest.Headers.Contains(key))
                {
                    httpRequest.Headers.Add(key, value);
                }
            }
        }

        // HttpCompletionOption.ResponseHeadersRead improves performance by avoiding buffering the entire response into memory.
        // This reduces allocations and allows faster access to the content stream.
        // Note: Requires proper disposal of HttpResponseMessage to free up the connection.
        // Ref: https://www.stevejgordon.co.uk/using-httpcompletionoption-responseheadersread-to-improve-httpclient-performance-dotnet
        return _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}
