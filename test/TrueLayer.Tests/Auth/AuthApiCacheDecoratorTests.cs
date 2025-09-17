using System.Net;
using System.Threading.Tasks;
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
    private const string ClientId = "clientId";

    public AuthApiCacheDecoratorTests()
    {
        _authClient = new AuthApiCacheDecorator(
            _authApiMock,
            _authTokenCache,
            new TrueLayerOptions { ClientId = ClientId});
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse, response.Data);
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedResponse, response.Data);
        _authTokenCache.TryGetValue($"tl-auth-token:{ClientId}:{scope}", out var cachedResponse);
        Assert.Equal(HttpStatusCode.OK, cachedResponse!.StatusCode);
        Assert.Equal(expectedResponse, cachedResponse.Data);
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(expectedResponse, response.Problem);
        Assert.True(_authTokenCache.IsEmpty);
    }
}
