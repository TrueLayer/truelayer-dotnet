using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using OneOf;
using Shouldly;
using TrueLayer.Auth;
using TrueLayer.Common;
using TrueLayer.Payments;
using TrueLayer.Payouts;
using TrueLayer.Payouts.Model;
using Xunit;
using AccountIdentifier = TrueLayer.Payouts.Model.AccountIdentifier;
using Beneficiary = TrueLayer.Payouts.Model.Beneficiary;

namespace TrueLayer.Tests.Payouts
{
    using PayoutBeneficiary = OneOf<Beneficiary.PaymentSource, Beneficiary.ExternalAccount, Beneficiary.BusinessAccount>;

    public class PayoutsApiTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly Mock<IAuthApi> _authApiMock;
        private readonly PayoutsApi _sut;

        public PayoutsApiTests()
        {
            TrueLayerOptions trueLayerOptions = new()
            {
                Payments = new PaymentsOptions
                {
                    SigningKey = new SigningKey
                    {
                        KeyId = "key-id",
                        PrivateKey = "private-key",
                    },
                    Uri = new Uri("http://test.payouts.test"),
                },
            };

            _apiClientMock = new Mock<IApiClient>();
            _authApiMock = new Mock<IAuthApi>();
            _sut = new PayoutsApi(_apiClientMock.Object, _authApiMock.Object, trueLayerOptions);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Generic_Successful_PayoutSource_Request(CreatePayoutRequest createPayoutRequest)
        {
            // Arrange
            var payoutResponseData = new CreatePayoutResponse("some-id");
            _apiClientMock
                .Setup(x => x.PostAsync<CreatePayoutResponse>(It.IsAny<Uri>(), It.IsAny<object?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<SigningKey?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<CreatePayoutResponse>(payoutResponseData, HttpStatusCode.OK, "trace-id"));

            var authData = new GetAuthTokenResponse("access-token", 1000, "Bearer", "");
            _authApiMock
                .Setup(x => x.GetAuthToken(It.IsAny<GetAuthTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<GetAuthTokenResponse>(authData, HttpStatusCode.OK, "trace-id"));

            // Act
            var response = await _sut.CreatePayout(createPayoutRequest, "idempotency-key", CancellationToken.None);

            // Assert
            response.ShouldNotBeNull();
            response.IsSuccessful.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.TraceId.ShouldBe("trace-id");
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldBe("some-id");
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task CreatePayout_Returns_Empty_Response_With_Http_Status_Code_On_Auth_Failure(CreatePayoutRequest createPayoutRequest)
        {
            // Arrange
            _authApiMock.Setup(x => x.GetAuthToken(It.IsNotNull<GetAuthTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            // Act
            var actual = await _sut.CreatePayout(createPayoutRequest, "idempotency-key", CancellationToken.None);

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
            _authApiMock
                .Setup(x => x.GetAuthToken(It.IsNotNull<GetAuthTokenRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            // Act
            var actual = await _sut.GetPayout("payout-id", CancellationToken.None);

            //Assert
            actual.ShouldNotBeNull();
            actual.IsSuccessful.ShouldBeFalse();
            actual.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            actual.TraceId.ShouldBe("trace-id");
        }

        private static CreatePayoutRequest CreatePayoutRequest(PayoutBeneficiary beneficiary) =>
            new(
                "merchant-account-id",
                100,
                Currencies.GBP,
                beneficiary,
                metadata: new() { { "a", "b" } });

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[]
                {
                    CreatePayoutRequest(new Beneficiary.ExternalAccount(
                        "Ms. Lucky",
                        "truelayer-dotnet",
                        new AccountIdentifier.Iban("GB33BUKB20201555555555"),
                        dateOfBirth: new DateTime(1970, 12, 31),
                        address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")
                    ))
                },
                new object[]
                {
                    CreatePayoutRequest(new Beneficiary.PaymentSource(
                        "payment-source-id",
                        "user-id",
                        "truelayer-dotnet"
                    ))
                },
                new object[]
                {
                    CreatePayoutRequest(new Beneficiary.BusinessAccount(
                        "truelayer-dotnet"
                    ))
                },
            };
    }
}
