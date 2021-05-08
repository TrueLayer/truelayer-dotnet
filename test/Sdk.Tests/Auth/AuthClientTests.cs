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
        private readonly TrueLayerTokenManager _tokenManager;
        
        public AuthClientTests()
        {
            _apiClient = new Mock<IApiClient>();
            TrueLayerOptions options = new()
            {
                ClientId = "client_id",
                ClientSecret = "secret",
                UseSandbox = true,
            };
            _tokenManager = new TrueLayerTokenManager();
            _sut = new AuthClient(_apiClient.Object, options, _tokenManager);
        }
        
        [Fact]
        public async Task AuthClient_ShouldNotCallAuthApi_WhenTokenStillValid()
        {
            // ARRANGE
            const string expectedToken = "valid-token";
            _tokenManager.SetPaymentToken(expectedToken, 3600, "payments", "Beared");
            _apiClient
                .Setup(x => x.PostAsync<AuthTokenResponse>(It.IsAny<Uri>(), It.IsAny<HttpContent>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("This should not be called"));
            
            // ACT
            var result = await _sut.GetPaymentToken();
            
            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result.AccessToken);
        }
    }
}
