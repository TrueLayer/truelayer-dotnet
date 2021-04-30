using System.Threading.Tasks;
using Shouldly;
using TrueLayer.Auth.Model;
using Xunit;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class AuthTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        
        public AuthTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_request_payments_access_token_using_client_credentials()
        {
            GetPaymentTokenResponse response = await _fixture.Api.Auth.GetPaymentToken(new GetPaymentTokenRequest());
            response.ShouldNotBeNull();
            response.AccessToken.ShouldNotBeNullOrEmpty();
            response.ExpiresIn.ShouldBeGreaterThan(0);
            response.TokenType.ShouldBe("Bearer");
            response.Scope.ShouldBe("payments");
        }
    }
}
