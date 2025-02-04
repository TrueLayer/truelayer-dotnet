using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Common;
using TrueLayer.Payments;
using TrueLayer.Payouts;
using TrueLayer.Payouts.Model;
using TrueLayer.Tests.Mocks;
using Xunit;
using AccountIdentifier = TrueLayer.Payouts.Model.AccountIdentifier;
using Beneficiary = TrueLayer.Payouts.Model.Beneficiary;

namespace TrueLayer.Tests.Payouts
{
    using PayoutBeneficiary = OneOf<Beneficiary.PaymentSource, Beneficiary.ExternalAccount, Beneficiary.BusinessAccount>;

    public class PayoutsApiTests
    {
        private readonly ApiClientMock _apiClientMock;
        private readonly AuthApiMock _authApiMock;
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

            _apiClientMock = new ApiClientMock();
            _authApiMock = new AuthApiMock();
            _sut = new PayoutsApi(_apiClientMock, _authApiMock, trueLayerOptions);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Generic_Successful_PayoutSource_Request(CreatePayoutRequest createPayoutRequest)
        {
            // Arrange
            var payoutResponseData = new CreatePayoutResponse("some-id");
            _apiClientMock.SetPostAsync(new ApiResponse<CreatePayoutResponse>(payoutResponseData, HttpStatusCode.OK, "trace-id"));

            var authData = new GetAuthTokenResponse("access-token", 1000, "Bearer", "");
            _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(authData, HttpStatusCode.OK, "trace-id"));

            // Act
            var response = await _sut.CreatePayout(createPayoutRequest, "idempotency-key", CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.TraceId.Should().Be("trace-id");
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().Be("some-id");
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task CreatePayout_Returns_Empty_Response_With_Http_Status_Code_On_Auth_Failure(CreatePayoutRequest createPayoutRequest)
        {
            // Arrange
            _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            // Act
            var actual = await _sut.CreatePayout(createPayoutRequest, "idempotency-key", CancellationToken.None);

            //Assert
            actual.Should().NotBeNull();
            actual.IsSuccessful.Should().BeFalse();
            actual.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.TraceId.Should().Be("trace-id");
        }

        [Fact]
        public async Task GetPayout_Returns_Empty_Response_With_Http_Status_Code_On_Auth_Failure()
        {
            // Arrange
            _authApiMock.SetGetAuthToken(new ApiResponse<GetAuthTokenResponse>(HttpStatusCode.BadRequest, "trace-id"));

            // Act
            var actual = await _sut.GetPayout("payout-id", CancellationToken.None);

            //Assert
            actual.Should().NotBeNull();
            actual.IsSuccessful.Should().BeFalse();
            actual.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actual.TraceId.Should().Be("trace-id");
        }

        private static CreatePayoutRequest CreatePayoutRequest(PayoutBeneficiary beneficiary) =>
            new(
                "merchant-account-id",
                100,
                Currencies.GBP,
                beneficiary,
                metadata: new() { { "a", "b" } },
                schemeSelection: new SchemeSelection.InstantPreferred());

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
