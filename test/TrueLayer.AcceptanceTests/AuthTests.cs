using System.Net;
using System.Threading.Tasks;
using Shouldly;
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

            apiResponse.IsSuccessful.ShouldBeTrue();
            apiResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            apiResponse.Data.ShouldNotBeNull();

            apiResponse.Data.AccessToken.ShouldNotBeNullOrWhiteSpace();
            apiResponse.Data.TokenType.ShouldBe("Bearer");
            apiResponse.Data.Scope.ShouldNotBeNullOrWhiteSpace();
            apiResponse.Data.ExpiresIn.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Can_get_scoped_access_token()
        {
            GetAuthTokenResponse? apiResponse
                = await _fixture.Client.Auth.GetAuthToken(new GetAuthTokenRequest("payments"));

            apiResponse.ShouldNotBeNull();
            apiResponse.Scope.ShouldBe("payments");
        }
    }
}
