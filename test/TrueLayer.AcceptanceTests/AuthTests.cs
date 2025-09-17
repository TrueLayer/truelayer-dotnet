using System.Net;
using System.Threading.Tasks;
using TrueLayer.Auth;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    public class AuthTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public AuthTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_auth_token()
        {
            ApiResponse<GetAuthTokenResponse> apiResponse
                = await _fixture.TlClients[0].Auth.GetAuthToken(new GetAuthTokenRequest());

            Assert.True(apiResponse.IsSuccessful);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.NotNull(apiResponse.Data);

            Assert.False(string.IsNullOrWhiteSpace(apiResponse.Data!.AccessToken));
            Assert.Equal("Bearer", apiResponse.Data.TokenType);
            Assert.False(string.IsNullOrWhiteSpace(apiResponse.Data.Scope));
            Assert.True(apiResponse.Data.ExpiresIn > 0);
        }

        [Fact]
        public async Task Can_get_scoped_access_token()
        {
            GetAuthTokenResponse? apiResponse
                = await _fixture.TlClients[0].Auth.GetAuthToken(new GetAuthTokenRequest("payments"));

            Assert.NotNull(apiResponse);
            Assert.Equal("payments", apiResponse!.Scope);
        }
    }
}
