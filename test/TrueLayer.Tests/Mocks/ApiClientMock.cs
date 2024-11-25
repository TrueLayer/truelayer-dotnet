using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.Tests.Mocks;

public class ApiClientMock : IApiClient
{
    private ApiResponse? _postResponse;

    public Task<ApiResponse<TData>> GetAsync<TData>(Uri uri, string? accessToken = null, IDictionary<string, string>? customHeaders = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void SetPostAsync<TData>(ApiResponse<TData> response)
    {
        _postResponse = response;
    }

    public Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, HttpContent? httpContent = null, string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult((ApiResponse<TData>)_postResponse!);
    }

    public Task<ApiResponse<TData>> PostAsync<TData>(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null,
        SigningKey? signingKey = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult((ApiResponse<TData>)_postResponse!);
    }

    public Task<ApiResponse> PostAsync(Uri uri, HttpContent? httpContent = null, string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> PostAsync(Uri uri, object? request = null, string? idempotencyKey = null, string? accessToken = null,
        SigningKey? signingKey = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
