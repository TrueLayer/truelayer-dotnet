using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OneOf;
using TrueLayer.Auth;
using TrueLayer.Common;
using Provider = TrueLayer.Payments.Model.Provider;
using TrueLayer.Payments;
using TrueLayer.Payouts;
using TrueLayer.Payouts.Model;
using TrueLayer.Tests.Mocks;
using Xunit;
using AccountIdentifier = TrueLayer.Payouts.Model.AccountIdentifier;
using Beneficiary = TrueLayer.Payouts.Model.CreatePayoutBeneficiary;
using static TrueLayer.Payouts.Model.CreatePayoutResponse;

namespace TrueLayer.Tests.Payouts
{
    using PayoutBeneficiary = OneOf<Beneficiary.PaymentSource, Beneficiary.ExternalAccount, Beneficiary.BusinessAccount, Beneficiary.UserDetermined>;
    using CreatePayoutUnion = OneOf<AuthorizationRequired, Created>;

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
            // Arrange - non-verified payouts only return Id
            var payoutResponseData = new Created { Id = "some-id" };
            _apiClientMock.SetPostAsync(new ApiResponse<CreatePayoutUnion>(payoutResponseData, HttpStatusCode.OK, "trace-id"));

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

            response.Data!.Match(
                authRequired => throw new Exception("Expected Created, got AuthorizationRequired"),
                created =>
                {
                    created.Id.Should().Be("some-id");
                    return true;
                });
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

        [Fact]
        public async Task CreatePayout_With_Verified_Payout_Returns_AuthorizationRequired()
        {
            // Arrange
            var verification = new Verification(verifyName: true);
            var user = new PayoutUserRequest(name: "John Doe", email: "john@example.com");
            var providerSelection = new Provider.UserSelected();
            var beneficiary = new Beneficiary.UserDetermined(
                reference: "verified-payout-ref",
                user: user,
                verification: verification,
                providerSelection: providerSelection
            );

            var createPayoutRequest = new CreatePayoutRequest(
                "merchant-account-id",
                100,
                Currencies.GBP,
                beneficiary
            );

            var payoutResponseData = new AuthorizationRequired
            {
                Id = "verified-payout-id",
                Status = "authorization_required",
                ResourceToken = "resource-token-123",
                User = new PayoutUserResponse("user-id-123")
            };

            _apiClientMock.SetPostAsync(new ApiResponse<CreatePayoutUnion>(payoutResponseData, HttpStatusCode.OK, "trace-id"));

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

            response.Data!.Match(
                authRequired =>
                {
                    authRequired.Id.Should().Be("verified-payout-id");
                    authRequired.Status.Should().Be("authorization_required");
                    authRequired.ResourceToken.Should().Be("resource-token-123");
                    authRequired.User.Should().NotBeNull();
                    authRequired.User.Id.Should().Be("user-id-123");
                    return true;
                },
                created => throw new Exception("Expected AuthorizationRequired, got Created"));
        }

        [Fact]
        public void CreateVerificationLink_Should_Build_Correct_Sandbox_Url()
        {
            // Arrange
            var payoutId = "payout-123";
            var resourceToken = "token-456";
            var returnUri = "https://example.com/callback";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: true);

            // Assert
            hppLink.Should().Be("https://app.truelayer-sandbox.com/payouts#payout_id=payout-123&resource_token=token-456&return_uri=https%3a%2f%2fexample.com%2fcallback");
        }

        [Fact]
        public void CreateVerificationLink_Should_Build_Correct_Production_Url()
        {
            // Arrange
            var payoutId = "payout-123";
            var resourceToken = "token-456";
            var returnUri = "https://example.com/callback";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: false);

            // Assert
            hppLink.Should().Be("https://app.truelayer.com/payouts#payout_id=payout-123&resource_token=token-456&return_uri=https%3a%2f%2fexample.com%2fcallback");
        }

        [Fact]
        public void CreateVerificationLink_From_Response_Should_Build_Correct_Url()
        {
            // Arrange
            var response = new AuthorizationRequired
            {
                Id = "payout-789",
                Status = "authorization_required",
                ResourceToken = "token-abc",
                User = new PayoutUserResponse("user-xyz")
            };
            const string returnUri = "https://example.com/callback";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(response, returnUri, useSandbox: true);

            // Assert
            hppLink.Should().Be("https://app.truelayer-sandbox.com/payouts#payout_id=payout-789&resource_token=token-abc&return_uri=https%3a%2f%2fexample.com%2fcallback");
        }

        [Fact]
        public void CreateVerificationLink_With_Empty_ResourceToken_Should_Throw()
        {
            // Arrange - this test verifies the base method validation
            const string payoutId = "payout-123";
            const string resourceToken = "";
            const string returnUri = "https://example.com/callback";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: true));

            exception.Message.Should().Contain("Resource token cannot be null or empty");
        }

        [Fact]
        public void CreateVerificationLink_With_Real_Production_Data_Should_Build_Valid_Link()
        {
            // Arrange - using realistic payout ID and resource token formats
            const string payoutId = "c2a6c5e8-6e7f-4b3a-9d2e-1f8b3c4d5e6f";
            const string resourceToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U";
            const string returnUri = "https://myapp.example.com/payout/callback?state=xyz123";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: false);

            // Assert - verify the link structure is correct for production
            hppLink.Should().StartWith("https://app.truelayer.com/payouts#");
            hppLink.Should().Contain($"payout_id={payoutId}");
            hppLink.Should().Contain($"resource_token={resourceToken}");
            hppLink.Should().Contain("return_uri=https%3a%2f%2fmyapp.example.com%2fpayout%2fcallback%3fstate%3dxyz123");

            // Verify complete URL format
            var expectedLink = $"https://app.truelayer.com/payouts#payout_id={payoutId}&resource_token={resourceToken}&return_uri=https%3a%2f%2fmyapp.example.com%2fpayout%2fcallback%3fstate%3dxyz123";
            hppLink.Should().Be(expectedLink);
        }

        [Fact]
        public void CreateVerificationLink_With_Real_Sandbox_Data_Should_Build_Valid_Link()
        {
            // Arrange - using realistic sandbox payout data
            const string payoutId = "d3b7d6f9-7f8g-5c4b-0e3f-2g9c4d5f6g7h";
            const string resourceToken = "sandbox_token_abc123def456ghi789jkl012mno345pqr678stu901vwx234yz";
            const string returnUri = "http://localhost:3000/payout-verification-complete";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: true);

            // Assert - verify the link structure is correct for sandbox
            hppLink.Should().StartWith("https://app.truelayer-sandbox.com/payouts#");
            hppLink.Should().Contain($"payout_id={payoutId}");
            hppLink.Should().Contain($"resource_token={resourceToken}");
            hppLink.Should().Contain("return_uri=http%3a%2f%2flocalhost%3a3000%2fpayout-verification-complete");

            // Verify complete URL format
            var expectedLink = $"https://app.truelayer-sandbox.com/payouts#payout_id={payoutId}&resource_token={resourceToken}&return_uri=http%3a%2f%2flocalhost%3a3000%2fpayout-verification-complete";
            hppLink.Should().Be(expectedLink);
        }

        [Fact]
        public void CreateVerificationLink_With_Complex_Return_Uri_Should_Encode_Properly()
        {
            // Arrange - test with return URI containing special characters and query params
            const string payoutId = "payout-test-123";
            const string resourceToken = "token-test-456";
            const string returnUri = "https://merchant.example.com/webhooks/payout?merchant_id=12345&session=abc-def-ghi&redirect=true&lang=en-GB";

            // Act
            var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(payoutId, resourceToken, returnUri, useSandbox: true);

            // Assert - verify proper URL encoding of complex return URI
            hppLink.Should().StartWith("https://app.truelayer-sandbox.com/payouts#");
            hppLink.Should().Contain("payout_id=payout-test-123");
            hppLink.Should().Contain("resource_token=token-test-456");

            // The return_uri should be fully URL encoded
            hppLink.Should().Contain("return_uri=https%3a%2f%2fmerchant.example.com%2fwebhooks%2fpayout%3fmerchant_id%3d12345%26session%3dabc-def-ghi%26redirect%3dtrue%26lang%3den-GB");
        }

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
