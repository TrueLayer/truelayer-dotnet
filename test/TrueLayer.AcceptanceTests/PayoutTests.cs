using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TrueLayer.Common;
using TrueLayer.Payouts.Model;
using Xunit;
using Beneficiary = TrueLayer.Payouts.Model.CreatePayoutBeneficiary;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;
using static TrueLayer.Payments.Model.CreateProviderSelection;
using ProviderFilter = TrueLayer.Payments.Model.ProviderFilter;
using PayoutVerification = TrueLayer.Payouts.Model.Verification;
using PayoutSchemeSelection = TrueLayer.Payouts.Model.SchemeSelection;
using PayoutAccountIdentifier = TrueLayer.Payouts.Model.AccountIdentifier;

namespace TrueLayer.AcceptanceTests;

public class PayoutTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;
    private static readonly string[] MockProviderIds = ["mock"];
    private static readonly string[] TransactionVerificationTokens = ["18db38", "Betropolis LTD", "LC Betropolis"];

    public PayoutTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Can_create_payout()
    {
        CreatePayoutRequest payoutRequest = CreatePayoutRequest();

        var response = await _fixture.TlClients[0].Payouts.CreatePayout(payoutRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        // Extract payout ID - can be either Created or AuthorizationRequired depending on API behavior
        var payoutId = response.Data!.Match(
            authRequired => authRequired.Id,
            created => created.Id);

        payoutId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Can_create_pln_payout()
    {
        CreatePayoutRequest payoutRequest = CreatePlnPayoutRequest();

        var response = await _fixture.TlClients[0].Payouts.CreatePayout(payoutRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        // Extract payout ID - can be either Created or AuthorizationRequired depending on API behavior
        var payoutId = response.Data!.Match(
            authRequired => authRequired.Id,
            created => created.Id);

        payoutId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Can_get_payout()
    {
        CreatePayoutRequest payoutRequest = CreatePayoutRequest();

        var response = await _fixture.TlClients[0].Payouts.CreatePayout(
            payoutRequest, idempotencyKey: Guid.NewGuid().ToString());

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        // Extract the payout ID from the Created response
        var payoutId = response.Data!.Match(
            authRequired => authRequired.Id,
            created => created.Id);

        payoutId.Should().NotBeNullOrWhiteSpace();

        var getPayoutResponse = await _fixture.TlClients[0].Payouts.GetPayout(payoutId);

        getPayoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getPayoutResponse.Data.Value.Should().NotBeNull();
        PayoutDetails? details = getPayoutResponse.Data.Value as PayoutDetails;

        details.Should().NotBeNull();
        details!.Id.Should().Be(payoutId);
        details.Currency.Should().Be(payoutRequest.Currency);
        details.Beneficiary.AsT1.Should().NotBeNull();
        details.Status.Should().BeOneOf("pending", "authorized", "executed", "failed");
        details.CreatedAt.Should().NotBe(DateTime.MinValue);
        details.CreatedAt.Should().NotBe(DateTime.MaxValue);
        details.Metadata.Should().BeEquivalentTo(payoutRequest.Metadata);
    }

    [Fact]
    public async Task GetPayout_Url_As_PayoutId_Should_Throw_Exception()
    {
        var client = _fixture.TlClients[0];
        const string payoutId = "https://test.com";

        var result = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.Payouts.GetPayout(payoutId));
        result.Message.Should().Be("Value is malformed (Parameter 'id')");
    }

    [Fact]
    public async Task Can_create_verified_payout_with_name_verification()
    {
        // Arrange - Create a verified payout with name verification only
        // Based on https://docs.truelayer.com/docs/make-a-verified-payout#test-verified-payouts-in-sandbox
        // For success: Account Holder Name must be "TRANSACTION ACCOUNT 1" and use "mock" provider
        var verification = new PayoutVerification(verifyName: true);

        var user = new PayoutUserRequest(
            name: "John Doe",
            email: "john.doe@example.com");

        var providerSelection = new UserSelected
        {
            Filter = new ProviderFilter { ProviderIds = MockProviderIds }
        };

        var beneficiary = new Beneficiary.UserDetermined(
            reference: "verified-payout-name-check",
            user: user,
            verification: verification,
            providerSelection: providerSelection);

        var payoutRequest = new CreatePayoutRequest(
            _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId,
            100,
            Currencies.GBP,
            beneficiary,
            metadata: new Dictionary<string, string> { { "test", "name-verification" } });

        // Act
        var response = await _fixture.TlClients[0].Payouts.CreatePayout(
            payoutRequest,
            idempotencyKey: Guid.NewGuid().ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        // For verified payouts, we should get an AuthorizationRequired response
        response.Data!.Match(
            authRequired =>
            {
                authRequired.Id.Should().NotBeNullOrWhiteSpace();
                authRequired.Status.Should().Be("authorization_required");
                authRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
                authRequired.User.Should().NotBeNull();
                authRequired.User.Id.Should().NotBeNullOrWhiteSpace();

                // Verify we can build an HPP link
                var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(
                    authRequired,
                    "https://example.com/callback",
                    useSandbox: true);
                hppLink.Should().NotBeNullOrWhiteSpace();
                hppLink.Should().Contain("app.truelayer-sandbox.com/payouts");
                hppLink.Should().Contain($"payout_id={authRequired.Id}");
                hppLink.Should().Contain($"resource_token={authRequired.ResourceToken}");

                return true;
            },
            created => throw new Exception("Expected AuthorizationRequired for verified payout, got Created"));
    }

    [Fact]
    public async Task Can_create_verified_payout_with_transaction_verification()
    {
        // Arrange - Create a verified payout with both name and transaction verification
        // Based on https://docs.truelayer.com/docs/make-a-verified-payout#test-verified-payouts-in-sandbox
        // For success: use tokens "18db38", "Betropolis LTD", or "LC Betropolis"
        // Amount: 1000 minor, Date: 1st-7th of any month
        var transactionSearchCriteria = new TransactionSearchCriteria(
            tokens: TransactionVerificationTokens,
            amountInMinor: 1000,
            currency: Currencies.GBP,
            createdAt: new DateTime(2024, 1, 5)); // 5th of January

        var verification = new PayoutVerification(
            verifyName: true,
            transactionSearchCriteria: transactionSearchCriteria);

        var user = new PayoutUserRequest(
            name: "John Doe",
            email: "john.doe@example.com");

        var providerSelection = new UserSelected
        {
            Filter = new ProviderFilter { ProviderIds = MockProviderIds }
        };

        var beneficiary = new Beneficiary.UserDetermined(
            reference: "verified-payout-transaction-check",
            user: user,
            verification: verification,
            providerSelection: providerSelection);

        var payoutRequest = new CreatePayoutRequest(
            _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId,
            100,
            Currencies.GBP,
            beneficiary,
            metadata: new Dictionary<string, string> { { "test", "transaction-verification" } });

        // Act
        var response = await _fixture.TlClients[0].Payouts.CreatePayout(
            payoutRequest,
            idempotencyKey: Guid.NewGuid().ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        // For verified payouts, we should get an AuthorizationRequired response
        response.Data!.Match(
            authRequired =>
            {
                authRequired.Id.Should().NotBeNullOrWhiteSpace();
                authRequired.Status.Should().Be("authorization_required");
                authRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
                authRequired.User.Should().NotBeNull();
                authRequired.User.Id.Should().NotBeNullOrWhiteSpace();

                // Verify we can build an HPP link using the base method
                var hppLink = PayoutHppLinkBuilder.CreateVerificationLink(
                    authRequired.Id,
                    authRequired.ResourceToken,
                    "https://example.com/callback",
                    useSandbox: true);
                hppLink.Should().NotBeNullOrWhiteSpace();
                hppLink.Should().Contain("app.truelayer-sandbox.com/payouts");

                return true;
            },
            created => throw new Exception("Expected AuthorizationRequired for verified payout, got Created"));
    }

    [Fact]
    public async Task Can_create_verified_payout_with_preselected_provider()
    {
        // Arrange - Create a verified payout with preselected provider
        var verification = new PayoutVerification(verifyName: true);

        var user = new PayoutUserRequest(
            name: "Jane Smith",
            email: "jane.smith@example.com",
            phone: "+442079460087");

        var providerSelection = new Preselected(
            providerId: "mock",
            schemeSelection: new Payments.Model.SchemeSelection.Preselected { SchemeId = "faster_payments_service" });

        var beneficiary = new Beneficiary.UserDetermined(
            reference: "verified-payout-preselected",
            user: user,
            verification: verification,
            providerSelection: providerSelection);

        var payoutRequest = new CreatePayoutRequest(
            _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId,
            500,
            Currencies.GBP,
            beneficiary);

        // Act
        var response = await _fixture.TlClients[0].Payouts.CreatePayout(
            payoutRequest,
            idempotencyKey: Guid.NewGuid().ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Data.Should().NotBeNull();

        response.Data!.Match(
            authRequired =>
            {
                authRequired.Id.Should().NotBeNullOrWhiteSpace();
                authRequired.Status.Should().Be("authorization_required");
                authRequired.ResourceToken.Should().NotBeNullOrWhiteSpace();
                authRequired.User.Should().NotBeNull();
                return true;
            },
            created => throw new Exception("Expected AuthorizationRequired for verified payout, got Created"));
    }

    private CreatePayoutRequest CreatePayoutRequest()
        => new(
            _fixture.ClientMerchantAccounts[0].GbpMerchantAccountId,
            100,
            Currencies.GBP,
            new Beneficiary.ExternalAccount(
                "Ms. Lucky",
                "truelayer-dotnet",
                new PayoutAccountIdentifier.Iban("GB33BUKB20201555555555"),
                dateOfBirth: new DateTime(1970, 12, 31),
                address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
            metadata: new() { { "a", "b" } },
            schemeSelection: new PayoutSchemeSelection.InstantOnly()
        );

    private static CreatePayoutRequest CreatePlnPayoutRequest()
        => new(
            "fdb6007b-78c0-dbc0-60dd-d4c6f6908e3b", //pln merchant account
            100,
            Currencies.PLN,
            new Beneficiary.ExternalAccount(
                "Ms. Lucky",
                "truelayer-dotnet",
                new PayoutAccountIdentifier.Iban("GB25CLRB04066800046876"),
                dateOfBirth: new DateTime(1970, 12, 31),
                address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
            metadata: new() { { "a", "b" } },
            schemeSelection: new PayoutSchemeSelection.InstantOnly()
        );
}
