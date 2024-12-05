using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TrueLayer.Auth;
using TrueLayer.Models;
using TrueLayer.Tests.Mocks;
using Xunit;

namespace TrueLayer.Tests.Auth;

public class AuthApiCacheDecoratorTests
{
    private readonly InMemoryAuthTokenCacheMock _authTokenCache = new();
    private readonly AuthApiMock _authApiMock = new();
    private readonly AuthApiCacheDecorator _authClient;

    public AuthApiCacheDecoratorTests()
    {
        _authClient = new AuthApiCacheDecorator(_authApiMock ,_authTokenCache);
    }

    [Fact]
    public async Task GetAuthToken_ResponseInCache_ReturnsCachedAuthToken()
    {
        const string scope = AuthorizationScope.Payments;

        var expectedResponse = new GetAuthTokenResponse("token", 3600, "Bearer", scope);
        var request = new GetAuthTokenRequest(scope);
        _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(expectedResponse, HttpStatusCode.OK, "trace"));
        await _authClient.GetAuthToken(request);
        _authApiMock.ResetGetAuthToken();

        //Act
        var response = await _authClient.GetAuthToken(request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAuthToken_SuccessfulResponse_SetCache()
    {
        const string scope = AuthorizationScope.Payments;

        var expectedResponse = new GetAuthTokenResponse("token123", 3600, "Bearer", scope);
        _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(expectedResponse, HttpStatusCode.OK, "trace"));

        var request = new GetAuthTokenRequest(scope);

        //Act
        var response = await _authClient.GetAuthToken(request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().BeEquivalentTo(expectedResponse);
        _authTokenCache.TryGetValue($"tl-auth-token-{scope}", out var cachedResponse);
        cachedResponse!.StatusCode.Should().Be(HttpStatusCode.OK);
        cachedResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetAuthToken_UnsuccessfulResponse_CacheIsNotSet()
    {
        const string scope = AuthorizationScope.Payments;

        var expectedResponse = new ProblemDetails("Type", "Title", null, null, null);
        _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(expectedResponse, HttpStatusCode.BadRequest, "trace"));

        var request = new GetAuthTokenRequest(scope);

        //Act
        var response = await _authClient.GetAuthToken(request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Problem.Should().BeEquivalentTo(expectedResponse);
        _authTokenCache.IsEmpty.Should().BeTrue();
    }
}
