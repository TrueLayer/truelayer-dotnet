using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;
using Xunit;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrueLayer.Models;

namespace TrueLayer.AcceptanceTests
{
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;
    using AuthorizationResponseUnion = OneOf<
        AuthorisationFlowResponse.AuthorizationFlowAuthorizing,
        AuthorisationFlowResponse.AuthorizationFlowAuthorizationFailed>;
    using MandateDetailUnion = OneOf<
        MandateDetail.AuthorizationRequiredMandateDetail,
        MandateDetail.AuthorizingMandateDetail,
        MandateDetail.AuthorizedMandateDetail,
        MandateDetail.FailedMandateDetail,
        MandateDetail.RevokedMandateDetail>;
    using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Mandates.Model.Provider.Preselected>;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private readonly TrueLayerOptions _configuration;
        private const string ReturnUri = "http://localhost:3000/callback";
        private const string ProviderId = "ob-uki-mock-bank-sbox"; // Beta provider in closed access, requires a whitelisted ClientId.
        private const string CommercialProviderId = "ob-natwest-vrp-sandbox"; // Provider to satisfy commercial mandates creation.
        private static readonly AccountIdentifier.SortCodeAccountNumber AccountIdentifier = new("140662", "10003957");

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
            _configuration = fixture.ServiceProvider.GetRequiredService<IOptions<TrueLayerOptions>>().Value;
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialPreselectedMandateRequests))]
        public async Task Can_create_mandate(CreateMandateRequest mandateRequest)
        {
            // Act
            var response = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Data!.User.Id.Should().Be(mandateRequest.User!.Id);
        }


        [Theory]
        [MemberData(nameof(CreateTestSweepingUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialUserSelectedMandateRequests), Skip = "It returns forbidden. Need to investigate.")]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialPreselectedMandateRequests), Skip = "It returns forbidden. Need to investigate.")]
        public async Task Can_get_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var mandateId = createResponse.Data!.Id;

            // Act
            var response = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.AsT0.User!.Id.Should().Be(createResponse.Data.User!.Id);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialPreselectedMandateRequests))]
        public async Task Can_list_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act
            var response = await _fixture.Client.Mandates.ListMandates(new ListMandatesQuery(createResponse.Data!.User.Id, null, 10), MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data!.Items.Count().Should().BeLessThanOrEqualTo(10);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        public async Task Can_start_authorization(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));

            // Act
            var response = await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);
            await AuthorizeMandate(response);
            var mandate = await WaitForMandateToBeAuthorized(mandateId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            mandate.AsT2.Status.Should().Be("authorized");
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialUserSelectedMandateRequests))]
        public async Task Can_submit_provider_selection(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            SubmitProviderSelectionRequest request = new(CommercialProviderId);
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));
            await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);
            // Act
            var response = await _fixture.Client.Mandates.SubmitProviderSelection(
                mandateId, request, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestCommercialUserSelectedMandateRequests))]
        public async Task Can_submit_consent(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());

            var mandateId = createResponse.Data!.Id;
            var mandateType = mandateRequest.Mandate.IsT0 ? MandateType.Commercial : MandateType.Sweeping;

            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)),
                new Consent());

            await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            SubmitProviderSelectionRequest submitProviderRequest = new(mandateRequest.Mandate.Match(
                c => CommercialProviderId,
                s => ProviderId));

            await _fixture.Client.Mandates.SubmitProviderSelection(
                mandateId, submitProviderRequest, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            // Act
            var response = await _fixture.Client.Mandates.SubmitConsent(
                mandateId, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        public async Task Can_Get_Funds(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));

            // Act
            var response = await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);
            await AuthorizeMandate(response);
            await WaitForMandateToBeAuthorized(mandateId);
            var fundsResponse = await _fixture.Client.Mandates.GetConfirmationOfFunds(mandateId, 1, "GBP", MandateType.Sweeping);

            // Assert
            fundsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            fundsResponse.Data!.ConfirmedAt.Date.Should().Be(DateTime.UtcNow.Date);
            fundsResponse.Data!.Confirmed.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        public async Task Can_get_mandate_constraints(CreateMandateRequest mandateRequest)
        {
            // Arrange
            string mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);

            // Act
            var response = await _fixture.Client.Mandates.GetMandateConstraints(
                mandateId,
                mandateRequest.Mandate.Match(
                    commercialMandate => MandateType.Commercial,
                    sweepingMandate => MandateType.Sweeping));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        public async Task Can_revoke_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            string mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);

            // Act
            var response = await _fixture.Client.Mandates.RevokeMandate(
                mandateId,
                idempotencyKey: Guid.NewGuid().ToString(),
                mandateRequest.Mandate.Match(
                    commercialMandate => MandateType.Commercial,
                    sweepingMandate => MandateType.Sweeping));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [MemberData(nameof(CreateTestSweepingPreselectedMandateRequests))]
        public async Task Can_create_mandate_payment(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);

            var paymentRequest = CreateTestMandatePaymentRequest(mandateRequest, mandateId, false);

            // Act
            var response = await _fixture.Client.Payments.CreatePayment(
                paymentRequest,
                idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Data.IsT1.Should().BeTrue();
            response.Data.AsT1.Id.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.ResourceToken.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.User.Should().NotBeNull();
            response.Data.AsT1.User.Id.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.Status.Should().Be("authorized");
        }

        private static CreateMandateRequest CreateTestMandateRequest(
            MandateUnion mandate,
            string currency = Currencies.GBP)
            => new(
                mandate,
                currency,
                new Constraints(
                    MaximumIndividualAmount: 1000,
                    new PeriodicLimits(Month: new Limit(2000, PeriodAlignment.Calendar))),
                new PaymentUserRequest(
                    id: "f9b48c9d-176b-46dd-b2da-fe1a2b77350c",
                    name: "Remi Terr",
                    email: "remi.terr@example.com",
                    phone: "+44777777777"),
                Metadata: new Dictionary<string, string> { { "a_custom_key", "a-custom-value" } });

        private static CreatePaymentRequest CreateTestMandatePaymentRequest(
            CreateMandateRequest mandateRequest,
            string mandateId,
            bool setRelatedProducts = true)
            => new(
                mandateRequest.Constraints.MaximumIndividualAmount,
                mandateRequest.Currency,
                new PaymentMethod.Mandate(mandateId, "reference", null),
                mandateRequest.User,
                setRelatedProducts ? new RelatedProducts(new SignupPlus()) : null);

        //TODO: replace with new common utility to authorize resources
        private async Task AuthorizeMandate(AuthorizationResponseUnion authorizationFlowResponse)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            var client = new HttpClient(handler);

            var redirectUri = authorizationFlowResponse.AsT0.AuthorizationFlow.Actions.Next.AsT4.Uri;
            var redirectResponse = await client.GetAsync(redirectUri);
            var paymentsSpaRedirectUrl = redirectResponse.Headers.Location;

            var isFragment = paymentsSpaRedirectUrl?.Fragment is not null;
            var rawParameters = isFragment ? paymentsSpaRedirectUrl?.Fragment : paymentsSpaRedirectUrl?.Query;
            var sanitizedParameters = rawParameters?.Replace("state=mandate-", "state=");

            var jsonPayload = isFragment
                ? "{\"fragment\":\"" + sanitizedParameters + "\"}"
                : "{\"query\":\"" + sanitizedParameters + "\"}";

            var authUri = new Uri($"{_configuration.Payments?.Uri}spa/payments-provider-return");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var submitProviderParamsResponse =
                await client.PostAsync(
                    authUri,
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            submitProviderParamsResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        private async Task<MandateDetailUnion> WaitForMandateToBeAuthorized(string mandateId)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                var mandate = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);
                if (mandate.Data.IsT2 && mandate.Data.AsT2.Status == "authorized")
                {
                    return mandate;
                }
            }
            return await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);
        }

        private async Task<string> CreateAuthorizedSweepingMandate(CreateMandateRequest mandateRequest)
        {
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));

            var startAuthResponse = await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);
            await AuthorizeMandate(startAuthResponse);
            await WaitForMandateToBeAuthorized(mandateId);
            return mandateId;
        }

        public static IEnumerable<object[]> CreateTestSweepingPreselectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", ProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "Bob NET SDK",
                        AccountIdentifierUnion.FromT0(AccountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestCommercialPreselectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", CommercialProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestSweepingUserSelectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter {Countries = ["GB"], ReleaseChannel = "general_availability"},
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestCommercialUserSelectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter {Countries = ["GB"], ReleaseChannel = "general_availability"},
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier)))))
            ];
        }
    }
}
