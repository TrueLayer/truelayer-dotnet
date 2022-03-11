using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using TrueLayer.Auth;
using TrueLayer.Payments;
using TrueLayer.Payouts;
using TrueLayer.Payouts.Model;
using Xunit;
using AccountIdentifier = TrueLayer.Payouts.Model.AccountIdentifier;
using Beneficiary = TrueLayer.Payouts.Model.Beneficiary;

namespace TrueLayer.Tests.Payouts
{
    public class PayoutsApiTests
    {
        private readonly TrueLayerOptions _trueLayerOptions;

        public PayoutsApiTests()
        {
            _trueLayerOptions = new TrueLayerOptions()
            {
                Payments = new PaymentsOptions()
                {
                    SigningKey = new SigningKey()
                    {
                        KeyId = "key-id",
                        PrivateKey = "private-key",
                    },
                    Uri = new Uri("http://test.payouts.test"),
                },
            };
        }

        [Fact]
        public async Task CreatePayout_Returns_Empty_Response_With_Http_Status_Code_On_Auth_Failure()
        {
            // Arrange
            var authApiMock = new Mock<IAuthApi>();
            authApiMock.Setup(x => x.GetAuthToken(It.IsNotNull<GetAuthTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            var sut = new PayoutsApi(Mock.Of<IApiClient>(), authApiMock.Object, _trueLayerOptions);

            // Act
            var actual = await sut.CreatePayout(CreatePayoutRequest(), "indepotency-key", CancellationToken.None);

            //Assert
            actual.ShouldNotBeNull();
            actual.IsSuccessful.ShouldBeFalse();
            actual.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            actual.TraceId.ShouldBe("trace-id");
        }

        [Fact]
        public async Task GetPayout_Returns_Empty_Response_With_Http_Status_Code_On_Auth_Failure()
        {
            // Arrange
            var authApiMock = new Mock<IAuthApi>();
            authApiMock.Setup(x => x.GetAuthToken(It.IsNotNull<GetAuthTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            var sut = new PayoutsApi(Mock.Of<IApiClient>(), authApiMock.Object, _trueLayerOptions);

            // Act
            var actual = await sut.GetPayout("payout-id", CancellationToken.None);

            //Assert
            actual.ShouldNotBeNull();
            actual.IsSuccessful.ShouldBeFalse();
            actual.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            actual.TraceId.ShouldBe("trace-id");
        }

        private static CreatePayoutRequest CreatePayoutRequest()
            => new CreatePayoutRequest(
                "merchant-account-id",
                100,
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "Ms. Lucky",
                    "truelayer-dotnet",
                    new AccountIdentifier.Iban("GB33BUKB20201555555555")
                )
            );
    }
}
