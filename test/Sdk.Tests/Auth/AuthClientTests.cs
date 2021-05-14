using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TrueLayer.Auth;
using TrueLayer.Auth.Model;
using Xunit;

namespace TrueLayer.Sdk.Tests.Auth
{
    public class AuthClientTests
    {
        private readonly Mock<IApiClient> _apiClient;
        private readonly AuthClient _sut;
        private readonly ITokenCache _tokenCache;

        public AuthClientTests()
        {
            _apiClient = new Mock<IApiClient>();
            TrueLayerOptions options = new()
            {
                ClientId = "client_id",
                ClientSecret = "secret",
                UseSandbox = true,
            };
            _tokenCache = new LocalTokenCache();
            _sut = new AuthClient(_apiClient.Object, _tokenCache, options);
        }

        [Fact]
        public async Task AuthClient_ShouldNotCallAuthApi_WhenTokenStillValid()
        {
            // ARRANGE
            const string expectedToken = "valid-token";
            await _tokenCache.SetPaymentToken(expectedToken, 3600, "payments", "Beared");

            // ACT
            var result = await _sut.GetPaymentToken();

            // ASSERT
            _apiClient.Verify(x => x.PostAsync<AuthTokenResponse>(It.IsAny<Uri>(), It.IsAny<HttpContent>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Never);
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result.AccessToken);
        }
    }
}
