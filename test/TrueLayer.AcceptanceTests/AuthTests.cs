using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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
                = await _fixture.Client.Auth.GetAuthToken(new GetAuthTokenRequest());

            apiResponse.IsSuccessful.Should().BeTrue();
            apiResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            apiResponse.Data.Should().NotBeNull();

            apiResponse.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
            apiResponse.Data.TokenType.Should().Be("Bearer");
            apiResponse.Data.Scope.Should().NotBeNullOrWhiteSpace();
            apiResponse.Data.ExpiresIn.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Can_get_scoped_access_token()
        {
            GetAuthTokenResponse? apiResponse
                = await _fixture.Client.Auth.GetAuthToken(new GetAuthTokenRequest("payments"));

            apiResponse.Should().NotBeNull();
            apiResponse!.Scope.Should().Be("payments");
        }
    }
}
